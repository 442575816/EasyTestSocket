﻿using System;
using System.Net.Sockets;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks.Sources;

namespace EasyTestSocket.Network;

/// <summary>
/// 异步化改造的SocketAsyncEventArgs
/// A slimmed down version of https://github.com/dotnet/runtime/blob/82ca681cbac89d813a3ce397e0c665e6c051ed67/src/libraries/System.Net.Sockets/src/System/Net/Sockets/Socket.Tasks.cs#L798 that
/// 1. Doesn't support any custom scheduling other than the PipeScheduler (no sync context, no task scheduler)
/// 2. Doesn't do ValueTask validation using the token
/// 3. Doesn't support usage outside of async/await (doesn't try to capture and restore the execution context)
/// 4. Doesn't use cancellation tokens
/// </summary>
public class SocketAwaitableEventArgs : SocketAsyncEventArgs, IValueTaskSource<SocketOperationResult>
{
    // 完成Action
    private static readonly Action<object?> _continuationCompleted = _ => { };

    // io scheduler
    private readonly PipeScheduler _ioScheduler;

    // 回调action
    private Action<object?>? _continuation;

    /// <summary>
    /// 构造函数
    /// </summary>
    public SocketAwaitableEventArgs(PipeScheduler ioScheduler)
        : base(unsafeSuppressExecutionContextFlow: true)
    {
        _ioScheduler = ioScheduler;
    }

    /// <summary>
    /// 获取结果，返回的是传输的字节数
    /// </summary>
    /// <returns></returns>
    public SocketOperationResult GetResult(short token)
    {
        _continuation = null;

        if (SocketError != SocketError.Success)
        {
            throw new SocketException((int)SocketError);
        }

        return new SocketOperationResult(BytesTransferred);
    }

    public ValueTaskSourceStatus GetStatus(short token)
    {
        return !ReferenceEquals(_continuation, _continuationCompleted) ? ValueTaskSourceStatus.Pending :
            SocketError == SocketError.Success ? ValueTaskSourceStatus.Succeeded :
            ValueTaskSourceStatus.Faulted;
    }

    protected override void OnCompleted(SocketAsyncEventArgs _)
    {
        var c = _continuation;

        if (c != null || (c = Interlocked.CompareExchange(ref _continuation, _continuationCompleted, null)) != null)
        {
            var continuationState = UserToken;
            UserToken = null;
            _continuation = _continuationCompleted; // in case someone's polling IsCompleted

            _ioScheduler.Schedule(c, continuationState);
        }
    }

    public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
    {
        UserToken = state;
        var prevContinuation = Interlocked.CompareExchange(ref _continuation, continuation, null);
        if (ReferenceEquals(prevContinuation, _continuationCompleted))
        {
            UserToken = null;
            ThreadPool.UnsafeQueueUserWorkItem(continuation, state, preferLocal: true);
        }
    }

    protected static SocketException CreateException(SocketError e)
    {
        return new SocketException((int)e);
    }
}
