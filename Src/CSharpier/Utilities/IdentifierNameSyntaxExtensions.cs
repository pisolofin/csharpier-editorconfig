using CSharpier.SyntaxPrinter;

namespace CSharpier.Utilities;

internal static class IdentifierNameSyntaxExtensions
{
    /// <summary>Returns if for the <paramref name="context"/> configuration need to add qualification for <paramref name="identifierNode"/></summary>
    public static bool HasToAddQualification(this FormattingContext context, IdentifierNameSyntax identifierNode)
    {
        var contextReferenceLevel = context.State.LocalContextReferenceLevel(identifierNode);

        return
            ((context.QualificationForField ?? false) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Field)) ||
            ((context.QualificationForProperty ?? false) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Property)) ||
            ((context.QualificationForMethod ?? false) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Method)) ||
            ((context.QualificationForEvent ?? false) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Event))
        ;
    }

    /// <summary>Returns if for the <paramref name="context"/> configuration need to remove qualification for <paramref name="identifierNode"/></summary>
    public static bool HasToRemoveQualification(this FormattingContext context, IdentifierNameSyntax identifierNode)
    {
        var contextReferenceLevel = context.State.LocalContextReferenceLevel(identifierNode);

        return
            (!(context.QualificationForField ?? true) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Field)) ||
            (!(context.QualificationForProperty ?? true) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Property)) ||
            (!(context.QualificationForMethod ?? true) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Method)) ||
            (!(context.QualificationForEvent ?? true) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Event))
        ;
    }
}
