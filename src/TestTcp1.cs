using System.Text;
using EasyTestSocket.Buf;
using EasyTestSocket.Network;

namespace EasyTestSocket;

public class TestTcp1
{
    public string Host { get; set; }
    public int Port { get; set; }
    public int Timeout { get; set; }
    
    private readonly SocketFactory _socketFactory;
    
    public TestTcp1(string host, int port, int timeout = 2000)
    {
        Host = host;
        Port = port;
        Timeout = timeout;

        _socketFactory = new SocketFactory(new SocketOption
        {
            ConnectTimeout = timeout,
            ReadTimeout = timeout,
            SendTimeout = timeout,
        });
    }
    
    public async Task StartAsync(string sessionId, long playerId)
    {
        var socket = new TestSocket2(Host, Port, _socketFactory);
        await Task.Delay(1000);

        var requestId = 1;
        
        // reconnect
        requestId++;
        var buf = WrapperPack("reconnect", 1, 1, requestId, Encoding.ASCII.GetBytes($"sessionId={sessionId}"));
        await socket.SendBuf(buf);
        
        // getPlayerInfo
        requestId++;
        buf = WrapperPack("getPlayerInfo", 2, 1, requestId, Encoding.ASCII.GetBytes($"playerId={playerId}"));
        await socket.SendBuf(buf);
        
        requestId++;
        buf = WrapperPack("numbalance/getInfo", 2, 1, requestId, Encoding.ASCII.GetBytes(""));
        await socket.SendBuf(buf);
            
        requestId++;
        buf = WrapperPack("numbalance/play", 2, 1, requestId, Encoding.ASCII.GetBytes("num=1000"));
        await socket.SendBuf(buf);
        
        // 每1分钟发送一个heartbeat
        while (true)
        {
            await Task.Delay(1000);
        }
    }
    
    public ByteBuf WrapperPack(string command, int serverType, int serverId, int requestId, byte[] bytes, int packType = 1)
    {
        var commandBytes = Encoding.ASCII.GetBytes(command);
        if (commandBytes.Length > 32)
        {
            throw new IndexOutOfRangeException($"command {command} length greater than 32");
        }

        var buff = new ByteBuf();
        buff.WriteInt(45 + bytes.Length);
        buff.WriteByte(packType);
        buff.WriteInt(serverType);
        buff.WriteInt(serverId);
        buff.WriteInt(requestId);
        buff.WriteBytes(commandBytes);
        buff.WriteEmpty(32 - commandBytes.Length);
        buff.WriteBytes(bytes);

        return buff;
    }
}



public class TestSocket2 : MessageDecoder
{
    public BenchmarkResult Result { get; }
    
    private readonly SocketChannel _channel;
    private readonly SocketEventCode _connEventCode;
    public Exception? Error { get; set; }
    
    public TestSocket2(string host, int port, SocketFactory factory)
    {
        Result = new BenchmarkResult();
        _channel = factory.Create(host, port, this);
        try
        {
            _connEventCode = _channel.ConnectSync();
        }
        catch (Exception e)
        {
            Error = e;
        }
    }
    
    public async Task<bool> SendBuf(ByteBuf buf)
    {
        if (_connEventCode != SocketEventCode.ConnectSucc)
        {
            return false;
        }
        
        try
        {
            return await _channel.SendAsync(buf);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("Send Error");
            Console.Error.WriteLine(e);
        }

        return true;
    }
    
    public new void OnEvent(SocketChannel channel, SocketEventCode eventCode, string content = "")
    {
    }

    protected override void Decode(SocketChannel channel, ByteBuf buf)
    {
        var readableBytes = buf.ReadableBytes;
        if (readableBytes < 4)
        {
            return;
        }

        var dataLen = buf.GetInt();
        if (dataLen < 0)
        {
            // 错误的包
            buf.SkipBytes(readableBytes);
            return;
        }
        
        if (readableBytes < dataLen + 4)
        {
            return;
        }
        
        buf.SkipBytes(5);
        buf.SkipBytes(8); // long
        buf.SkipBytes(4); // requestId
        var bytes = new byte[32];
        buf.ReadBytes(bytes);
        var command = Encoding.ASCII.GetString(bytes);
        
        bytes = new byte[dataLen - 45];
        buf.ReadBytes(bytes);
        var content = Encoding.UTF8.GetString(bytes);
        
        Console.WriteLine($"[response]command:{command}, echo:{content}");
    }
}