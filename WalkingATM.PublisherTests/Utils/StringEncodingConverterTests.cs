using System.Text;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using WalkingATM.Publisher;
using WalkingATM.Publisher.Utils;

namespace WalkingATM.PublisherTests.Utils;

[TestFixture]
public class StringEncodingConverterTests
{
    private IOptions<AppSettings> _options;
    private StringEncodingConverter _stringEncodingConverter;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
    
    [SetUp]
    public void SetUp()
    {
        _options = Substitute.For<IOptions<AppSettings>>();
        _stringEncodingConverter = new StringEncodingConverter(_options);
    }

    [Test]
    public void GetUtf8String()
    {
        _options.Value.Returns(
            new AppSettings()
            {
                LogFileEncoding = "big5"
            });

        var big5Bytes = new byte[]
        {
            0xBE,
            0xDE,
            0xA7,
            0x41,
            0xB6,
            0xFD,
            0xB6,
            0xFD,
            0xB9,
            0x47,
        };

        var big5String = Encoding.GetEncoding("big5").GetString(big5Bytes);

        var utf8String = _stringEncodingConverter.GetUtf8String(big5String);

        StringAssert.AreEqualIgnoringCase(utf8String, "操你媽媽逼");
    }
}