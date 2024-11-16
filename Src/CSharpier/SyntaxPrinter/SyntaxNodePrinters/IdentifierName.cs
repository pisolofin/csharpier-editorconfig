using CSharpier.Utilities;
using System.Xml.Linq;

namespace CSharpier.SyntaxPrinter.SyntaxNodePrinters;

internal static class IdentifierName
{
    private static bool IsIdentifierUse(IdentifierNameSyntax node)
    {
        return node.Parent is AssignmentExpressionSyntax or ConditionalAccessExpressionSyntax or MemberAccessExpressionSyntax;
    }

    private static bool IsIdentifierWithThis(IdentifierNameSyntax node)
    {
        return ((node.Parent as ConditionalAccessExpressionSyntax)?.Expression is ThisExpressionSyntax) ||
            ((node.Parent as MemberAccessExpressionSyntax)?.Expression is ThisExpressionSyntax);
    }

    public static Doc Print(IdentifierNameSyntax node, FormattingContext context)
    {
        if (IsIdentifierUse(node) && !IsIdentifierWithThis(node))
        {
            var contextReferenceLevel = context.State.LocalContextReferenceLevel(node);

            if (
                ((context.QualificationForField ?? false) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Field)) ||
                ((context.QualificationForProperty ?? false) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Property)) ||
                ((context.QualificationForMethod ?? false) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Method)) ||
                ((context.QualificationForEvent ?? false) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Event))
            )
            {
                var memberAccessExpression = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.ThisExpression(),
                    node
                );
                return MemberAccessExpression.Print(memberAccessExpression, context);
            }

            return Doc.Group(
                Token.Print(node.Identifier, context)
            );
        }

        return Token.Print(node.Identifier, context);
    }
}
