namespace CSharpier.Tests;

using System.IO.Abstractions.TestingHelpers;
using CSharpier.Cli.Options;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

[TestFixture]
[Parallelizable(ParallelScope.Fixtures)]
public class OptionsProviderTests
{
    [Test]
    public async Task Should_Return_Default_Options_With_Empty_Json()
    {
        var context = new TestContext();
        context.WhenAFileExists("c:/test/.csharpierrc", "{}");

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        ShouldHaveDefaultOptions(result);
    }

    [Test]
    public async Task Should_Return_Default_Options_With_No_File_And_Known_Extension()
    {
        var context = new TestContext();
        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        ShouldHaveDefaultOptions(result);
    }

    [Test]
    public async Task Should_Return_Default_Options_With_No_File_And_Unknown_Extension()
    {
        var context = new TestContext();
        var result = async () =>
            await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cst");

        await result.Should().ThrowAsync<Exception>();
    }

    [TestCase(".csharpierrc")]
    [TestCase(".csharpierrc.json")]
    [TestCase(".csharpierrc.yaml")]
    public async Task Should_Return_Default_Options_With_Empty_File(string fileName)
    {
        var context = new TestContext();
        context.WhenAFileExists($"c:/test/{fileName}", string.Empty);
        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        ShouldHaveDefaultOptions(result);
    }

