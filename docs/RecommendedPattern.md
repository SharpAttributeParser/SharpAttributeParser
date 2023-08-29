# Recommended Pattern

> The add-on packages [SharpAttributeParser.Mappers](https://www.nuget.org/packages/SharpAttributeParser.Mappers/) and [SharpAttributeParser.RecordBuilders](https://www.nuget.org/packages/SharpAttributeParser.RecordBuilders/) are required for the recommended pattern.

This article will present the recommended pattern. It is *recommended* in the sense that it is what I use in my personal projects, but is not necessarily the best way of using `SharpAttributeParser`. The article will assume that attributes are parsed semantically, but the pattern can easily be modified for the syntactic and combined [parsing modes](ParsingModes.md).

1. [Attribute Class](#1-attribute-class)
2. [Record](#2-record)
3. [Record Builder](#3-record-builder)
4. [Mapper Implementation](#4-mapper-implementation)
5. [Recorder-factory Implementation](#5-recorder-factory-implementation)
6. [Usage](#6-usage)
7. [Dependency Injection](#7-dependency-injection)
   1. [Recorder-factory Abstraction](#71-recorder-factory-abstraction)
   2. [Service Registration](#72-service-registration)
   3. [Usage](#73-usage)

#### 1. Attribute Class

First, we define the attribute-class.

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

Next, we define the record - which will represent a parsed attribute.

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

To construct instances of the record, we define a builder.

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

We implement our `Mapper`, extending the abstract class `ASemanticMapper`. Note that the type-argument of the base-class is `IExampleRecordBuilder`, rather than `IExampleRecord`.

It is also recommended to replace the literal `strings` with `nameof`. This will always work for named parameters, and will work for constructor parameters if the name is the same as the corresponding property (different casing is supported).

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

We implement a `Recorder`-factory, which is responsible for constructing `Recorders`, using `Mappers` - and able to create multiple independent `Recorders`.  The factory has an internal implementation of `IExampleRecordBuilder` - with invokations of `VerifyCanModify` that ensure that the `IExampleRecord` has not yet been built when attempting to modify it. The builder also overrides `CanBuildRecord`, which ensures that the `IExampleRecord` is in a valid state before building it. 

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

#### 6. Usage

At this point, we can parse attributes of type `ExampleAttribute<T>`.

```csharp
// The AttributeData representing the attribute
AttributeData attributeData;

var parser = new SemanticParser();

var mapper = new ExampleMapper();

var recorderFactory = new SemanticRecorderFactory();
var exampleRecorderFactory = new ExampleRecorderFactory(recorderFactory, mapper);

ISemanticRecorder<IExampleRecord> recorder = exampleRecorderFactory.Create();

bool success = parser.TryParse(recorder, attributeData);

if (success)
{
    IExampleRecord dataRecord = recorder.GetRecord();
}
```

#### 7. Dependency Injection

If Dependency Injection is used, the pattern can be further improved.

##### 7.1. Recorder-factory Abstraction

Define an abstraction of `ExampleRecorderFactory`, and apply it to the implementation.

```csharp
interface IExampleRecorderFactory
{
    ISemanticAttributeRecorder<IExampleRecord> Create();
}
```

##### 7.2. Service Registration

Register `ExampleMapper` and `ExampleRecorderFactory` with a `IServiceCollection`.

```csharp
// Register dependencies provided by SharpAttributeParser.Mappers. Requires the add-on package SharpAttributeParser.Mappers.DependencyInjection
services.AddSharpAttributeParserMappers();

services.AddSingleton<ISemanticMapper<IExampleRecordBuilder>, ExampleMapper>();
services.AddSingleton<IExampleRecorderFactory, ExampleRecorderFactory>();
```

##### 7.3 Usage

The `IExampleRecorderFactory` service can now be injected.

```csharp
// The AttributeData representing the attribute.
AttributeData attributeData;

// Service is injected through DI.
IExampleRecorderFactory recorderFactory;

ISemanticAttributeRecorder<IExampleRecord> recorder = recorderFactory.Create();

bool success = parser.TryParse(recorder, attributeData);

if (success)
{
    IExampleRecord dataRecord = recorder.GetRecord();
}
```