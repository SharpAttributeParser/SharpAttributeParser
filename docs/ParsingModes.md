# Parsing Modes

`SharpAttributeParser` defines two parsing modes: **semantic** and **syntactic**. These two modes can also be applied simultaneously, to perform **combined** parsing.

The following attribute will be used as an example throughout this article:

```csharp
[Example<Type>(new[] { 0, 1, 1, 2 }, name: "Fib", Answer = 41 + 1)]
class Foo { }
```

##### Semantic Parsing

Semantic parsing is the most common mode, and involves recording the actual attribute arguments. The following structure could be used to represent the result of semantically parsing the attribute shown above:

```csharp
interface ISemanticExampleRecord
{
    ITypeSymbol T { get; }
    IReadOnlyList<int> Sequence { get; }
    string Name { get; }
    int? Answer { get; }
}
```

##### Syntactic Parsing

> Syntactic parsing is not always possible, as it requires access to the `AttributeSyntax` - which may not be available for imported types.

Syntactic parsing records how each attribute argument was expressed. When using a syntactic `Parser`, the `Recorder` will be provided `Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax`, which can be further analyzed. For example, the `Microsoft.CodeAnalysis.Location` of each argument can be extracted - useful when issuing diagnostics about some attribute argument. The following structure could be used to represent the result of syntactically parsing the attribute shown above:

```csharp
interface ISyntacticExampleRecord
{
    ExpressionSyntax T { get; }
    ExpressionSyntax Sequence { get; }
    ExpressionSyntax Name { get; }
    ExpressionSyntax Answer { get; }
}
```