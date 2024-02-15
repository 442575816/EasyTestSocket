// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace EasyTestSocket.Log;

public interface ILogFactory
{
    /// <summary>
    /// 初始化日志工厂
    /// </summary>、
    void Setup(string? logConfigFilePath = null);

    /// <summary>
    /// 根据名称获取日志
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Logger GetLog(string name);

    /// <summary>
    /// 泛型获取日志
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Logger GetLog<T>();

    /// <summary>
    /// 根据类型获取日志
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    Logger GetLog(Type type);

    /// <summary>
    /// 获取默认Logger
    /// </summary>
    Logger? Default { get; }

    /// <summary>
    /// 获取错误日志
    /// </summary>
    Logger? Error { get; }

    /// <summary>
    /// 销毁日志工厂
    /// </summary>
    void Dispose();
}
