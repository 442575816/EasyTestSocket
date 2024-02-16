using EasyTestSocket.Buf;
using EasyTestSocket.Network;

namespace EasyTestSocket;

public class TestTcp
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string InputFormat { get; set; }
    public string OutputFormat { get; set; }
    public string Input { get; set; }
    public int Timeout { get; set; }
    public int MaxConnections { get; set; }
    public TimeSpan Duration { get; set; }
    
    private readonly PackFormatter _inputFormatter;
    private readonly PackFormatter _outputFormatter;
    private readonly SocketFactory _socketFactory;
    private readonly CancellationToken _token;

    private readonly List<TestSocket> _testSockets;
    private Exception? _error;
    
    public TestTcp(string host, int port, string inputFormat = ">ni32s4s", string outputFormat = ">ii32s16s", string input = "0,helloworld,name", int timeout = 2000, int maxConnections = 200, int duration = 10000)
    {
        Host = host;
        Port = port;
        InputFormat = inputFormat;
        OutputFormat = outputFormat;
        Input = input;
        Timeout = timeout;
        MaxConnections = maxConnections;
        Duration = TimeSpan.FromMilliseconds(duration);
        
        _token = new CancellationTokenSource(Duration).Token;
        _inputFormatter = new PackFormatter(inputFormat);
        _outputFormatter = new PackFormatter(outputFormat);
        _testSockets = new();

        _socketFactory = new SocketFactory(new SocketOption
        {
            ConnectTimeout = timeout,
            ReadTimeout = timeout,
            SendTimeout = timeout,
        });
    }
    
    public async Task StartAsync()
    {
        for (var i = 0; i < MaxConnections; i++)
        {
            try
            {
                var socket = new TestSocket(Host, Port, _socketFactory, _token, _inputFormatter.Format(Input), Timeout, _outputFormatter.Length);
                _testSockets.Add(socket);
            }
            catch (Exception e)
            {
                _error = e;
            }
        }

        var testTasks = new List<Task>();
        foreach (var client in _testSockets)
        {
            testTasks.Add(client.StartBenchmark());
        }
        
        await Task.WhenAll(testTasks);
    }

    public async Task PrintAsync()
    {
        Console.WriteLine($"Test {Host}:{Port} with {MaxConnections} connections for {Duration.TotalSeconds} seconds.");
        var width = Console.BufferWidth / Duration.TotalSeconds;
        var index = 0;
        var currTop = Console.CursorTop;

        var list = new List<int>();
        var lastRequestNum = 0;
        while (!_token.IsCancellationRequested)
        {
            Console.SetCursorPosition(0, currTop);
            Console.Write('[');
            Console.Write(new string('=', (int)(width * index)));
            Console.Write(new string(' ', (int)(width * (Duration.TotalSeconds - index - 1))));
            Console.Write(']');
            index++;
            await Task.Delay(1000);
            var num = _testSockets.Select(s => s.Result.Success).Sum();
            list.Add(num - lastRequestNum);
            lastRequestNum = num;
        }
        Console.WriteLine();
        Console.WriteLine("Done!");
        var total = _testSockets.Select(s => s.Result.Total).Sum();
        var success = _testSockets.Select(s => s.Result.Success).Sum();
        var failed = _testSockets.Select(s => s.Result.Failed).Sum();
        var timeout = _testSockets.Select(s => s.Result.Timeout).Sum();
        var error = _testSockets.Select(s => s.Result.Error).Sum();
        
        var minTps = list.Min();
        var maxTps = list.Max();
        var avgTps = success / Duration.TotalSeconds;
        var stdDevTps = Math.Sqrt(list.Select(x => Math.Pow(x - avgTps, 2)).Sum() / list.Count);
        var min = _testSockets.Select(s => s.Result.Min).Min();
        var max = _testSockets.Select(s => s.Result.Max).Max();
        var avg = _testSockets.Select(s => s.Result.Avg).Sum() / _testSockets.Count;
        var stdDev = Math.Sqrt(_testSockets.Select(s => Math.Pow(s.Result.Avg - avg, 2)).Sum() / _testSockets.Count);
        var throughput = _testSockets.Select(s => s.Result.Throughput).Sum() / Duration.TotalSeconds;
        
        Console.Write("{0,-20}", "Statistics");
        Console.Write("{0,-15}", "Min");
        Console.Write("{0,-15}", "Avg");
        Console.Write("{0,-15}", "Max");
        Console.Write("{0,-15}", "Stdev");
        Console.WriteLine();
        
        Console.Write("{0,-20}", "Reqs/sec");
        Console.Write("{0,-15}", minTps);
        Console.Write("{0,-15:f2}", avgTps);
        Console.Write("{0,-15}", maxTps);
        Console.Write("{0,-15:f2}", stdDevTps);
        Console.WriteLine();
        
        Console.Write("{0,-20}", "Latency");
        Console.Write("{0,-15}", FormatElapsedTime(min));
        Console.Write("{0,-15}", FormatElapsedTime(avg));
        Console.Write("{0,-15}", FormatElapsedTime(max));
        Console.Write("{0,-15}", FormatElapsedTime((long)stdDev));
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine($"Total: {total}, Success: {success}, Failed: {failed}, Timeout: {timeout}, Error: {error}");
        Console.WriteLine($"Throughput: {FormatSize(throughput)}/s");

        if (_error != null)
        {
            Console.WriteLine($"Exception: {_error.Message}");
            Console.WriteLine(_error);
        }
        else
        {
            var ex = _testSockets.FirstOrDefault(s => s.Error != null)?.Error;
            if (ex != null)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine(ex);
            }
        }
    }
    
    public string FormatElapsedTime(long elapsed)
    {
        if (elapsed > 1000)
        {
            return $"{elapsed / 1000.0:f2}ms";
        }

        return $"{elapsed}us";
    }

    public string FormatSize(double size)
    {
        return size switch
        {
            > 1024 * 1024 => $"{size / 1024.0 / 1024.0:f2}MB",
            > 1024 => $"{size / 1024.0:f2}KB",
            _ => $"{size}B"
        };
    }
}

