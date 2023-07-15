namespace SharpAttributeParser.ASemanticAttributeMapperCases;

using Microsoft.CodeAnalysis;

using Moq;

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

    private sealed class UnmodifiedMapper : ASemanticAttributeMapper<Data> { }

    private sealed class NullMappingsCollectionMapper : ASemanticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(string, DArgumentRecorder)> AddParameterMappings() => null!;
    }

    private sealed class EmptyMappingsCollectionMapper : ASemanticAttributeMapper<Data>
    {
        protected override IEnumerable<(string, DArgumentRecorder)> AddParameterMappings() => Enumerable.Empty<(string, DArgumentRecorder)>();
    }

    private sealed class NullNameMapper : ASemanticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(string, DArgumentRecorder)> AddParameterMappings()
        {
            yield return (null!, RecordValue);
        }

        private static bool RecordValue(Data dataRecord, object? argument) => true;
    }

    private sealed class NullDelegateMapper : ASemanticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(string, DArgumentRecorder)> AddParameterMappings()
        {
            yield return (string.Empty, null!);
        }
    }

    private sealed class DuplicateNameMapper : ASemanticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(string, DArgumentRecorder)> AddParameterMappings()
        {
            yield return (string.Empty, RecordValue);
            yield return (string.Empty, RecordValue);
        }

        private static bool RecordValue(Data dataRecord, object? argument) => true;
    }

    private sealed class DuplicateNameDueToComparerMapper : ASemanticAttributeMapper<Data>
    {
        public void Initialize() => InitializeMapper();

        protected override IEqualityComparer<string> GetComparer() => StringComparer.OrdinalIgnoreCase;

        protected override IEnumerable<(string, DArgumentRecorder)> AddParameterMappings()
        {
            yield return ("A", RecordValue);
            yield return ("a", RecordValue);
        }

        private static bool RecordValue(Data dataRecord, object? argument) => true;
    }

    private sealed class NameMapper : ASemanticAttributeMapper<Data>
    {
        public static string ParameterName => string.Empty;
        public static string DifferentParameterName => $"{ParameterName} ";

        protected override IEnumerable<(string, DArgumentRecorder)> AddParameterMappings()
        {
            yield return (ParameterName, RecordValue);
        }

        private static bool RecordValue(Data dataRecord, object? argument) => true;
    }
}
