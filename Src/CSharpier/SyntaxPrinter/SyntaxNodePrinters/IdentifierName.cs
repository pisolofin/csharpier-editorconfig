using CSharpier.Utilities;

namespace CSharpier.SyntaxPrinter.SyntaxNodePrinters;

internal static class IdentifierName
{
    public static Doc Print(IdentifierNameSyntax node, FormattingContext context)
    {
        // TODO: Fabio: Check if parent is AssignmentExpressionSyntax SimpleAssignmentExpression Value = 2
        if (node.Parent is AssignmentExpressionSyntax)
        {
            if (context.State.LocalContextReferenceLevel(node) > 0)
            {
                return Doc.Group(
                    "ciao.",
                    Token.Print(node.Identifier, context)
                );
            }

            return Doc.Group(
                "no.",
                Token.Print(node.Identifier, context)
            );
        }

        return Token.Print(node.Identifier, context);
    }
}
