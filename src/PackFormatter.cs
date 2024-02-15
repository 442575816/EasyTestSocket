using System.Text;
using EasyTestSocket.Buf;

namespace EasyTestSocket;

internal interface PackPattern
{
    public int Length { get; }
    public int Index { get; }
    public void Pack(PackFormatter formatter, ByteBuf buf, string value);
}

internal sealed class IntPattern : PackPattern
{
    public int Length => 4;
    
    public int Index { get; set; }

    public void Pack(PackFormatter formatter, ByteBuf buf, string value)
    {
        buf.WriteInt(int.Parse(value));
    }
}

internal sealed class FloatPattern : PackPattern
{
    public int Length => 4;

    public int Index { get; set; }
    
    public void Pack(PackFormatter formatter, ByteBuf buf, string value)
    {
        buf.WriteFloat(float.Parse(value));
    }
}

internal sealed class LongPattern : PackPattern
{
    public int Length => 8;

    public int Index { get; set; }
    
    public void Pack(PackFormatter formatter, ByteBuf buf, string value)
    {
        buf.WriteLong(long.Parse(value));
    }
}

internal sealed class DoublePattern : PackPattern
{
    public int Length => 8;

    public int Index { get; set; }
    
    public void Pack(PackFormatter formatter, ByteBuf buf, string value)
    {
        buf.WriteDouble(double.Parse(value));
    }
}

internal sealed class ShortPattern : PackPattern
{
    public int Length => 2;

    public int Index { get; set; }
    
    public void Pack(PackFormatter formatter, ByteBuf buf, string value)
    {
        buf.WriteShort(short.Parse(value));
    }
}

internal sealed class StringPattern : PackPattern
{
    public int Length { get; internal set; }

    public int Index { get; set; }
    
    public void Pack(PackFormatter formatter, ByteBuf buf, string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        buf.WriteBytes(bytes, 0, Math.Min(bytes.Length, Length));
        if (bytes.Length < Length)
        {
            buf.WriteEmpty(Length - bytes.Length);
        }
    }
}

internal sealed class BytePattern : PackPattern
{
    public int Length => 1;

    public int Index { get; set; }
    
    public void Pack(PackFormatter formatter, ByteBuf buf, string value)
    {
        buf.WriteByte(byte.Parse(value));
    }
}

internal sealed class HeaderPattern : PackPattern
{
    public int Length { get; internal set; }

    public int Index { get; set; }
    
    public void Pack(PackFormatter formatter, ByteBuf buf, string value)
    {
        var len = formatter.Length - Length;
        switch (Length)
        {
            case 1:
                buf.WriteByte(len);
                break;
            case 2:
                buf.WriteShort(len);
                break;
            case 4:
                buf.WriteInt(len);
                break;
            default:
                throw new ArgumentException("Invalid header length");
        }
    }
}

public class PackFormatter
{
    public const char BigEndian = '>';
    public const char LittleEndian = '<';
    
    public const char Header = 'n';
    public const char Int = 'i';
    public const char Float = 'f';
    public const char Long = 'l';
    public const char Double = 'd';
    public const char Short = 'h';
    public const char String = 's';
    public const char Byte = 'b';
    
    private bool _isBigEndian;
    private readonly List<PackPattern> _patterns = new();
    

    public PackFormatter(string format)
    {
        ParseFormat(format);
    }

    public ByteBuf Format(string input)
    {
        var array = input.Split(",");
        var buf = new ByteBuf();

        foreach (var pattern in _patterns)
        {
            pattern.Pack(this, buf, pattern.Index == -1 ? "" : array[pattern.Index]);
        }

        return buf;
    }
    
    public int Length => _patterns.Sum(p => p.Length);

    private void ParseFormat(string format)
    {
        var array = format.ToCharArray();
        var first = array[0];
        _isBigEndian = first switch
        {
            BigEndian => true,
            LittleEndian => false,
            _ => throw new ArgumentException("Invalid format")
        };

        var text = "";
        var index = 0;
        for (var i = 1; i < array.Length; i++)
        {
            var c = array[i];
            switch (c)
            {
                case Header:
                    _patterns.Add(string.IsNullOrEmpty(text)
                        ? new HeaderPattern { Index = -1, Length = 4 }
                        : new HeaderPattern { Index = -1, Length = int.Parse(text) });
                    text = "";
                    break;
                case Int:
                    _patterns.Add(new IntPattern { Index = index++ });
                    break;
                case Float:
                    _patterns.Add(new FloatPattern { Index = index++ });
                    break;
                case Long:
                    _patterns.Add(new LongPattern { Index = index++ });
                    break;
                case Double:
                    _patterns.Add(new DoublePattern { Index = index++ });
                    break;
                case Short:
                    _patterns.Add(new ShortPattern { Index = index++ });
                    break;
                case Byte:
                    _patterns.Add(new BytePattern { Index = index++ });
                    break;
                case String:
                    if (string.IsNullOrEmpty(text))
                    {
                        throw new ArgumentException("Invalid format");
                    }

                    _patterns.Add(new StringPattern { Index = index++, Length = int.Parse(text) });
                    text = "";
                    break;
                default:
                    text += c;
                    break;
            }
        }
    }
}

