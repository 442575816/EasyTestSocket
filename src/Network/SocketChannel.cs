using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using EasyTestSocket.Buf;
using EasyTestSocket.Log;

namespace EasyTestSocket.Network;

public class SocketChannel
{
    private static readonly Logger Log = LogFactory.GetLog("com.will.socket");

    public string Id { get; }

    /// <summary>
    /// 是否连接中
    /// </summary>
    public bool IsConnected { get; set; }

    public Socket? Socket { get; private set; }

    private readonly string _host;
    private readonly int _port;

    /// <summary>
    /// socket消息适配器
    /// </summary>
    private readonly SocketChannelHandler _socketHandler;

    private readonly bool _waitForData; // 分配buff之前先等待数据
    private readonly SocketSenderPool _senderPool; // 发送者池

    // 默认最小分配buff大小
    private readonly int _minAllocBufferSize;
    private volatile bool _socketDisposed;
    private int _connectFlag; // 0 未进行过连接 1 已进行过连接
    private readonly object _shutdownLock = new();
    private Task? _recvTask;
    private readonly SocketOption _socketOption;
    private readonly ByteBufPool _byteBufPool;
    private readonly PipeScheduler _awaiterScheduler;

    private SocketReceiver? _receiver; // Socket数据接受者

    private volatile bool _connectionClosed;
    private readonly CancellationTokenSource _connectionClosedTokenSource = new();
    public CancellationToken ConnectionClosed { get; }
    public EndPoint? RemoteEndPoint { get; set; }


    /// <summary>
    /// 构造函数
    /// <code>
    /// Pipe的环是这样的
    /// Transport.Output -> Application.Input
    /// Application.Output -> Transport.Input
    /// </code>
    /// </summary>
    /// <param name="socketServer"></param>
    public SocketChannel(string host, int port,
                                SocketOption option,
                                SocketChannelHandler handler,
                                PipeScheduler awaiterScheduler,
                                SocketSenderPool senderPool,
                                bool waitForData = false)
    {
        ConnectionClosed = _connectionClosedTokenSource.Token;
        Id = Guid.NewGuid().ToString();
        _waitForData = waitForData;
        _host = host;
        _port = port;
        _socketOption = option;
        _socketHandler = handler;
        _senderPool = senderPool;
        _awaiterScheduler = awaiterScheduler;

        _byteBufPool = option.ByteBufPool;
        _minAllocBufferSize = _socketOption.BuffSize;
    }

    public void Connect()
    {
        var code = ConnectSync();

        // 使用另外线程通知，避免递归
        ThreadPool.UnsafeQueueUserWorkItem(state =>
        {
            state._socketHandler.OnEvent(this, code);
            if (code != SocketEventCode.ConnectSucc)
            {
                state._socketHandler.OnEvent(this, SocketEventCode.Close);
            }
        }, this, false);
    }

