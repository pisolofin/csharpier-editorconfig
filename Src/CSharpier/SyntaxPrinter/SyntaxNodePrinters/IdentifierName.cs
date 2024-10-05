using CSharpier.Utilities;

namespace CSharpier.SyntaxPrinter.SyntaxNodePrinters;

internal static class IdentifierName
{
    public static Doc Print(IdentifierNameSyntax node, FormattingContext context)
    {
        // TODO: Fabio: Check if parent is AssignmentExpressionSyntax SimpleAssignmentExpression Value = 2
        if (node.Parent is AssignmentExpressionSyntax)
        {
            var contextReferenceLevel = context.State.LocalContextReferenceLevel(node);

            if (
                ((context.QualificationForField ?? false) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Field)) ||
                ((context.QualificationForProperty ?? false) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Property)) ||
                ((context.QualificationForEvent ?? false) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Event))
            )
            {
                return Doc.Group(
                    Constants.ThisQualification,
                    Constants.ThingsSeparator,
                    Token.Print(node.Identifier, context)
                );
            }

            return Doc.Group(
                Token.Print(node.Identifier, context)
            );
        }

        return Token.Print(node.Identifier, context);
    }
}
