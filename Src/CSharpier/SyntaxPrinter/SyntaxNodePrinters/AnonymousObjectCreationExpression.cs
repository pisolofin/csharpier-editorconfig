namespace CSharpier.SyntaxPrinter.SyntaxNodePrinters;

internal static class AnonymousObjectCreationExpression
{
    public static Doc Print(AnonymousObjectCreationExpressionSyntax node, FormattingContext context)
    {
        var alwaysBreak = context.NewLineBeforeMembersInAnonymousTypes ?? (node.Initializers.Count >= 3);

        return Doc.Group(
            Token.PrintWithSuffix(
                node.NewKeyword,
                context.NewLineBeforeOpenBrace.HasFlag(BraceNewLine.AnonymousTypes) ? Doc.Line : " ",
                context
            ),
            Token.Print(node.OpenBraceToken, context),
            node.Initializers.Any()
                ? Doc.Indent(
                    alwaysBreak ? Doc.HardLine : Doc.Line,
                    SeparatedSyntaxList.PrintWithTrailingComma(
                        node.Initializers,
                        AnonymousObjectMemberDeclarator.Print,
                        Doc.Line,
                        context,
                        node.CloseBraceToken
                    )
                )
                : Doc.Null,
            Doc.Line,
            Token.Print(node.CloseBraceToken, context)
        );
    }
}
