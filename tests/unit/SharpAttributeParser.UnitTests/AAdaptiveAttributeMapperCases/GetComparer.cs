namespace SharpAttributeParser.AAdaptiveAttributeMapperCases;

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
    public void TryMapTypeArgument_Shared_DifferentStringsButMatching_ComparerInvokedAndRecorderProduced()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        ComparerMapper mapper = new(comparerMock.Object);

        var recorder = mapper.TryMapTypeParameter(Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == string.Empty), new SharedData());

        Assert.NotNull(recorder);

        StringComparerMock.VerifyInvoked(comparerMock);
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
    public void TryMapConstructorArgument_Shared_DifferentStringsButMatching_ComparerInvokedAndRecorderProduced()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        ComparerMapper mapper = new(comparerMock.Object);

        var recorder = mapper.TryMapConstructorParameter(Mock.Of<IParameterSymbol>((symbol) => symbol.Name == string.Empty), new SharedData());

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
    public void TryMapNamedArgument_Shared_DifferentStringsButMatching_ComparerInvokedAndRecorderProduced()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        ComparerMapper mapper = new(comparerMock.Object);

        var recorder = mapper.TryMapNamedParameter(string.Empty, new SharedData());

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

    private sealed class ComparerMapper : AAdaptiveAttributeMapper<SharedData, SemanticData>
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

        private static bool RecordT(SharedData dataRecord, ITypeSymbol argument, ExpressionSyntax syntax) => true;
        private static bool RecordT(SemanticData dataRecord, ITypeSymbol argument) => true;

        private static bool RecordValue(SharedData dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => true;
        private static bool RecordValue(SemanticData dataRecord, object? argument) => true;
    }
}
