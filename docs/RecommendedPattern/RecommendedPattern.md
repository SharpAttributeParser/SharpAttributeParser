# Recommended Pattern

This article will present the *recommended* pattern. It is *recommended* in the sense that it is what I use in my personal projects, but it is by no means the only way of doing things. Discussion will be limited to semantic parsing, but the pattern can be modified for syntactic or combined parsing.

1. [Attribute Definition](#1-attribute-definition)
2. [Representation Structure](#2-representation-structure)
3. [Representation Builder](#3-representation-builder)
4. [Mapper Implementation](#4-mapper-implementation)
5. [Recorder Factory Implementation](#5-recorder-factory-implementation)
6. [Usage](#6-usage)
7. [Dependency Injection](#7-dependency-injection)
   1. [Recorder Factory Abstraction](#71-recorder-factory-abstraction)
   2. [Service Registration](#72-service-registration)
   3. [Usage](#73-usage)

#### 1. Attribute Definition

First, the attribute that will be used as an example throughout this article:

```csharp
[Example<Type>(new[] { 0, 1, 1, 2 }, name: "Fib", Answer = 41 + 1)]
class Foo { }
```

#### 2. Representation Structure

The desired result of parsing the attribute is the following structure:

```csharp
interface IExampleRecord
{
    ITypeSymbol T { get; }
    IReadOnlyList<int> Sequence { get; }
    string Name { get; }
    int? Answer { get; }
}
```

#### 3. Representation Builder

Note that the representation shown above, `IExampleRecord`, intentionally lacks setters. We define a builder, responsible for building instances of `IExampleRecord`. The builder extends `IRecordBuilder`, which is provided by `SharpAttributeParser`.

```csharp
interface IExampleRecordBuilder : IRecordBuilder<IExampleRecord>
{
    void WithT(ITypeSymbol t);
    void WithSequence(IReadOnlyList<int> sequence);
    void WithName(string name);
    void WithAnswer(int answer);
}
```

#### 4. Mapper Implementation

We implement our `Mapper`, extending the abstract class `ASemanticAttributeMapper`. Note that the type-argument to the base-class is `IExampleRecordBuilder`, rather than `IExampleRecord`.

```csharp
class ExampleMapper : ASemanticAttributeMapper<IExampleRecordBuilder>
{
    protected override IEnumerable<(OneOf<int, string>, DTypeArgumentRecorder)> AddTypeParameterMappings()
    {
        yield return (0, Adapters.TypeArgument.For(RecordT));
    }

    protected override IEnumerable<(string, DArgumentRecorder)> AddParameterMappings()
    {
        yield return (nameof(ExampleAttribute<object>.Sequence), Adapters.ArrayArgument.For<int>(RecordSequence));
        yield return (nameof(ExampleAttribute<object>.Name), Adapters.SimpleArgument.For<string>(RecordName));
        yield return (nameof(ExampleAttribute<object>.Answer), Adapters.SimpleArgument.For<int>(RecordAnswer));
    }

    void RecordT(IExampleRecordBuilder builder, ITypeSymbol t) => builder.WithT(t);
    void RecordSequence(IExampleRecordBuilder builder, IReadOnlyList<int> sequence) => builder.WithSequence(sequence);
    void RecordName(IExampleRecordBuilder builder, string name) => builder.WithName(name);
    void RecordAnswer(IExampleRecordBuilder builder, int answer) => builder.WithAnswer(answer);
}
```

#### 5. Recorder Factory Implementation

We implement a `Recorder`-factory, which uses a `Mapper` to construct `Recorders`.

```csharp
class ExampleRecorderFactory
{
    private ISemanticAttributeRecorderFactory Factory { get; }
    private ISemanticAttributeMapper<IExampleRecordBuilder> Mapper { get; }

    public ExampleRecorderFactory(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<IExampleRecordBuilder> mapper)
    {
        Factory = factory;
        Mapper = mapper;
    }

    ISemanticAttributeRecorder<IExampleRecord> IExampleRecorderFactory.Create()
    {
        return Factory.Create<IExampleRecord, IExampleRecordBuilder>(Mapper, new ExampleRecordBuilder());
    }

    class ExampleRecordBuilder : ARecordBuilder<IExampleRecord>, IExampleRecordBuilder
    {
        private ExampleRecord Target { get; } = new();

        public void WithT(ITypeSymbol t)
        {
            VerifyCanModify();

            Target.T = t;
        }

        public void WithSequence(IReadOnlyList<int> sequence)
        {
            VerifyCanModify();

            Target.Sequence = sequence;
        }

        public void WithName(string name)
        {
            VerifyCanModify();

            Target.Name = name;
        }

        public void WithAnswer(int answer)
        {
            VerifyCanModify();

            Target.Answer = answer;
        }

        protected override IExampleRecord GetTarget() => Target;
        protected override bool CheckFullyConstructed() => Target.T is not null && Target.Sequence is not null && Target.Name is not null;

        private sealed class ExampleRecord : IExampleRecord
        {
            public ITypeSymbol T { get; set; } = null!;
            public IReadOnlyList<int> Sequence { get; set; } = null!;
            public string Name { get; set; } = null!;
            public int? Answer { get; set; }
        }
    }
}
```

#### 6. Usage

At this point, we are able to parse the attribute:

```csharp
// The AttributeData representing the attribute.
AttributeData attributeData;

SemanticAttributeParser parser = new SemanticAttributeParser();

ExampleMapper mapper = new ExampleMapper();

SemanticAttributeRecorderFactory generalFactory = new SemanticAttributeRecorderFactory();
ExampleRecorderFactory factory = new ExampleRecorderFactory(generalFactory, mapper);

ISemanticAttributeRecorder<IExampleRecord> recorder = factory.Create();

bool outcome = parser.TryParse(recorder, attributeData);

if (outcome is true)
{
    IExampleRecord result = recorder.GetRecord();
}
```

#### 7. Dependency Injection

If Dependency Injection is used, the usage can be further simplified. Otherwise, step 6 presents the final usage.

##### 7.1. Recorder Factory Abstraction

We define an abstraction of out `ExampleRecorderFactory`. Of course, the `ExampleRecorderFactory` needs to implement this interface.

```csharp
interface IExampleRecorderFactory
{
    ISemanticAttributeRecorder<IExampleRecord> Create();
}
```

##### 7.2. Service Registration

The implemented `ExampleMapper` and `ExampleRecorderFactory` can now be registered with a `IServiceCollection`. The other dependencies can be registered by invoking `AddSharpAttributeParser`.

```csharp
using SharpAttributeParser.Extensions;

services.AddSingleton<ISemanticAttributeMapper<IExampleRecordBuilder>, ExampleMapper>();
services.AddSingleton<IExampleRecorderFactory, ExampleRecorderFactory>();

services.AddSharpAttributeParser();
```

##### 7.3 Usage

The `IExampleRecorderFactory` service can now be injected - and this is how we apply everything:

```csharp
// The AttributeData representing the attribute.
AttributeData attributeData;

// Service is injected through DI.
IExampleRecorderFactory factory;

ISemanticAttributeRecorder<IExampleRecord> recorder = factory.Create();

bool outcome = parser.TryParse(recorder, attributeData);

if (outcome is true)
{
    IExampleRecord result = recorder.GetRecord();
}
```