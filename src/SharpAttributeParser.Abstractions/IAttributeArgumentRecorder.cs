namespace SharpAttributeParser;

/// <summary>Records the argument of some attribute parameter, together with syntactical information about the argument.</summary>
public interface IAttributeArgumentRecorder : ISemanticAttributeArgumentRecorder, ISyntacticAttributeArgumentRecorder { }
