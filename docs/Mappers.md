# Attribute Mappers

> `Mappers` are not an essential component of `SharpAttributeParser`, but is used in the [recommended pattern](RecommendedPattern/RecommendedPattern.md). If using another pattern, you will likely need to implement a `Recorder` instead, which you can read more about [here](Recorders.md).

Each `Parser` of `SharpAttributeParser` requires a user-provided `Recorder`-object. The role of `Mappers` is to act as blueprints when constructing such `Recorders`. Below is part of the API of the service `ISemanticAttributeRecorderFactory`, which uses the provided `Mapper` to construct a `ISemanticAttributeRecorder`. Similar patterns exists for each [parsing mode](ParsingModes.md), but this article will only consider semantic parsing.

```csharp
ISemanticAttributeRecorder Create<TRecord>(ISemanticAttributeMapper<TRecord>, TRecord);
```

As can be deduced, the `Recorder` is closely linked to the provided `TRecord` (which represents the parsed data), while the `Mapper` is kept separated from the instantiated `TRecord`. This separation is one of the main benefits of using `Mappers` over `Recorders`.

#### Usage

The API of `ISemanticAttributeMapper` is shown below. The produced `ISemanticAttributeArgumentRecorder` is responsible for recording arguments to the provided `TRecord`.

```csharp
ISemanticAttributeArgumentRecorder? TryMapTypeParameter(ITypeParameterSymbol, TRecord);
ISemanticAttributeArgumentRecorder? TryMapConstructorParameter(IParameterSymbol, TRecord);
ISemanticAttributeArgumentRecorder? TryMapNamedParameter(string, TRecord);
```

The first argument passed to the `Mapper` identifies an attribute parameter, while the second argument represents the `TRecord` to which the parsed argument of that parameter will be recorded (by the produced `ISemanticAttributeArgumentRecorder`).

#### Implementation

Typically, each distinct attribute-type that should be parse-able will require separate `Mapper`-implementations.

In most cases, each implementation of a `Mapper` may extend the abstract class `ASemanticAttributeMapper`. Below is an example of such an implementation. The [recommended pattern](RecommendedPattern/RecommendedPattern.md) presents some minor adjustments to this example.

First, the attribute that is handled by the example:

```csharp
[Example<Type>(new[] { 0, 1, 1, 2 }, name: "Fib", Answer = 41 + 1)]
class Foo { }
```

... the type representing the parsed data:

```csharp
interface IExampleRecord
{
    ITypeSymbol T { get; set; }
    IReadOnlyList<int> Sequence { get; set; }
    string Name { get; set; }
    int? Answer { get; set; }
}
```

... and, finally, the implemented `Mapper`:

```csharp
class ExampleMapper : ASemanticAttributeMapper<IExampleRecord>
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

    void RecordT(IExampleRecord dataRecord, ITypeSymbol t) => dataRecord.T = t;
    void RecordSequence(IExampleRecord dataRecord, IReadOnlyList<int> sequence) => dataRecord.Sequence = sequence;
    void RecordName(IExampleRecord dataRecord, string name) => dataRecord.Name = name;
    void RecordAnswer(IExampleRecord dataRecord, int answer) => dataRecord.Answer = answer;
}
```