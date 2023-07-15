namespace SharpAttributeParser.ASplitAttributeMapperCases;

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

        var semanticRecorder = mapper.TryMapConstructorParameter(parameter, new SemanticData());
        var syntacticRecorder = mapper.TryMapConstructorParameter(parameter, new SyntacticData());

        Assert.Null(semanticRecorder);
        Assert.Null(syntacticRecorder);
    }

    [Fact]
    public void UnmodifiedMappings_TryMapNamedParameterReturnsNull()
    {
        UnmodifiedMapper mapper = new();

        var parameterName = string.Empty;

        var semanticRecorder = mapper.TryMapNamedParameter(parameterName, new SemanticData());
        var syntacticRecorder = mapper.TryMapNamedParameter(parameterName, new SyntacticData());

        Assert.Null(semanticRecorder);
        Assert.Null(syntacticRecorder);
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

        var semanticRecorder = mapper.TryMapConstructorParameter(parameter, new SemanticData());
        var syntacticRecorder = mapper.TryMapConstructorParameter(parameter, new SyntacticData());

        Assert.Null(semanticRecorder);
        Assert.Null(syntacticRecorder);
    }

    [Fact]
    public void EmptyMappingsCollection_TryMapNamedParameterReturnsNull()
    {
        EmptyMappingsCollectionMapper mapper = new();

        var parameterName = string.Empty;

        var semanticRecorder = mapper.TryMapNamedParameter(parameterName, new SemanticData());
        var syntacticRecorder = mapper.TryMapNamedParameter(parameterName, new SyntacticData());

        Assert.Null(semanticRecorder);
        Assert.Null(syntacticRecorder);
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

        var semanticRecorder = mapper.TryMapConstructorParameter(parameter, new SemanticData());
        var syntacticRecorder = mapper.TryMapConstructorParameter(parameter, new SyntacticData());

        Assert.NotNull(semanticRecorder);
        Assert.NotNull(syntacticRecorder);
    }

    [Fact]
    public void MatchingName_TryMapNamedParameterReturnsRecorder()
    {
        NameMapper mapper = new();

        var parameterName = NameMapper.ParameterName;

        var semanticRecorder = mapper.TryMapNamedParameter(parameterName, new SemanticData());
        var syntacticRecorder = mapper.TryMapNamedParameter(parameterName, new SyntacticData());

        Assert.NotNull(semanticRecorder);
        Assert.NotNull(syntacticRecorder);
    }

    [Fact]
    public void NotMatchingName_TryMapConstructorParameterReturnsNull()
    {
        NameMapper mapper = new();

        var parameter = Mock.Of<IParameterSymbol>((symbol) => symbol.Name == NameMapper.DifferentParameterName);

        var semanticRecorder = mapper.TryMapConstructorParameter(parameter, new SemanticData());
        var syntacticRecorder = mapper.TryMapConstructorParameter(parameter, new SyntacticData());

        Assert.Null(semanticRecorder);
        Assert.Null(syntacticRecorder);
    }

    [Fact]
    public void NotMatchingName_TryMapNamedParameterReturnsNull()
    {
        NameMapper mapper = new();

        var parameterName = NameMapper.DifferentParameterName;

        var semanticRecorder = mapper.TryMapNamedParameter(parameterName, new SemanticData());
        var syntacticRecorder = mapper.TryMapNamedParameter(parameterName, new SyntacticData());

        Assert.Null(semanticRecorder);
        Assert.Null(syntacticRecorder);
    }

    private sealed class UnmodifiedMapper : ASplitAttributeMapper<SemanticData, SyntacticData> { }

    private sealed class NullMappingsCollectionMapper : ASplitAttributeMapper<SemanticData, SyntacticData>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(string, IArgumentRecorderProvider)> AddParameterMappings() => null!;
    }

    private sealed class EmptyMappingsCollectionMapper : ASplitAttributeMapper<SemanticData, SyntacticData>
    {
        protected override IEnumerable<(string, IArgumentRecorderProvider)> AddParameterMappings() => Enumerable.Empty<(string, IArgumentRecorderProvider)>();
    }

    private sealed class NullNameMapper : ASplitAttributeMapper<SemanticData, SyntacticData>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(string, IArgumentRecorderProvider)> AddParameterMappings()
        {
            yield return (null!, new ArgumentRecorderProvider(RecordValue, RecordValue));
        }

        private static bool RecordValue(SemanticData dataRecord, object? argument) => true;
        private static bool RecordValue(SyntacticData dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => true;
    }

    private sealed class NullProviderMapper : ASplitAttributeMapper<SemanticData, SyntacticData>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(string, IArgumentRecorderProvider)> AddParameterMappings()
        {
            yield return (string.Empty, null!);
        }
    }

    private sealed class DuplicateNameMapper : ASplitAttributeMapper<SemanticData, SyntacticData>
    {
        public void Initialize() => InitializeMapper();

        protected override IEnumerable<(string, IArgumentRecorderProvider)> AddParameterMappings()
        {
            yield return (string.Empty, new ArgumentRecorderProvider(RecordValue, RecordValue));
            yield return (string.Empty, new ArgumentRecorderProvider(RecordValue, RecordValue));
        }

        private static bool RecordValue(SemanticData dataRecord, object? argument) => true;
        private static bool RecordValue(SyntacticData dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => true;
    }

    private sealed class DuplicateNameDueToComparerMapper : ASplitAttributeMapper<SemanticData, SyntacticData>
    {
        public void Initialize() => InitializeMapper();

        protected override IEqualityComparer<string> GetComparer() => StringComparer.OrdinalIgnoreCase;

        protected override IEnumerable<(string, IArgumentRecorderProvider)> AddParameterMappings()
        {
            yield return ("A", new ArgumentRecorderProvider(RecordValue, RecordValue));
            yield return ("a", new ArgumentRecorderProvider(RecordValue, RecordValue));
        }

        private static bool RecordValue(SemanticData dataRecord, object? argument) => true;
        private static bool RecordValue(SyntacticData dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => true;
    }

    private sealed class NameMapper : ASplitAttributeMapper<SemanticData, SyntacticData>
    {
        public static string ParameterName => string.Empty;
        public static string DifferentParameterName => $"{ParameterName} ";

        protected override IEnumerable<(string, IArgumentRecorderProvider)> AddParameterMappings()
        {
            yield return (ParameterName, new ArgumentRecorderProvider(RecordValue, RecordValue));
        }

        private static bool RecordValue(SemanticData dataRecord, object? argument) => true;
        private static bool RecordValue(SyntacticData dataRecord, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>> syntax) => true;
    }
}
