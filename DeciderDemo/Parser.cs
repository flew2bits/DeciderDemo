namespace DeciderDemo;
#if !NET7_0_OR_GREATER
public class Parser<T>
{
    public Parser(Parse<T> parse, TryParse<T> tryParse)
    {
        Parse = parse;
        TryParse = tryParse;
        TryParseWithoutFormat = null;
    }

    public Parser(ParseWithoutFormat<T> parse, TryParseWithoutFormat<T> tryParse)
    {
        Parse = (v, _) => parse(v);
        TryParseWithoutFormat = tryParse;
        TryParse = TryParseWithoutFormatInternal;
    }

    private bool TryParseWithoutFormatInternal(string value, IFormatProvider? formatProvider, out T? output)
    {
        output = default;
        return TryParseWithoutFormat?.Invoke(value, out output) ?? false;
    }
    
    public Parse<T> Parse { get; }
    public TryParse<T> TryParse { get; }
    
    private TryParseWithoutFormat<T>? TryParseWithoutFormat { get; }
}

public delegate T ParseWithoutFormat<out T>(string value);

public delegate bool TryParseWithoutFormat<T>(string value, out T? output);


public delegate T Parse<out T>(string value, IFormatProvider? formatProvider);

public delegate bool TryParse<T>(string value, IFormatProvider? formatProvider, out T? output);

#endif