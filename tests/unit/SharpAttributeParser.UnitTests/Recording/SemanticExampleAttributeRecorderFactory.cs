namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;
using SharpAttributeParser;

using System.Collections.Generic;
using System;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticExampleAttributeRecorderFactory : ISemanticExampleAttributeRecorderFactory
{
    private ISemanticAttributeRecorderFactory Factory { get; }
    private ISemanticAttributeMapper<ISemanticExampleAttributeRecordBuilder> ArgumentMapper { get; }

    public SemanticExampleAttributeRecorderFactory(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<ISemanticExampleAttributeRecordBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISemanticAttributeRecorder<ISemanticExampleAttributeRecord> ISemanticExampleAttributeRecorderFactory.Create() => Factory.Create<ISemanticExampleAttributeRecord, ISemanticExampleAttributeRecordBuilder>(ArgumentMapper, new ExampleAttributeDataBuilder());

    private sealed class ExampleAttributeDataBuilder : ISemanticExampleAttributeRecordBuilder
    {
        private ExampleAttributeData Target { get; } = new();

        private bool HasBeenBuilt { get; set; }

        ISemanticExampleAttributeRecord IRecordBuilder<ISemanticExampleAttributeRecord>.Build()
        {
            if (Target.T is null || Target.Sequence is null || Target.Name is null)
            {
                throw new InvalidOperationException($"The {nameof(ISemanticExampleAttributeRecord)} has not yet been fully constructed.");
            }

            HasBeenBuilt = true;

            return Target;
        }

        public void WithT(ITypeSymbol t)
        {
            VerifyNotBuilt();

            Target.T = t;
        }

        public void WithSequence(IReadOnlyList<int> sequence)
        {
            VerifyNotBuilt();

            Target.Sequence = sequence;
        }

        public void WithName(string name)
        {
            VerifyNotBuilt();

            Target.Name = name;
        }

        public void WithAnswer(int answer)
        {
            VerifyNotBuilt();

            Target.Answer = answer;
        }

        private void VerifyNotBuilt()
        {
            if (HasBeenBuilt)
            {
                throw new InvalidOperationException($"The {nameof(ISemanticExampleAttributeRecord)} has been built, and may not be modified.");
            }
        }

        private sealed class ExampleAttributeData : ISemanticExampleAttributeRecord
        {
            public ITypeSymbol T { get; set; } = null!;
            public IReadOnlyList<int> Sequence { get; set; } = null!;
            public string Name { get; set; } = null!;
            public int? Answer { get; set; }
        }
    }
}
