using System.Xml.Linq;

namespace CSharpier.SyntaxPrinter;

// TODO rename this to PrintingContext
internal class FormattingContext
{
    // TODO these go into Options
    // context.Options.LineEnding
    public required string LineEnding { get; init; }
    public required int IndentSize { get; init; }
    public required bool UseTabs { get; init; }

    public required BraceNewLine NewLineBeforeOpenBrace { get; init; }
    public required bool NewLineBeforeElse { get; init; }
    public required bool NewLineBeforeCatch { get; init; }
    public required bool NewLineBeforeFinally { get; init; }
    public required bool? NewLineBeforeMembersInObjectInitializers { get; init; }
    public required bool? NewLineBeforeMembersInAnonymousTypes { get; init; }
    public required bool? NewLineBetweenQueryExpressionClauses { get; init; }

    public bool? QualificationForField { get; init; }
    public bool? QualificationForProperty { get; init; }
    public bool? QualificationForMethod { get; init; }
    public bool? QualificationForEvent { get; init; }

    public required bool UsePrettierStyleTrailingCommas { get; init; }

    // TODO the rest of these go into State
    // context.State.PrintingDepth
    public int PrintingDepth { get; set; }
    public bool NextTriviaNeedsLine { get; set; }
    public bool SkipNextLeadingTrivia { get; set; }
    public Stack<FormattingContextState> State { get; } = new Stack<FormattingContextState>();

    // we need to keep track if we reordered modifiers because when modifiers are moved inside
    // of an #if, then we can't compare the before and after disabled text in the source file
    public bool ReorderedModifiers { get; set; }

    // we also need to keep track if we move around usings with disabledText
    public bool ReorderedUsingsWithDisabledText { get; set; }

    public TrailingCommaContext? TrailingComma { get; set; }

    public FormattingContext WithSkipNextLeadingTrivia()
    {
        this.SkipNextLeadingTrivia = true;
        return this;
    }

    public FormattingContext WithTrailingComma(SyntaxTrivia syntaxTrivia, Doc doc)
    {
        this.TrailingComma = new TrailingCommaContext(syntaxTrivia, doc);
        return this;
    }

    public record TrailingCommaContext(SyntaxTrivia TrailingComment, Doc PrintedTrailingComma);
}

internal class FormattingContextState
{
    public FormattingContextState? Parent { get; set; } = null;

    public bool IsScanOnly { get; set; } = false;

    public HashSet<VariableDeclaratorSyntax> FieldList = new HashSet<VariableDeclaratorSyntax>();
    public HashSet<MemberDeclarationSyntax> PropertyList = new HashSet<MemberDeclarationSyntax>();
    public HashSet<MethodDeclarationSyntax> MethodList = new HashSet<MethodDeclarationSyntax>();
    public HashSet<VariableDeclaratorSyntax> EventList = new HashSet<VariableDeclaratorSyntax>();
}
