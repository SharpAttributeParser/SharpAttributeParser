namespace SharpAttributeParser.ExampleCases.RecommendedPatternCases;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Mappers;
using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Patterns;

using System;

public sealed class ExampleMapper : ASemanticMapper<IExampleRecordBuilder>
{
    public ExampleMapper() { }
    public ExampleMapper(ISemanticMapperDependencyProvider<IExampleRecordBuilder> dependencyProvider) : base(dependencyProvider) { }

    protected override void AddMappings(IAppendableSemanticMappingRepository<IExampleRecordBuilder> repository)
    {
        repository.TypeParameters.AddIndexedMapping(0, static (factory) => factory.Create(RecordTypeArgument));
        repository.ConstructorParameters.AddNamedMapping(nameof(ExampleAttribute<object>.ConstructorArgument), static (factory) => factory.Create(ConstructorArgumentPattern, RecordConstructorArgument));
        repository.ConstructorParameters.AddNamedMapping(nameof(ExampleAttribute<object>.OptionalArgument), static (factory) => factory.Create(OptionalArgumentPattern, RecordOptionalArgument));
        repository.ConstructorParameters.AddNamedMapping(nameof(ExampleAttribute<object>.ParamsArgument), static (factory) => factory.Create(ParamsArgumentPattern, RecordParamsArgument));
        repository.NamedParameters.AddNamedMapping(nameof(ExampleAttribute<object>.NamedArgument), static (factory) => factory.Create(NamedArgumentPattern, RecordNamedArgument));
    }

    private static IArgumentPattern<StringComparison> ConstructorArgumentPattern(IArgumentPatternFactory factory) => factory.Enum<StringComparison>();
    private static IArgumentPattern<string?> OptionalArgumentPattern(IArgumentPatternFactory factory) => factory.NullableString();
    private static IArgumentPattern<int[]> ParamsArgumentPattern(IArgumentPatternFactory factory) => factory.NonNullableArray(factory.Int());
    private static IArgumentPattern<ITypeSymbol?> NamedArgumentPattern(IArgumentPatternFactory factory) => factory.NullableType();

    private static void RecordTypeArgument(IExampleRecordBuilder recordBuilder, ITypeSymbol typeArgument) => recordBuilder.WithTypeArgument(typeArgument);
    private static void RecordConstructorArgument(IExampleRecordBuilder recordBuilder, StringComparison constructorArgument) => recordBuilder.WithConstructorArgument(constructorArgument);
    private static void RecordOptionalArgument(IExampleRecordBuilder recordBuilder, string? optionalArgument) => recordBuilder.WithOptionalArgument(optionalArgument);
    private static void RecordParamsArgument(IExampleRecordBuilder recordBuilder, int[] paramsArgument) => recordBuilder.WithParamsArgument(paramsArgument);
    private static void RecordNamedArgument(IExampleRecordBuilder recordBuilder, ITypeSymbol? namedArgument) => recordBuilder.WithNamedArgument(namedArgument);
}
