namespace SharpAttributeParser.ASplitAttributeMapperCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class GetComparer
{
    [Fact]
    public void Null_InvalidOperationExceptionWhenInitialized()
    {
        ComparerMapper mapper = new(null!);

        var exception = Record.Exception(mapper.Initialize);

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void TryMapTypeArgument_Semantic_DifferentStringsButMatching_ComparerInvokedAndRecorderProduced()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        ComparerMapper mapper = new(comparerMock.Object);

        var recorder = mapper.TryMapTypeParameter(Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == string.Empty), new SemanticData());

        Assert.NotNull(recorder);

        StringComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryMapTypeArgument_Syntactic_DifferentStringsButMatching_ComparerInvokedAndRecorderProduced()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        ComparerMapper mapper = new(comparerMock.Object);

        var recorder = mapper.TryMapTypeParameter(Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == string.Empty), new SyntacticData());

        Assert.NotNull(recorder);

        StringComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryMapConstructorArgument_Semantic_DifferentStringsButMatching_ComparerInvokedAndRecorderProduced()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        ComparerMapper mapper = new(comparerMock.Object);

        var recorder = mapper.TryMapConstructorParameter(Mock.Of<IParameterSymbol>((symbol) => symbol.Name == string.Empty), new SemanticData());

        Assert.NotNull(recorder);

        StringComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryMapConstructorArgument_Syntactic_DifferentStringsButMatching_ComparerInvokedAndRecorderProduced()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        ComparerMapper mapper = new(comparerMock.Object);

        var recorder = mapper.TryMapConstructorParameter(Mock.Of<IParameterSymbol>((symbol) => symbol.Name == string.Empty), new SyntacticData());

        Assert.NotNull(recorder);

        StringComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryMapNamedArgument_Semantic_DifferentStringsButMatching_ComparerInvokedAndRecorderProduced()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        ComparerMapper mapper = new(comparerMock.Object);

        var recorder = mapper.TryMapNamedParameter(string.Empty, new SemanticData());

        Assert.NotNull(recorder);

        StringComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryMapNamedArgument_Syntactic_DifferentStringsButMatching_ComparerInvokedAndRecorderProduced()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        ComparerMapper mapper = new(comparerMock.Object);

        var recorder = mapper.TryMapNamedParameter(string.Empty, new SyntacticData());

        Assert.NotNull(recorder);

        StringComparerMock.VerifyInvoked(comparerMock);
    }

    private sealed class ComparerMapper : ASplitAttributeMapper<SemanticData, SyntacticData>
    {
        public void Initialize() => InitializeMapper();

        private IEqualityComparer<string> Comparer { get; }

        public ComparerMapper(IEqualityComparer<string> comparer)
        {
            Comparer = comparer;
        }

        protected override IEqualityComparer<string> GetComparer() => Comparer;

        protected override IEnumerable<(OneOf<int, string> IndexOrName, ITypeArgumentRecorderProvider Recorders)> AddTypeParameterMappings()
        {
            yield return (string.Empty, new TypeArgumentRecorderProvider(RecordT, RecordT));
        }

        protected override IEnumerable<(string Name, IArgumentRecorderProvider Recorders)> AddParameterMappings()
        {
            yield return (string.Empty, new ArgumentRecorderProvider(RecordValue, RecordValue));
        }

        private static bool RecordT(SemanticData dataRecord, ITypeSymbol argument) => true;
        private static bool RecordT(SyntacticData dataRecord, ExpressionSyntax syntax) => true;

        private static bool RecordValue(SemanticData dataRecord, object? argument) => true;
        private static bool RecordValue(SyntacticData dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> argument) => true;
    }
}
