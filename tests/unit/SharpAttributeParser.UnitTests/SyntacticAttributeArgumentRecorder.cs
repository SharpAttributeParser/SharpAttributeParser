namespace SharpAttributeParser;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using OneOf;

using System;
using System.Collections.Generic;

internal sealed class SyntacticAttributeArgumentRecorder : ISyntacticAttributeArgumentRecorder
{
    private Func<OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> Recorder { get; }

    public SyntacticAttributeArgumentRecorder(Func<OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>, bool> recorder)
    {
        Recorder = recorder;
    }

    bool ISyntacticAttributeArgumentRecorder.RecordArgumentSyntax(ExpressionSyntax syntax) => Recorder(syntax);
    bool ISyntacticAttributeArgumentRecorder.RecordParamsArgumentSyntax(IReadOnlyList<ExpressionSyntax> elementSyntax) => Recorder(OneOf<ExpressionSyntax, IReadOnlyList<ExpressionSyntax>>.FromT1(elementSyntax));
}
