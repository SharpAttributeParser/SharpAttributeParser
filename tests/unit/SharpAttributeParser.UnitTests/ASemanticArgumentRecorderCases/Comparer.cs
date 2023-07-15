namespace SharpAttributeParser.ASemanticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;

using Xunit;

public sealed class Comparer
{
    [Fact]
    public void Null_InvalidOperationExceptionWhenUsed()
    {
        SemanticArgumentRecorder recorder = new(null!);

        var exception = Record.Exception(() => RecordGenericArgument(recorder));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void TryRecordGenericArgument_InvokedAtLeastOnce()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        RecordGenericArgument(recorder);

        StringComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryRecordConstructorArgument_Single_InvokedAtLeastOnce()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        RecordSingleConstructorArgument(recorder);

        StringComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryRecordConstructorArgument_Array_InvokedAtLeastOnce()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        RecordArrayConstructorArgument(recorder);

        StringComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryRecordNamedArgument_Single_InvokedAtLeastOnce()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        RecordSingleNamedArgument(recorder);

        StringComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryRecordNamedArgument_Array_InvokedAtLeastOnce()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        SemanticArgumentRecorder recorder = new(comparerMock.Object);

        RecordArrayNamedArgument(recorder);

        StringComparerMock.VerifyInvoked(comparerMock);
    }

    private static void RecordGenericArgument(ASemanticArgumentRecorder recorder) => recorder.TryRecordGenericArgument(Mock.Of<ITypeParameterSymbol>(static (symbol) => symbol.Name == string.Empty), Mock.Of<ITypeSymbol>());
    private static void RecordSingleConstructorArgument(ASemanticArgumentRecorder recorder) => recorder.TryRecordConstructorArgument(Mock.Of<IParameterSymbol>(static (symbol) => symbol.Name == string.Empty), (object?)null);
    private static void RecordArrayConstructorArgument(ASemanticArgumentRecorder recorder) => recorder.TryRecordConstructorArgument(Mock.Of<IParameterSymbol>(static (symbol) => symbol.Name == string.Empty), null);
    private static void RecordSingleNamedArgument(ASemanticArgumentRecorder recorder) => recorder.TryRecordNamedArgument(string.Empty, (object?)null);
    private static void RecordArrayNamedArgument(ASemanticArgumentRecorder recorder) => recorder.TryRecordNamedArgument(string.Empty, null);
}
