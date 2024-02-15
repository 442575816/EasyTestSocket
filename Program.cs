// See https://aka.ms/new-console-template for more information

using System.Diagnostics.CodeAnalysis;
using CommandLine;
using EasyTestSocket;
using EasyTestSocket.Log;

LogFactory.Provitor = LogProvider.Serilog;
LogFactory.Setup();

var result = Parser.Default.ParseArguments<Options>(args);

[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Options))]
void TestAction(Options option)
{
    var test = new TestTcp(option.Host, option.Port, option.InputFormat, option.OutputFormat, option.Input, option.Timeout, option.MaxConnections, option.Duration);
    var task1 = test.StartAsync();
    var task2 = test.PrintAsync();
    Task.WaitAll(task1, task2);
}
result.WithParsed(TestAction);

// var test = new TestTcp("127.0.0.1", 8011, ">nbiii32s11s", ">ibli32s17s", "1,2,1,0,helloworld,name=ddd111");
// var task = test.StartAsync();
// var task1 = test.PrintAsync();
// await Task.WhenAll(task, task1);

public class Options
{
    [Option('h', "host", Required = true, HelpText = "Target Host")]
    public string Host { get; set; }

    [Option('p', "port", Required = true, HelpText = "Target Port")]
    public int Port { get; set; }
    
    [Option('c', "connections", Required = false, HelpText = "Maximum number of concurrent connections")]
    public int MaxConnections { get; set; } = 200;

    [Option("req_format", Required = false, HelpText = "Request format")]
    public string InputFormat { get; set; } = ">nbiii32s11s";
    
    [Option("resp_format", Required = false, HelpText = "Response format")]
    public string OutputFormat { get; set; } = ">ibli32s17s";
    
    [Option("input", Required = true, HelpText = "Test input data")]
    public string Input { get; set; } = "1,2,1,0,helloworld,name=ddd111";
    
    [Option('t', "timeout", Required = false, HelpText = "Socket/request timeout")]
    public int Timeout { get; set; } = 2000;
    
    [Option('d', "duration", Required = false, HelpText = "Duration of test")]
    public int Duration { get; set; } = 10000;
}