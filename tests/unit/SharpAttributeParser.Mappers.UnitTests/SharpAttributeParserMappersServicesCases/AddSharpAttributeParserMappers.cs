namespace SharpAttributeParser.Mappers.SharpAttributeParserMappersServicesCases;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;

using Xunit;

public sealed class AddSharpAttributeParserMappers
{
    private static IServiceCollection Target(IServiceCollection services) => SharpAttributeParserMappersServices.AddSharpAttributeParserMappers(services);

    private IServiceProvider ServiceProvider { get; }

    public AddSharpAttributeParserMappers()
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
    public void ICombinedRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<ICombinedRecorderFactory>();

    [Fact]
    public void ISemanticRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<ISemanticRecorderFactory>();

    [Fact]
    public void ISyntacticRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<ISyntacticRecorderFactory>();

    [AssertionMethod]
    private void ServiceCanBeResolved<TService>() where TService : notnull
    {
        var service = ServiceProvider.GetRequiredService<TService>();

        Assert.NotNull(service);
    }
}
