using EasyTestSocket.Buf;

namespace EasyTestSocket.Network;

public abstract class MessageDecoder : SocketChannelHandler
{
    private ByteBuf? _cumulation;
    private bool _first;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="networkClient"></param>
    protected MessageDecoder()
    {
    }

    public void OnRead(SocketChannel channel, ByteBuf buf)
    {
        _first = _cumulation == null;
        _cumulation = _first ? buf : Cumulate(_cumulation!, buf);

        try
        {
            var index = _cumulation.ReaderIndex;
            while (true)
            {
                Decode(channel, _cumulation);
                if (index == _cumulation.ReaderIndex || _cumulation.ReadableBytes < 4)
                {
                    break;
                }
                index = _cumulation.ReaderIndex;
            }
        }
        finally
        {
            if (_cumulation is { ReadableBytes: <= 0 })
            {
                _cumulation.Release();
                _cumulation = null;
            }
        }
    }

    private ByteBuf Cumulate(ByteBuf cumulation, ByteBuf data)
    {
        try
        {
            cumulation.WriteBytes(data);
            return cumulation;
        }
        finally
        {
            data.Release();
        }
    }

    public void OnEvent(SocketChannel channel, SocketEventCode eventCode, string content = "")
    {
        // Ignore
    }
    
    protected abstract void Decode(SocketChannel channel, ByteBuf buf);
}