    [Test]
    public async Task Should_Return_Json_Extension_Options()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.csharpierrc.json",
            @"{ 
    ""printWidth"": 10, 
    ""endOfLine"": ""crlf""
}"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.Width.Should().Be(10);
        result.EndOfLine.Should().Be(EndOfLine.CRLF);
    }

    [TestCase("yaml")]
    [TestCase("yml")]
    public async Task Should_Return_Yaml_Extension_Options(string extension)
    {
        var context = new TestContext();
        context.WhenAFileExists(
            $"c:/test/.csharpierrc.{extension}",
            @"
printWidth: 10
endOfLine: crlf
"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.Width.Should().Be(10);
        result.EndOfLine.Should().Be(EndOfLine.CRLF);
    }

    [TestCase("{ \"printWidth\": 10 }")]
    [TestCase("printWidth: 10")]
    public async Task Should_Read_ExtensionLess_File(string contents)
    {
        var context = new TestContext();
        context.WhenAFileExists($"c:/test/.csharpierrc", contents);

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.Width.Should().Be(10);
    }

    [TestCase("", "printWidth: 10")]
    [TestCase("", "{ \"printWidth\": 10 }")]
    [TestCase(".yml", "printWidth: 10")]
    [TestCase(".yaml", "printWidth: 10")]
    [TestCase(".json", "{ \"printWidth\": 10 }")]
    public async Task Should_Find_Configuration_In_Parent_Directory(
        string extension,
        string contents
    )
    {
        var context = new TestContext();
        context.WhenAFileExists($"c:/test/.csharpierrc{extension}", contents);

        var result = await context.CreateProviderAndGetOptionsFor(
            "c:/test/subfolder",
            "c:/test/subfolder/test.cs"
        );

        result.Width.Should().Be(10);
    }

    [Test]
    public async Task Should_Prefer_No_Extension()
    {
        var context = new TestContext();
        context.WhenAFileExists("c:/test/.csharpierrc", "{ \"printWidth\": 1 }");

        context.WhenAFileExists("c:/test/.csharpierrc.json", "{ \"printWidth\": 2 }");
        context.WhenAFileExists("c:/test/.csharpierrc.yaml", "printWidth: 3");

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.Width.Should().Be(1);
    }

    [Test]
    public async Task Should_Return_PrintWidth_With_Json()
    {
        var context = new TestContext();
        context.WhenAFileExists("c:/test/.csharpierrc", "{ \"printWidth\": 10 }");

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.Width.Should().Be(10);
    }

    [Test]
    public async Task Should_Return_TabWidth_With_Json()
    {
        var context = new TestContext();
        context.WhenAFileExists("c:/test/.csharpierrc", "{ \"tabWidth\": 10 }");

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.IndentSize.Should().Be(10);
    }

    [Test]
    public async Task Should_Return_UseTabs_With_Json()
    {
        var context = new TestContext();
        context.WhenAFileExists("c:/test/.csharpierrc", "{ \"useTabs\": true }");

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.UseTabs.Should().BeTrue();
    }

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

    [TestCase(true)]
    [TestCase(false)]
    public async Task Should_Return_NewLineBeforeElse_With_Json(bool value)
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.csharpierrc",
            $"{{ \"newLineBeforeElse\": {value.ToString().ToLower()} }}"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.NewLineBeforeElse.Should().Be(value);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Should_Return_NewLineBeforeCatch_With_Json(bool value)
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.csharpierrc",
            $"{{ \"newLineBeforeCatch\": {value.ToString().ToLower()} }}"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.NewLineBeforeCatch.Should().Be(value);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Should_Return_NewLineBeforeFinally_With_Json(bool value)
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.csharpierrc",
            $"{{ \"newLineBeforeFinally\": {value.ToString().ToLower()} }}"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.NewLineBeforeFinally.Should().Be(value);
    }

    [TestCase(true)]
    [TestCase(false)]
    [TestCase(null)]
    public async Task Should_Return_NewLineBeforeMembersInObjectInitializers_With_Json(bool? value)
    {
        var context = new TestContext();
        if (value.HasValue)
        {
            context.WhenAFileExists(
                "c:/test/.csharpierrc",
                $"{{ \"newLineBeforeMembersInObjectInitializers\": {value.Value.ToString().ToLower()} }}"
            );
        }
        else
        {
            context.WhenAFileExists(
                "c:/test/.csharpierrc",
                $"{{ \"newLineBeforeMembersInObjectInitializers\": null }}"
            );
        }

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.NewLineBeforeMembersInObjectInitializers.Should().Be(value);
    }

    [TestCase(true)]
    [TestCase(false)]
    [TestCase(null)]
    public async Task Should_Return_NewLineBeforeMembersInAnonymousTypes_With_Json(bool? value)
    {
        var context = new TestContext();
        if (value.HasValue)
        {
            context.WhenAFileExists(
                "c:/test/.csharpierrc",
                $"{{ \"newLineBeforeMembersInAnonymousTypes\": {value.Value.ToString().ToLower()} }}"
            );
        }
        else
        {
            context.WhenAFileExists(
                "c:/test/.csharpierrc",
                $"{{ \"newLineBeforeMembersInAnonymousTypes\": null }}"
            );
        }

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.NewLineBeforeMembersInAnonymousTypes.Should().Be(value);
    }

    [TestCase(true)]
    [TestCase(false)]
    [TestCase(null)]
    public async Task Should_Return_NewLineBetweenQueryExpressionClauses_With_Json(bool? value)
    {
        var context = new TestContext();
        if (value.HasValue)
        {
            context.WhenAFileExists(
                "c:/test/.csharpierrc",
                $"{{ \"newLineBetweenQueryExpressionClauses\": {value.Value.ToString().ToLower()} }}"
            );
        }
        else
        {
            context.WhenAFileExists(
                "c:/test/.csharpierrc",
                $"{{ \"newLineBetweenQueryExpressionClauses\": null }}"
            );
        }

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.NewLineBetweenQueryExpressionClauses.Should().Be(value);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Should_Return_UsePrettierStyleTrailingCommas_With_Json(bool value)
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.csharpierrc",
            $"{{ \"usePrettierStyleTrailingCommas\": {value.ToString().ToLower()} }}"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.UsePrettierStyleTrailingCommas.Should().Be(value);
    }

    [Test]
    public async Task Should_Return_PrintWidth_With_Yaml()
    {
        var context = new TestContext();
        context.WhenAFileExists("c:/test/.csharpierrc", "printWidth: 10");

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.Width.Should().Be(10);
    }

    [Test]
    public async Task Should_Return_TabWidth_With_Yaml()
    {
        var context = new TestContext();
        context.WhenAFileExists("c:/test/.csharpierrc", "tabWidth: 10");

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.IndentSize.Should().Be(10);
    }

    [Test]
    public async Task Should_Return_UseTabs_With_Yaml()
    {
        var context = new TestContext();
        context.WhenAFileExists("c:/test/.csharpierrc", "useTabs: true");

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.UseTabs.Should().BeTrue();
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

    [TestCase(true)]
    [TestCase(false)]
    public async Task Should_Return_NewLineBeforeElse_With_Yaml(bool value)
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.csharpierrc",
            $"newLineBeforeElse: {value.ToString().ToLower()}"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.NewLineBeforeElse.Should().Be(value);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Should_Return_NewLineBeforeCatch_With_Yaml(bool value)
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.csharpierrc",
            $"newLineBeforeCatch: {value.ToString().ToLower()}"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.NewLineBeforeCatch.Should().Be(value);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Should_Return_NewLineBeforeFinally_With_Yaml(bool value)
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.csharpierrc",
            $"newLineBeforeFinally: {value.ToString().ToLower()}"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.NewLineBeforeFinally.Should().Be(value);
    }

    [TestCase(true)]
    [TestCase(false)]
    [TestCase(null)]
    public async Task Should_Return_NewLineBeforeMembersInObjectInitializers_With_Yaml(bool? value)
    {
        var context = new TestContext();
        if (value.HasValue)
        {
            context.WhenAFileExists(
                "c:/test/.csharpierrc",
                $"newLineBeforeMembersInObjectInitializers: {value.Value.ToString().ToLower()}"
            );
        }
        else
        {
            context.WhenAFileExists(
                "c:/test/.csharpierrc",
                $"newLineBeforeMembersInObjectInitializers: null"
            );
        }

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.NewLineBeforeMembersInObjectInitializers.Should().Be(value);
    }

    [TestCase(true)]
    [TestCase(false)]
    [TestCase(null)]
    public async Task Should_Return_NewLineBeforeMembersInAnonymousTypes_With_Yaml(bool? value)
    {
        var context = new TestContext();
        if (value.HasValue)
        {
            context.WhenAFileExists(
                "c:/test/.csharpierrc",
                $"newLineBeforeMembersInAnonymousTypes: {value.Value.ToString().ToLower()}"
            );
        }
        else
        {
            context.WhenAFileExists(
                "c:/test/.csharpierrc",
                $"newLineBeforeMembersInAnonymousTypes: null"
            );
        }

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.NewLineBeforeMembersInAnonymousTypes.Should().Be(value);
    }

    [TestCase(true)]
    [TestCase(false)]
    [TestCase(null)]
    public async Task Should_Return_NewLineBetweenQueryExpressionClauses_With_Yaml(bool? value)
    {
        var context = new TestContext();
        if (value.HasValue)
        {
            context.WhenAFileExists(
                "c:/test/.csharpierrc",
                $"newLineBetweenQueryExpressionClauses: {value.Value.ToString().ToLower()}"
            );
        }
        else
        {
            context.WhenAFileExists(
                "c:/test/.csharpierrc",
                $"newLineBetweenQueryExpressionClauses: null"
            );
        }

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.NewLineBetweenQueryExpressionClauses.Should().Be(value);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Should_Return_UsePrettierStyleTrailingCommas_With_Yaml(bool value)
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.csharpierrc",
            $"usePrettierStyleTrailingCommas: {value.ToString().ToLower()}"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.UsePrettierStyleTrailingCommas.Should().Be(value);
    }

    [Test]
    public async Task Should_Return_TabWidth_For_Overrid()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.csharpierrc",
            """
            overrides:
                - files: "*.cst"
                  formatter: "csharp"
                  tabWidth: 2
            """
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cst");

        result.IndentSize.Should().Be(2);
    }

    [Test]
    public async Task Should_Support_EditorConfig_Basic()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
