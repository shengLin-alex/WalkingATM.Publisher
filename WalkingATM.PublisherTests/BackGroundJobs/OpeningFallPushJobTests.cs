using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using WalkingATM.Publisher.BackgroundJobs.Opening;

namespace WalkingATM.PublisherTests.BackGroundJobs;

[TestFixture]
public class OpeningFallPushJobTests : PushJobTestBase
{
    private ILogger<OpeningFallPushJob> _logger;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        _logger = Substitute.For<ILogger<OpeningFallPushJob>>();
        PushJob = new OpeningFallPushJob(
            LogFileMonitor,
            Strategy,
            Options,
            _logger,
            LifetimeScope,
            TimeProvider,
            HostEnvironment);
    }

    [Test]
    public override Task Execute_Once()
    {
        return base.Execute_Once();
    }

    [Test]
    public override Task Execute_Twice_With_Same_Line()
    {
        return base.Execute_Twice_With_Same_Line();
    }

    [Test]
    public override Task Execute_Twice_With_Diff_Line()
    {
        return base.Execute_Twice_With_Diff_Line();
    }
}