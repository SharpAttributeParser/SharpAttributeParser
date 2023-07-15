namespace SharpAttributeParser.ASyntacticAttributeMapperCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;

using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

public sealed class AddParameterMappings
{
    [Fact]
    public void UnmodifiedMappings_TryMapConstructorParameterReturnsNull()
    {
        UnmodifiedMapper mapper = new();

        var recorder = mapper.TryMapConstructorParameter(Mock.Of<IParameterSymbol>((symbol) => symbol.Name == string.Empty), new Data());

        Assert.Null(recorder);
    }

    [Fact]
    public void UnmodifiedMappings_TryMapNamedParameterReturnsNull()
    {
        UnmodifiedMapper mapper = new();

        var recorder = mapper.TryMapNamedParameter(string.Empty, new Data());

        Assert.Null(recorder);
    }

    [Fact]
    public void NullMappingsCollection_InvalidOperationExceptionWhenInitialized()
    {
        NullMappingsCollectionMapper mapper = new();

        var exception = Record.Exception(mapper.Initialize);

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void EmptyMappingsCollection_TryMapConstructorParameterReturnsNull()
    {
        EmptyMappingsCollectionMapper mapper = new();

        var recorder = mapper.TryMapConstructorParameter(Mock.Of<IParameterSymbol>((symbol) => symbol.Name == string.Empty), new Data());

        Assert.Null(recorder);
    }

    [Fact]
    public void EmptyMappingsCollection_TryMapNamedParameterReturnsNull()
    {
        EmptyMappingsCollectionMapper mapper = new();

        var recorder = mapper.TryMapNamedParameter(string.Empty, new Data());

        Assert.Null(recorder);
    }

    [Fact]
    public void NullName_InvalidOperationExceptionWhenInitialized()
    {
        NullNameMapper mapper = new();

        var exception = Record.Exception(mapper.Initialize);

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void NullDelegate_InvalidOperationExceptionWhenInitialized()
    {
        NullDelegateMapper mapper = new();

        var exception = Record.Exception(mapper.Initialize);

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void DuplicateName_InvalidOperationExceptionWhenInitialized()
    {
        DuplicateNameMapper mapper = new();

        var exception = Record.Exception(mapper.Initialize);

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void DuplicateNameDueToComparer_InvalidOperationExceptionWhenInitialized()
    {
        DuplicateNameDueToComparerMapper mapper = new();

        var exception = Record.Exception(mapper.Initialize);

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void MatchingName_TryMapConstructorParameterReturnsRecorder()
    {
        NameMapper mapper = new();

        var recorder = mapper.TryMapConstructorParameter(Mock.Of<IParameterSymbol>((symbol) => symbol.Name == NameMapper.ParameterName), new Data());

        Assert.NotNull(recorder);
    }

    [Fact]
    public void MatchingName_TryMapNamedParameterReturnsRecorder()
    {
        NameMapper mapper = new();

        var recorder = mapper.TryMapNamedParameter(NameMapper.ParameterName, new Data());

        Assert.NotNull(recorder);
    }

    [Fact]
    public void NotMatchingName_TryMapConstructorParameterReturnsNull()
    {
        NameMapper mapper = new();

        var recorder = mapper.TryMapConstructorParameter(Mock.Of<IParameterSymbol>((symbol) => symbol.Name == NameMapper.DifferentParameterName), new Data());

        Assert.Null(recorder);
    }

    [Fact]
    public void NotMatchingName_TryMapNamedParameterReturnsNull()
    {
        NameMapper mapper = new();

        var recorder = mapper.TryMapNamedParameter(NameMapper.DifferentParameterName, new Data());

        Assert.Null(recorder);
    }

    private sealed class UnmodifiedMapper : ASyntacticAttributeMapper<Data> { }

    private sealed class NullMappingsCollectionMapper : ASyntacticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(string, DArgumentSyntaxRecorder)> AddParameterMappings() => null!;
    }

    private sealed class EmptyMappingsCollectionMapper : ASyntacticAttributeMapper<Data>
    {
        protected override IEnumerable<(string, DArgumentSyntaxRecorder)> AddParameterMappings() => Enumerable.Empty<(string, DArgumentSyntaxRecorder)>();
    }

    private sealed class NullNameMapper : ASyntacticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(string, DArgumentSyntaxRecorder)> AddParameterMappings()
        {
            yield return (null!, RecordValue);
        }

        private static bool RecordValue(Data dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => true;
    }

    private sealed class NullDelegateMapper : ASyntacticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(string, DArgumentSyntaxRecorder)> AddParameterMappings()
        {
            yield return (string.Empty, null!);
        }
    }

    private sealed class DuplicateNameMapper : ASyntacticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(string, DArgumentSyntaxRecorder)> AddParameterMappings()
        {
            yield return (string.Empty, RecordValue);
            yield return (string.Empty, RecordValue);
        }

        private static bool RecordValue(Data dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => true;
    }

    private sealed class DuplicateNameDueToComparerMapper : ASyntacticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEqualityComparer<string> GetComparer() => StringComparer.OrdinalIgnoreCase;

        protected override IEnumerable<(string, DArgumentSyntaxRecorder)> AddParameterMappings()
        {
            yield return ("A", RecordValue);
            yield return ("a", RecordValue);
        }

        private static bool RecordValue(Data dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => true;
    }

    private sealed class NameMapper : ASyntacticAttributeMapper<Data>
    {
        public static string ParameterName => string.Empty;
        public static string DifferentParameterName => $"{ParameterName} ";

        protected override IEnumerable<(string, DArgumentSyntaxRecorder)> AddParameterMappings()
        {
            yield return (ParameterName, RecordValue);
        }

        private static bool RecordValue(Data dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => true;
    }
}
