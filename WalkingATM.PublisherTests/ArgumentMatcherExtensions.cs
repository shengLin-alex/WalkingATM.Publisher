using System;
using FluentAssertions;
using FluentAssertions.Equivalency;

namespace WalkingATM.PublisherTests;

public static class ArgumentMatcherExtensions
{
    /// <summary>
    /// check with strict ordering as default
    /// </summary>
    /// <param name="source"></param>
    /// <param name="expected"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool ShouldEqual<T>(this T source, T expected)
    {
        return ShouldEqual(source, expected, options => options.WithStrictOrdering());
    }

    private static bool ShouldEqual<T>(
        this T source,
        T expected,
        Func<EquivalencyAssertionOptions<T>,
            EquivalencyAssertionOptions<T>> config)
    {
        try
        {
            source.Should().BeEquivalentTo(expected, config);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}