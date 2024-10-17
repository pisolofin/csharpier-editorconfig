namespace CSharpier.Tests;

using CSharpier.Utilities;
using FluentAssertions;
using NUnit.Framework;

public partial class OptionsProviderTests
{
    [Test]
    public async Task Should_Return_NewLineBeforeOpenBrace_With_Json()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.csharpierrc",
            "{ \"newLineBeforeOpenBrace\": \"Accessors,AnonymousMethods\" }"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result
            .NewLineBeforeOpenBrace.Should()
            .Be(BraceNewLine.Accessors | BraceNewLine.AnonymousMethods);
    }

    [TestCase("NewLineBeforeElse", true)]
    [TestCase("NewLineBeforeElse", false)]
    [TestCase("NewLineBeforeCatch", true)]
    [TestCase("NewLineBeforeCatch", false)]
    [TestCase("NewLineBeforeFinally", true)]
    [TestCase("NewLineBeforeFinally", false)]
    [TestCase("NewLineBeforeMembersInObjectInitializers", true)]
    [TestCase("NewLineBeforeMembersInObjectInitializers", false)]
    [TestCase("NewLineBeforeMembersInObjectInitializers", null)]
    [TestCase("NewLineBeforeMembersInAnonymousTypes", true)]
    [TestCase("NewLineBeforeMembersInAnonymousTypes", false)]
    [TestCase("NewLineBeforeMembersInAnonymousTypes", null)]
    [TestCase("NewLineBetweenQueryExpressionClauses", true)]
    [TestCase("NewLineBetweenQueryExpressionClauses", false)]
    [TestCase("NewLineBetweenQueryExpressionClauses", null)]
    [TestCase("UsePrettierStyleTrailingCommas", true)]
    [TestCase("UsePrettierStyleTrailingCommas", false)]
    [TestCase("QualificationForEvent", true)]
    [TestCase("QualificationForEvent", false)]
    [TestCase("QualificationForEvent", null)]
    public async Task Should_Return_In_Json(string propertyName, bool? value)
    {
        var context = new TestContext();
        if (value.HasValue)
        {
            context.WhenAFileExists(
                "c:/test/.csharpierrc",
                $"{{ \"{propertyName.ToCamelCase()}\": {value.Value.ToString().ToLower()} }}"
            );
        }
        else
        {
            context.WhenAFileExists(
                "c:/test/.csharpierrc",
                $"{{ \"{propertyName.ToCamelCase()}\": null }}"
            );
        }

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        var propertyInfo = result.GetType().GetProperty(propertyName);
        propertyInfo.Should().NotBeNull();

        propertyInfo!.GetValue(result).Should().Be(value);
    }

    [Test]
    public async Task Should_Return_NewLineBeforeOpenBrace_With_Yaml()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.csharpierrc",
            "newLineBeforeOpenBrace: accessors,AnonymousMethods"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result
            .NewLineBeforeOpenBrace.Should()
            .Be(BraceNewLine.Accessors | BraceNewLine.AnonymousMethods);
    }

    [TestCase("NewLineBeforeElse", true)]
    [TestCase("NewLineBeforeElse", false)]
    [TestCase("NewLineBeforeCatch", true)]
    [TestCase("NewLineBeforeCatch", false)]
    [TestCase("NewLineBeforeFinally", true)]
    [TestCase("NewLineBeforeFinally", false)]
    [TestCase("NewLineBeforeMembersInObjectInitializers", true)]
    [TestCase("NewLineBeforeMembersInObjectInitializers", false)]
    [TestCase("NewLineBeforeMembersInObjectInitializers", null)]
    [TestCase("NewLineBeforeMembersInAnonymousTypes", true)]
    [TestCase("NewLineBeforeMembersInAnonymousTypes", false)]
    [TestCase("NewLineBeforeMembersInAnonymousTypes", null)]
    [TestCase("NewLineBetweenQueryExpressionClauses", true)]
    [TestCase("NewLineBetweenQueryExpressionClauses", false)]
    [TestCase("NewLineBetweenQueryExpressionClauses", null)]
    [TestCase("UsePrettierStyleTrailingCommas", true)]
    [TestCase("UsePrettierStyleTrailingCommas", false)]
    [TestCase("QualificationForEvent", true)]
    [TestCase("QualificationForEvent", false)]
    [TestCase("QualificationForEvent", null)]
    public async Task Should_Return_In_With_Yaml(string propertyName, bool? value)
    {
        var context = new TestContext();
        if (value.HasValue)
        {
            context.WhenAFileExists(
                "c:/test/.csharpierrc",
                $"{propertyName.ToCamelCase()}: {value.Value.ToString().ToLower()}"
            );
        }
        else
        {
            context.WhenAFileExists(
                "c:/test/.csharpierrc",
                $"{propertyName.ToCamelCase()}: null"
            );
        }

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        var propertyInfo = result.GetType().GetProperty(propertyName);
        propertyInfo.Should().NotBeNull();

        propertyInfo!.GetValue(result).Should().Be(value);
    }

    [Test]
    public async Task Should_Support_EditorConfig_NewLineBeforeOpenBrace()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
    [*]
    csharp_new_line_before_open_brace = accessors,anonymous_methods
    "
        );

        var result = await context.CreateProviderAndGetOptionsFor(
            "c:/test/subfolder",
            "c:/test/subfolder/test.cs",
            limitEditorConfigSearch: true
        );
        result
            .NewLineBeforeOpenBrace.Should()
            .Be(BraceNewLine.Accessors | BraceNewLine.AnonymousMethods);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Should_Support_EditorConfig_NewLineBeforeElse(bool value)
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
    [*]
    csharp_new_line_before_else = "
                + value.ToString().ToLower()
                + @"
    "
        );

        var result = await context.CreateProviderAndGetOptionsFor(
            "c:/test/subfolder",
            "c:/test/subfolder/test.cs",
            limitEditorConfigSearch: true
        );
        result.NewLineBeforeElse.Should().Be(value);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Should_Support_EditorConfig_NewLineBeforeCatch(bool value)
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
    [*]
    csharp_new_line_before_catch = "
                + value.ToString().ToLower()
                + @"
    "
        );

        var result = await context.CreateProviderAndGetOptionsFor(
            "c:/test/subfolder",
            "c:/test/subfolder/test.cs",
            limitEditorConfigSearch: true
        );
        result.NewLineBeforeCatch.Should().Be(value);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Should_Support_EditorConfig_NewLineBeforeFinally(bool value)
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
    [*]
    csharp_new_line_before_finally = "
                + value.ToString().ToLower()
                + @"
    "
        );

        var result = await context.CreateProviderAndGetOptionsFor(
            "c:/test/subfolder",
            "c:/test/subfolder/test.cs",
            limitEditorConfigSearch: true
        );
        result.NewLineBeforeFinally.Should().Be(value);
    }

    [TestCase(true)]
    [TestCase(false)]
    [TestCase(null)]
    public async Task Should_Support_EditorConfig_NewLineBeforeMembersInObjectInitializers(
        bool? value
    )
    {
        var context = new TestContext();

        if (value.HasValue)
        {
            context.WhenAFileExists(
                "c:/test/.editorconfig",
                @"
        [*]
        csharp_new_line_before_members_in_object_initializers = "
                    + value.Value.ToString().ToLower()
                    + @"
        "
            );
        }

        var result = await context.CreateProviderAndGetOptionsFor(
            "c:/test/subfolder",
            "c:/test/subfolder/test.cs",
            limitEditorConfigSearch: true
        );
        result.NewLineBeforeMembersInObjectInitializers.Should().Be(value);
    }

    [TestCase(true)]
    [TestCase(false)]
    [TestCase(null)]
    public async Task Should_Support_EditorConfig_NewLineBeforeMembersInAnonymousTypes(bool? value)
    {
        var context = new TestContext();

        if (value.HasValue)
        {
            context.WhenAFileExists(
                "c:/test/.editorconfig",
                @"
        [*]
        csharp_new_line_before_members_in_anonymous_types = "
                    + value.Value.ToString().ToLower()
                    + @"
        "
            );
        }

        var result = await context.CreateProviderAndGetOptionsFor(
            "c:/test/subfolder",
            "c:/test/subfolder/test.cs",
            limitEditorConfigSearch: true
        );
        result.NewLineBeforeMembersInAnonymousTypes.Should().Be(value);
    }

    [TestCase(true)]
    [TestCase(false)]
    [TestCase(null)]
    public async Task Should_Support_EditorConfig_NewLineBetweenQueryExpressionClauses(bool? value)
    {
        var context = new TestContext();

        if (value.HasValue)
        {
            context.WhenAFileExists(
                "c:/test/.editorconfig",
                @"
        [*]
        csharp_new_line_between_query_expression_clauses = "
                    + value.Value.ToString().ToLower()
                    + @"
        "
            );
        }

        var result = await context.CreateProviderAndGetOptionsFor(
            "c:/test/subfolder",
            "c:/test/subfolder/test.cs",
            limitEditorConfigSearch: true
        );
        result.NewLineBetweenQueryExpressionClauses.Should().Be(value);
    }
}
