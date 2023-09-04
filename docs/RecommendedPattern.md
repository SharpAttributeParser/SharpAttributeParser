# Recommended Pattern

> The add-on packages [SharpAttributeParser.Mappers](https://www.nuget.org/packages/SharpAttributeParser.Mappers/) and [SharpAttributeParser.RecordBuilders](https://www.nuget.org/packages/SharpAttributeParser.RecordBuilders/) are required for the recommended pattern.

This article will present the recommended pattern. It is *recommended* in the sense that it is what I use in my personal projects, but is not necessarily the best way of using `SharpAttributeParser`. The article will assume that attributes are parsed semantically, but the pattern can easily be modified for the syntactic and combined [parsing modes](ParsingModes.md).

1. [Attribute Class](#1-attribute-class)
2. [Record](#2-record)
3. [Record Builder](#3-record-builder)
4. [Mapper Implementation](#4-mapper-implementation)
5. [Recorder-factory Implementation](#5-recorder-factory-implementation)
6. [Parser Implementation](#6-parser-implementation)
7. [Usage](#7-usage)
8. [Dependency Injection](#8-dependency-injection)
   1. [Recorder-factory Abstraction](#81-recorder-factory-abstraction)
   2. [Parser Abstraction](#82-parser-abstraction)
   3. [Service Registration](#83-service-registration)
   4. [Mapper Dependencies](#84-mapper-dependencies)
   5. [Usage](#85-usage)

#### 1. Attribute Class

First, the attribute-class is defined.

```csharp
public sealed class ExampleAttribute<T> : Attribute
{
    public StringComparison ConstructorArgument { get; }
    public string? OptionalArgument { get; }
    public int[] ParamsArgument { get; }

    public Type? NamedArgument { get; init; }

    public ExampleAttribute(StringComparison constructorArgument, string? optionalArgument = null, params int[] paramsArgument)
    {
        ConstructorArgument = constructorArgument;
        OptionalArgument = optionalArgument;
        ParamsArgument = paramsArgument;
    }
}
```

#### 2. Record

Next, the record representing parsed attributes is defined.

```csharp
interface IExampleRecord
{
    ITypeSymbol TypeArgument { get; }
    StringComparison ConstructorArgument { get; }
    string? OptionalArgument { get; }
    IReadOnlyList<int> ParamsArgument { get; }
    ITypeSymbol? NamedArgument { get; }

}
```

#### 3. Record Builder

To construct instances of the record, a builder is defined.

```csharp
interface IExampleRecordBuilder : IRecordBuilder<IExampleRecord>
{
    void WithTypeArgument(ITypeSymbol typeArgument);
    void WithConstructorArgument(StringComparison constructorArgument);
    void WithOptionalArgument(string? optionalArgument);
    void WithParamsArgument(IReadOnlyList<int> paramsArgument);
    void WithNamedArgument(ITypeSymbol? namedArgument);
}
```

#### 4. Mapper Implementation

A `Mapper` is implemented, extending the abstract class `ASemanticMapper`. Note that the type-argument of the base-class is `IExampleRecordBuilder`, rather than `IExampleRecord`.

It is also recommended to replace the literal `strings` with `nameof`. This will always work for named parameters, and will work for constructor parameters if the name is the same as the corresponding property (by default, differences in casing are ignored).

```csharp
class ExampleMapper : ASemanticMapper<IExampleRecordBuilder>
{
    protected override void AddMappings(IAppendableSemanticMappingRepository<ExampleRecord> repository)
    {
        repository.TypeParameters.AddIndexedMapping(0, (factory) => factory.Create(RecordTypeArgument));
        repository.ConstructorParameters.AddNamedMapping("constructorArgument", (factory) => factory.Create(ConstructorArgumentPattern, RecordConstructorArgument));
        repository.ConstructorParameters.AddNamedMapping("optionalArgument", (factory) => factory.Create(OptionalArgumentPattern, RecordOptionalArgument));
        repository.ConstructorParameters.AddNamedMapping("paramsArgument", (factory) => factory.Create(ParamsArgumentPattern, RecordParamsArgument));
        repository.NamedParameters.AddNamedMapping("NamedArgument", (factory) => factory.Create(NamedArgumentPattern, RecordNamedArgument));
    }

    IArgumentPattern<StringComparison> ConstructorArgumentPattern(IArgumentPatternFactory factory) => factory.Enum<StringComparison>();
    IArgumentPattern<string?> OptionalArgumentPattern(IArgumentPatternFactory factory) => factory.NullableString();
    IArgumentPattern<int[]> ParamsArgumentPattern(IArgumentPatternFactory factory) => factory.NonNullableArray(factory.Int());
    IArgumentPattern<ITypeSymbol?> NamedArgumentPattern(IArgumentPatternFactory factory) => factory.NullableType();

    void RecordTypeArgument(IExampleRecordBuilder recordBuilder, ITypeSymbol typeArgument) => recordBuilder.WithTypeArgument(typeArgument);
    void RecordConstructorArgument(IExampleRecordBuilder recordBuilder, StringComparison constructorArgument) => recordBuilder.WithConstructorArgument(constructorArgument);
    void RecordOptionalArgument(IExampleRecordBuilder recordBuilder, string? optionalArgument) => recordBuilder.WithOptionalArgument(optionalArgument);
    void RecordParamsArgument(IExampleRecordBuilder recordBuilder, int[] paramsArgument) => recordBuilder.WithParamsArgument(paramsArgument);
    void RecordNamedArgument(IExampleRecordBuilder recordBuilder, ITypeSymbol? namedArgument) => recordBuilder.WithNamedArgument(namedArgument);
}
```

#### 5. Recorder-factory Implementation

A `Recorder`-factory is implemented, which is responsible for constructing `Recorders`, using the `Mapper` implemented in the previous step.  The factory has an internal implementation of `IExampleRecordBuilder` - with invokations of `VerifyCanModify` that ensure that the `IExampleRecord` has not yet been built when attempting to modify it. The builder also overrides `CanBuildRecord`, which ensures that the `IExampleRecord` is in a valid state before building it. 

```csharp
class ExampleRecorderFactory
{
    private ISemanticRecorderFactory Factory { get; }
    private ISemanticMapper<IExampleRecordBuilder> Mapper { get; }

    public ExampleRecorderFactory(ISemanticRecorderFactory factory, ISemanticMapper<IExampleRecordBuilder> mapper)
    {
        Factory = factory;
        Mapper = mapper;
    }

    public ISemanticRecorder<IExampleRecord> Create() => Factory.Create<IExampleRecord, IExampleRecordBuilder>(Mapper, new ExampleRecordBuilder());

    private sealed class ExampleRecordBuilder : ARecordBuilder<IExampleRecord>, IExampleRecordBuilder
    {
        private ExampleRecord Target { get; } = new();

        protected override IExampleRecord GetRecord() => Target;
        protected override bool CanBuildRecord() => Target.TypeArgument is not null && Target.ParamsArgument is not null;

        void IExampleRecordBuilder.WithTypeArgument(ITypeSymbol typeArgument)
        {
            VerifyCanModify();

            Target.TypeArgument = typeArgument;
        }

        void IExampleRecordBuilder.WithConstructorArgument(StringComparison constructorArgument)
        {
            VerifyCanModify();

            Target.ConstructorArgument = constructorArgument;
        }

        void IExampleRecordBuilder.WithOptionalArgument(string? optionalArgument)
        {
            VerifyCanModify();

            Target.OptionalArgument = optionalArgument;
        }

        void IExampleRecordBuilder.WithParamsArgument(IReadOnlyList<int> paramsArgument)
        {
            VerifyCanModify();

            Target.ParamsArgument = paramsArgument;
        }

        void IExampleRecordBuilder.WithNamedArgument(ITypeSymbol? namedArgument)
        {
            VerifyCanModify();

            Target.NamedArgument = namedArgument;
        }

        private sealed class ExampleRecord : IExampleRecord
        {
            public ITypeSymbol TypeArgument { get; set; } = null!;
            public StringComparison ConstructorArgument { get; set; }
            public string? OptionalArgument { get; set; }
            public IReadOnlyList<int> ParamsArgument { get; set; } = null!;
            public ITypeSymbol? NamedArgument { get; set; }
        }
    }
}
```

#### 6. Parser Implementation

A `Parser` is implemented, connecting the `Parser` provided by `SharpAttributeParser` with the `Recorder`-factory defined in the previous step.

```csharp
class ExampleParser
{
    private ISemanticParser Parser { get; }
    private ExampleRecorderFactory RecorderFactory { get; }

    public ExampleParser(ISemanticParser parser, ExampleRecorderFactory recorderFactory)
    {
        Parser = parser;
        RecorderFactory = recorderFactory;
    }

    public IExampleRecord? TryParse(AttributeData attributeData)
    {
        var recorder = RecorderFactory.Create();

        if (Parser.TryParse(recorder, attributeData) is false)
        {
            return null;
        }

        return recorder.GetRecord();
    }
}

```

#### 7. Usage

At this point, we can parse attributes of type `ExampleAttribute<T>`.

```csharp
// The AttributeData representing the attribute
AttributeData attributeData;

var parser = new SemanticParser();
var recorderFactory = new SemanticRecorderFactory();

var mapper = new ExampleMapper();
var exampleRecorderFactory = new ExampleRecorderFactory(recorderFactory, mapper);
var exampleParser = new ExampleParser(parser, exampleRecorderFactory);

if (exampleParser.TryParse(attributeData) is IExampleRecord dataRecord)
{
    // Attribute was successfully parsed
}
```

#### 8. Dependency Injection

If Dependency Injection is used, the pattern can be further improved. Otherwise, the previous step demonstrates the final pattern.

##### 8.1. Recorder-factory Abstraction

Define an abstraction of `ExampleRecorderFactory`, and apply it to the implementation. The constructor for `ExampleParser` should also be modified to use this interface rather than the implementation.

```csharp
interface IExampleRecorderFactory
{
    ISemanticRecorder<IExampleRecord> Create();
}
```

##### 8.2. Parser Abstraction

Define an abstraction of `ExampleParser`, and apply it to the implementation.

```csharp
interface IExampleParser
{
    IExampleRecord? TryParse(AttributeData attributeData);
}
```

##### 8.3. Service Registration

Register `ExampleMapper`, `ExampleRecorderFactory`, and `ExampleParser` with a `IServiceCollection`.

```csharp
// Register dependencies provided by 'SharpAttributeParser.Mappers'. Requires the add-on package 'SharpAttributeParser.Mappers.DependencyInjection'
services.AddSharpAttributeParserMappers();

services.AddSingleton<ISemanticMapper<IExampleRecordBuilder>, ExampleMapper>();
services.AddSingleton<IExampleRecorderFactory, ExampleRecorderFactory>();
services.AddSingleton<IExampleParser, ExampleParser>();
```

##### 8.4 Mapper Dependencies

Optionally, the abstract `Mapper` has some dependencies that can be injected. Add a constructor to `ExampleMapper` which passes the dependencies to the base-class.

```csharp

public ExampleMapper(ISemanticMapperDependencyProvider<IExampleRecordBuilder> dependencyProvider) : base(dependencyProvider) { }
```

##### 8.5 Usage

The `IExampleParser` service can now be injected.

```csharp
// The AttributeData representing the attribute
AttributeData attributeData;

// Service is injected
IExampleParser parser;

if (parser.TryParse(attributeData) is IExampleRecord dataRecord)
{
    // Attribute was successfully parsed
}
```