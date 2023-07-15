namespace SharpAttributeParser;

using System;

internal sealed class SemanticAttributeArgumentRecorder : ISemanticAttributeArgumentRecorder
{
    private Func<object?, bool> Recorder { get; }

    public SemanticAttributeArgumentRecorder(Func<object?, bool> recorder)
    {
        Recorder = recorder;
    }

    bool ISemanticAttributeArgumentRecorder.RecordArgument(object? argument) => Recorder(argument);
}
