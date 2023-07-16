namespace SharpAttributeParser.Recording;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticParamsAttributeRecorderFactory : ISemanticParamsAttributeRecorderFactory
{
    private ISemanticAttributeRecorderFactory Factory { get; }
    private ISemanticAttributeMapper<ISemanticParamsAttributeRecordBuilder> ArgumentMapper { get; }

    public SemanticParamsAttributeRecorderFactory(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<ISemanticParamsAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISemanticAttributeRecorder<ISemanticParamsAttributeRecord> ISemanticParamsAttributeRecorderFactory.Create() => Factory.Create<ISemanticParamsAttributeRecord, ISemanticParamsAttributeRecordBuilder>(ArgumentMapper, new ParamsAttributeRecordBuilder());

    private sealed class ParamsAttributeRecordBuilder : ISemanticParamsAttributeRecordBuilder
    {
        private ParamsAttributeRecord Target { get; } = new();

        ISemanticParamsAttributeRecord IRecordBuilder<ISemanticParamsAttributeRecord>.Build() => Target;

        void ISemanticParamsAttributeRecordBuilder.WithValue(IReadOnlyList<object?>? value)
        {
            Target.Value = value;
            Target.ValueRecorded = true;
        }

        private sealed class ParamsAttributeRecord : ISemanticParamsAttributeRecord
        {
            public IReadOnlyList<object?>? Value { get; set; }
            public bool ValueRecorded { get; set; }
        }
    }
}
