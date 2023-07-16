namespace SharpAttributeParser.ExampleCases;

using Microsoft.CodeAnalysis;

using SharpAttributeParser;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticExampleAttributeRecorderFactory : ISemanticExampleAttributeRecorderFactory
{
    private ISemanticAttributeRecorderFactory Factory { get; }
    private ISemanticAttributeMapper<ISemanticExampleAttributeRecordBuilder> Mapper { get; }

    public SemanticExampleAttributeRecorderFactory(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<ISemanticExampleAttributeRecordBuilder> mapper)
    {
        Factory = factory;
        Mapper = mapper;
    }

    ISemanticAttributeRecorder<ISemanticExampleAttributeRecord> ISemanticExampleAttributeRecorderFactory.Create() => Factory.Create<ISemanticExampleAttributeRecord, ISemanticExampleAttributeRecordBuilder>(Mapper, new ExampleAttributeRecordBuilder());

    private sealed class ExampleAttributeRecordBuilder : ARecordBuilder<ISemanticExampleAttributeRecord>, ISemanticExampleAttributeRecordBuilder
    {
        private ExampleAttributeRecord Target { get; } = new();

        public void WithT(ITypeSymbol t)
        {
            VerifyCanModify();

            Target.T = t;
        }

        public void WithSequence(IReadOnlyList<int> sequence)
        {
            VerifyCanModify();

            Target.Sequence = sequence;
        }

        public void WithName(string name)
        {
            VerifyCanModify();

            Target.Name = name;
        }

        public void WithAnswer(int answer)
        {
            VerifyCanModify();

            Target.Answer = answer;
        }

        protected override ISemanticExampleAttributeRecord GetTarget() => Target;
        protected override bool CheckFullyConstructed() => Target.T is not null && Target.Sequence is not null && Target.Name is not null;

        private sealed class ExampleAttributeRecord : ISemanticExampleAttributeRecord
        {
            public ITypeSymbol T { get; set; } = null!;
            public IReadOnlyList<int> Sequence { get; set; } = null!;
            public string Name { get; set; } = null!;
            public int? Answer { get; set; }
        }
    }
}
