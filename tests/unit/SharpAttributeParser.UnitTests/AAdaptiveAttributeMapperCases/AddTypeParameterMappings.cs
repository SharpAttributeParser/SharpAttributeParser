namespace SharpAttributeParser.AAdaptiveAttributeMapperCases;

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

        var parameter = Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == string.Empty);

        var sharedRecorder = mapper.TryMapTypeParameter(parameter, new SharedData());
        var semanticRecorder = mapper.TryMapTypeParameter(parameter, new SemanticData());

        Assert.Null(sharedRecorder);
        Assert.Null(semanticRecorder);
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

        var parameter = Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == string.Empty);

        var sharedRecorder = mapper.TryMapTypeParameter(parameter, new SharedData());
        var semanticRecorder = mapper.TryMapTypeParameter(parameter, new SemanticData());

        Assert.Null(sharedRecorder);
        Assert.Null(semanticRecorder);
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
    public void NullProvider_InvalidOperationExceptionWhenInitialized()
    {
        NullProviderMapper mapper = new();

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

        var parameter = Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == NameMapper.ParameterName);

        var sharedRecorder = mapper.TryMapTypeParameter(parameter, new SharedData());
        var semanticRecorder = mapper.TryMapTypeParameter(parameter, new SemanticData());

        Assert.NotNull(sharedRecorder);
        Assert.NotNull(semanticRecorder);
    }

    [Fact]
    public void NotMatchingName_TryMapTypeParameterReturnsNull()
    {
        NameMapper mapper = new();

        var parameter = Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == NameMapper.DifferentParameterName);

        var sharedRecorder = mapper.TryMapTypeParameter(parameter, new SharedData());
        var semanticRecorder = mapper.TryMapTypeParameter(parameter, new SemanticData());

        Assert.Null(sharedRecorder);
        Assert.Null(semanticRecorder);
    }

    [Fact]
    public void MatchingIndex_TryMapTypeParameterReturnsRecorder()
    {
        IndexMapper mapper = new();

        var parameter = Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == string.Empty && symbol.Ordinal == IndexMapper.ParameterIndex);

        var sharedRecorder = mapper.TryMapTypeParameter(parameter, new SharedData());
        var semanticRecorder = mapper.TryMapTypeParameter(parameter, new SemanticData());

        Assert.NotNull(sharedRecorder);
        Assert.NotNull(semanticRecorder);
    }

    [Fact]
    public void NotMatchingIndex_TryMapTypeParameterReturnsNull()
    {
        IndexMapper mapper = new();

        var parameter = Mock.Of<ITypeParameterSymbol>((symbol) => symbol.Name == string.Empty && symbol.Ordinal == IndexMapper.DifferentParameterIndex);

        var sharedRecorder = mapper.TryMapTypeParameter(parameter, new SharedData());
        var semanticRecorder = mapper.TryMapTypeParameter(parameter, new SemanticData());

        Assert.Null(sharedRecorder);
        Assert.Null(semanticRecorder);
    }

    private sealed class UnmodifiedMapper : AAdaptiveAttributeMapper<SharedData, SemanticData> { }

    private sealed class NullMappingsCollectionMapper : AAdaptiveAttributeMapper<SharedData, SemanticData>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(OneOf<int, string>, ITypeArgumentRecorderProvider)> AddTypeParameterMappings() => null!;
    }

    private sealed class EmptyMappingsCollectionMapper : AAdaptiveAttributeMapper<SharedData, SemanticData>
    {
        protected override IEnumerable<(OneOf<int, string>, ITypeArgumentRecorderProvider)> AddTypeParameterMappings() => Enumerable.Empty<(OneOf<int, string>, ITypeArgumentRecorderProvider)>();
    }

    private sealed class NullNameMapper : AAdaptiveAttributeMapper<SharedData, SemanticData>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(OneOf<int, string>, ITypeArgumentRecorderProvider)> AddTypeParameterMappings()
        {
            yield return (null!, new TypeArgumentRecorderProvider(RecordValue, RecordValue));
        }

        private static bool RecordValue(SharedData dataRecord, ITypeSymbol argument, ExpressionSyntax syntax) => true;
        private static bool RecordValue(SemanticData dataRecord, ITypeSymbol argument) => true;
    }

    private sealed class NegativeIndexMapper : AAdaptiveAttributeMapper<SharedData, SemanticData>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(OneOf<int, string>, ITypeArgumentRecorderProvider)> AddTypeParameterMappings()
        {
            yield return (-1, new TypeArgumentRecorderProvider(RecordValue, RecordValue));
        }

        private static bool RecordValue(SharedData dataRecord, ITypeSymbol argument, ExpressionSyntax syntax) => true;
        private static bool RecordValue(SemanticData dataRecord, ITypeSymbol argument) => true;
    }

    private sealed class NullProviderMapper : AAdaptiveAttributeMapper<SharedData, SemanticData>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(OneOf<int, string>, ITypeArgumentRecorderProvider)> AddTypeParameterMappings()
        {
            yield return (0, null!);
        }
    }

    private sealed class DuplicateNameMapper : AAdaptiveAttributeMapper<SharedData, SemanticData>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(OneOf<int, string>, ITypeArgumentRecorderProvider)> AddTypeParameterMappings()
        {
            yield return (string.Empty, new TypeArgumentRecorderProvider(RecordValue, RecordValue));
            yield return (string.Empty, new TypeArgumentRecorderProvider(RecordValue, RecordValue));
        }

        private static bool RecordValue(SharedData dataRecord, ITypeSymbol argument, ExpressionSyntax syntax) => true;
        private static bool RecordValue(SemanticData dataRecord, ITypeSymbol argument) => true;
    }

    private sealed class DuplicateNameDueToComparerMapper : AAdaptiveAttributeMapper<SharedData, SemanticData>
    {
        public void Initialize() => InitializeMapper();

        protected override IEqualityComparer<string> GetComparer() => StringComparer.OrdinalIgnoreCase;

        protected override IEnumerable<(OneOf<int, string>, ITypeArgumentRecorderProvider)> AddTypeParameterMappings()
        {
            yield return ("A", new TypeArgumentRecorderProvider(RecordValue, RecordValue));
            yield return ("a", new TypeArgumentRecorderProvider(RecordValue, RecordValue));
        }

        private static bool RecordValue(SharedData dataRecord, ITypeSymbol argument, ExpressionSyntax syntax) => true;
        private static bool RecordValue(SemanticData dataRecord, ITypeSymbol argument) => true;
    }

    private sealed class DuplicateIndexMapper : AAdaptiveAttributeMapper<SharedData, SemanticData>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(OneOf<int, string>, ITypeArgumentRecorderProvider)> AddTypeParameterMappings()
        {
            yield return (0, new TypeArgumentRecorderProvider(RecordValue, RecordValue));
            yield return (0, new TypeArgumentRecorderProvider(RecordValue, RecordValue));
        }

        private static bool RecordValue(SharedData dataRecord, ITypeSymbol argument, ExpressionSyntax syntax) => true;
        private static bool RecordValue(SemanticData dataRecord, ITypeSymbol argument) => true;
    }

    private sealed class NameMapper : AAdaptiveAttributeMapper<SharedData, SemanticData>
    {
        public static string ParameterName => string.Empty;
        public static string DifferentParameterName => $"{ParameterName} ";

        protected override IEnumerable<(OneOf<int, string>, ITypeArgumentRecorderProvider)> AddTypeParameterMappings()
        {
            yield return (ParameterName, new TypeArgumentRecorderProvider(RecordValue, RecordValue));
        }

        private static bool RecordValue(SharedData dataRecord, ITypeSymbol argument, ExpressionSyntax syntax) => true;
        private static bool RecordValue(SemanticData dataRecord, ITypeSymbol argument) => true;
    }

    private sealed class IndexMapper : AAdaptiveAttributeMapper<SharedData, SemanticData>
    {
        public static int ParameterIndex => 0;
        public static int DifferentParameterIndex => ParameterIndex + 1;

        protected override IEnumerable<(OneOf<int, string>, ITypeArgumentRecorderProvider)> AddTypeParameterMappings()
        {
            yield return (ParameterIndex, new TypeArgumentRecorderProvider(RecordValue, RecordValue));
        }

        private static bool RecordValue(SharedData dataRecord, ITypeSymbol argument, ExpressionSyntax syntax) => true;
        private static bool RecordValue(SemanticData dataRecord, ITypeSymbol argument) => true;
    }
}
