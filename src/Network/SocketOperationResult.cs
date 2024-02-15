// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;

namespace EasyTestSocket.Network;

public readonly struct SocketOperationResult
{
    public readonly SocketException? SocketError;

    public readonly int BytesTransferred;

    [MemberNotNullWhen(true, nameof(SocketError))]
    public readonly bool HasError => SocketError != null;

    public SocketOperationResult(int bytesTransferred)
    {
        SocketError = null;
        BytesTransferred = bytesTransferred;
    }

    public SocketOperationResult(SocketException exception)
    {
        SocketError = exception;
        BytesTransferred = 0;
    }
}
