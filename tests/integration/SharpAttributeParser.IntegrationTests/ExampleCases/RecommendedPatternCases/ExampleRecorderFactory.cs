namespace SharpAttributeParser.ExampleCases.RecommendedPatternCases;

using Microsoft.CodeAnalysis;

using SharpAttributeParser.Mappers;

using System;
using System.Collections.Generic;

public sealed class ExampleRecorderFactory : IExampleRecorderFactory
{
    private readonly ISemanticRecorderFactory Factory;
    private readonly ISemanticMapper<IExampleRecordBuilder> Mapper;

    public ExampleRecorderFactory(ISemanticRecorderFactory factory, ISemanticMapper<IExampleRecordBuilder> mapper)
    {
        Factory = factory;
        Mapper = mapper;
    }

    ISemanticRecorder<IExampleRecord> IExampleRecorderFactory.Create() => Factory.Create<IExampleRecord, IExampleRecordBuilder>(Mapper, new ExampleRecordBuilder());

    private sealed class ExampleRecordBuilder : ARecordBuilder<IExampleRecord>, IExampleRecordBuilder
    {
        private readonly ExampleRecord Target = new();

        protected override IExampleRecord GetRecord() => Target;
        protected override bool CanBuildRecord() => Target.TypeArgument is not null && Target.ParamsArgument is not null;

        void IExampleRecordBuilder.WithTypeArgument(ITypeSymbol typeArgument)
        {
            VerifyCanModify();

            Target.TypeArgument = typeArgument;
        }

        void IExampleRecordBuilder.WithConstructorArgument(StringComparison constructorArgument)
        {
            VerifyCanModify();

            Target.ConstructorArgument = constructorArgument;
        }

        void IExampleRecordBuilder.WithOptionalArgument(string? optionalArgument)
        {
            VerifyCanModify();

            Target.OptionalArgument = optionalArgument;
        }

        void IExampleRecordBuilder.WithParamsArgument(IReadOnlyList<int> paramsArgument)
        {
            VerifyCanModify();

            Target.ParamsArgument = paramsArgument;
        }

        void IExampleRecordBuilder.WithNamedArgument(ITypeSymbol? namedArgument)
        {
            VerifyCanModify();

            Target.NamedArgument = namedArgument;
        }

        private sealed class ExampleRecord : IExampleRecord
        {
            public ITypeSymbol TypeArgument { get; set; } = null!;
            public StringComparison ConstructorArgument { get; set; }
            public string? OptionalArgument { get; set; }
            public IReadOnlyList<int> ParamsArgument { get; set; } = null!;
            public ITypeSymbol? NamedArgument { get; set; }
        }
    }
}
