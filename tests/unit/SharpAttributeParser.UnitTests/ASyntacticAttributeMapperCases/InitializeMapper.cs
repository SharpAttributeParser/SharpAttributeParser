namespace SharpAttributeParser.ASyntacticAttributeMapperCases;

using System;
using System.Collections.Generic;

using Xunit;

public sealed class InitializeMapper
{
    [Fact]
    public void WithNullComparer_InvalidOperationException()
    {
        Mapper mapper = new() { Comparer = null! };

        var exception = Record.Exception(mapper.Initialize);

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void SetToNullComparerAfterFirstInvokation_NoException()
    {
        Mapper mapper = new();

        mapper.Initialize();

        mapper.Comparer = null!;

        var exception = Record.Exception(mapper.Initialize);

        Assert.Null(exception);
    }

    private sealed class Mapper : ASyntacticAttributeMapper<Data>
    {
        public IEqualityComparer<string> Comparer { get; set; } = StringComparer.Ordinal;
        public void Initialize() => InitializeMapper();

        protected override IEqualityComparer<string> GetComparer() => Comparer;
    }
}
