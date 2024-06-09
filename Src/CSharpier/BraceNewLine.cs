﻿namespace CSharpier;

// https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/csharp-formatting-options#csharp_new_line_before_open_brace
[Flags]
public enum BraceNewLine
{
    None = 0, // Done
	Accessors = 1 << 0,
    AnonymousMethods = 1 << 1,
    AnonymousTypes = 1 << 2,
    ControlBlocks = 1 << 3, // Done
    Events = 1 << 4,
    Indexers = 1 << 5,
    Lambdas = 1 << 6,
    LocalFunctions = 1 << 7, // Done
    Methods = 1 << 8, // Done
    ObjectCollectionArrayInitializers = 1 << 9,
    Properties = 1 << 10,
    Types = 1 << 11, // Done

    All = 0xFFFF
}
