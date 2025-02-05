using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using SharpSource.Utilities;

namespace SharpSource.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class EqualsAndGetHashcodeNotImplementedTogetherAnalyzer : DiagnosticAnalyzer
{
    public static DiagnosticDescriptor Rule => new(
        DiagnosticId.EqualsAndGetHashcodeNotImplementedTogether,
        "Implement Equals() and GetHashcode() together.",
        "Equals() and GetHashcode() must be implemented together on {0}",
        Categories.Correctness,
        DiagnosticSeverity.Warning,
        true,
        helpLinkUri: "https://github.com/Vannevelj/SharpSource/blob/master/docs/SS005-EqualsAndGetHashcodeNotImplementedTogether.md");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterCompilationStartAction((compilationContext) =>
    {
        var objectSymbol = compilationContext.Compilation.GetSpecialType(SpecialType.System_Object);
        IMethodSymbol? objectEquals = null;
        IMethodSymbol? objectGetHashCode = null;

        foreach (var symbol in objectSymbol.GetMembers())
        {
            if (symbol is not IMethodSymbol)
            {
                continue;
            }

            var method = (IMethodSymbol)symbol;
            if (method is { MetadataName: nameof(Equals), Parameters.Length: 1 })
            {
                objectEquals = method;
            }

            if (method is { MetadataName: nameof(GetHashCode), Parameters.Length: 0 })
            {
                objectGetHashCode = method;
            }
        }

        compilationContext.RegisterSyntaxNodeAction((syntaxNodeContext) =>
        {
            var classDeclaration = (ClassDeclarationSyntax)syntaxNodeContext.Node;
            var classSymbol = syntaxNodeContext.SemanticModel.GetDeclaredSymbol(classDeclaration);
            if (classSymbol == null)
            {
                return;
            }

            var equalsImplemented = false;
            var getHashcodeImplemented = false;

            foreach (var node in classSymbol.GetMembers())
            {
                if (node is not IMethodSymbol method)
                {
                    continue;
                }

                method = method.GetBaseDefinition();

                if (method.Equals(objectEquals, SymbolEqualityComparer.Default))
                {
                    equalsImplemented = true;
                }

                if (method.Equals(objectGetHashCode, SymbolEqualityComparer.Default))
                {
                    getHashcodeImplemented = true;
                }
            }

            if (equalsImplemented ^ getHashcodeImplemented)
            {
                var properties = ImmutableDictionary.CreateRange(new[] { new KeyValuePair<string, string?>("IsEqualsImplemented", equalsImplemented.ToString()) });
                syntaxNodeContext.ReportDiagnostic(Diagnostic.Create(Rule, classDeclaration.Identifier.GetLocation(), properties, classDeclaration.Identifier.ValueText));
            }
        }, SyntaxKind.ClassDeclaration);
    });
    }
}