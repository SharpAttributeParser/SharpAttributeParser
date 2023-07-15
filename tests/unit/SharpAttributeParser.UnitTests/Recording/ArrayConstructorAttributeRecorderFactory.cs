namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class ArrayConstructorAttributeRecorderFactory : IArrayConstructorAttributeRecorderFactory
{
    private IAttributeRecorderFactory Factory { get; }
    private IAttributeMapper<IArrayConstructorAttributeRecordBuilder> ArgumentMapper { get; }

    public ArrayConstructorAttributeRecorderFactory(IAttributeRecorderFactory factory, IAttributeMapper<IArrayConstructorAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    IAttributeRecorder<IArrayConstructorAttributeRecord> IArrayConstructorAttributeRecorderFactory.Create() => Factory.Create<IArrayConstructorAttributeRecord, IArrayConstructorAttributeRecordBuilder>(ArgumentMapper, new ArrayConstructorAttributeDataBuilder());

    private sealed class ArrayConstructorAttributeDataBuilder : IArrayConstructorAttributeRecordBuilder
    {
        private ArrayConstructorAttributeData Target { get; } = new();

        IArrayConstructorAttributeRecord IRecordBuilder<IArrayConstructorAttributeRecord>.Build() => Target;
        ISemanticArrayConstructorAttributeRecord IRecordBuilder<ISemanticArrayConstructorAttributeRecord>.Build() => Target;
        ISyntacticArrayConstructorAttributeRecord IRecordBuilder<ISyntacticArrayConstructorAttributeRecord>.Build() => Target;

        void ISemanticArrayConstructorAttributeRecordBuilder.WithValue(IReadOnlyList<object?>? value)
        {
            Target.Value = value;
            Target.ValueRecorded = true;
        }

        void ISyntacticArrayConstructorAttributeRecordBuilder.WithValueSyntax(ExpressionSyntax syntax)
        {
            Target.ValueSyntax = syntax;
            Target.ValueSyntaxRecorded = true;
        }

        private sealed class ArrayConstructorAttributeData : IArrayConstructorAttributeRecord
        {
            public IReadOnlyList<object?>? Value { get; set; }
            public bool ValueRecorded { get; set; }

            public ExpressionSyntax? ValueSyntax { get; set; }
            public bool ValueSyntaxRecorded { get; set; }
        }
    }
}
