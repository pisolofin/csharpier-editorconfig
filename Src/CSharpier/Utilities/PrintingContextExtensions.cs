using CSharpier.SyntaxPrinter;
using System.Linq;

namespace CSharpier.Utilities;

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

    public static int LocalContextReferenceLevel(this Stack<FormattingContextState> contextState, IdentifierNameSyntax identifier)
    {
        // Convert to Array to check every State
        var contextStateArray = contextState.ToArray();
        for (var contextIndex = 0; contextIndex < contextStateArray.Length; contextIndex++)
        {
            var currentState = contextStateArray[contextIndex];
            if (
                currentState.FieldList.HasContextReference(identifier) ||
                currentState.PropertyList.HasContextReference(identifier) ||
                currentState.EventList.HasContextReference(identifier)
            )
            {
                return contextIndex;
            }
        }

        // Not found
        return -1;
    }

    public static bool HasContextReference(this IEnumerable<VariableDeclaratorSyntax> syntaxList, IdentifierNameSyntax identifier)
    {
        return syntaxList.Any(syntax => syntax.Identifier.Text == identifier.Identifier.Text);
    }

    public static bool HasContextReference(this IEnumerable<MemberDeclarationSyntax> syntaxList, IdentifierNameSyntax identifier)
    {
        return syntaxList.Any(syntax => (syntax as PropertyDeclarationSyntax)?.Identifier.Text == identifier.Identifier.Text);
    }
}