    public SocketEventCode ConnectSync()
    {
        if (Interlocked.CompareExchange(ref _connectFlag, 1, 0) == 0)
        {
            if (!IPAddress.TryParse(_host, out var address))
            {
                var addresseList = Dns.GetHostAddresses(_host);
                if (addresseList.Length > 0)
                {
                    address = addresseList[0];
                    if (addresseList.Length > 1)
                    {
                        address = addresseList[1];
                    }
                }
            }

            RemoteEndPoint = new IPEndPoint(address!, _port);
            var socket = new Socket (address!.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.SendTimeout = _socketOption.SendTimeout;
            socket.ReceiveTimeout = _socketOption.ReadTimeout;
            socket.NoDelay = _socketOption.NoDelay;
            socket.SendBufferSize = _socketOption.BuffSize;
            socket.ReceiveBufferSize = _socketOption.BuffSize;
            // socket.LingerState = new LingerOption(true, 10);
            // socket.Blocking = false;

            return ConnectWithTimeout(socket, address, _socketOption.ConnectTimeout);
        }
        else
        {
            throw new Exception("socket is connected already.");
        }
    }

    private SocketEventCode ConnectWithTimeout(Socket socket, IPAddress address, int timeout)
    {
        try
        {
            var result = socket.BeginConnect(address, _port, null, null);
            var succ = result.AsyncWaitHandle.WaitOne(timeout, true);
            if (succ)
            {
                // 连接成功
                socket.EndConnect(result);

                Socket = socket;
                IsConnected = Socket.Connected;

                // 创建数据接受者
                _receiver = new SocketReceiver(_awaiterScheduler);

                // 读取消息
                _ = Task.Run(async () => await StartAsync());

                // 通知连接成功
                return SocketEventCode.ConnectSucc;
            }
            else
            {
                // 连接超时
                Close(socket);

                return SocketEventCode.ConnectTimeout;
            }
        }
        catch (SocketException e)
        {
            Log.Error(e, "connect error");

            Close(socket);

            return SocketEventCode.ConnectFail;
        }
    }

    /// <summary>
    /// 异步开始
    /// </summary>
    /// <returns></returns>
    public async Task StartAsync()
    {
        try
        {
            // ChannelPipeline = new DefaultChannelPipeline(this);
            _recvTask = DoReceive();
            
            await _recvTask;
            _receiver?.Dispose();
            // _sender?.Dispose();
        }
        catch (Exception e)
        {
            Log.Error(e, $"unexpected error in");
        }
    }

    #region socket接受数据
    /// <summary>
    /// 开始接收任务
    /// </summary>
    /// <returns></returns>
    private async Task DoReceive()
    {
        var waitForData = _waitForData;
        try
        {
            while (true)
            {
                if (!IsConnected)
                {
                    break;
                }

                if (waitForData)
                {
                    var waitForDataResult = await _receiver!.WaitForDataAsync(Socket!);
                    if (!IsNormalCompletion(waitForDataResult))
                    {
                        break;
                    }
                }

                // 分配buffer
                var buffer = _byteBufPool.Allocate();
                buffer.EnsureCapacity(_minAllocBufferSize);
                var memory = buffer.Data.AsMemory(buffer.WriterIndex, _minAllocBufferSize);
                var receiveResult = await _receiver!.ReceiveAsync(Socket!, memory);
                if (!IsNormalCompletion(receiveResult))
                {
                    break;
                }

                var bytesReceived = receiveResult.BytesTransferred;
                if (bytesReceived == 0)
                {
                    // FIN
                    buffer.Release();
                    break;
                }

                // 通知读取到消息
                buffer.WriterIndex += bytesReceived;
                _socketHandler.OnRead(this, buffer);
            }
        }
        catch (Exception e)
        {
            if (!_socketDisposed)
            {
                Log.Error(e, "recv data error");
            }
        }
        finally
        {
            // 通知连接被关闭，整个socket周期终结
            FireConnectionClosed();
        }
    }
    // 判断是否是正常返回的result
    private bool IsNormalCompletion(SocketOperationResult result)
    {
        if (!result.HasError)
        {
            return true;
        }

        if (IsConnectionResetError(result.SocketError.SocketErrorCode))
        {
            // The connection was reset by the remote peer
            return false;
        }

        if (IsConnectionAbortError(result.SocketError.SocketErrorCode))
        {
            // The connection was aborted by the remote peer
            return false;
        }

        // 正常错误
        return true;
    }

    /// <summary>
    /// 是否是连接被重置的错误
    /// </summary>
    /// <param name="errorCode"></param>
    /// <returns></returns>
    private static bool IsConnectionResetError(SocketError errorCode)
    {
        return errorCode == SocketError.ConnectionReset ||
               errorCode == SocketError.Shutdown ||
               (errorCode == SocketError.ConnectionAborted && OperatingSystem.IsWindows());
    }

    /// <summary>
    /// 是否是连接被终止的错误
    /// </summary>
    /// <param name="errorCode"></param>
    /// <returns></returns>
    private static bool IsConnectionAbortError(SocketError errorCode)
    {
        // Calling Dispose after ReceiveAsync can cause an "InvalidArgument" error on *nix.
        return errorCode == SocketError.OperationAborted ||
               errorCode == SocketError.Interrupted ||
               (errorCode == SocketError.InvalidArgument && !OperatingSystem.IsWindows());
    }
    #endregion

    #region socket发送数据

    public bool Send(ByteBuf buf)
    {
        return SendAsync(buf).GetAwaiter().GetResult();
    }

    /// <summary>
    /// 异步发送数据
    /// </summary>
    /// <param name="buf"></param>
    public async Task<bool> SendAsync(ByteBuf buf)
    {
        try
        {
            if (!IsConnected)
            {
                return false;
            }

            if (buf.ReadableBytes == 0)
            {
                return false;
            }

            var sender = _senderPool.Rent();
            SocketOperationResult transferResult;
            var bufList = buf.WrappedByteBufList;
            if (bufList == null)
            {
                transferResult = await sender.SendAsync(Socket!, buf.Data.AsMemory(buf.ReaderIndex, buf.ReadableBytes));
            }
            else
            {
                var list = new List<ArraySegment<byte>>(bufList.Count + 1) { new(buf.Data, buf.ReaderIndex, buf.ReadableBytes) };
                foreach (var _buff in bufList)
                {
                    list.Add(new ArraySegment<byte>(_buff.Data, _buff.ReaderIndex, _buff.ReadableBytes));
                }
                transferResult = await sender.SendAsync(Socket!, list);
            }
            _senderPool.Return(sender);

            if (transferResult.HasError)
            {
                FireConnectionClosed();
                return false;
            }
            return true;
        }
        catch (Exception e)
        {
            Log.Error(e, "send data error");
        }
        finally
        {
            buf.Release();
        }

        return false;
    }
    #endregion

    #region 关闭连接

    /// <summary>
    /// 关闭Socket
    /// </summary>
    private void Close(Socket? socket)
    {
        socket?.Close();
    }

    private void FireConnectionClosed()
    {
        // Guard against scheduling this multiple times
        if (_connectionClosed)
        {
            return;
        }

        // 关闭链接
        Disconnect();

        _connectionClosed = true;

        // 使用另外线程通知，避免递归
        ThreadPool.UnsafeQueueUserWorkItem(state => state.CancelConnectionClosedToken(), this, false);
    }

    private void CancelConnectionClosedToken()
    {
        try
        {
            _connectionClosedTokenSource.Cancel();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"unexpected error");
        }
    }

