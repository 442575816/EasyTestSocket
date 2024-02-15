# EasyTestSocket
socket benchmark tools

# 使用说明
```
-h, --host           Required. Target Host

-p, --port           Required. Target Port

-c, --connections    Maximum number of concurrent connections

--req_format         Request format

--resp_format        Response format

--input              Required. Test input data

-t, --timeout        Socket/request timeout

-d, --duration       Duration of test

--help               Display this help screen.

--version            Display version information.
```

`./EasyTestSocket -h 127.0.0.1 -p 1000 --input 1,2,1,0,helloworld,name=ddd111`

```
Test 127.0.0.1:1000 with 200 connections for 10 seconds.
[====================================================================================================================================================================================================================]
Done!
Statistics          Min            Avg            Max            Stdev
Reqs/sec            55716          68357.00       73051          4809.56
Latency             7us            2.91ms         39.49ms        6us

Total: 683570, Success: 683570, Failed: 0, Timeout: 0, Error: 0
Throughput: 8.21MB/s
```

上面是一个常见的使用案例

### 一、Tcp请求包的构造

这里使用了类似`Python Struct`，参数`req_format`描述包的结构. 举几个例子

- `>nbiii32s11s`  `包长(4)|请求类型(1)|服务器模块(4)|服务器Id(4)|请求Id(4)|请求Command(32)|请求Body(11)`

包的格式描述如下：
|Format|Type|Size|Notes|
|:-----|:-----|:-----|:-----|
|`>`| BigEndian | | 字节编码类型大端 |
|`<`| LittleEndian | | 字节编码类型小端 |
|`n`| int | 1,2,4| 包长Header(n) |
|`b`| byte | 1 | byte类型(1) |
|`i`| int | 4 | int类型(4) |
|`f`| float | 4 | float类型(4) |
|`l`| long | 8 | long类型(8) |
|`d`| double | 8 | double类型(8) |
|`h`| short | 2 | short类型(2) |
|`s`| string | 自定义长度 | 字符串类型(x) |

- `input` 参数输入，以`,`分割，需要保证和包的定义一致。header是不需要主动输入的，会自动根据包的格式计算填入合适的值

### 二、Tcp响应解析

会根据定义的`resp_format`, 自动计算返回包长，只要返回的包长和定义的包长相等就计算为请求成功.