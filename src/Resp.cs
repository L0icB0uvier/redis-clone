using System.Text;

namespace codecrafters_redis;

public class RespParser
{
    private readonly TextReader _reader;
    
    public RespParser(TextReader reader)
    {
        _reader = reader;
    }
    
    public RespObject Parse()
    {
        int prefix = _reader.Read();
        if (prefix == -1) throw new EndOfStreamException();
        
        return prefix switch
        {
            '+' => new RespSimpleString(ReadLine()),
            '-' => new RespError(ReadLine()),
            ':' => new RespInteger(long.Parse(ReadLine())),
            '$' => ParseBulkString(),
            '*' => ParseArray(),
            _ => throw new InvalidDataException($"Unknown RESP type: {(char)prefix}"),
        };
    }
    
    private string ReadLine()
    {
        var sb = new StringBuilder();
        while (true)
        {
            int c = _reader.Read();

            if (c == '\r')
            {
                if (_reader.Read() == '\n')
                {
                    break;
                }
            }
            sb.Append((char)c);
        }
        return sb.ToString();
    }
    
    private RespBulkString ParseBulkString()
    {
        int length = int.Parse(ReadLine());
        if (length == -1)
            return new RespBulkString(null); // null bulk string

        char[] buffer = new char[length];
        int read = _reader.ReadBlock(buffer, 0, length);
        _reader.Read(); // \r
        _reader.Read(); // \n
        return new RespBulkString(new string(buffer, 0, read));
    }

    private RespArray ParseArray()
    {
        int length = int.Parse(ReadLine());
        if (length == -1)
            return new RespArray(null); // null array

        var elements = new List<RespObject>();
        for (int i = 0; i < length; i++)
            elements.Add(Parse());

        return new RespArray(elements);
    }
}

public class RespWriter
{
    private readonly TextWriter _writer;

    public RespWriter(TextWriter writer)
    {
        _writer = writer;
    }

    public void Write(RespObject obj)
    {
        switch (obj)
        {
            case RespSimpleString ss:
                _writer.Write($"+{ss.Value}\r\n");
                break;
            case RespError err:
                _writer.Write($"-{err.Message}\r\n");
                break;
            case RespInteger i:
                _writer.Write($":{i.Value}\r\n");
                break;
            case RespBulkString bs:
                if (bs.Value == null)
                {
                    _writer.Write("$-1\r\n");
                }
                else
                {
                    _writer.Write($"${Encoding.UTF8.GetByteCount(bs.Value)}\r\n{bs.Value}\r\n");
                }

                break;
            case RespArray arr:

                if (arr.Elements == null)
                {
                    _writer.Write($"*-1\r\n");
                }
                else
                {
                    _writer.Write($"*{arr.Elements.Count}\r\n");
                    foreach (var element in arr.Elements)
                    {
                        Write(element);
                    }
                }
                break;
            default:
                throw new InvalidOperationException("Unknown RESP object type");
        }
    }
}

public abstract class RespObject { }

public class RespSimpleString : RespObject
{
    public string Value { get; }
    public RespSimpleString(string value) => Value = value;
}

public class RespError : RespObject
{
    public string Message { get; }
    public RespError(string message) => Message = message;
}

public class RespInteger : RespObject
{
    public long Value { get; }
    public RespInteger(long value) => Value = value;
}

public class RespBulkString : RespObject
{
    public string? Value { get; } // can be null
    public RespBulkString(string? value) => Value = value;
}

public class RespArray : RespObject
{
    public List<RespObject> Elements { get; }
    public RespArray(List<RespObject> elements) => Elements = elements;
}