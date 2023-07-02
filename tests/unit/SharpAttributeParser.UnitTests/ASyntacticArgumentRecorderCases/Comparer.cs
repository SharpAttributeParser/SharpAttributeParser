namespace SharpAttributeParser.ASyntacticArgumentRecorderCases;

using Microsoft.CodeAnalysis;

using Moq;

using System;

using Xunit;

public sealed class Comparer
{
    [Fact]
    public void Null_InvalidOperationExceptionWhenUsed()
    {
        SyntacticArgumentRecorder recorder = new(null!);

        var exception = Record.Exception(() => RecordGenericArgument(recorder));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void TryRecordGenericArgument_InvokedAtLeastOnce()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        RecordGenericArgument(recorder);

        StringComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryRecordConstructorArgument_Single_InvokedAtLeastOnce()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        RecordSingleConstructorArgument(recorder);

        StringComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryRecordConstructorArgument_Array_InvokedAtLeastOnce()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        RecordArrayConstructorArgument(recorder);

        StringComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryRecordNamedArgument_Single_InvokedAtLeastOnce()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        RecordSingleNamedArgument(recorder);

        StringComparerMock.VerifyInvoked(comparerMock);
    }

    [Fact]
    public void TryRecordNamedArgument_Array_InvokedAtLeastOnce()
    {
        var comparerMock = StringComparerMock.CreateMock(true);

        SyntacticArgumentRecorder recorder = new(comparerMock.Object);

        RecordArrayNamedArgument(recorder);

        StringComparerMock.VerifyInvoked(comparerMock);
    }

    private static void RecordGenericArgument(ASyntacticArgumentRecorder recorder) => recorder.TryRecordGenericArgument(Mock.Of<ITypeParameterSymbol>(static (symbol) => symbol.Name == string.Empty), Mock.Of<ITypeSymbol>(), Location.None);
    private static void RecordSingleConstructorArgument(ASyntacticArgumentRecorder recorder) => recorder.TryRecordConstructorArgument(Mock.Of<IParameterSymbol>(static (symbol) => symbol.Name == string.Empty), null, Location.None);
    private static void RecordArrayConstructorArgument(ASyntacticArgumentRecorder recorder) => recorder.TryRecordConstructorArgument(Mock.Of<IParameterSymbol>(static (symbol) => symbol.Name == string.Empty), null, CollectionLocation.None);
    private static void RecordSingleNamedArgument(ASyntacticArgumentRecorder recorder) => recorder.TryRecordNamedArgument(string.Empty, null, Location.None);
    private static void RecordArrayNamedArgument(ASyntacticArgumentRecorder recorder) => recorder.TryRecordNamedArgument(string.Empty, null, CollectionLocation.None);
}
