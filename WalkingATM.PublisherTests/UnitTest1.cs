using System;
using NUnit.Framework;

namespace WalkingATM.PublisherTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Console.WriteLine(TimeZoneInfo.Local.ToSerializedString());

        var readOnlyCollection = TimeZoneInfo.GetSystemTimeZones();
        
        Assert.IsTrue(true);
    }
}