using System.IO.Pipelines;

namespace EasyTestSocket.Network;

public class SocketFactory
{
    private readonly bool _waitForData;
    private readonly SocketOption _socketOption;
    private readonly PipeScheduler _awaiterScheduler;
    private readonly SocketSenderPool _socketSenderPool;

    public SocketFactory(SocketOption option)
    {
        _socketOption = option;
        _waitForData = option.WaitForDataBeforeAllocatingBuffer;
        _awaiterScheduler = OperatingSystem.IsWindows() ? PipeScheduler.ThreadPool : PipeScheduler.Inline;
        _socketSenderPool = new SocketSenderPool(_awaiterScheduler);
    }

    public SocketChannel Create(string host, int port, SocketChannelHandler handler)
    {
        return new SocketChannel(host, port, _socketOption, handler, _awaiterScheduler, _socketSenderPool, _waitForData);
    }
}

