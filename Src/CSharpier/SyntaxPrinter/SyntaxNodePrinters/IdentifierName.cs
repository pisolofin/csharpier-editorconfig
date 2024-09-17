namespace CSharpier.SyntaxPrinter.SyntaxNodePrinters;

internal static class IdentifierName
{
    public static Doc Print(IdentifierNameSyntax node, FormattingContext context)
    {
        // TODO: Fabio: Check if parent is AssignmentExpressionSyntax SimpleAssignmentExpression Value = 2
        // node.Declaration.Variables[0].ToString()

        return Doc.Group(
            "ciao.",
            Token.Print(node.Identifier, context)
        );
        //return Token.Print(node.Identifier, context);
    }
}
