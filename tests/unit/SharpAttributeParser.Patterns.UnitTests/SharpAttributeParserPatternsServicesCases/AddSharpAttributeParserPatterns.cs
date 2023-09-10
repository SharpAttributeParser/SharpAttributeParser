namespace SharpAttributeParser.Patterns.SharpAttributeParserPatternsServicesCases;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Moq;

using System;

using Xunit;

public sealed class AddSharpAttributeParserPatterns
{
    private static IServiceCollection Target(IServiceCollection services) => SharpAttributeParserPatternsServices.AddSharpAttributeParserPatterns(services);

    private IServiceProvider ServiceProvider { get; }

    public AddSharpAttributeParserPatterns()
    {
        HostBuilder host = new();

        host.ConfigureServices(static (services) => Target(services));

        ServiceProvider = host.Build().Services;
    }

    [Fact]
    public void NullServiceCollection_ArgumentNullException()
    {
        var exception = Record.Exception(() => Target(null!));

        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void ValidServiceCollection_ReturnsSameServiceCollection()
    {
        var serviceCollection = Mock.Of<IServiceCollection>();

        var actual = Target(serviceCollection);

        Assert.Same(serviceCollection, actual);
    }

    [Fact]
    public void IArgumentPatternFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IArgumentPatternFactory>();

    [AssertionMethod]
    private void ServiceCanBeResolved<TService>() where TService : notnull
    {
        var service = ServiceProvider.GetRequiredService<TService>();

        Assert.NotNull(service);
    }
}
