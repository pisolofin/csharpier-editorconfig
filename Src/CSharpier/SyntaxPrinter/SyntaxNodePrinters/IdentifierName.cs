namespace CSharpier.SyntaxPrinter.SyntaxNodePrinters;

internal static class IdentifierName
{
    public static Doc Print(IdentifierNameSyntax node, FormattingContext context)
    {
        // TODO: Fabio: Check if parent is AssignmentExpressionSyntax SimpleAssignmentExpression Value = 2
        if (node.Parent is AssignmentExpressionSyntax)
        {
            return Doc.Group(
                "ciao.",
                Token.Print(node.Identifier, context)
            );
        }

        return Token.Print(node.Identifier, context);
    }
}
