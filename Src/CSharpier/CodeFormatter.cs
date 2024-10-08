using System.Text;
using System.Text.Json;
using CSharpier.SyntaxPrinter;

namespace CSharpier;

public static class CodeFormatter
{
    public static CodeFormatterResult Format(string code, CodeFormatterOptions? options = null)
    {
        return FormatAsync(code, options).Result;
    }

    public static Task<CodeFormatterResult> FormatAsync(
        string code,
        CodeFormatterOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        options ??= new();

        return CSharpFormatter.FormatAsync(
            code,
            new PrinterOptions
            {
                Width = options.Width,
                UseTabs = options.IndentStyle == IndentStyle.Tabs,
                IndentSize = options.IndentSize,
                EndOfLine = options.EndOfLine,
                IncludeGenerated = options.IncludeGenerated,
                Formatter = "csharp",

                NewLineBeforeOpenBrace = options.NewLineBeforeOpenBrace,
                NewLineBeforeElse = options.NewLineBeforeElse,
                NewLineBeforeCatch = options.NewLineBeforeCatch,
                NewLineBeforeFinally = options.NewLineBeforeFinally,
                NewLineBeforeMembersInObjectInitializers =
                    options.NewLineBeforeMembersInObjectInitializers,
                NewLineBeforeMembersInAnonymousTypes = options.NewLineBeforeMembersInAnonymousTypes,
                NewLineBetweenQueryExpressionClauses = options.NewLineBetweenQueryExpressionClauses,
                UsePrettierStyleTrailingCommas = options.UsePrettierStyleTrailingCommas,
            },
            cancellationToken
        );
    }

    public static CodeFormatterResult Format(
        SyntaxTree syntaxTree,
        CodeFormatterOptions? options = null
    )
    {
        return FormatAsync(syntaxTree, options).Result;
    }

    public static Task<CodeFormatterResult> FormatAsync(
        SyntaxTree syntaxTree,
        CodeFormatterOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        options ??= new();

        return CSharpFormatter.FormatAsync(
            syntaxTree,
            new PrinterOptions
            {
                Width = options.Width,
                UseTabs = options.IndentStyle == IndentStyle.Tabs,
                IndentSize = options.IndentSize,
                EndOfLine = options.EndOfLine,
                Formatter = "csharp",
            },
            SourceCodeKind.Regular,
            cancellationToken
        );
    }
}
