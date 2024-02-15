using System.IO.Pipelines;
using System.Net.Sockets;

namespace EasyTestSocket.Network;

/// <summary>
/// Socket消息接受者
/// </summary>
public sealed class SocketReceiver : SocketAwaitableEventArgs
{

    /// <summary>
    /// 构造函数
    /// </summary>
    public SocketReceiver(PipeScheduler ioScheduler) : base(ioScheduler)
    {
    }

    /// <summary>
    /// 等待接收数据
    /// </summary>
    /// <param name="socket"></param>
    /// <returns></returns>
    public ValueTask<SocketOperationResult> WaitForDataAsync(Socket socket)
    {
        SetBuffer(Memory<byte>.Empty);

        if (socket.ReceiveAsync(this))
        {
            return new ValueTask<SocketOperationResult>(this, 0);
        }

        var bytesTransferred = BytesTransferred;
        var error = SocketError;

        return error == SocketError.Success ?
            new ValueTask<SocketOperationResult>(new SocketOperationResult(bytesTransferred)) :
            new ValueTask<SocketOperationResult>(new SocketOperationResult(CreateException(error)));
    }

    /// <summary>
    /// 接收数据
    /// </summary>
    /// <param name="socket"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public ValueTask<SocketOperationResult> ReceiveAsync(Socket socket, Memory<byte> buffer)
    {
        SetBuffer(buffer);

        if (socket.ReceiveAsync(this))
        {
            return new ValueTask<SocketOperationResult>(this, 0);
        }

        var bytesTransferred = BytesTransferred;
        var error = SocketError;

        return error == SocketError.Success ?
            new ValueTask<SocketOperationResult>(new SocketOperationResult(bytesTransferred)) :
            new ValueTask<SocketOperationResult>(new SocketOperationResult(CreateException(error)));
    }

    /// <summary>
    /// 接收数据
    /// </summary>
    /// <param name="socket"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public ValueTask<SocketOperationResult> ReceiveFromAsync(Socket socket, Memory<byte> buffer)
    {
        SetBuffer(buffer);

        if (socket.ReceiveFromAsync(this))
        {
            return new ValueTask<SocketOperationResult>(this, 0);
        }

        var bytesTransferred = BytesTransferred;
        var error = SocketError;

        return error == SocketError.Success ?
            new ValueTask<SocketOperationResult>(new SocketOperationResult(bytesTransferred)) :
            new ValueTask<SocketOperationResult>(new SocketOperationResult(CreateException(error)));
    }
}
