using System.Text;
using EasyTestSocket.Buf;

namespace EasyTestSocket;

internal interface PackPattern
{
    public int Length { get; }
    public void Pack(ByteBuf buf, string value);
}

internal sealed class IntPattern : PackPattern
{
    public int Length => 4;

    public void Pack(ByteBuf buf, string value)
    {
        buf.WriteInt(int.Parse(value));
    }
}

internal sealed class FloatPattern : PackPattern
{
    public int Length => 4;

    public void Pack(ByteBuf buf, string value)
    {
        buf.WriteFloat(float.Parse(value));
    }
}

internal sealed class LongPattern : PackPattern
{
    public int Length => 8;

    public void Pack(ByteBuf buf, string value)
    {
        buf.WriteLong(long.Parse(value));
    }
}

internal sealed class DoublePattern : PackPattern
{
    public int Length => 8;

    public void Pack(ByteBuf buf, string value)
    {
        buf.WriteDouble(double.Parse(value));
    }
}

internal sealed class ShortPattern : PackPattern
{
    public int Length => 2;

    public void Pack(ByteBuf buf, string value)
    {
        buf.WriteShort(short.Parse(value));
    }
}

internal sealed class StringPattern : PackPattern
{
    public int Length { get; internal set; }

    public void Pack(ByteBuf buf, string value)
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

    public void Pack(ByteBuf buf, string value)
    {
        buf.WriteByte(byte.Parse(value));
    }
}

public class PackFormatter
{
    public const char BigEndian = '>';
    public const char LittleEndian = '<';
    
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

    public ByteBuf Format(string input, bool writeLen = true)
    {
        var array = input.Split(",");
        var buf = new ByteBuf();

        if (writeLen)
        {
            buf.WriteInt(Length);
        }
        for (var i = 0; i < array.Length; i++)
        {
            _patterns[i].Pack(buf, array[i]);
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
        for (var i = 1; i < array.Length; i++)
        {
            var c = array[i];
            switch (c)
            {
                case Int:
                    _patterns.Add(new IntPattern());
                    break;
                case Float:
                    _patterns.Add(new FloatPattern());
                    break;
                case Long:
                    _patterns.Add(new LongPattern());
                    break;
                case Double:
                    _patterns.Add(new DoublePattern());
                    break;
                case Short:
                    _patterns.Add(new ShortPattern());
                    break;
                case Byte:
                    _patterns.Add(new BytePattern());
                    break;
                case String:
                    if (string.IsNullOrEmpty(text))
                    {
                        throw new ArgumentException("Invalid format");
                    }
                    _patterns.Add(new StringPattern {Length = int.Parse(text)});
                    text = "";
                    break;
                default:
                    text += c;
                    break;
            }
        }
    }
}

