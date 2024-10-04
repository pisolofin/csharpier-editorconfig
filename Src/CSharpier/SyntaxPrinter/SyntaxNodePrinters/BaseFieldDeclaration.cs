namespace CSharpier.SyntaxPrinter.SyntaxNodePrinters;

internal static class BaseFieldDeclaration
{
    public static Doc Print(BaseFieldDeclarationSyntax node, FormattingContext context)
    {
        // TODO: Fabio: Save field name
        // Compute the field name
        var variable = node.Declaration.Variables[0];
        var fieldName = Token.Print(variable.Identifier, context).ToString();
        if (node is EventFieldDeclarationSyntax)
        {
            context.State.Peek().EventList.Add(variable);
        }
        else
        {
            context.State.Peek().FieldList.Add(variable);
        }

        var docs = new List<Doc>
        {
            AttributeLists.Print(node, node.AttributeLists, context),
            Modifiers.PrintSorted(node.Modifiers, context),
        };
        if (node is EventFieldDeclarationSyntax eventFieldDeclarationSyntax)
        {
            docs.Add(Token.PrintWithSuffix(eventFieldDeclarationSyntax.EventKeyword, " ", context));
        }

        docs.Add(
            VariableDeclaration.Print(node.Declaration, context),
            Token.Print(node.SemicolonToken, context)
        );
        return Doc.Concat(docs);
    }
}
