namespace CSharpier.SyntaxPrinter;

internal static class TrailingComma
{
    public static Doc Print(
        SyntaxToken closingToken,
        FormattingContext context,
        bool skipIfBreak = false
    )
    {
        if (!context.UsePrettierStyleTrailingCommas)
        {
            return Doc.Null;
        }

        var printedToken = Token.Print(SyntaxFactory.Token(SyntaxKind.CommaToken), context);

        return closingToken.LeadingTrivia.Any(o => o.IsDirective) ? Doc.Null
            : skipIfBreak ? printedToken
            : Doc.IfBreak(printedToken, Doc.Null);
    }
}
