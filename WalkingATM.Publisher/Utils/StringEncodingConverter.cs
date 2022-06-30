using System.Text;
using Microsoft.Extensions.Options;

namespace WalkingATM.Publisher.Utils;

public static class EncodingCode
{
    public const string Big5 = "big5";
    public const string Utf8 = "utf-8";
}

public interface IStringEncodingConverter
{
    string GetUtf8String(string input);
}

public class StringEncodingConverter : IStringEncodingConverter
{
    private readonly IOptions<AppSettings> _appSettings;

    public StringEncodingConverter(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings;
    }

    public string GetUtf8String(string input)
    {
        switch (_appSettings.Value.LogFileEncoding)
        {
            case EncodingCode.Big5:
                var big5 = Encoding.GetEncoding("big5");
                var utf8 = Encoding.GetEncoding("utf-8");
                var convert = Encoding.Convert(big5, utf8, big5.GetBytes(input));
                return Encoding.UTF8.GetString(convert);
            
            case EncodingCode.Utf8:
                return input;
            
            default:
                return input;
        }
    }
}