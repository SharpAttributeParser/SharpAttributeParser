namespace SharpAttributeParser.ExampleCases.MainReadmeCases;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Mappers;
using SharpAttributeParser.Mappers.Repositories.Semantic;
using SharpAttributeParser.Patterns;

using System;

public sealed class ExampleMapper : ASemanticMapper<ExampleRecord>
{
    protected override void AddMappings(IAppendableSemanticMappingRepository<ExampleRecord> repository)
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

    private static void RecordTypeArgument(ExampleRecord dataRecord, ITypeSymbol typeArgument) => dataRecord.TypeArgument = typeArgument;
    private static void RecordConstructorArgument(ExampleRecord dataRecord, StringComparison constructorArgument) => dataRecord.ConstructorArgument = constructorArgument;
    private static void RecordOptionalArgument(ExampleRecord dataRecord, string? optionalArgument) => dataRecord.OptionalArgument = optionalArgument;
    private static void RecordParamsArgument(ExampleRecord dataRecord, int[] paramsArgument) => dataRecord.ParamsArgument = paramsArgument;
    private static void RecordNamedArgument(ExampleRecord dataRecord, ITypeSymbol? namedArgument) => dataRecord.NamedArgument = namedArgument;
}
