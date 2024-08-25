
# csharpier-config

[![Validate Pull Request](https://github.com/pisolofin/csharpier-editorconfig/actions/workflows/validate_pull_request.yml/badge.svg?branch=feature%2Ftest-config-file)](https://github.com/pisolofin/csharpier-editorconfig/actions/workflows/validate_pull_request.yml)

[![NuGet Version](https://img.shields.io/nuget/v/CSharpier-Config?style=flat&color=blue)](https://www.nuget.org/packages/CSharpier-Config/)


This is as non opinionated version of [csharpier](https://github.com/belav/csharpier) version 0.29.1 tool created to add EditorConfig file style guide, allowing users to define and apply custom styling options.

All documentation that you find about csharpier is also valid for csharpier-config, you have only to change command like
All documentation you find about `CSharpier` is also valid for `CSharpier-Config`. You only need to adjust the commands accordingly.

- `dotnet csharpier` -> `dotnet csharpier-config`
- `dotnet-csharpier` -> `dotnet-csharpier-config`

## Extra rules than CSharpier

**CSharp formatting**

- Support to all [C# formatting options](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/csharp-formatting-options) into `.editorconfig` file

```csharp
#  CSharp formatting rules:
[*.cs]
csharp_new_line_before_open_brace = methods, properties, control_blocks, types
csharp_new_line_before_else = true
csharp_new_line_before_catch = true
csharp_new_line_before_finally = true
csharp_new_line_before_members_in_object_initializers = true
csharp_new_line_before_members_in_anonymous_types = true
csharp_new_line_between_query_expression_clauses = true
```

For more information, please refer to the [documentation](/docs/Configuration.md)

*From*

```csharp
public class ClassName
{
    private int _property = 0;
    public int Property {
        get
            {
            // Some logic
            return _property;
        }
        set {
            // Some logic
            _property = value;
        }
    }

    public void MethodName()
    {
        var anonymous = delegate(int param)
        {
            // Do something
            return param;
        };

        int localFunction(int paramB) {
            int def = paramB;
            return def;
        }

        var condition = true;
        if (condition)
        {
            // Do something
        }    else
        {
            // Do other
        }

        var localObject = new
        {
            Property = 1,
        };
    }
}
```

*To*
```csharp
public class ClassName
{
    private int _property = 0;
    public int Property
    {
        get {
            // Some logic
            return _property;
        }
        set {
            // Some logic
            _property = value;
        }
    }

    public void MethodName()
    {
        var anonymous = delegate(int param) {
            // Do something
            return param;
        };

        int localFunction(int paramB) {
            int def = paramB;
            return def;
        }

        var condition = true;
        if (condition)
        {
            // Do something
        }
        else
        {
            // Do other
        }

        var localObject = new {
            Property = 1,
        };
    }
}

```