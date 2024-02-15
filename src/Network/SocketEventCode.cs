namespace EasyTestSocket.Network;

/// <summary>
/// 网络事件
/// </summary>
public enum SocketEventCode
{
    /// <summary>
    /// 网络连接成功
    /// </summary>
    ConnectSucc = 1,

    /// <summary>
    /// 网络连接失败
    /// </summary>
    ConnectFail = 2,

    /// <summary>
    /// 网络连接失败
    /// </summary>
    ConnectTimeout = 3,

    /// <summary>
    /// 网络失去连接
    /// </summary>
    DisConnect = 4,

    /// <summary>
    /// 网络Socket错误
    /// </summary>
    SocketError = 5,

    /// <summary>
    /// 网络连接错误
    /// </summary>
    ConnectError = 6,

    /// <summary>
    /// 读取消息错误
    /// </summary>
    ReadError = 7,

    /// <summary>
    /// 发送消息错误
    /// </summary>
    SendError = 8,

    /// <summary>
    /// 关闭事件
    /// </summary>
    Close = 9
}