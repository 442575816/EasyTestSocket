using EasyTestSocket.Buf;

namespace EasyTestSocket.Network;

public interface SocketChannelHandler
{
    /// <summary>
    /// 处理事件
    /// </summary>
    /// <param name="client"></param>
    /// <param name="eventCode"></param>
    void OnEvent(SocketChannel channel, SocketEventCode eventCode, string content = "");

    /// <summary>
    /// 处理读取消息
    /// </summary>
    /// <param name="client"></param>
    /// <param name="buf"></param>
    void OnRead(SocketChannel channel, ByteBuf buf);
}
