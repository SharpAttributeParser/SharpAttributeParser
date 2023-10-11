namespace SharpAttributeParser.Mappers.SharpAttributeParserMappersServicesCases;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Moq;

using SharpAttributeParser.Mappers.Repositories;

using System;

using Xunit;

public sealed class AddSharpAttributeParserMappers
{
    private static IServiceCollection Target(IServiceCollection services) => SharpAttributeParserMappersServices.AddSharpAttributeParserMappers(services);

    private readonly IServiceProvider ServiceProvider;

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
    public void ValidServiceCollection_ReturnsSameServiceCollection()
    {
        var serviceCollection = Mock.Of<IServiceCollection>();

        var actual = Target(serviceCollection);

        Assert.Same(serviceCollection, actual);
    }

    [Fact]
    public void ICombinedRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<ICombinedRecorderFactory>();

    [Fact]
    public void ISemanticRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<ISemanticRecorderFactory>();

    [Fact]
    public void ISyntacticRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<ISyntacticRecorderFactory>();

    [Fact]
    public void ICombinedMapperDependencyProvider_ServiceCanBeResolved() => ServiceCanBeResolved<ICombinedMapperDependencyProvider<object>>();

    [Fact]
    public void ISemanticMapperDependencyProvider_ServiceCanBeResolved() => ServiceCanBeResolved<ISemanticMapperDependencyProvider<object>>();

    [Fact]
    public void ISyntacticMapperDependencyProvider_ServiceCanBeResolved() => ServiceCanBeResolved<ISyntacticMapperDependencyProvider<object>>();

    [Fact]
    public void IAdaptiveMapperDependencyProvider_ServiceCanBeResolved() => ServiceCanBeResolved<IAdaptiveMapperDependencyProvider<object, object>>();

    [Fact]
    public void ISplitMapperDependencyProvider_ServiceCanBeResolved() => ServiceCanBeResolved<ISplitMapperDependencyProvider<object, object>>();

    [Fact]
    public void IParameterComparer_ServiceCanBeResolved() => ServiceCanBeResolved<IParameterComparer>();

    [Fact]
    public void ITypeParameterComparer_ServiceCanBeResolved() => ServiceCanBeResolved<ITypeParameterComparer>();

    [Fact]
    public void IConstructorParameterComparer_ServiceCanBeResolved() => ServiceCanBeResolved<IConstructorParameterComparer>();

    [Fact]
    public void INamedParameterComparer_ServiceCanBeResolved() => ServiceCanBeResolved<INamedParameterComparer>();

    [AssertionMethod]
    private void ServiceCanBeResolved<TService>() where TService : notnull
    {
        var service = ServiceProvider.GetRequiredService<TService>();

        Assert.NotNull(service);
    }
}
