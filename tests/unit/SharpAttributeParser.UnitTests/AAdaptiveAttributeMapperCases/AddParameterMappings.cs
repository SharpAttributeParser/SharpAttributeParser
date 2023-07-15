namespace SharpAttributeParser.AAdaptiveAttributeMapperCases;

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

        var parameter = Mock.Of<IParameterSymbol>((symbol) => symbol.Name == string.Empty);

        var sharedRecorder = mapper.TryMapConstructorParameter(parameter, new SharedData());
        var semanticRecorder = mapper.TryMapConstructorParameter(parameter, new SemanticData());

        Assert.Null(sharedRecorder);
        Assert.Null(semanticRecorder);
    }

    [Fact]
    public void UnmodifiedMappings_TryMapNamedParameterReturnsNull()
    {
        UnmodifiedMapper mapper = new();

        var parameterName = string.Empty;

        var sharedRecorder = mapper.TryMapNamedParameter(parameterName, new SharedData());
        var semanticRecorder = mapper.TryMapNamedParameter(parameterName, new SemanticData());

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
    public void EmptyMappingsCollection_TryMapConstructorParameterReturnsNull()
    {
        EmptyMappingsCollectionMapper mapper = new();

        var parameter = Mock.Of<IParameterSymbol>((symbol) => symbol.Name == string.Empty);

        var sharedRecorder = mapper.TryMapConstructorParameter(parameter, new SharedData());
        var semanticRecorder = mapper.TryMapConstructorParameter(parameter, new SemanticData());

        Assert.Null(sharedRecorder);
        Assert.Null(semanticRecorder);
    }

    [Fact]
    public void EmptyMappingsCollection_TryMapNamedParameterReturnsNull()
    {
        EmptyMappingsCollectionMapper mapper = new();

        var parameterName = string.Empty;

        var sharedRecorder = mapper.TryMapNamedParameter(parameterName, new SharedData());
        var semanticRecorder = mapper.TryMapNamedParameter(parameterName, new SemanticData());

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
    public void MatchingName_TryMapConstructorParameterReturnsRecorder()
    {
        NameMapper mapper = new();

        var parameter = Mock.Of<IParameterSymbol>((symbol) => symbol.Name == NameMapper.ParameterName);

        var sharedRecorder = mapper.TryMapConstructorParameter(parameter, new SharedData());
        var semanticRecorder = mapper.TryMapConstructorParameter(parameter, new SemanticData());

        Assert.NotNull(sharedRecorder);
        Assert.NotNull(semanticRecorder);
    }

    [Fact]
    public void MatchingName_TryMapNamedParameterReturnsRecorder()
    {
        NameMapper mapper = new();

        var parameterName = NameMapper.ParameterName;

        var sharedRecorder = mapper.TryMapNamedParameter(parameterName, new SharedData());
        var semanticRecorder = mapper.TryMapNamedParameter(parameterName, new SemanticData());

        Assert.NotNull(sharedRecorder);
        Assert.NotNull(semanticRecorder);
    }

    [Fact]
    public void NotMatchingName_TryMapConstructorParameterReturnsNull()
    {
        NameMapper mapper = new();

        var parameter = Mock.Of<IParameterSymbol>((symbol) => symbol.Name == NameMapper.DifferentParameterName);

        var sharedRecorder = mapper.TryMapConstructorParameter(parameter, new SharedData());
        var semanticRecorder = mapper.TryMapConstructorParameter(parameter, new SemanticData());

        Assert.Null(sharedRecorder);
        Assert.Null(semanticRecorder);
    }

    [Fact]
    public void NotMatchingName_TryMapNamedParameterReturnsNull()
    {
        NameMapper mapper = new();

        var parameterName = NameMapper.DifferentParameterName;

        var sharedRecorder = mapper.TryMapNamedParameter(parameterName, new SharedData());
        var semanticRecorder = mapper.TryMapNamedParameter(parameterName, new SemanticData());

        Assert.Null(sharedRecorder);
        Assert.Null(semanticRecorder);
    }

    private sealed class UnmodifiedMapper : AAdaptiveAttributeMapper<SharedData, SemanticData> { }

    private sealed class NullMappingsCollectionMapper : AAdaptiveAttributeMapper<SharedData, SemanticData>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(string, IArgumentRecorderProvider)> AddParameterMappings() => null!;
    }

    private sealed class EmptyMappingsCollectionMapper : AAdaptiveAttributeMapper<SharedData, SemanticData>
    {
        protected override IEnumerable<(string, IArgumentRecorderProvider)> AddParameterMappings() => Enumerable.Empty<(string, IArgumentRecorderProvider)>();
    }

    private sealed class NullNameMapper : AAdaptiveAttributeMapper<SharedData, SemanticData>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(string, IArgumentRecorderProvider)> AddParameterMappings()
        {
            yield return (null!, new ArgumentRecorderProvider(RecordValue, RecordValue));
        }

        private static bool RecordValue(SharedData dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => true;
        private static bool RecordValue(SemanticData dataRecord, object? argument) => true;
    }

    private sealed class NullProviderMapper : AAdaptiveAttributeMapper<SharedData, SemanticData>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(string, IArgumentRecorderProvider)> AddParameterMappings()
        {
            yield return (string.Empty, null!);
        }
    }

    private sealed class DuplicateNameMapper : AAdaptiveAttributeMapper<SharedData, SemanticData>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(string, IArgumentRecorderProvider)> AddParameterMappings()
        {
            yield return (string.Empty, new ArgumentRecorderProvider(RecordValue, RecordValue));
            yield return (string.Empty, new ArgumentRecorderProvider(RecordValue, RecordValue));
        }

        private static bool RecordValue(SharedData dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => true;
        private static bool RecordValue(SemanticData dataRecord, object? argument) => true;
    }

    private sealed class DuplicateNameDueToComparerMapper : AAdaptiveAttributeMapper<SharedData, SemanticData>
    {
        public void Initialize() => InitializeMapper();

        protected override IEqualityComparer<string> GetComparer() => StringComparer.OrdinalIgnoreCase;

        protected override IEnumerable<(string, IArgumentRecorderProvider)> AddParameterMappings()
        {
            yield return ("A", new ArgumentRecorderProvider(RecordValue, RecordValue));
            yield return ("a", new ArgumentRecorderProvider(RecordValue, RecordValue));
        }

        private static bool RecordValue(SharedData dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => true;
        private static bool RecordValue(SemanticData dataRecord, object? argument) => true;
    }

    private sealed class NameMapper : AAdaptiveAttributeMapper<SharedData, SemanticData>
    {
        public static string ParameterName => string.Empty;
        public static string DifferentParameterName => $"{ParameterName} ";

        protected override IEnumerable<(string, IArgumentRecorderProvider)> AddParameterMappings()
        {
            yield return (ParameterName, new ArgumentRecorderProvider(RecordValue, RecordValue));
        }

        private static bool RecordValue(SharedData dataRecord, object? argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => true;
        private static bool RecordValue(SemanticData dataRecord, object? argument) => true;
    }
}
