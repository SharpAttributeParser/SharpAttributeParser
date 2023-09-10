namespace SharpAttributeParser.SharpAttributeParserServicesCases;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Moq;

using System;

using Xunit;

public sealed class AddSharpAttributeParser
{
    private static IServiceCollection Target(IServiceCollection services) => SharpAttributeParserServices.AddSharpAttributeParser(services);

    private IServiceProvider ServiceProvider { get; }

    public AddSharpAttributeParser()
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
    public void ICombinedParser_ServiceCanBeResolved() => ServiceCanBeResolved<ICombinedParser>();

    [Fact]
    public void ISemanticParser_ServiceCanBeResolved() => ServiceCanBeResolved<ISemanticParser>();

    [Fact]
    public void ISyntacticParser_ServiceCanBeResolved() => ServiceCanBeResolved<ISyntacticParser>();

    [AssertionMethod]
    private void ServiceCanBeResolved<TService>() where TService : notnull
    {
        var service = ServiceProvider.GetRequiredService<TService>();

        Assert.NotNull(service);
    }
}
