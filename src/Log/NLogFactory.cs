using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace EasyTestSocket.Log;

internal class NLogConfig
{
    public required string Name { get; init; }
    public required Target Target { get; init; }
    public required bool RedirectError { get; init; }
    public required NLog.LogLevel Level { get; init; }
    public required bool LogConsole { get; init; }
    public required bool IsConsoleTarget { get; init; }
}

/// <summary>
/// Log类
/// </summary>
public sealed class NLogFactory : ILogFactory
{
    /// <summary>
    /// Log字典
    /// </summary>
    private readonly Dictionary<string, Logger> _logDict = new ();

    /// <summary>
    /// 默认Logger
    /// </summary>
    private Logger? _defaultLog;

    /// <summary>
    /// 错误日志
    /// </summary>
    private Logger? _errorLog;

    /// <summary>
    /// 获取默认Logger
    /// </summary>
    public Logger? Default => _defaultLog;

    /// <summary>
    /// 获取错误日志
    /// </summary>
    public Logger? Error => _errorLog;

    /// <summary>
    /// 初始化Log
    /// </summary>
    public void Setup(string? logConfigFilePath = null)
    {
        var config = new LoggingConfiguration();
        LogManager.Configuration = config;

        var targetMap = new Dictionary<string, NLogConfig>();
        NLogConfig? consoleLogConfig = null;
        if (File.Exists(logConfigFilePath))
        {
            using var fs = new FileStream(logConfigFilePath, FileMode.Open);
            var doc = JsonDocument.Parse(fs);
            var elements = doc.RootElement.GetProperty("logs").EnumerateArray();
            foreach (var log in elements)
            {
                var name = log.GetProperty("name").GetString() ?? throw new ArgumentNullException("name can't be null");
                var target = log.GetProperty("target").GetString();
                if (target == "file")
                {
                    targetMap[name] = CreateFileLog(name, log);
                }
                else
                {
                    targetMap[name] = CreateConsoleLog(name, log);
                    consoleLogConfig = targetMap[name];
                }
            }

            // 优先创建控制台日志
            if (consoleLogConfig != null)
            {
                var name = consoleLogConfig.Name;
                var rule = new LoggingRule(name, consoleLogConfig.Level, consoleLogConfig.Target);
                config.LoggingRules.Add(rule);
                var logger = new NLogLogger(name, LogManager.GetLogger(name), consoleLogConfig.RedirectError);
                _logDict[name] = logger;
            }

            // 创建日志
            foreach (var (name, logConfig) in targetMap)
            {
                if (logConfig == consoleLogConfig)
                {
                    continue;
                }

                var rule = new LoggingRule(name, logConfig.Level, logConfig.Target);
                config.LoggingRules.Add(rule);
                var logger = new NLogLogger(name, LogManager.GetLogger(name), logConfig.RedirectError);
                _logDict[name] = logger;

                if (logConfig.LogConsole)
                {
                    consoleLogConfig ??= CreateInternalConsoleLog(config);
                    config.LoggingRules.Add(new LoggingRule(name, logConfig.Level, consoleLogConfig.Target));
                }

            }

            var mappings = doc.RootElement.GetProperty("mappings").EnumerateObject();
            foreach (var mapping in mappings)
            {
                var key = mapping.Name;
                var value = mapping.Value.GetString() ?? throw new ArgumentNullException("mappings value can't be null");

                if (_logDict.ContainsKey(value))
                {
                    _logDict[key] = _logDict[value];
                }
            }
        }


        if (!_logDict.ContainsKey("root"))
        {
            // 不包含默认Log
            consoleLogConfig ??= CreateInternalConsoleLog(config);
            config.LoggingRules.Add(new LoggingRule("root", NLog.LogLevel.Info, consoleLogConfig.Target));

            _defaultLog = new NLogLogger("root", LogManager.GetLogger("root"), true);
            _logDict["root"] = _defaultLog;
        }
        else
        {
            _defaultLog = _logDict["root"];
        }

        if (!_logDict.ContainsKey("error"))
        {
            // 不包含默认ErrorLog
            consoleLogConfig ??= CreateInternalConsoleLog(config);
            config.LoggingRules.Add(new LoggingRule("error", NLog.LogLevel.Info, consoleLogConfig.Target));

            _errorLog = new NLogLogger("error", LogManager.GetLogger("error"), false);
            _logDict["error"] = _errorLog;
        }
        else
        {
            _errorLog = _logDict["error"];
            (_errorLog as NLogLogger)?.SetRedirectError(false);
        }
        LogManager.ReconfigExistingLoggers();

        // 添加未补货异常
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception e)
            {
                _errorLog.Error(e, "uncatch exception");
            }
        };
        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            _errorLog.Error("uncatch task exception");
            var index = 0;
            foreach (var e in args.Exception.InnerExceptions)
            {
                _errorLog.Error(e, $"exception#{index++}");
            }
        };
    }

    public Logger GetLog(string name)
    {
        return _logDict.TryGetValue(name, out var log) ? log : _logDict["root"];
    }

    public Logger GetLog<T>()
    {
        return GetLog(typeof(T));
    }

    public Logger GetLog(Type type)
    {
        return GetLog(type.FullName ?? "root");
    }

    public void Dispose()
    {
        LogManager.Shutdown();
    }

    private NLogConfig CreateConsoleLog(string name, JsonElement json)
    {
        var level = "info";
        if (json.TryGetProperty("level", out var prop))
        {
            level = prop.GetString();
        }

        var redirectError = true;
        if (json.TryGetProperty("redirectError", out prop))
        {
            redirectError = prop.GetBoolean();
        }

        var format = json.GetProperty("format").GetString() ?? throw new ArgumentNullException("format can't be null");

        // 设置日志级别
        level ??= "info";
        var target = new ConsoleTarget();
        target.Layout = format;

        return new NLogConfig
        {
            Name = name, Level = GetNLogLogLevel(level), Target = target, RedirectError = redirectError, LogConsole = true, IsConsoleTarget = true
        };
    }

    private NLogConfig CreateFileLog(string name, JsonElement json)
    {
        var rollingConfig = json.GetProperty("rolling");
        var level = "info";
        if (json.TryGetProperty("level", out var prop))
        {
            level = prop.GetString();
        }

        var redirectError = true;
        if (json.TryGetProperty("redirectError", out prop))
        {
            redirectError = prop.GetBoolean();
        }

        var async = false;
        if (json.TryGetProperty("async", out prop))
        {
            async = prop.GetBoolean();
        }

        // var bufferSize = 500;
        // if (json.TryGetProperty("bufferSize", out prop))
        // {
        //     bufferSize = prop.GetInt32();
        // }

        var format = json.GetProperty("format").GetString() ?? throw new ArgumentNullException("format can't be null");

        var fileTarget = new FileTarget();
        fileTarget.Layout = format;

        // 设置rolling策略
        var rollingType = rollingConfig.GetProperty("type").GetString();
        if (rollingType == "time")
        {
            var path = rollingConfig.GetProperty("path").GetString() ?? throw new ArgumentNullException("path can't be null");

            fileTarget.FileName = path;
            fileTarget.ConcurrentWrites = false;
            fileTarget.KeepFileOpen = true;
            fileTarget.Encoding = Encoding.UTF8;

        }
        else if (rollingType == "size")
        {
            var limitSize = rollingConfig.GetProperty("size").GetString() ?? throw new ArgumentNullException("size can't be null");
            var path = rollingConfig.GetProperty("path").GetString() ?? throw new ArgumentNullException("format can't be null");
            var maxCount = rollingConfig.GetProperty("maxCount").GetInt32();

            fileTarget.FileName = path;
            fileTarget.ArchiveAboveSize = GetFileLimitSize(limitSize);
            fileTarget.ArchiveNumbering = ArchiveNumberingMode.Rolling;
            fileTarget.ArchiveFileName = Path.Combine(Path.GetDirectoryName(path)!, Path.GetFileNameWithoutExtension(path) + "_{#}" + Path.GetExtension(path));
            fileTarget.MaxArchiveFiles = maxCount;
            fileTarget.ConcurrentWrites = false;
            fileTarget.KeepFileOpen = true;
        }

        Target target = fileTarget;
        if (async)
        {
            var asyncTargetWrapper = new AsyncTargetWrapper(target);
            target = asyncTargetWrapper;
            asyncTargetWrapper.TimeToSleepBetweenBatches = 0;
        }

        // 是否打印到控制台
        var console = true;
        if (json.TryGetProperty("console", out prop))
        {
            console = prop.GetBoolean();
        }

        // 设置日志级别
        level ??= "info";
        return new NLogConfig
        {
            Name = name, Level = GetNLogLogLevel(level), RedirectError = redirectError, Target = target, LogConsole = console, IsConsoleTarget = false
        };
    }

    private long GetFileLimitSize(string limitSize)
    {
        if (limitSize.EndsWith("m"))
        {
            var num = Convert.ToInt32(limitSize.Substring(0, limitSize.Length - 1));
            return num * 1024 * 1024;
        }
        else if (limitSize.EndsWith("g"))
        {
            var num = Convert.ToInt32(limitSize.Substring(0, limitSize.Length - 1));
            return num * 1024 * 1024 * 1024;
        }
        else if (limitSize.EndsWith("k"))
        {
            var num = Convert.ToInt32(limitSize.Substring(0, limitSize.Length - 1));
            return num * 1024;
        }
        else
        {
            var num = Convert.ToInt32(limitSize);
            return num;
        }
    }

    /// <summary>
    /// 创建内部控制台日志
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    private NLogConfig CreateInternalConsoleLog(LoggingConfiguration config)
    {
        var target = new ConsoleTarget();
        target.Layout = "${longdate}#[${level}]#${message}${exception}";
        var consoleLoggerName = "_console_log_";
        config.LoggingRules.Add(new LoggingRule(consoleLoggerName, NLog.LogLevel.Info, target));

        var consoleLogger = new NLogLogger(consoleLoggerName, LogManager.GetLogger(consoleLoggerName), false);
        _logDict[consoleLoggerName] = consoleLogger;

        return new NLogConfig
        {
            Name = consoleLoggerName,
            Level = NLog.LogLevel.Info,
            Target = target,
            RedirectError = false,
            LogConsole = true,
            IsConsoleTarget = true
        };
    }

    /// <summary>
    /// 设置Log日志级别
    /// </summary>
    /// <param name="config"></param>
    /// <param name="level"></param>
    private NLog.LogLevel GetNLogLogLevel(string level)
    {
        switch (level)
        {
            case "info":
                return NLog.LogLevel.Info;
            case "debug":
                return NLog.LogLevel.Debug;
            case "warn":
                return NLog.LogLevel.Warn;
            case "error":
                return NLog.LogLevel.Error;
            case "verbose":
                return NLog.LogLevel.Trace;
            case "fatal":
                return NLog.LogLevel.Fatal;
            default:
                return NLog.LogLevel.Info;
        }
    }


}
