using System;

namespace EasyTestSocket.Log;

public interface Logger
{
    /// <summary>
    /// 日志名称
    /// </summary>
    string Name { get; }


    bool IsVerboseEnabled() => IsEnabled(LogLevel.Verbose);
    void Verbose(string message) => Log(LogLevel.Verbose, message);

    void Verbose<T>(string messageTemplate, T propertyValue) =>
        Log(LogLevel.Verbose, messageTemplate, propertyValue);

    void Verbose<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        Log(LogLevel.Verbose, messageTemplate, propertyValue0, propertyValue1);

    void Verbose<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) =>
        Log(LogLevel.Verbose, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    void Verbose(string messageTemplate, params object[] propertyValues) =>
        Log(LogLevel.Verbose, messageTemplate, propertyValues);

    void Verbose(Exception exception) => Log(LogLevel.Verbose, exception);
    void Verbose(Exception exception, string message) => Log(LogLevel.Verbose, exception, message);

    void Verbose<T>(Exception exception, string messageTemplate, T propertyValue) =>
        Log(LogLevel.Verbose, exception, messageTemplate, propertyValue);

    void Verbose<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        Log(LogLevel.Verbose, exception, messageTemplate, propertyValue0, propertyValue1);

    void Verbose<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
        T2 propertyValue2) => Log(LogLevel.Verbose, exception, messageTemplate, propertyValue0, propertyValue1,
        propertyValue2);

    void Verbose(Exception exception, string messageTemplate, params object[] propertyValues) =>
        Log(LogLevel.Verbose, exception, messageTemplate, propertyValues);

    bool IsDebugEnabled() => IsEnabled(LogLevel.Debug);
    void Debug(string message) => Log(LogLevel.Debug, message);
    void Debug<T>(string messageTemplate, T propertyValue) => Log(LogLevel.Debug, messageTemplate, propertyValue);

    void Debug<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        Log(LogLevel.Debug, messageTemplate, propertyValue0, propertyValue1);

    void Debug<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) =>
        Log(LogLevel.Debug, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    void Debug(string messageTemplate, params object[] propertyValues) =>
        Log(LogLevel.Debug, messageTemplate, propertyValues);

    void Debug(Exception exception) => Log(LogLevel.Debug, exception);
    void Debug(Exception exception, string message) => Log(LogLevel.Debug, exception, message);

    void Debug<T>(Exception exception, string messageTemplate, T propertyValue) =>
        Log(LogLevel.Debug, exception, messageTemplate, propertyValue);

    void Debug<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        Log(LogLevel.Debug, exception, messageTemplate, propertyValue0, propertyValue1);

    void Debug<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
        T2 propertyValue2) => Log(LogLevel.Debug, exception, messageTemplate, propertyValue0, propertyValue1,
        propertyValue2);

    void Debug(Exception exception, string messageTemplate, params object[] propertyValues) =>
        Log(LogLevel.Debug, exception, messageTemplate, propertyValues);

    bool IsInfoEnabled() => IsEnabled(LogLevel.Info);
    void Info(string message) => Log(LogLevel.Info, message);
    void Info<T>(string messageTemplate, T propertyValue) => Log(LogLevel.Info, messageTemplate, propertyValue);

    void Info<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        Log(LogLevel.Info, messageTemplate, propertyValue0, propertyValue1);

    void Info<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) =>
        Log(LogLevel.Info, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    void Info(string messageTemplate, params object[] propertyValues) =>
        Log(LogLevel.Info, messageTemplate, propertyValues);

    void Info(Exception exception) => Log(LogLevel.Info, exception);
    void Info(Exception exception, string message) => Log(LogLevel.Info, exception, message);

    void Info<T>(Exception exception, string messageTemplate, T propertyValue) =>
        Log(LogLevel.Info, exception, messageTemplate, propertyValue);

    void Info<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        Log(LogLevel.Info, exception, messageTemplate, propertyValue0, propertyValue1);

    void Info<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
        T2 propertyValue2) => Log(LogLevel.Info, exception, messageTemplate, propertyValue0, propertyValue1,
        propertyValue2);

    void Info(Exception exception, string messageTemplate, params object[] propertyValues) =>
        Log(LogLevel.Info, exception, messageTemplate, propertyValues);

    bool IsWarnEnabled() => IsEnabled(LogLevel.Warn);
    void Warn(string message) => Log(LogLevel.Warn, message);
    void Warn<T>(string messageTemplate, T propertyValue) => Log(LogLevel.Warn, messageTemplate, propertyValue);

    void Warn<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        Log(LogLevel.Warn, messageTemplate, propertyValue0, propertyValue1);

    void Warn<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) =>
        Log(LogLevel.Warn, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    void Warn(string messageTemplate, params object[] propertyValues) =>
        Log(LogLevel.Warn, messageTemplate, propertyValues);

    void Warn(Exception exception) => Log(LogLevel.Warn, exception);
    void Warn(Exception exception, string message) => Log(LogLevel.Warn, exception, message);

    void Warn<T>(Exception exception, string messageTemplate, T propertyValue) =>
        Log(LogLevel.Warn, exception, messageTemplate, propertyValue);

    void Warn<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        Log(LogLevel.Warn, exception, messageTemplate, propertyValue0, propertyValue1);

    void Warn<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
        T2 propertyValue2) => Log(LogLevel.Warn, exception, messageTemplate, propertyValue0, propertyValue1,
        propertyValue2);

    void Warn(Exception exception, string messageTemplate, params object[] propertyValues) =>
        Log(LogLevel.Warn, exception, messageTemplate, propertyValues);

    bool IsErrorEnabled() => IsEnabled(LogLevel.Error);
    void Error(string message) => Log(LogLevel.Error, message);
    void Error<T>(string messageTemplate, T propertyValue) => Log(LogLevel.Error, messageTemplate, propertyValue);

    void Error<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        Log(LogLevel.Error, messageTemplate, propertyValue0, propertyValue1);

    void Error<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) =>
        Log(LogLevel.Error, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    void Error(string messageTemplate, params object[] propertyValues) =>
        Log(LogLevel.Error, messageTemplate, propertyValues);

    void Error(Exception exception) => Log(LogLevel.Error, exception);
    void Error(Exception exception, string message) => Log(LogLevel.Error, exception, message);

    void Error<T>(Exception exception, string messageTemplate, T propertyValue) =>
        Log(LogLevel.Error, exception, messageTemplate, propertyValue);

    void Error<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        Log(LogLevel.Error, exception, messageTemplate, propertyValue0, propertyValue1);

    void Error<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
        T2 propertyValue2) => Log(LogLevel.Error, exception, messageTemplate, propertyValue0, propertyValue1,
        propertyValue2);

    void Error(Exception exception, string messageTemplate, params object[] propertyValues) =>
        Log(LogLevel.Error, exception, messageTemplate, propertyValues);

    bool IsFatalEnabled() => IsEnabled(LogLevel.Fatal);
    void Fatal(string message) => Log(LogLevel.Fatal, message);
    void Fatal<T>(string messageTemplate, T propertyValue) => Log(LogLevel.Fatal, messageTemplate, propertyValue);

    void Fatal<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        Log(LogLevel.Fatal, messageTemplate, propertyValue0, propertyValue1);

    void Fatal<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) =>
        Log(LogLevel.Fatal, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    void Fatal(string messageTemplate, params object[] propertyValues) =>
        Log(LogLevel.Fatal, messageTemplate, propertyValues);

    void Fatal(Exception exception) => Log(LogLevel.Fatal, exception);
    void Fatal(Exception exception, string message) => Log(LogLevel.Fatal, exception, message);

    void Fatal<T>(Exception exception, string messageTemplate, T propertyValue) =>
        Log(LogLevel.Fatal, exception, messageTemplate, propertyValue);

    void Fatal<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        Log(LogLevel.Fatal, exception, messageTemplate, propertyValue0, propertyValue1);

    void Fatal<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
        T2 propertyValue2) => Log(LogLevel.Fatal, exception, messageTemplate, propertyValue0, propertyValue1,
        propertyValue2);

    void Fatal(Exception exception, string messageTemplate, params object[] propertyValues) =>
        Log(LogLevel.Fatal, exception, messageTemplate, propertyValues);

    /// <summary>
    /// 是否启用指定日志级别
    /// </summary>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    bool IsEnabled(LogLevel logLevel);

    /// <summary>
    /// 输出指定日志级别
    /// </summary>
    void Log(LogLevel logLevel, string message);

    void Log<T>(LogLevel logLevel, string messageTemplate, T propertyValue);
    void Log<T0, T1>(LogLevel logLevel, string messageTemplate, T0 propertyValue0, T1 propertyValue1);

    void Log<T0, T1, T2>(LogLevel logLevel, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
        T2 propertyValue2);

    void Log(LogLevel logLevel, string messageTemplate, params object[] propertyValues);
    void Log(LogLevel logLevel, Exception exception);
    void Log(LogLevel logLevel, Exception exception, string message);
    void Log<T>(LogLevel logLevel, Exception exception, string messageTemplate, T propertyValue);

    void Log<T0, T1>(LogLevel logLevel, Exception exception, string messageTemplate, T0 propertyValue0,
        T1 propertyValue1);

    void Log<T0, T1, T2>(LogLevel logLevel, Exception exception, string messageTemplate, T0 propertyValue0,
        T1 propertyValue1,
        T2 propertyValue2);

    void Log(LogLevel logLevel, Exception exception, string messageTemplate, params object[] propertyValues);

    /// <summary>
    /// 获取原始异常，去掉包裹异常
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    Exception GetOriginException(Exception e);
}