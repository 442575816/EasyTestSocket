using System;

namespace EasyTestSocket.Log;

public class BaseLogger : Logger
{
    /// <summary>
    /// 内部Log
    /// </summary>
    private readonly Logger _log;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="log"></param>
    public BaseLogger(Logger log)
    {
        _log = log;
    }

    public string Name => _log.Name;
    public bool IsEnabled(LogLevel logLevel)
    {
        return _log.IsEnabled(logLevel);
    }

    public void Log(LogLevel logLevel, string message)
    {
        _log.Log(logLevel, message);
    }

    public void Log<T>(LogLevel logLevel, string messageTemplate, T propertyValue)
    {
        _log.Log(logLevel, messageTemplate, propertyValue);
    }

    public void Log<T0, T1>(LogLevel logLevel, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
    {
        _log.Log(logLevel, messageTemplate, propertyValue0, propertyValue1);
    }

    public void Log<T0, T1, T2>(LogLevel logLevel, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
        T2 propertyValue2)
    {
        _log.Log(logLevel, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
    }

    public void Log(LogLevel logLevel, string messageTemplate, params object[] propertyValues)
    {
        _log.Log(logLevel, messageTemplate, propertyValues);
    }

    public void Log(LogLevel logLevel, Exception exception)
    {
        _log.Log(logLevel, exception);
    }

    public void Log(LogLevel logLevel, Exception exception, string message)
    {
        _log.Log(logLevel, exception, message);
    }

    public void Log<T>(LogLevel logLevel, Exception exception, string messageTemplate, T propertyValue)
    {
        _log.Log(logLevel, exception, messageTemplate, propertyValue);
    }

    public void Log<T0, T1>(LogLevel logLevel, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
    {
        _log.Log(logLevel, exception, messageTemplate, propertyValue0, propertyValue1);
    }

    public void Log<T0, T1, T2>(LogLevel logLevel, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
    {
        _log.Log(logLevel, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
    }

    public void Log(LogLevel logLevel, Exception exception, string messageTemplate, params object[] propertyValues)
    {
        _log.Log(logLevel, exception, messageTemplate, propertyValues);
    }

    public Exception GetOriginException(Exception e)
    {
        return e;
    }
}