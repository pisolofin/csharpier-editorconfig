namespace CSharpier.SyntaxPrinter.SyntaxNodePrinters;

internal static class IdentifierName
{
    private static bool IsIdentifierUse(IdentifierNameSyntax node)
    {
        return node.Parent is AssignmentExpressionSyntax or ConditionalAccessExpressionSyntax or MemberAccessExpressionSyntax or InvocationExpressionSyntax;
    }

    private static bool IsIdentifierWithThis(IdentifierNameSyntax node)
    {
        return ((node.Parent as ConditionalAccessExpressionSyntax)?.Expression is ThisExpressionSyntax) ||
            ((node.Parent as MemberAccessExpressionSyntax)?.Expression is ThisExpressionSyntax) ||
            ((node.Parent as InvocationExpressionSyntax)?.Expression is ThisExpressionSyntax);
    }

    public static Doc Print(IdentifierNameSyntax node, FormattingContext context)
    {
        if (IsIdentifierUse(node) && !IsIdentifierWithThis(node))
        {
            if (context.HasToAddQualification(node))
            {
                var memberAccessExpression = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.ThisExpression(),
                    node
                );
                return MemberAccessExpression.Print(memberAccessExpression, context);
            }
        }

        return Token.Print(node.Identifier, context);
    }
}
