namespace SharpAttributeParser.Mappers.Repositories.SharpAttributeParserMappersServicesCases;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Moq;

using SharpAttributeParser.Mappers.Repositories.Adaptive;
using SharpAttributeParser.Mappers.Repositories.Combined;
using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Mappers.Repositories.Split;
using SharpAttributeParser.Mappers.Repositories.Syntactic;

using System;
using System.Diagnostics.CodeAnalysis;

using Xunit;

public sealed class AddSharpAttributeParserMapperRepositories
{
    private static IServiceCollection Target(IServiceCollection services) => SharpAttributeParserMapperRepositoriesServices.AddSharpAttributeParserMapperRepositories(services);

    private IServiceProvider ServiceProvider { get; }

    public AddSharpAttributeParserMapperRepositories()
    {
        HostBuilder host = new();

        host.ConfigureServices(configureServices);

        ServiceProvider = host.Build().Services;

        static void configureServices(IServiceCollection services)
        {
            Target(services);

            services.AddSingleton<IService, Service>();
        }
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
    public void IConstructorMappingRepositoryFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IConstructorMappingRepositoryFactory<object, IService>>();

    [Fact]
    public void INamedMappingRepositoryFactory_ServiceCanBeResolved() => ServiceCanBeResolved<INamedMappingRepositoryFactory<object, IService>>();

    [Fact]
    public void ITypeMappingRepositoryFactory_ServiceCanBeResolved() => ServiceCanBeResolved<ITypeMappingRepositoryFactory<object, IService>>();

    [Fact]
    public void ICombinedMappingRepositoryFactory_ServiceCanBeResolved() => ServiceCanBeResolved<ICombinedMappingRepositoryFactory<object>>();

    [Fact]
    public void ISemanticMappingRepositoryFactory_ServiceCanBeResolved() => ServiceCanBeResolved<ISemanticMappingRepositoryFactory<object>>();

    [Fact]
    public void ISyntacticMappingRepositoryFactory_ServiceCanBeResolved() => ServiceCanBeResolved<ISyntacticMappingRepositoryFactory<object>>();

    [Fact]
    public void IAdaptiveMappingRepositoryFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IAdaptiveMappingRepositoryFactory<object, object>>();

    [Fact]
    public void ISplitMappingRepositoryFactory_ServiceCanBeResolved() => ServiceCanBeResolved<ISplitMappingRepositoryFactory<object, object>>();

    [Fact]
    public void IDetachedMappedCombinedTypeArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedCombinedTypeArgumentRecorderFactory<object>>();

    [Fact]
    public void IDetachedMappedCombinedConstructorArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedCombinedConstructorArgumentRecorderFactory<object>>();

    [Fact]
    public void IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedCombinedNormalConstructorArgumentRecorderFactory<object>>();

    [Fact]
    public void IDetachedMappedCombinedParamsConstructorArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedCombinedParamsConstructorArgumentRecorderFactory<object>>();

    [Fact]
    public void IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedCombinedOptionalConstructorArgumentRecorderFactory<object>>();

    [Fact]
    public void IDetachedMappedCombinedNamedArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedCombinedNamedArgumentRecorderFactory<object>>();

    [Fact]
    public void IMappedCombinedArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IMappedCombinedArgumentRecorderFactory>();

    [Fact]
    public void IMappedCombinedTypeArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IMappedCombinedTypeArgumentRecorderFactory>();

    [Fact]
    public void IMappedCombinedConstructorArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IMappedCombinedConstructorArgumentRecorderFactory>();

    [Fact]
    public void IMappedCombinedNamedArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IMappedCombinedNamedArgumentRecorderFactory>();

    [Fact]
    public void IDetachedMappedSemanticTypeArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedSemanticTypeArgumentRecorderFactory<object>>();

    [Fact]
    public void IDetachedMappedSemanticConstructorArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedSemanticConstructorArgumentRecorderFactory<object>>();

    [Fact]
    public void IDetachedMappedSemanticNamedArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedSemanticNamedArgumentRecorderFactory<object>>();

    [Fact]
    public void IMappedSemanticArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IMappedSemanticArgumentRecorderFactory>();

    [Fact]
    public void IMappedSemanticTypeArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IMappedSemanticTypeArgumentRecorderFactory>();

    [Fact]
    public void IMappedSemanticConstructorArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IMappedSemanticConstructorArgumentRecorderFactory>();

    [Fact]
    public void IMappedSemanticNamedArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IMappedSemanticNamedArgumentRecorderFactory>();

    [Fact]
    public void IDetachedMappedSyntacticTypeArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedSyntacticTypeArgumentRecorderFactory<object>>();

    [Fact]
    public void IDetachedMappedSyntacticConstructorArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedSyntacticConstructorArgumentRecorderFactory<object>>();

    [Fact]
    public void IDetachedMappedSyntacticNormalConstructorArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedSyntacticNormalConstructorArgumentRecorderFactory<object>>();

    [Fact]
    public void IDetachedMappedSyntacticParamsConstructorArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedSyntacticParamsConstructorArgumentRecorderFactory<object>>();

    [Fact]
    public void IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedSyntacticOptionalConstructorArgumentRecorderFactory<object>>();

    [Fact]
    public void IDetachedMappedSyntacticNamedArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedSyntacticNamedArgumentRecorderFactory<object>>();

    [Fact]
    public void IMappedSyntacticArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IMappedSyntacticArgumentRecorderFactory>();

    [Fact]
    public void IMappedSyntacticTypeArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IMappedSyntacticTypeArgumentRecorderFactory>();

    [Fact]
    public void IMappedSyntacticConstructorArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IMappedSyntacticConstructorArgumentRecorderFactory>();

    [Fact]
    public void IMappedSyntacticNamedArgumentRecorderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IMappedSyntacticNamedArgumentRecorderFactory>();

    [Fact]
    public void IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedAdaptiveTypeArgumentRecorderProviderFactory<object, object>>();

    [Fact]
    public void IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedAdaptiveConstructorArgumentRecorderProviderFactory<object, object>>();

    [Fact]
    public void IDetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedAdaptiveNormalConstructorArgumentRecorderProviderFactory<object, object>>();

    [Fact]
    public void IDetachedMappedAdaptiveParamsConstructorArgumentRecorderProviderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedAdaptiveParamsConstructorArgumentRecorderProviderFactory<object, object>>();

    [Fact]
    public void IDetachedMappedAdaptiveOptionalConstructorArgumentRecorderProviderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedAdaptiveOptionalConstructorArgumentRecorderProviderFactory<object, object>>();

    [Fact]
    public void IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedAdaptiveNamedArgumentRecorderProviderFactory<object, object>>();

    [Fact]
    public void IDetachedMappedSplitTypeArgumentRecorderProviderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedSplitTypeArgumentRecorderProviderFactory<object, object>>();

    [Fact]
    public void IDetachedMappedSplitConstructorArgumentRecorderProviderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedSplitConstructorArgumentRecorderProviderFactory<object, object>>();

    [Fact]
    public void IDetachedMappedSplitNormalConstructorArgumentRecorderProviderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedSplitNormalConstructorArgumentRecorderProviderFactory<object, object>>();

    [Fact]
    public void IDetachedMappedSplitParamsConstructorArgumentRecorderProviderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedSplitParamsConstructorArgumentRecorderProviderFactory<object, object>>();

    [Fact]
    public void IDetachedMappedSplitOptionalConstructorArgumentRecorderProviderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedSplitOptionalConstructorArgumentRecorderProviderFactory<object, object>>();

    [Fact]
    public void IDetachedMappedSplitNamedArgumentRecorderProviderFactory_ServiceCanBeResolved() => ServiceCanBeResolved<IDetachedMappedSplitNamedArgumentRecorderProviderFactory<object, object>>();

    [AssertionMethod]
    private void ServiceCanBeResolved<TService>() where TService : notnull
    {
        var service = ServiceProvider.GetRequiredService<TService>();

        Assert.NotNull(service);
    }

    private interface IService { }

    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used with DI.")]
    private sealed class Service : IService { }
}
