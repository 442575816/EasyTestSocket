using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace EasyTestSocket.Network;

/// <summary>
/// Socket消息发送者
/// </summary>
public sealed class SocketSender : SocketAwaitableEventArgs
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="scheduler"></param>
    public SocketSender(PipeScheduler scheduler) : base(scheduler)
    {
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="socket"></param>
    /// <param name="buffers"></param>
    /// <returns></returns>
    public ValueTask<SocketOperationResult> SendAsync(Socket socket, in ReadOnlySequence<byte> buffers)
    {
        if (buffers.IsSingleSegment)
        {
            return SendAsync(socket, buffers.First);
        }

        SetBufferList(buffers);

        if (socket.SendAsync(this))
        {
            return new ValueTask<SocketOperationResult>(this, 0);
        }

        var bytesTransferred = BytesTransferred;
        var error = SocketError;

        return error == SocketError.Success
            ? new ValueTask<SocketOperationResult>(new SocketOperationResult(bytesTransferred))
            : new ValueTask<SocketOperationResult>(new SocketOperationResult(CreateException(error)));
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="socket"></param>
    /// <param name="buffers"></param>
    /// <returns></returns>
    public ValueTask<SocketOperationResult> SendAsync(Socket socket, in List<ArraySegment<byte>> buffers)
    {
        SetBufferList(buffers);

        if (socket.SendAsync(this))
        {
            return new ValueTask<SocketOperationResult>(this, 0);
        }

        var bytesTransferred = BytesTransferred;
        var error = SocketError;

        return error == SocketError.Success
            ? new ValueTask<SocketOperationResult>(new SocketOperationResult(bytesTransferred))
            : new ValueTask<SocketOperationResult>(new SocketOperationResult(CreateException(error)));
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="socket"></param>
    /// <param name="buffers"></param>
    /// <returns></returns>
    public ValueTask<SocketOperationResult> SendToAsync(Socket socket, in ReadOnlySequence<byte> buffers)
    {
        if (buffers.IsSingleSegment)
        {
            return SendToAsync(socket, buffers.First);
        }

        SetBufferList(buffers);

        if (socket.SendToAsync(this))
        {
            return new ValueTask<SocketOperationResult>(this, 0);
        }

        var bytesTransferred = BytesTransferred;
        var error = SocketError;

        return error == SocketError.Success
            ? new ValueTask<SocketOperationResult>(new SocketOperationResult(bytesTransferred))
            : new ValueTask<SocketOperationResult>(new SocketOperationResult(CreateException(error)));
    }

    public void Reset()
    {
        // We clear the buffer and buffer list before we put it back into the pool
        // it's a small performance hit but it removes the confusion when looking at dumps to see this still
        // holds onto the buffer when it's back in the pool
        if (BufferList != null)
        {
            BufferList = null;
        }
        else
        {
            SetBuffer(null, 0, 0);
        }
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="socket"></param>
    /// <param name="memory"></param>
    /// <returns></returns>
    public ValueTask<SocketOperationResult> SendAsync(Socket socket, ReadOnlyMemory<byte> memory)
    {
        SetBuffer(MemoryMarshal.AsMemory(memory));

        if (socket.SendAsync(this))
        {
            return new ValueTask<SocketOperationResult>(this, 0);
        }

        var bytesTransferred = BytesTransferred;
        var error = SocketError;

        return error == SocketError.Success
            ? new ValueTask<SocketOperationResult>(new SocketOperationResult(bytesTransferred))
            : new ValueTask<SocketOperationResult>(new SocketOperationResult(CreateException(error)));
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="socket"></param>
    /// <param name="memory"></param>
    /// <returns></returns>
    public ValueTask<SocketOperationResult> SendToAsync(Socket socket, ReadOnlyMemory<byte> memory)
    {
        SetBuffer(MemoryMarshal.AsMemory(memory));

        if (socket.SendToAsync(this))
        {
            return new ValueTask<SocketOperationResult>(this, 0);
        }

        var bytesTransferred = BytesTransferred;
        var error = SocketError;

        return error == SocketError.Success
            ? new ValueTask<SocketOperationResult>(new SocketOperationResult(bytesTransferred))
            : new ValueTask<SocketOperationResult>(new SocketOperationResult(CreateException(error)));
    }

    private void SetBufferList(in ReadOnlySequence<byte> buffer)
    {
        Debug.Assert(!buffer.IsEmpty);
        Debug.Assert(!buffer.IsSingleSegment);

        List<ArraySegment<byte>> bufferList = new();
        foreach (var b in buffer)
        {
            if (!MemoryMarshal.TryGetArray(b, out var result))
            {
                throw new InvalidOperationException("Buffer backed by array was expected");
            }
            bufferList.Add(result);
        }

        // The act of setting this list, sets the buffers in the internal buffer list
        BufferList = bufferList;
    }

    private void SetBufferList(in List<ArraySegment<byte>> buffer)
    {
        // The act of setting this list, sets the buffers in the internal buffer list
        BufferList = buffer;
    }
}
