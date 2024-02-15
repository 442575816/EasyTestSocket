using EasyTestSocket.Buf;

namespace EasyTestSocket.Network;

public class SocketOption
{
    /// <summary>
    /// 分配Buff之前先等待数据。Setting this to false can increase throughput at the cost of increased memory usage.
    /// </summary>
    public bool WaitForDataBeforeAllocatingBuffer { get; set; } = false;
    
    /// <summary>
    /// 连接超时时间
    /// </summary>
    public int ConnectTimeout { get; set; } = 1000;
    
    /// <summary>
    /// 读超时时间
    /// </summary>
    public int ReadTimeout { get; set; } = 2000;
    
    /// <summary>
    /// 发送超时时间
    /// </summary>
    public int SendTimeout { get; set; } = 2000;
    
    /// <summary>
    /// 网络读取缓冲队列大小 4K
    /// </summary>
    public int BuffSize { get; set; } = 4096;
    
    /// <summary>
    /// 不启用Nagle算法，立即发送数据
    /// </summary>
    public bool NoDelay { get; set; } = true;
    
    /// <summary>
    /// ByteBuf池
    /// </summary>
    public ByteBufPool ByteBufPool { get; set; } = new(() => new ByteBuf(4096), 1, 20);
}