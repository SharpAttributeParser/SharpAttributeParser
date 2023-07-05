namespace SharpAttributeParser.ASemanticAttributeMapperCases;

using Microsoft.CodeAnalysis;

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

        var recorder = mapper.TryMapTypeParameter(new Data(), Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == string.Empty));

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

        var recorder = mapper.TryMapTypeParameter(new Data(), Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == string.Empty));

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

        var recorder = mapper.TryMapTypeParameter(new Data(), Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == NameMapper.ParameterName));

        Assert.NotNull(recorder);
    }

    [Fact]
    public void NotMatchingName_TryMapTypeParameterReturnsNull()
    {
        NameMapper mapper = new();

        var recorder = mapper.TryMapTypeParameter(new Data(), Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == NameMapper.DifferentParameterName));

        Assert.Null(recorder);
    }

    [Fact]
    public void MatchingIndex_TryMapTypeParameterReturnsRecorder()
    {
        IndexMapper mapper = new();

        var recorder = mapper.TryMapTypeParameter(new Data(), Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == string.Empty && symbol.Ordinal == IndexMapper.ParameterIndex));

        Assert.NotNull(recorder);
    }

    [Fact]
    public void NotMatchingIndex_TryMapTypeParameterReturnsNull()
    {
        IndexMapper mapper = new();

        var recorder = mapper.TryMapTypeParameter(new Data(), Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == string.Empty && symbol.Ordinal == IndexMapper.DifferentParameterIndex));

        Assert.Null(recorder);
    }

    private sealed class UnmodifiedMapper : ASemanticAttributeMapper<Data> { }

    private sealed class NullMappingsCollectionMapper : ASemanticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(OneOf<int, string>, DSemanticAttributeTypeArgumentRecorder)> AddTypeParameterMappings() => null!;
    }

    private sealed class EmptyMappingsCollectionMapper : ASemanticAttributeMapper<Data>
    {
        protected override IEnumerable<(OneOf<int, string>, DSemanticAttributeTypeArgumentRecorder)> AddTypeParameterMappings() => Enumerable.Empty<(OneOf<int, string>, DSemanticAttributeTypeArgumentRecorder)>();
    }

    private sealed class NullNameMapper : ASemanticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(OneOf<int, string>, DSemanticAttributeTypeArgumentRecorder)> AddTypeParameterMappings()
        {
            yield return (null!, RecordValue);
        }

        private static bool RecordValue(Data dataRecord, ITypeSymbol argument) => true;
    }

    private sealed class NegativeIndexMapper : ASemanticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(OneOf<int, string>, DSemanticAttributeTypeArgumentRecorder)> AddTypeParameterMappings()
        {
            yield return (-1, RecordValue);
        }

        private static bool RecordValue(Data dataRecord, ITypeSymbol argument) => true;
    }

    private sealed class NullDelegateMapper : ASemanticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(OneOf<int, string>, DSemanticAttributeTypeArgumentRecorder)> AddTypeParameterMappings()
        {
            yield return (0, null!);
        }
    }

    private sealed class DuplicateNameMapper : ASemanticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(OneOf<int, string>, DSemanticAttributeTypeArgumentRecorder)> AddTypeParameterMappings()
        {
            yield return (string.Empty, RecordValue);
            yield return (string.Empty, RecordValue);
        }

        private static bool RecordValue(Data dataRecord, ITypeSymbol argument) => true;
    }

    private sealed class DuplicateNameDueToComparerMapper : ASemanticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEqualityComparer<string> GetComparer() => StringComparer.OrdinalIgnoreCase;

        protected override IEnumerable<(OneOf<int, string>, DSemanticAttributeTypeArgumentRecorder)> AddTypeParameterMappings()
        {
            yield return ("A", RecordValue);
            yield return ("a", RecordValue);
        }

        private static bool RecordValue(Data dataRecord, ITypeSymbol argument) => true;
    }

    private sealed class DuplicateIndexMapper : ASemanticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(OneOf<int, string>, DSemanticAttributeTypeArgumentRecorder)> AddTypeParameterMappings()
        {
            yield return (0, RecordValue);
            yield return (0, RecordValue);
        }

        private static bool RecordValue(Data dataRecord, ITypeSymbol argument) => true;
    }

    private sealed class NameMapper : ASemanticAttributeMapper<Data>
    {
        public static string ParameterName => string.Empty;
        public static string DifferentParameterName => $"{ParameterName} ";

        protected override IEnumerable<(OneOf<int, string>, DSemanticAttributeTypeArgumentRecorder)> AddTypeParameterMappings()
        {
            yield return (ParameterName, RecordValue);
        }

        private static bool RecordValue(Data dataRecord, ITypeSymbol argument) => true;
    }

    private sealed class IndexMapper : ASemanticAttributeMapper<Data>
    {
        public static int ParameterIndex => 0;
        public static int DifferentParameterIndex => ParameterIndex + 1;

        protected override IEnumerable<(OneOf<int, string>, DSemanticAttributeTypeArgumentRecorder)> AddTypeParameterMappings()
        {
            yield return (ParameterIndex, RecordValue);
        }

        private static bool RecordValue(Data dataRecord, ITypeSymbol argument) => true;
    }
}
