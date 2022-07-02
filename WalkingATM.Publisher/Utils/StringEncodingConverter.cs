using System.Text;
using Microsoft.Extensions.Options;

namespace WalkingATM.Publisher.Utils;

public static class EncodingCode
{
    public const string Big5 = "big5";
    public const string Utf8 = "utf-8";
    public const string Win1252 = "Windows-1252";
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
        var utf8 = Encoding.GetEncoding(EncodingCode.Utf8);
        switch (_appSettings.Value.LogFileEncoding)
        {
            case EncodingCode.Big5:
                var big5 = Encoding.GetEncoding(EncodingCode.Big5);
                return Encoding.UTF8.GetString(Encoding.Convert(big5, utf8, big5.GetBytes(input)));

            case EncodingCode.Utf8:
                return input;

            case EncodingCode.Win1252:
                var win1252 = Encoding.GetEncoding(EncodingCode.Win1252);
                return Encoding.UTF8.GetString(Encoding.Convert(win1252, utf8, win1252.GetBytes(input)));

            default:
                return input;
        }
    }
}