    /// <summary>
    /// 关闭连接
    /// </summary>
    public void Close()
    {
        // 关闭流
        Shutdown();
    }

    public void Shutdown(Exception? shutdownReason = null)
    {
        if (_socketDisposed)
        {
            return;
        }

        if (Socket == null)
        {
            return;
        }

        lock (_shutdownLock)
        {
            if (_socketDisposed)
            {
                return;
            }
            if (Socket == null)
            {
                return;
            }
            // Make sure to close the connection only after the _aborted flag is set.
            // Without this, the RequestsCanBeAbortedMidRead test will sometimes fail when
            // a BadHttpRequestException is thrown instead of a TaskCanceledException.
            _socketDisposed = true;
            IsConnected = false;

            if (null == shutdownReason)
            {
                Log.Info("{0} shutdown. completed gracefully.", Id);
            }
            else
            {
                Log.Error(shutdownReason, "{0} shutdown. reason:{1}.", Id, shutdownReason.Message);
            }
            
            try
            {
                // Try to gracefully close the socket even for aborts to match libuv behavior.
                Socket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
                // Ignore any errors from Socket.Shutdown() since we're tearing down the connection anyway.
            }

            try
            {
                Socket.Dispose();
            } catch
            {
                // Ignore any errors from Socket.Dispose() since we're tearing down the connection anyway.
            }

            _socketHandler.OnEvent(this, SocketEventCode.Close);
        }
    }

    public void Disconnect()
    {
        Shutdown();
    }
    #endregion
}
