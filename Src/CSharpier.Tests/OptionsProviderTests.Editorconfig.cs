namespace CSharpier.Tests;

using CSharpier.Utilities;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;

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
    [TestCase("QualificationForField", true)]
    [TestCase("QualificationForField", false)]
    [TestCase("QualificationForField", null)]
    [TestCase("QualificationForProperty", true)]
    [TestCase("QualificationForProperty", false)]
    [TestCase("QualificationForProperty", null)]
    [TestCase("QualificationForMethod", true)]
    [TestCase("QualificationForMethod", false)]
    [TestCase("QualificationForMethod", null)]
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
    [TestCase("QualificationForField", true)]
    [TestCase("QualificationForField", false)]
    [TestCase("QualificationForField", null)]
    [TestCase("QualificationForProperty", true)]
    [TestCase("QualificationForProperty", false)]
    [TestCase("QualificationForProperty", null)]
    [TestCase("QualificationForMethod", true)]
    [TestCase("QualificationForMethod", false)]
    [TestCase("QualificationForMethod", null)]
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

    [TestCase("NewLineBeforeElse", "csharp_new_line_before_else", true)]
    [TestCase("NewLineBeforeElse", "csharp_new_line_before_else", false)]
    [TestCase("NewLineBeforeCatch", "csharp_new_line_before_catch", true)]
    [TestCase("NewLineBeforeCatch", "csharp_new_line_before_catch", false)]
    [TestCase("NewLineBeforeFinally", "csharp_new_line_before_finally", true)]
    [TestCase("NewLineBeforeFinally", "csharp_new_line_before_finally", false)]
    [TestCase("NewLineBeforeMembersInObjectInitializers", "csharp_new_line_before_members_in_object_initializers", true)]
    [TestCase("NewLineBeforeMembersInObjectInitializers", "csharp_new_line_before_members_in_object_initializers", false)]
    [TestCase("NewLineBeforeMembersInObjectInitializers", "csharp_new_line_before_members_in_object_initializers", null)]
    [TestCase("NewLineBeforeMembersInAnonymousTypes", "csharp_new_line_before_members_in_anonymous_types", true)]
    [TestCase("NewLineBeforeMembersInAnonymousTypes", "csharp_new_line_before_members_in_anonymous_types", false)]
    [TestCase("NewLineBeforeMembersInAnonymousTypes", "csharp_new_line_before_members_in_anonymous_types", null)]
    [TestCase("NewLineBetweenQueryExpressionClauses", "csharp_new_line_between_query_expression_clauses", true)]
    [TestCase("NewLineBetweenQueryExpressionClauses", "csharp_new_line_between_query_expression_clauses", false)]
    [TestCase("NewLineBetweenQueryExpressionClauses", "csharp_new_line_between_query_expression_clauses", null)]
    [TestCase("QualificationForField", "dotnet_style_qualification_for_field", true)]
    [TestCase("QualificationForField", "dotnet_style_qualification_for_field", false)]
    [TestCase("QualificationForField", "dotnet_style_qualification_for_field", null)]
    [TestCase("QualificationForProperty", "dotnet_style_qualification_for_property", true)]
    [TestCase("QualificationForProperty", "dotnet_style_qualification_for_property", false)]
    [TestCase("QualificationForProperty", "dotnet_style_qualification_for_property", null)]
    [TestCase("QualificationForMethod", "dotnet_style_qualification_for_method", true)]
    [TestCase("QualificationForMethod", "dotnet_style_qualification_for_method", false)]
    [TestCase("QualificationForMethod", "dotnet_style_qualification_for_method", null)]
    [TestCase("QualificationForEvent", "dotnet_style_qualification_for_event", true)]
    [TestCase("QualificationForEvent", "dotnet_style_qualification_for_event", false)]
    [TestCase("QualificationForEvent", "dotnet_style_qualification_for_event", null)]
    public async Task Should_Support_EditorConfig(string propertyName, string editorconfigParameterName, bool? value)
    {
        var context = new TestContext();

        if (value.HasValue)
        {
            context.WhenAFileExists(
                "c:/test/.editorconfig",
                $@"
        [*]
        {editorconfigParameterName} = {value.Value.ToString().ToLower()}
        "
            );
        }

        var result = await context.CreateProviderAndGetOptionsFor(
            "c:/test/subfolder",
            "c:/test/subfolder/test.cs",
            limitEditorConfigSearch: true
        );

        var propertyInfo = result.GetType().GetProperty(propertyName);
        propertyInfo.Should().NotBeNull();

        propertyInfo!.GetValue(result).Should().Be(value);
    }
}
