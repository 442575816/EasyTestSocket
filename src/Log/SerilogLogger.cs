using System;
using Serilog;
using Serilog.Events;

namespace EasyTestSocket.Log;

class SerilogLogger : Logger
{
    public string Name { get; }

    /// <summary>
    /// serilog的logger
    /// </summary>
    private readonly ILogger _log;

    /// <summary>
    /// 是否重定向错误日志
    /// </summary>
    private bool _redirectError;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="name"></param>
    /// <param name="log"></param>
    /// <param name="redirectError"></param>
    internal SerilogLogger(string name, ILogger log, bool redirectError)
    {
        Name = name;
        _log = log;
        _redirectError = redirectError;
    }

    /// <summary>
    /// 设置是否重定向错误日志
    /// </summary>
    /// <param name="redirectError"></param>
    /// <returns></returns>
    public void SetRedirectError(bool redirectError)
    {
        _redirectError = redirectError;
    }

    public void Warn(string message)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Warn(message);
        }
        else
        {
            _log.Warning(message);
        }
    }

    public void Warn<T>(string messageTemplate, T propertyValue)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Warn(messageTemplate, propertyValue);
        }
        else
        {
            _log.Warning(messageTemplate, propertyValue);
        }
    }

    public void Warn<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Warn(messageTemplate, propertyValue0, propertyValue1);
        }
        else
        {
            _log.Warning(messageTemplate, propertyValue0, propertyValue1);
        }
    }

    public void Warn<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Warn(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
        else
        {
            _log.Warning(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
    }

    public void Warn(string messageTemplate, params object[] propertyValues)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Warn(messageTemplate, propertyValues);
        }
        else
        {
            _log.Warning(messageTemplate, propertyValues);
        }
    }

    public void Warn(Exception exception, string message)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Warn(exception, message);
        }
        else
        {
            _log.Warning(exception, message);
        }
    }

    public void Warn<T>(Exception exception, string messageTemplate, T propertyValue)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Warn(exception, messageTemplate, propertyValue);
        }
        else
        {
            _log.Warning(exception, messageTemplate, propertyValue);
        }
    }

    public void Warn<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Warn(exception, messageTemplate, propertyValue0, propertyValue1);
        }
        else
        {
            _log.Warning(exception, messageTemplate, propertyValue0, propertyValue1);
        }
    }

    public void Warn<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
        T2 propertyValue2)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Warn(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
        else
        {
            _log.Warning(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
    }

    public void Warn(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Warn(exception, messageTemplate, propertyValues);
        }
        else
        {
            _log.Warning(exception, messageTemplate, propertyValues);
        }
    }

    public void Error(string message)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Error(message);
        }
        else
        {
            _log.Error(message);
        }
    }

    public void Error<T>(string messageTemplate, T propertyValue)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Error(messageTemplate, propertyValue);
        }
        else
        {
            _log.Error(messageTemplate, propertyValue);
        }
    }

    public void Error<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Error(messageTemplate, propertyValue0, propertyValue1);
        }
        else
        {
            _log.Error(messageTemplate, propertyValue0, propertyValue1);
        }
    }

    public void Error<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Error(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
        else
        {
            _log.Error(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
    }

    public void Error(string messageTemplate, params object[] propertyValues)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Error(messageTemplate, propertyValues);
        }
        else
        {
            _log.Error(messageTemplate, propertyValues);
        }
    }

    public void Error(Exception exception, string message)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Error(exception, message);
        }
        else
        {
            _log.Error(exception, message);
        }
    }

    public void Error<T>(Exception exception, string messageTemplate, T propertyValue)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Error(exception, messageTemplate, propertyValue);
        }
        else
        {
            _log.Error(exception, messageTemplate, propertyValue);
        }
    }

    public void Error<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Error(exception, messageTemplate, propertyValue0, propertyValue1);
        }
        else
        {
            _log.Error(exception, messageTemplate, propertyValue0, propertyValue1);
        }
    }

    public void Error<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
        T2 propertyValue2)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Error(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
        else
        {
            _log.Error(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
    }

    public void Error(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Error(exception, messageTemplate, propertyValues);
        }
        else
        {
            _log.Error(exception, messageTemplate, propertyValues);
        }
    }

    public void Fatal(string message)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Fatal(message);
        }
        else
        {
            _log.Fatal(message);
        }
    }

    public void Fatal<T>(string messageTemplate, T propertyValue)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Fatal(messageTemplate, propertyValue);
        }
        else
        {
            _log.Fatal(messageTemplate, propertyValue);
        }
    }

    public void Fatal<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Fatal(messageTemplate, propertyValue0, propertyValue1);
        }
        else
        {
            _log.Fatal(messageTemplate, propertyValue0, propertyValue1);
        }
    }

    public void Fatal<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Fatal(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
        else
        {
            _log.Fatal(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
    }

    public void Fatal(string messageTemplate, params object[] propertyValues)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Fatal(messageTemplate, propertyValues);
        }
        else
        {
            _log.Fatal(messageTemplate, propertyValues);
        }
    }

    public void Fatal(Exception exception, string message)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Fatal(exception, message);
        }
        else
        {
            _log.Fatal(exception, message);
        }
    }

    public void Fatal<T>(Exception exception, string messageTemplate, T propertyValue)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Fatal(exception, messageTemplate, propertyValue);
        }
        else
        {
            _log.Fatal(exception, messageTemplate, propertyValue);
        }
    }

    public void Fatal<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Fatal(exception, messageTemplate, propertyValue0, propertyValue1);
        }
        else
        {
            _log.Fatal(exception, messageTemplate, propertyValue0, propertyValue1);
        }
    }

    public void Fatal<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
        T2 propertyValue2)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Fatal(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
        else
        {
            _log.Fatal(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
    }

    public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        if (_redirectError)
        {
            LogFactory.Error!.Fatal(exception, messageTemplate, propertyValues);
        }
        else
        {
            _log.Fatal(exception, messageTemplate, propertyValues);
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _log.IsEnabled((LogEventLevel) logLevel);
    }

    public void Log(LogLevel logLevel, string message)
    {
        _log.Write((LogEventLevel) logLevel, message);
    }

    public void Log<T>(LogLevel logLevel, string messageTemplate, T propertyValue)
    {
        _log.Write((LogEventLevel) logLevel, messageTemplate, propertyValue);
    }

    public void Log<T0, T1>(LogLevel logLevel, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
    {
        _log.Write((LogEventLevel) logLevel, messageTemplate, propertyValue0, propertyValue1);
    }

    public void Log<T0, T1, T2>(LogLevel logLevel, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
        T2 propertyValue2)
    {
        _log.Write((LogEventLevel) logLevel, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
    }

    public void Log(LogLevel logLevel, string messageTemplate, params object[] propertyValues)
    {
        _log.Write((LogEventLevel) logLevel, messageTemplate, propertyValues);
    }

    public void Log(LogLevel logLevel, Exception exception)
    {
        _log.Write((LogEventLevel) logLevel, exception, "");
    }

    public void Log(LogLevel logLevel, Exception exception, string message)
    {
        _log.Write((LogEventLevel) logLevel, exception, message);
    }

    public void Log<T>(LogLevel logLevel, Exception exception, string messageTemplate, T propertyValue)
    {
        _log.Write((LogEventLevel) logLevel, exception, messageTemplate, propertyValue);
    }

    public void Log<T0, T1>(LogLevel logLevel, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
    {
        _log.Write((LogEventLevel) logLevel, exception, messageTemplate, propertyValue0, propertyValue1);
    }

    public void Log<T0, T1, T2>(LogLevel logLevel, Exception exception, string messageTemplate, T0 propertyValue0,
        T1 propertyValue1, T2 propertyValue2)
    {
        _log.Write((LogEventLevel) logLevel, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
    }

    public void Log(LogLevel logLevel, Exception exception, string messageTemplate, params object[] propertyValues)
    {
        _log.Write((LogEventLevel) logLevel, exception, messageTemplate, propertyValues);
    }

    public Exception GetOriginException(Exception e)
    {
        return e;
    }
}