[*]
indent_style = space
indent_size = 2
max_line_length = 10
end_of_line = crlf
"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.UseTabs.Should().BeFalse();
        result.IndentSize.Should().Be(2);
        result.Width.Should().Be(10);
        result.EndOfLine.Should().Be(EndOfLine.CRLF);
    }

    [Test]
    public async Task Should_Support_EditorConfig_With_Comments()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
# EditorConfig is awesome: https://EditorConfig.org

# top-most EditorConfig file
root = true

[*]
indent_style = space
indent_size = 2
max_line_length = 10
; Windows-style line endings
end_of_line = crlf
"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.UseTabs.Should().BeFalse();
        result.IndentSize.Should().Be(2);
        result.Width.Should().Be(10);
        result.EndOfLine.Should().Be(EndOfLine.CRLF);
    }

    [Test]
    public async Task Should_Support_EditorConfig_With_Duplicated_Sections()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
[*]
indent_size = 2

[*]
indent_size = 4
"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.IndentSize.Should().Be(4);
    }

    [Test]
    public async Task Should_Support_EditorConfig_With_Duplicated_Rules()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
[*]
indent_size = 2
indent_size = 4
"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.IndentSize.Should().Be(4);
    }

    [Test]
    public async Task Should_Not_Fail_With_Bad_EditorConfig()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
