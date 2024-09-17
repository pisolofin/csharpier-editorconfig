using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace CSharpier.Playground.Controllers;

using CSharpier.Utilities;

public class FormatResult
{
    public required string Code { get; set; }
    public required string Json { get; set; }
    public required string Doc { get; set; }
    public required List<FormatError> Errors { get; set; }
    public required string SyntaxValidation { get; set; }
}

public class FormatError
{
    public required FileLinePositionSpan LineSpan { get; set; }
    public required string Description { get; set; }
}

[ApiController]
[Route("[controller]")]
public class FormatController : ControllerBase
{
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly ILogger logger;

    // ReSharper disable once SuggestBaseTypeForParameter
    public FormatController(
        IWebHostEnvironment webHostEnvironment,
        ILogger<FormatController> logger
    )
    {
        this.webHostEnvironment = webHostEnvironment;
        this.logger = logger;
    }

    public class PostModel
    {
        public string Code { get; set; } = string.Empty;
        public int PrintWidth { get; set; }
        public int IndentSize { get; set; }
        public bool UseTabs { get; set; }
        public string Parser { get; set; } = string.Empty;

        public BraceNewLine NewLineBeforeOpenBrace { get; set; } = BraceNewLine.All;
        public bool NewLineBeforeElse { get; set; } = true;
        public bool NewLineBeforeCatch { get; set; } = true;
        public bool NewLineBeforeFinally { get; set; } = true;
        public bool? NewLineBeforeMembersInObjectInitializers { get; set; } = null;
        public bool? NewLineBeforeMembersInAnonymousTypes { get; set; } = null;
        public bool? NewLineBetweenQueryExpressionClauses { get; set; } = null;

        public bool? QualificationForField { get; set; } = null;
        public bool? QualificationForProperty { get; set; } = null;
        public bool? QualificationForMethod { get; set; } = null;
        public bool? QualificationForEvent { get; set; } = null;

        public bool UsePrettierStyleTrailingCommas { get; set; } = true;
    }

    [HttpPost]
    public async Task<FormatResult> Post(
        [FromBody] PostModel model,
        CancellationToken cancellationToken
    )
    {
        var sourceCodeKind = model.Parser.EqualsIgnoreCase("CSharp")
            ? SourceCodeKind.Regular
            : SourceCodeKind.Script;
        var result = await CSharpFormatter.FormatAsync(
            model.Code,
            new PrinterOptions
            {
                IncludeAST = true,
                IncludeDocTree = true,
                Width = model.PrintWidth,
                IndentSize = model.IndentSize,
                UseTabs = model.UseTabs,

                NewLineBeforeOpenBrace = model.NewLineBeforeOpenBrace,
                NewLineBeforeElse = model.NewLineBeforeElse,
                NewLineBeforeCatch = model.NewLineBeforeCatch,
                NewLineBeforeFinally = model.NewLineBeforeFinally,
                NewLineBeforeMembersInObjectInitializers =
                    model.NewLineBeforeMembersInObjectInitializers,
                NewLineBeforeMembersInAnonymousTypes = model.NewLineBeforeMembersInAnonymousTypes,
                NewLineBetweenQueryExpressionClauses = model.NewLineBetweenQueryExpressionClauses,

                QualificationForField = model.QualificationForField,
                QualificationForProperty = model.QualificationForProperty,
                QualificationForMethod = model.QualificationForMethod,
                QualificationForEvent = model.QualificationForEvent,

                UsePrettierStyleTrailingCommas = model.UsePrettierStyleTrailingCommas,
            },
            sourceCodeKind,
            cancellationToken
        );

        var comparer = new SyntaxNodeComparer(
            model.Code,
            result.Code,
            result.ReorderedModifiers,
            result.ReorderedUsingsWithDisabledText,
            sourceCodeKind,
            cancellationToken
        );

        return new FormatResult
        {
            Code = result.Code,
            Json = result.AST,
            Doc = result.DocTree,
            Errors = result.CompilationErrors.Select(this.ConvertError).ToList(),
            SyntaxValidation = await comparer.CompareSourceAsync(CancellationToken.None),
        };
    }

    private FormatError ConvertError(Diagnostic diagnostic)
    {
        var lineSpan = diagnostic.Location.SourceTree!.GetLineSpan(diagnostic.Location.SourceSpan);
        return new FormatError { LineSpan = lineSpan, Description = diagnostic.ToString() };
    }

    public string ExecuteApplication(string pathToExe, string workingDirectory, string args)
    {
        var processStartInfo = new ProcessStartInfo(pathToExe, args)
        {
            UseShellExecute = false,
            RedirectStandardError = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            WorkingDirectory = workingDirectory,
            CreateNoWindow = true,
        };

        var process = Process.Start(processStartInfo);
        var output = process!.StandardError.ReadToEnd();
        process.WaitForExit();

        this.logger.LogInformation(
            "Output from '" + pathToExe + " " + args + "' was: " + Environment.NewLine + output
        );

        return output;
    }
}
