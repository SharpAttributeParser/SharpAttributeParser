namespace SharpAttributeParser;

using System;

/// <summary>Maps the provided <typeparamref name="TData"/> to a <see cref="DSemanticAttributeArgumentRecorder"/>, which uses the <typeparamref name="TData"/> to record the argument of some attribute parameter.</summary>
/// <typeparam name="TData">The type to which the produced <see cref="DSemanticAttributeArgumentRecorder"/> records the argument of some attribute parameter.</typeparam>
/// <param name="dataRecord">The <typeparamref name="TData"/> to which the produced <see cref="DSemanticAttributeArgumentRecorder"/> records the argument of some attribute parameter.</param>
/// <returns>The mapped <see cref="DSemanticAttributeArgumentRecorder"/>, which uses the provided <typeparamref name="TData"/> to record the argument of some attribute parameter.</returns>
/// <exception cref="ArgumentNullException"/>
public delegate DSemanticAttributeArgumentRecorder DSemanticAttributeParameterMapping<in TData>(TData dataRecord);
