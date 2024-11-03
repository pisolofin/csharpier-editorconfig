using CSharpier.SyntaxPrinter;

namespace CSharpier.Utilities;

internal enum ContextReferenceType
{
    None,
    Field,
    Property,
    Method,
    Event
}

internal record ContextReferenceLevel(ContextReferenceType ReferenceType, int Level);

internal static class PrintingContextExtensions
{
    /// <summary>Adds a new state to the current context and sets a reference to its parent</summary>
    public static FormattingContextState AddContextState(this Stack<FormattingContextState> contextState)
    {
        var currentState = new FormattingContextState
        {
            Parent = contextState.Count == 0 ? null : contextState.Peek()
        };
        contextState.Push(currentState);

        return currentState;
    }

    /// <summary>Enter in ScanOnly modality. In this modality scan only scoped declarations.</summary>
    public static FormattingContextState ScanOnlyEnter(this Stack<FormattingContextState> contextState)
    {
        var formattingContextState = contextState.Peek();
        formattingContextState.IsScanOnly = true;
        return formattingContextState;
    }

    /// <summary>Exit from ScanOnly modality. If this modality is false, check code and use pre scanning.</summary>
    public static FormattingContextState ScanOnlyExit(this Stack<FormattingContextState> contextState)
    {
        var formattingContextState = contextState.Peek();
        formattingContextState.IsScanOnly = false;
        return formattingContextState;
    }

    /// <summary>Returns if ScanOnly modality is active</summary>
    public static bool IsInScanOnly(this Stack<FormattingContextState> contextState)
    {
        return contextState.Peek().IsScanOnly;
    }

    public static ContextReferenceLevel LocalContextReferenceLevel(this Stack<FormattingContextState> contextState, IdentifierNameSyntax identifier)
    {
        // Convert to Array to check every State
        var contextStateArray = contextState.ToArray();
        for (var contextIndex = 0; contextIndex < contextStateArray.Length; contextIndex++)
        {
            var currentState = contextStateArray[contextIndex];
            if (currentState.FieldList.HasContextReference(identifier))
            {
                return new ContextReferenceLevel(ContextReferenceType.Field, contextIndex);
            }
            if (currentState.PropertyList.HasContextReference(identifier))
            {
                return new ContextReferenceLevel(ContextReferenceType.Property, contextIndex);
            }
            if (currentState.EventList.HasContextReference(identifier))
            {
                return new ContextReferenceLevel(ContextReferenceType.Event, contextIndex);
            }
        }

        // Not found
        return new ContextReferenceLevel(ContextReferenceType.None, -1);
    }

    public static bool HasContextReference(this IEnumerable<VariableDeclaratorSyntax> syntaxList, IdentifierNameSyntax identifier)
    {
        return syntaxList.Any(syntax => syntax.Identifier.Text == identifier.Identifier.Text);
    }

    public static bool HasContextReference(this IEnumerable<MemberDeclarationSyntax> syntaxList, IdentifierNameSyntax identifier)
    {
        return syntaxList.Any(syntax => (syntax as PropertyDeclarationSyntax)?.Identifier.Text == identifier.Identifier.Text);
    }

    public static void AddContextReference(this Stack<FormattingContextState> contextState, BaseFieldDeclarationSyntax fieldDeclaration)
    {
        var variable = fieldDeclaration.Declaration.Variables[0];
        if (fieldDeclaration is EventFieldDeclarationSyntax)
        {
            contextState.Peek().EventList.Add(variable);
        }
        else
        {
            contextState.Peek().FieldList.Add(variable);
        }
    }

    public static void AddContextReference(this Stack<FormattingContextState> contextState, BasePropertyDeclarationSyntax propertyDeclaration)
    {
        contextState.Peek().PropertyList.Add(propertyDeclaration);
    }

    public static void AddContextReference(this Stack<FormattingContextState> contextState, MethodDeclarationSyntax methodDeclaration)
    {
        contextState.Peek().MethodList.Add(methodDeclaration);
    }
}