[*
indent_size==
"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.IndentSize.Should().Be(4);
    }

    [TestCase("tab_width")]
    [TestCase("indent_size")]
    public async Task Should_Support_EditorConfig_Tabs(string propertyName)
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.editorconfig",
            $@"
    [*]
    indent_style = tab
    {propertyName} = 2
    "
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.UseTabs.Should().BeTrue();
        result.IndentSize.Should().Be(2);
    }

    [Test]
    public async Task Should_Support_EditorConfig_Tabs_With_Tab_Width()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
    [*]
    indent_style = tab
    indent_size = 1
    tab_width = 3
    "
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.UseTabs.Should().BeTrue();
        result.IndentSize.Should().Be(3);
    }

    [Test]
    public async Task Should_Support_EditorConfig_Tabs_With_Indent_Size_Tab()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
    [*]
    indent_size = tab
    tab_width = 3
    "
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.IndentSize.Should().Be(3);
    }

    [Test]
    public async Task Should_Support_EditorConfig_Tabs_With_Multiple_Files()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/subfolder/.editorconfig",
            @"
    [*]
    indent_size = 1
    "
        );

        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
    [*]
    indent_size = 2
    max_line_length = 10
    "
        );

        var result = await context.CreateProviderAndGetOptionsFor(
            "c:/test/subfolder",
            "c:/test/subfolder/test.cs"
        );
        result.IndentSize.Should().Be(1);
        result.Width.Should().Be(10);
    }

    [Test]
    public async Task Should_Support_EditorConfig_Tabs_With_Multiple_Files_And_Unset()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/subfolder/.editorconfig",
            @"
    [*]
    indent_size = unset
    "
        );

        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
    [*]
    indent_size = 2
    "
        );

        var result = await context.CreateProviderAndGetOptionsFor(
            "c:/test/subfolder",
            "c:/test/subfolder/test.cs"
        );
        result.IndentSize.Should().Be(4);
    }

    [Test]
    public async Task Should_Support_EditorConfig_Root()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/subfolder/.editorconfig",
            @"
    root = true

    [*]
    indent_size = 2
    "
        );

        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
    [*]
    max_line_length = 2
    "
        );

        var result = await context.CreateProviderAndGetOptionsFor(
            "c:/test/subfolder",
            "c:/test/subfolder/test.cs"
        );
        result.Width.Should().Be(100);
    }

    [Test]
    public async Task Should_Support_EditorConfig_Tabs_With_Globs()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
[*]
indent_size = 1

[*.cs]
indent_size = 2
"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");
        result.IndentSize.Should().Be(2);
    }

    [Test]
    public async Task Should_Support_EditorConfig_Tabs_With_Glob_Braces()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
[*]
indent_size = 1

[*.{cs}]
indent_size = 2
"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");
        result.IndentSize.Should().Be(2);
    }

    [Test]
    public async Task Should_Support_EditorConfig_Tabs_With_Glob_Braces_Multiples()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
[*]
indent_size = 1

[*.{csx,cs}]
indent_size = 2
"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");
        result.IndentSize.Should().Be(2);
    }

    [Test]
    public async Task Should_Find_EditorConfig_In_Parent_Directory()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
[*.cs]
indent_size = 2
"
        );

        var result = await context.CreateProviderAndGetOptionsFor(
            "c:/test/subfolder",
            "c:/test/subfolder/test.cs"
        );
        result.IndentSize.Should().Be(2);
    }

    [Test]
    public async Task Should_Prefer_CSharpierrc_In_SameFolder()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
[*.cs]
indent_size = 2
"
        );
        context.WhenAFileExists("c:/test/.csharpierrc", "tabWidth: 1");

        var result = await context.CreateProviderAndGetOptionsFor(
            "c:/test/subfolder",
            "c:/test/test.cs"
        );
        result.IndentSize.Should().Be(1);
    }

    [Test]
    public async Task Should_Not_Prefer_Closer_EditorConfig()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/subfolder/.editorconfig",
            @"
[*.cs]
indent_size = 2
"
        );
        context.WhenAFileExists("c:/test/.csharpierrc", "tabWidth: 1");

        var result = await context.CreateProviderAndGetOptionsFor(
            "c:/test",
            "c:/test/subfolder/test.cs"
        );
        result.IndentSize.Should().Be(1);
    }

    [Test]
    public async Task Should_Ignore_Invalid_EditorConfig_Lines()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
