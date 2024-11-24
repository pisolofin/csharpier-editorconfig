using CSharpier.SyntaxPrinter;

namespace CSharpier.Utilities;

internal static class IdentifierNameSyntaxExtensions
{
    /// <summary>Returns if for the <paramref name="context"/> configuration need to add qualification for <paramref name="identifierNode"/></summary>
    public static bool HasToAddQualification(this FormattingContext context, IdentifierNameSyntax identifierNode)
    {
        var contextReferenceLevel = context.State.LocalContextReferenceLevel(identifierNode);

        return
            ((context.QualificationForField ?? false) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Field) && (contextReferenceLevel.Level > 0)) ||
            ((context.QualificationForProperty ?? false) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Property) && (contextReferenceLevel.Level > 0)) ||
            ((context.QualificationForMethod ?? false) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Method) && (contextReferenceLevel.Level > 0)) ||
            ((context.QualificationForEvent ?? false) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Event) && (contextReferenceLevel.Level > 0))
        ;
    }

    /// <summary>Returns if for the <paramref name="context"/> configuration need to remove qualification for <paramref name="identifierNode"/></summary>
    public static bool HasToRemoveQualification(this FormattingContext context, IdentifierNameSyntax identifierNode)
    {
        var contextReferenceLevel = context.State.LocalContextReferenceLevel(identifierNode);

        return
            (!(context.QualificationForField ?? true) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Field) && (contextReferenceLevel.Level > 0)) ||
            (!(context.QualificationForProperty ?? true) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Property) && (contextReferenceLevel.Level > 0)) ||
            (!(context.QualificationForMethod ?? true) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Method) && (contextReferenceLevel.Level > 0)) ||
            (!(context.QualificationForEvent ?? true) && (contextReferenceLevel.ReferenceType == ContextReferenceType.Event) && (contextReferenceLevel.Level > 0))
        ;
    }
}
