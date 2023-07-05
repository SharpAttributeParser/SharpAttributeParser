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
    private ISemanticAttributeMapper<IExampleAttributeDataBuilder> ArgumentMapper { get; }

    public SemanticExampleAttributeRecorderFactory(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<IExampleAttributeDataBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISemanticAttributeRecorder<IExampleAttributeData> ISemanticExampleAttributeRecorderFactory.Create() => Factory.Create<IExampleAttributeData, IExampleAttributeDataBuilder>(ArgumentMapper, new ExampleAttributeDataBuilder());

    private sealed class ExampleAttributeDataBuilder : IExampleAttributeDataBuilder
    {
        private ExampleAttributeData Target { get; } = new();

        private bool HasBeenBuilt { get; set; }

        IExampleAttributeData IAttributeDataBuilder<IExampleAttributeData>.Build()
        {
            if (Target.T is null || Target.Sequence is null || Target.Name is null)
            {
                throw new InvalidOperationException($"The {nameof(IExampleAttributeData)} has not yet been fully constructed.");
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
                throw new InvalidOperationException($"The {nameof(IExampleAttributeData)} has been built, and may not be modified.");
            }
        }

        private sealed class ExampleAttributeData : IExampleAttributeData
        {
            public ITypeSymbol T { get; set; } = null!;
            public IReadOnlyList<int> Sequence { get; set; } = null!;
            public string Name { get; set; } = null!;
            public int? Answer { get; set; }
        }
    }
}