[*]
indent_size = 2
INVALID
"
        );

        var result = await context.CreateProviderAndGetOptionsFor("c:/test", "c:/test/test.cs");

        result.IndentSize.Should().Be(2);
    }

    [Test]
    public async Task Should_Ignore_Ignored_EditorConfig()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/subfolder/.editorconfig",
            @"
    [*]
    indent_size = 2
    "
        );

        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
    [*]
    indent_size = 1
    "
        );

        context.WhenAFileExists("c:/test/.csharpierignore", "/subfolder/.editorconfig");

        var result = await context.CreateProviderAndGetOptionsFor(
            "c:/test",
            "c:/test/subfolder/test.cs"
        );
        result.IndentSize.Should().Be(1);
    }

    [Test]
    public async Task Should_Prefer_Closer_CSharpierrc()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
[*.cs]
indent_size = 2
"
        );
        context.WhenAFileExists("c:/test/subfolder/.csharpierrc", "tabWidth: 1");

        var result = await context.CreateProviderAndGetOptionsFor(
            "c:/test",
            "c:/test/subfolder/test.cs"
        );
        result.IndentSize.Should().Be(1);
    }

    [Test]
    public async Task Should_Not_Look_For_Subfolders_EditorConfig_When_Limited()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/subfolder/.editorconfig",
            @"
    [*]
    indent_size = 1
    "
        );

        // this shouldn't happen in the real world, but validates we correctly limit
        // the search to the top directory only
        var result = await context.CreateProviderAndGetOptionsFor(
            "c:/test/",
            "c:/test/subfolder/test.cs",
            limitEditorConfigSearch: true
        );
        result.IndentSize.Should().Be(4);
    }

    [Test]
    public async Task Should_Not_Look_For_Subfolders_CSharpierRc_When_Limited()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/subfolder/.csharpierrc",
            @"
    [*]
    tabWidth: 1
    "
        );

        // this shouldn't happen in the real world, but validates we correctly limit
        // the search to the top directory only
        var result = await context.CreateProviderAndGetOptionsFor(
            "c:/test/",
            "c:/test/subfolder/test.cs",
            limitEditorConfigSearch: true
        );
        result.IndentSize.Should().Be(4);
    }

    [Test]
    public async Task Should_Look_For_Subfolders_When_Limited()
    {
        var context = new TestContext();
        context.WhenAFileExists(
            "c:/test/.editorconfig",
            @"
    [*]
    indent_size = 1
    "
        );

        var result = await context.CreateProviderAndGetOptionsFor(
            "c:/test/subfolder",
            "c:/test/subfolder/test.cs",
            limitEditorConfigSearch: true
        );
        result.IndentSize.Should().Be(1);
    }

    private static void ShouldHaveDefaultOptions(PrinterOptions printerOptions)
    {
        printerOptions.Width.Should().Be(100);
        printerOptions.IndentSize.Should().Be(4);
        printerOptions.UseTabs.Should().BeFalse();
        printerOptions.EndOfLine.Should().Be(EndOfLine.Auto);
    }

    private class TestContext
    {
        private readonly MockFileSystem fileSystem = new();

        public void WhenAFileExists(string path, string contents)
        {
            if (!OperatingSystem.IsWindows())
            {
                path = path.Replace("c:", string.Empty);
            }

            this.fileSystem.AddFile(path, new MockFileData(contents));
        }

        public async Task<PrinterOptions> CreateProviderAndGetOptionsFor(
            string directoryName,
            string filePath,
            bool limitEditorConfigSearch = false
        )
        {
            if (!OperatingSystem.IsWindows())
            {
                directoryName = directoryName.Replace("c:", string.Empty);
                filePath = filePath.Replace("c:", string.Empty);
            }

            this.fileSystem.AddDirectory(directoryName);
            var provider = await OptionsProvider.Create(
                directoryName,
                null,
                this.fileSystem,
                NullLogger.Instance,
                CancellationToken.None,
                limitEditorConfigSearch
            );

            var printerOptions = provider.GetPrinterOptionsFor(filePath);

            if (printerOptions is null)
            {
                throw new Exception("PrinterOptions was null");
            }

            return printerOptions;
        }
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
