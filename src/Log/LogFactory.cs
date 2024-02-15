using System;

namespace EasyTestSocket.Log;

public enum LogProvider
{
    Serilog,
    NLog
}

/// <summary>
/// Log类
/// </summary>
public sealed class LogFactory
{
    /// <summary>
    /// 默认日志提供者
    /// </summary>
    public static LogProvider Provitor { get; set; } = LogProvider.Serilog;

    /// <summary>
    /// 获取错误日志
    /// </summary>
    public static Logger? Error => _logFactory?.Error;

    /// <summary>
    /// 获取默认日志
    /// </summary>
    public static Logger? Default => _logFactory?.Default;

    /// <summary>
    /// 内部log工厂
    /// </summary>
    private static ILogFactory? _logFactory;

    /// <summary>
    /// 初始化Log
    /// </summary>
    public static void Setup(string? logConfigFilePath = null)
    {
        if (Provitor == LogProvider.Serilog)
        {
            _logFactory = new SeriLogFactory();
        }
        else
        {
            _logFactory = new NLogFactory();
        }
        _logFactory.Setup(logConfigFilePath);
    }

    public static Logger GetLog(string name)
    {
        return _logFactory!.GetLog(name);
    }

    public static Logger GetLog<T>()
    {
        return GetLog(typeof(T));
    }

    public static Logger GetLog(Type type)
    {
        return GetLog(type.FullName ?? "root");
    }

    public static void Dispose()
    {
        _logFactory!.Dispose();
    }
}
