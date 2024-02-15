// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace EasyTestSocket.Log;

class NLogLogger : Logger
{
    public string Name { get; }

    /// <summary>
    /// serilog的logger
    /// </summary>
    private readonly NLog.Logger _log;

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
    internal NLogLogger(string name, NLog.Logger log, bool redirectError)
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

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Debug => _log.IsDebugEnabled,
            LogLevel.Info => _log.IsInfoEnabled,
            LogLevel.Warn => _log.IsWarnEnabled,
            LogLevel.Error => _log.IsErrorEnabled,
            LogLevel.Fatal => _log.IsFatalEnabled,
            _ => false
        };
    }

    public void Warn(string message)
    {
        if (_redirectError)
        {
             LogFactory.Error?.Warn(message);
        }
        else
        {
            _log.Warn(message);
        }
    }

    public void Warn<T>(string messageTemplate, T propertyValue)
    {
        if (_redirectError)
        {
            LogFactory.Error?.Warn(messageTemplate, propertyValue);
        }
        else
        {
            _log.Warn(messageTemplate, propertyValue);
        }
    }

    public void Warn<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
    {
        if (_redirectError)
        {
             LogFactory.Error?.Warn(messageTemplate, propertyValue0, propertyValue1);
        }
        else
        {
            _log.Warn(messageTemplate, propertyValue0, propertyValue1);
        }
    }

    public void Warn<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
    {
        if (_redirectError)
        {
             LogFactory.Error?.Warn(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
        else
        {
            _log.Warn(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
    }

    public void Warn(string messageTemplate, params object[] propertyValues)
    {
        if (_redirectError)
        {
             LogFactory.Error?.Warn(messageTemplate, propertyValues);
        }
        else
        {
            _log.Warn(messageTemplate, propertyValues);
        }
    }

    public void Warn(Exception exception, string message)
    {
        if (_redirectError)
        {
             LogFactory.Error?.Warn(exception, message);
        }
        else
        {
            _log.Warn(exception, message);
        }
    }

    public void Warn<T>(Exception exception, string messageTemplate, T propertyValue)
    {
        if (_redirectError)
        {
             LogFactory.Error?.Warn(exception, messageTemplate, propertyValue);
        }
        else
        {
            _log.Warn(exception, messageTemplate, propertyValue);
        }
    }

    public void Warn<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
    {
        if (_redirectError)
        {
             LogFactory.Error?.Warn(exception, messageTemplate, propertyValue0, propertyValue1);
        }
        else
        {
            _log.Warn(exception, messageTemplate, propertyValue0, propertyValue1);
        }
    }

    public void Warn<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
        T2 propertyValue2)
    {
        if (_redirectError)
        {
             LogFactory.Error?.Warn(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
        else
        {
            _log.Warn(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
    }

    public void Warn(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        if (_redirectError)
        {
             LogFactory.Error?.Warn(exception, messageTemplate, propertyValues);
        }
        else
        {
            _log.Warn(exception, messageTemplate, propertyValues);
        }
    }

    public void Error(string message)
    {
        if (_redirectError)
        {
             LogFactory.Error?.Error(message);
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
             LogFactory.Error?.Error(messageTemplate, propertyValue);
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
             LogFactory.Error?.Error(messageTemplate, propertyValue0, propertyValue1);
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
             LogFactory.Error?.Error(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
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
             LogFactory.Error?.Error(messageTemplate, propertyValues);
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
             LogFactory.Error?.Error(exception, message);
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
             LogFactory.Error?.Error(exception, messageTemplate, propertyValue);
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
             LogFactory.Error?.Error(exception, messageTemplate, propertyValue0, propertyValue1);
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
             LogFactory.Error?.Error(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
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
             LogFactory.Error?.Error(exception, messageTemplate, propertyValues);
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
             LogFactory.Error?.Fatal(message);
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
             LogFactory.Error?.Fatal(messageTemplate, propertyValue);
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
             LogFactory.Error?.Fatal(messageTemplate, propertyValue0, propertyValue1);
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
             LogFactory.Error?.Fatal(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
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
             LogFactory.Error?.Fatal(messageTemplate, propertyValues);
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
             LogFactory.Error?.Fatal(exception, message);
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
             LogFactory.Error?.Fatal(exception, messageTemplate, propertyValue);
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
             LogFactory.Error?.Fatal(exception, messageTemplate, propertyValue0, propertyValue1);
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
             LogFactory.Error?.Fatal(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
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
             LogFactory.Error?.Fatal(exception, messageTemplate, propertyValues);
        }
        else
        {
            _log.Fatal(exception, messageTemplate, propertyValues);
        }
    }

    public void Log(LogLevel logLevel, string message)
    {
        _log.Log(GetNLogLogLevel(logLevel), message);
    }

    public void Log<T>(LogLevel logLevel, string messageTemplate, T propertyValue)
    {
        _log.Log(GetNLogLogLevel(logLevel), messageTemplate, propertyValue);
    }

    public void Log<T0, T1>(LogLevel logLevel, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
    {
        _log.Log(GetNLogLogLevel(logLevel), messageTemplate, propertyValue0, propertyValue1);
    }

    public void Log<T0, T1, T2>(LogLevel logLevel, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
        T2 propertyValue2)
    {
        _log.Log(GetNLogLogLevel(logLevel), messageTemplate, propertyValue0, propertyValue1, propertyValue2);
    }

    public void Log(LogLevel logLevel, string messageTemplate, params object[] propertyValues)
    {
        _log.Log(GetNLogLogLevel(logLevel), messageTemplate, propertyValues);
    }

    public void Log(LogLevel logLevel, Exception exception)
    {
        _log.Log(GetNLogLogLevel(logLevel), exception, "");
    }

    public void Log(LogLevel logLevel, Exception exception, string message)
    {
        _log.Log(GetNLogLogLevel(logLevel), exception, message);
    }

    public void Log<T>(LogLevel logLevel, Exception exception, string messageTemplate, T propertyValue)
    {
        _log.Log(GetNLogLogLevel(logLevel), exception, messageTemplate, propertyValue);
    }

    public void Log<T0, T1>(LogLevel logLevel, Exception exception, string messageTemplate, T0 propertyValue0,
        T1 propertyValue1)
    {
        _log.Log(GetNLogLogLevel(logLevel), exception, messageTemplate, propertyValue0, propertyValue1);
    }

    public void Log<T0, T1, T2>(LogLevel logLevel, Exception exception, string messageTemplate, T0 propertyValue0,
        T1 propertyValue1, T2 propertyValue2)
    {
        _log.Log(GetNLogLogLevel(logLevel), exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
    }

    public void Log(LogLevel logLevel, Exception exception, string messageTemplate, params object[] propertyValues)
    {
        _log.Log(GetNLogLogLevel(logLevel), exception, messageTemplate, propertyValues);
    }

    public Exception GetOriginException(Exception e)
    {
        return e;
    }

    private NLog.LogLevel GetNLogLogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Debug => NLog.LogLevel.Debug,
            LogLevel.Info => NLog.LogLevel.Info,
            LogLevel.Warn => NLog.LogLevel.Warn,
            LogLevel.Error => NLog.LogLevel.Error,
            LogLevel.Fatal => NLog.LogLevel.Fatal,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }
}
