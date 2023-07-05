namespace SharpAttributeParser.ASemanticAttributeMapperCases;

using Microsoft.CodeAnalysis;

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
    public void TryMapTypeArgument_DifferentStringsButMatching_ComparerInvokedAndRecordedProduced()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        ComparerMapper mapper = new(comparerMock.Object);

        var recorder = mapper.TryMapTypeParameter(new Data(), Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == string.Empty));

        Assert.NotNull(recorder);

        StringComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryMapConstructorArgument_DifferentStringsButMatching_ComparerInvokedAndRecordedProduced()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        ComparerMapper mapper = new(comparerMock.Object);

        var recorder = mapper.TryMapConstructorParameter(new Data(), Mock.Of<IParameterSymbol>((symbol) => symbol.Name == string.Empty));

        Assert.NotNull(recorder);

        StringComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryMapNamedArgument_DifferentStringsButMatching_ComparerInvokedAndRecordedProduced()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        ComparerMapper mapper = new(comparerMock.Object);

        var recorder = mapper.TryMapNamedParameter(new Data(), string.Empty);

        Assert.NotNull(recorder);

        StringComparerMock.VerifyInvoked(comparerMock);
    }

    private sealed class ComparerMapper : ASemanticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        private IEqualityComparer<string> Comparer { get; }

        public ComparerMapper(IEqualityComparer<string> comparer)
        {
            Comparer = comparer;
        }

        protected override IEqualityComparer<string> GetComparer() => Comparer;

        protected override IEnumerable<(OneOf<int, string> Index, DSemanticAttributeTypeArgumentRecorder Mapping)> AddTypeParameterMappings()
        {
            yield return (string.Empty, RecordT);
        }

        protected override IEnumerable<(string Name, DSemanticAttributeArgumentRecorder Mapping)> AddParameterMappings()
        {
            yield return (string.Empty, RecordValue);
        }

        private static bool RecordT(Data dataRecord, ITypeSymbol argument) => true;
        private static bool RecordValue(Data dataRecord, object? argument) => true;
    }
}
