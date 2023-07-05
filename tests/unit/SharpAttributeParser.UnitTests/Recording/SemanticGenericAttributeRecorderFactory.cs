namespace SharpAttributeParser.Recording;

using Microsoft.CodeAnalysis;

using SharpAttributeParser;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Used through DI.")]
internal sealed class SemanticGenericAttributeRecorderFactory : ISemanticGenericAttributeRecorderFactory
{
    private ISemanticAttributeRecorderFactory Factory { get; }
    private ISemanticAttributeMapper<IGenericAttributeDataBuilder> ArgumentMapper { get; }

    public SemanticGenericAttributeRecorderFactory(ISemanticAttributeRecorderFactory factory, ISemanticAttributeMapper<IGenericAttributeDataBuilder> argumentMapper)
    {
        Factory = factory;
        ArgumentMapper = argumentMapper;
    }

    ISemanticAttributeRecorder<IGenericAttributeData> ISemanticGenericAttributeRecorderFactory.Create() => Factory.Create<IGenericAttributeData, IGenericAttributeDataBuilder>(ArgumentMapper, new GenericAttributeDataBuilder());

    private sealed class GenericAttributeDataBuilder : IGenericAttributeDataBuilder
    {
        private GenericAttributeData Target { get; } = new();

        IGenericAttributeData IAttributeDataBuilder<IGenericAttributeData>.Build() => Target;

        void IGenericAttributeDataBuilder.WithT1(ITypeSymbol t1, Location t1Location)
        {
            Target.T1 = t1;
            Target.T1Recorded = true;
            Target.T1Location = t1Location;
        }

        void IGenericAttributeDataBuilder.WithT2(ITypeSymbol t2, Location t2Location)
        {
            Target.T2 = t2;
            Target.T2Recorded = true;
            Target.T2Location = t2Location;
        }

        private sealed class GenericAttributeData : IGenericAttributeData
        {
            public ITypeSymbol? T1 { get; set; }
            public bool T1Recorded { get; set; }
            public Location T1Location { get; set; } = Location.None!;

            public ITypeSymbol? T2 { get; set; }
            public bool T2Recorded { get; set; }
            public Location T2Location { get; set; } = Location.None;
        }
    }
}
