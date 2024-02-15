using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace EasyTestSocket.Log;

/// <summary>
/// Log类
/// </summary>
public sealed class SeriLogFactory : ILogFactory
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
                    CreateFileLog(name, log);
                }
                else
                {
                    CreateConsoleLog(name, log);
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
            var config = new LoggerConfiguration().Enrich.FromLogContext();
            config.WriteTo.Console(outputTemplate:"{Timestamp:yyyy-MM-dd HH:mm:ss.fff}#[{Level:u4}]#{Message:lj}{NewLine}{Exception}").MinimumLevel.Information();

            _defaultLog = new SerilogLogger("root", config.CreateLogger(), true);
            _logDict["root"] = _defaultLog;
        }
        else
        {
            _defaultLog = _logDict["root"];
        }

        if (!_logDict.ContainsKey("error"))
        {
            // 不包含默认ErrorLog
            var config = new LoggerConfiguration().Enrich.FromLogContext();
            config.WriteTo.Console(outputTemplate:"{Timestamp:yyyy-MM-dd HH:mm:ss.fff}#[{Level:u4}]#{Message:lj}{NewLine}{Exception}").MinimumLevel.Information();

            _errorLog = new SerilogLogger("error", config.CreateLogger(), false);
            _logDict["error"] = _errorLog;
        }
        else
        {
            _errorLog = _logDict["error"];
            (_errorLog as SerilogLogger)?.SetRedirectError(false);
        }

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
        Serilog.Log.CloseAndFlush();
    }

    private void CreateConsoleLog(string name, JsonElement json)
    {
        var config = new LoggerConfiguration().Enrich.FromLogContext();
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
        SetMinimumLevel(config, level ?? "info");

        config.WriteTo.Console(outputTemplate: format, theme: ConsoleTheme.None);

        var _logger = config.CreateLogger();
        _logDict[name] = new SerilogLogger(name, _logger, redirectError);
    }

    private void CreateFileLog(string name, JsonElement json)
    {
        var config = new LoggerConfiguration().Enrich.FromLogContext();

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

        var bufferSize = 500;
        if (json.TryGetProperty("bufferSize", out prop))
        {
            bufferSize = prop.GetInt32();
        }

        var format = json.GetProperty("format").GetString() ?? throw new ArgumentNullException("format can't be null");

        // 设置日志级别
        SetMinimumLevel(config, level ?? "info");

        // 设置rolling策略
        var rollingType = rollingConfig.GetProperty("type").GetString();
        if (rollingType == "time")
        {
            var interval = rollingConfig.GetProperty("interval").GetString();
            var path = rollingConfig.GetProperty("path").GetString() ?? throw new ArgumentNullException("path can't be null");

            var rollingInterval = GetRollingInterval(interval);

            if (async)
            {
                config.WriteTo.Async(f => f.File(path, rollingInterval: rollingInterval,
                        flushToDiskInterval: TimeSpan.FromTicks(100), outputTemplate: format),
                    bufferSize: bufferSize);
            }
            else
            {
                config.WriteTo.File(path, rollingInterval: rollingInterval,
                    flushToDiskInterval: TimeSpan.FromTicks(100), outputTemplate: format);
            }

        }
        else if (rollingType == "size")
        {
            var limitSize = rollingConfig.GetProperty("size").GetString() ?? throw new ArgumentNullException("size can't be null");
            var path = rollingConfig.GetProperty("path").GetString() ?? throw new ArgumentNullException("format can't be null");
            var maxCount = rollingConfig.GetProperty("maxCount").GetInt32();

            if (async)
            {
                config.WriteTo.Async(
                    f => f.File(path, rollOnFileSizeLimit: true, fileSizeLimitBytes: GetFileLimitSize(limitSize),
                        retainedFileCountLimit: maxCount, flushToDiskInterval: TimeSpan.FromTicks(100),
                        outputTemplate: format),
                    bufferSize: bufferSize);
            }
            else
            {
                config.WriteTo.File(path, rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: GetFileLimitSize(limitSize),
                    retainedFileCountLimit: maxCount, flushToDiskInterval: TimeSpan.FromTicks(100),
                    outputTemplate: format);
            }
        }

        // 是否打印到控制台
        var console = true;
        if (json.TryGetProperty("console", out prop))
        {
            console = prop.GetBoolean();
        }

        if (console)
        {
            config.WriteTo.Console(outputTemplate: format, theme: ConsoleTheme.None);
        }

        var _logger = config.CreateLogger();
        _logDict[name] = new SerilogLogger(name, _logger, redirectError);
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

    private RollingInterval GetRollingInterval(string? interval)
    {
        return interval switch
        {
            "day" => RollingInterval.Day,
            "hour" => RollingInterval.Hour,
            "minute" => RollingInterval.Minute,
            _ => RollingInterval.Day
        };
    }

    /// <summary>
    /// 设置Log日志级别
    /// </summary>
    /// <param name="config"></param>
    /// <param name="level"></param>
    private void SetMinimumLevel(LoggerConfiguration config, string level)
    {
        switch (level)
        {
            case "info":
                config.MinimumLevel.Information();
                break;
            case "debug":
                config.MinimumLevel.Debug();
                break;
            case "warn":
                config.MinimumLevel.Warning();
                break;
            case "error":
                config.MinimumLevel.Error();
                break;
            case "verbose":
                config.MinimumLevel.Verbose();
                break;
            default:
                config.MinimumLevel.Fatal();
                break;
        }
    }


}