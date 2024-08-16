namespace CSharpier.Cli.Options;

using System.Text.Json.Serialization;
using CSharpier.Cli.EditorConfig;

internal class ConfigurationFileOptions
{
    public int PrintWidth { get; init; } = 100;
    public int TabWidth { get; init; } = 4;
    public bool UseTabs { get; init; }

    [JsonConverter(typeof(CaseInsensitiveEnumConverter<EndOfLine>))]
    public EndOfLine EndOfLine { get; init; }

    [JsonConverter(typeof(CaseInsensitiveEnumConverter<BraceNewLine>))]
    public BraceNewLine NewLineBeforeOpenBrace { get; init; } = BraceNewLine.All;
    public bool NewLineBeforeElse { get; init; } = true;
    public bool NewLineBeforeCatch { get; init; } = true;
    public bool NewLineBeforeFinally { get; init; } = true;
    public bool? NewLineBeforeMembersInObjectInitializers { get; init; } = null;
    public bool? NewLineBeforeMembersInAnonymousTypes { get; init; } = null;
    public bool NewLineBetweenQueryExpressionClauses { get; init; } = true;
    public bool UsePrettierStyleTrailingCommas { get; init; } = true;

    public Override[] Overrides { get; init; } = [];

    public PrinterOptions? ConvertToPrinterOptions(string filePath)
    {
        DebugLogger.Log("finding options for " + filePath);
        var matchingOverride = this.Overrides.LastOrDefault(o => o.IsMatch(filePath));
        if (matchingOverride is not null)
        {
            return new PrinterOptions
            {
                IndentSize = matchingOverride.TabWidth,
                UseTabs = matchingOverride.UseTabs,
                Width = matchingOverride.PrintWidth,
                EndOfLine = matchingOverride.EndOfLine,
                Formatter = matchingOverride.Formatter,

                NewLineBeforeOpenBrace = matchingOverride.NewLineBeforeOpenBrace,
                NewLineBeforeElse = matchingOverride.NewLineBeforeElse,
                NewLineBeforeCatch = matchingOverride.NewLineBeforeCatch,
                NewLineBeforeFinally = matchingOverride.NewLineBeforeFinally,
                NewLineBeforeMembersInObjectInitializers = matchingOverride.NewLineBeforeMembersInObjectInitializers,
                NewLineBeforeMembersInAnonymousTypes = matchingOverride.NewLineBeforeMembersInAnonymousTypes,
                NewLineBetweenQueryExpressionClauses = matchingOverride.NewLineBetweenQueryExpressionClauses,
                UsePrettierStyleTrailingCommas = matchingOverride.UsePrettierStyleTrailingCommas
            };
        }

        if (filePath.EndsWith(".cs") || filePath.EndsWith(".csx"))
        {
            return new PrinterOptions
            {
                IndentSize = this.TabWidth,
                UseTabs = this.UseTabs,
                Width = this.PrintWidth,
                EndOfLine = this.EndOfLine,
                Formatter = "csharp",

                NewLineBeforeOpenBrace = this.NewLineBeforeOpenBrace,
                NewLineBeforeElse = this.NewLineBeforeElse,
                NewLineBeforeCatch = this.NewLineBeforeCatch,
                NewLineBeforeFinally = this.NewLineBeforeFinally,
                NewLineBeforeMembersInObjectInitializers = this.NewLineBeforeMembersInObjectInitializers,
                NewLineBeforeMembersInAnonymousTypes = this.NewLineBeforeMembersInAnonymousTypes,
                NewLineBetweenQueryExpressionClauses = this.NewLineBetweenQueryExpressionClauses,
                UsePrettierStyleTrailingCommas = this.UsePrettierStyleTrailingCommas
            };
        }

        return null;
    }

    public void Init(string directory)
    {
        foreach (var thing in this.Overrides)
        {
            thing.Init(directory);
        }
    }
}

internal class Override
{
    private GlobMatcher? matcher;

    public int PrintWidth { get; init; } = 100;
    public int TabWidth { get; init; } = 4;
    public bool UseTabs { get; init; }

    [JsonConverter(typeof(CaseInsensitiveEnumConverter<EndOfLine>))]
    public EndOfLine EndOfLine { get; init; }

    [JsonConverter(typeof(CaseInsensitiveEnumConverter<BraceNewLine>))]
    public BraceNewLine NewLineBeforeOpenBrace { get; init; } = BraceNewLine.All;
    public bool NewLineBeforeElse { get; init; } = true;
    public bool NewLineBeforeCatch { get; init; } = true;
    public bool NewLineBeforeFinally { get; init; } = true;
    public bool? NewLineBeforeMembersInObjectInitializers { get; init; } = null;
    public bool? NewLineBeforeMembersInAnonymousTypes { get; init; } = null;
    public bool NewLineBetweenQueryExpressionClauses { get; init; } = true;
    public bool UsePrettierStyleTrailingCommas { get; init; } = true;

    public string Files { get; init; } = string.Empty;

    public string Formatter { get; init; } = string.Empty;

    public void Init(string directory)
    {
        this.matcher = Globber.Create(this.Files, directory);
    }

    public bool IsMatch(string fileName)
    {
        return this.matcher?.IsMatch(fileName) ?? false;
    }
}
