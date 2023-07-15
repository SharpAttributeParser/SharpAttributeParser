namespace SharpAttributeParser;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;

internal sealed class AttributeArgumentRecorder : IAttributeConstructorArgumentRecorder
{
    private Func<object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> Recorder { get; }

    public AttributeArgumentRecorder(Func<object?, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder)
    {
        Recorder = recorder;
    }

    bool IAttributeArgumentRecorder.RecordArgument(object? argument, ExpressionSyntax syntax) => Recorder(argument, syntax);
    bool IAttributeConstructorArgumentRecorder.RecordParamsArgument(object? argument, IReadOnlyList<ExpressionSyntax> elementSyntax) => Recorder(argument, OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(elementSyntax));
}