public sealed class BenchmarkResult
{
    public int Total { get; set; }
    public int Success { get; set; }
    public int Failed { get; set; }
    public int Timeout { get; set; }
    public int Error { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }
    public int Avg { get; set; }
    public long Throughput { get; set; }
    
    private DateTime _start;
    private long _totalElapsed;

    public void StartRequest(int length)
    {
        Total++;
        Throughput += length;
        _start = DateTime.UtcNow;
    }

    public void EndRequest(TaskCompletionSource tcs, int outputLength)
    {
        var elapsed = (int) (DateTime.UtcNow - _start).TotalMicroseconds;
        _totalElapsed += elapsed;
        if (Min == 0 || elapsed < Min)
        {
            Min = elapsed;
        }
        if (elapsed > Max)
        {
            Max = elapsed;
        }
        Avg = (int) (_totalElapsed / Total);
        if (tcs.Task.IsCompletedSuccessfully)
        {
            Success++;
            Throughput += outputLength;
        }
        else if (tcs.Task.IsCanceled)
        {
            Timeout++;
        }
        else
        {
            Failed++;
        }
        
    }
}

public class TestSocket : MessageDecoder
{
    public BenchmarkResult Result { get; }
    
    private readonly SocketChannel _channel;
    private readonly CancellationToken _token;
    private readonly ByteBuf _buf;
    private readonly int _outputLength;
    private TaskCompletionSource? _tcs;
    private readonly TimeSpan _timeout;
    private readonly SocketEventCode _connEventCode;
    public Exception? Error { get; set; }
    
    public TestSocket(string host, int port, SocketFactory factory, CancellationToken token, ByteBuf buf, int timeout, int size)
    {
        Result = new BenchmarkResult();
        _channel = factory.Create(host, port, this);
        try
        {
            _connEventCode = _channel.ConnectSync();
        }
        catch (Exception e)
        {
            Error = e;
        }
        
        _token = token;
        _outputLength = size;
        _buf = buf;
        _timeout = TimeSpan.FromMilliseconds(timeout);
        
    }
    
    public async Task StartBenchmark()
    {
        if (_connEventCode != SocketEventCode.ConnectSucc)
        {
            return;
        }
        
        while (!_token.IsCancellationRequested && Error == null)
        {
            _tcs = new TaskCompletionSource();
            _buf.Retain();
            try
            {
                var result = await _channel.SendAsync(_buf);
                if (!result)
                {
                    Result.Error++;
                    continue;
                }
            }
            catch (Exception e)
            {
                Error = e;
                Result.Error++;
                continue;
            }
            
            Result.StartRequest(_buf.ReadableBytes);
            try
            {
                await _tcs.Task.WaitAsync(_timeout);
            }
            catch (Exception)
            {
                // Ignore
            }
            Result.EndRequest(_tcs, _outputLength);
        }
    }
    
    public new void OnEvent(SocketChannel channel, SocketEventCode eventCode, string content = "")
    {
    }

    protected override void Decode(SocketChannel channel, ByteBuf buf)
    {
        if (buf.ReadableBytes >= _outputLength)
        {
            buf.SkipBytes(_outputLength);
            _tcs!.TrySetResult();
        }
    }
}