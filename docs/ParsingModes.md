# Parsing Modes

`SharpAttributeParser` defines three parsing modes:

##### Semantic Parsing

Semantic parsing is the most common mode, and involves recording the actual arguments of attribute parameters.

##### Syntactic Parsing

> Syntactic parsing is not always possible, as it requires a syntactic description of the attribute - which may not be available for imported attributes. Note that a local attribute of an imported attribute-class is supported.

Syntactic parsing is used to record information about how each argument was expressed. A `ISyntacticRecorder` will be provided `ExpressionSyntax`, which can be further analyzed to extract the desired syntactic information. For example, the `Location` of each argument can be extracted - useful when issuing diagnostics about attribute arguments.

##### Combined Parsing

> Combined parsing is not always possible, as it involves syntactic parsing.

Combined parsing is a combination of semantic and syntactic parsing - simultaneously recording arguments and syntactic information about the arguments.