namespace SharpAttributeParser.ASyntacticAttributeMapperCases;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Moq;

using OneOf;

using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

public sealed class AddTypeParameterMappings
{
    [Fact]
    public void UnmodifiedMappings_TryMapTypeParameterReturnsNull()
    {
        UnmodifiedMapper mapper = new();

        var recorder = mapper.TryMapTypeParameter(Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == string.Empty), new Data());

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
    public void EmptyMappingsCollection_TryMapTypeParameterReturnsNull()
    {
        EmptyMappingsCollectionMapper mapper = new();

        var recorder = mapper.TryMapTypeParameter(Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == string.Empty), new Data());

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
    public void NegativeIndex_InvalidOperationExceptionWhenInitialized()
    {
        NegativeIndexMapper mapper = new();

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
    public void DuplicateIndex_InvalidOperationExceptionWhenInitialized()
    {
        DuplicateIndexMapper mapper = new();

        var exception = Record.Exception(mapper.Initialize);

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void MatchingName_TryMapTypeParameterReturnsRecorder()
    {
        NameMapper mapper = new();

        var recorder = mapper.TryMapTypeParameter(Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == NameMapper.ParameterName), new Data());

        Assert.NotNull(recorder);
    }

    [Fact]
    public void NotMatchingName_TryMapTypeParameterReturnsNull()
    {
        NameMapper mapper = new();

        var recorder = mapper.TryMapTypeParameter(Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == NameMapper.DifferentParameterName), new Data());

        Assert.Null(recorder);
    }

    [Fact]
    public void MatchingIndex_TryMapTypeParameterReturnsRecorder()
    {
        IndexMapper mapper = new();

        var recorder = mapper.TryMapTypeParameter(Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == string.Empty && symbol.Ordinal == IndexMapper.ParameterIndex), new Data());

        Assert.NotNull(recorder);
    }

    [Fact]
    public void NotMatchingIndex_TryMapTypeParameterReturnsNull()
    {
        IndexMapper mapper = new();

        var recorder = mapper.TryMapTypeParameter(Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == string.Empty && symbol.Ordinal == IndexMapper.DifferentParameterIndex), new Data());

        Assert.Null(recorder);
    }

    private sealed class UnmodifiedMapper : ASyntacticAttributeMapper<Data> { }

    private sealed class NullMappingsCollectionMapper : ASyntacticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(OneOf<int, string>, DTypeArgumentSyntaxRecorder)> AddTypeParameterMappings() => null!;
    }

    private sealed class EmptyMappingsCollectionMapper : ASyntacticAttributeMapper<Data>
    {
        protected override IEnumerable<(OneOf<int, string>, DTypeArgumentSyntaxRecorder)> AddTypeParameterMappings() => Enumerable.Empty<(OneOf<int, string>, DTypeArgumentSyntaxRecorder)>();
    }

    private sealed class NullNameMapper : ASyntacticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(OneOf<int, string>, DTypeArgumentSyntaxRecorder)> AddTypeParameterMappings()
        {
            yield return (null!, RecordValue);
        }

        private static bool RecordValue(Data dataRecord, ExpressionSyntax syntax) => true;
    }

    private sealed class NegativeIndexMapper : ASyntacticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(OneOf<int, string>, DTypeArgumentSyntaxRecorder)> AddTypeParameterMappings()
        {
            yield return (-1, RecordValue);
        }

        private static bool RecordValue(Data dataRecord, ExpressionSyntax syntax) => true;
    }

    private sealed class NullDelegateMapper : ASyntacticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(OneOf<int, string>, DTypeArgumentSyntaxRecorder)> AddTypeParameterMappings()
        {
            yield return (0, null!);
        }
    }

    private sealed class DuplicateNameMapper : ASyntacticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(OneOf<int, string>, DTypeArgumentSyntaxRecorder)> AddTypeParameterMappings()
        {
            yield return (string.Empty, RecordValue);
            yield return (string.Empty, RecordValue);
        }

        private static bool RecordValue(Data dataRecord, ExpressionSyntax syntax) => true;
    }

    private sealed class DuplicateNameDueToComparerMapper : ASyntacticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEqualityComparer<string> GetComparer() => StringComparer.OrdinalIgnoreCase;

        protected override IEnumerable<(OneOf<int, string>, DTypeArgumentSyntaxRecorder)> AddTypeParameterMappings()
        {
            yield return ("A", RecordValue);
            yield return ("a", RecordValue);
        }

        private static bool RecordValue(Data dataRecord, ExpressionSyntax syntax) => true;
    }

    private sealed class DuplicateIndexMapper : ASyntacticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(OneOf<int, string>, DTypeArgumentSyntaxRecorder)> AddTypeParameterMappings()
        {
            yield return (0, RecordValue);
            yield return (0, RecordValue);
        }

        private static bool RecordValue(Data dataRecord, ExpressionSyntax syntax) => true;
    }

    private sealed class NameMapper : ASyntacticAttributeMapper<Data>
    {
        public static string ParameterName => string.Empty;
        public static string DifferentParameterName => $"{ParameterName} ";

        protected override IEnumerable<(OneOf<int, string>, DTypeArgumentSyntaxRecorder)> AddTypeParameterMappings()
        {
            yield return (ParameterName, RecordValue);
        }

        private static bool RecordValue(Data dataRecord, ExpressionSyntax syntax) => true;
    }

    private sealed class IndexMapper : ASyntacticAttributeMapper<Data>
    {
        public static int ParameterIndex => 0;
        public static int DifferentParameterIndex => ParameterIndex + 1;

        protected override IEnumerable<(OneOf<int, string>, DTypeArgumentSyntaxRecorder)> AddTypeParameterMappings()
        {
            yield return (ParameterIndex, RecordValue);
        }

        private static bool RecordValue(Data dataRecord, ExpressionSyntax syntax) => true;
    }
}
