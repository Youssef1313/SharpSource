using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SharpSource.Utilities;

namespace SharpSource.Diagnostics.RecursiveEqualityOperatorOverload
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RecursiveOperatorOverloadAnalyzer : DiagnosticAnalyzer
    {
        private const DiagnosticSeverity Severity = DiagnosticSeverity.Error;

        private static readonly string Category = Resources.GeneralCategory;
        private static readonly string Message = Resources.RecursiveEqualityOperatorOverloadMessage;
        private static readonly string Title = Resources.RecursiveEqualityOperatorOverloadMessage;

        public static DiagnosticDescriptor Rule
            => new(DiagnosticId.RecursiveEqualityOperatorOverload, Title, Message, Category, Severity, isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.OperatorDeclaration);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var operatorDeclaration = (OperatorDeclarationSyntax)context.Node;
            var definedToken = operatorDeclaration.OperatorToken;
            var hasBody = operatorDeclaration.Body != null;
            var hasExpression = operatorDeclaration.ExpressionBody != null;

            if (!hasBody && !hasExpression)
            {
                return;
            }

            var operatorUsages = hasBody ? operatorDeclaration.Body.DescendantTokens().Where(x => x.IsKind(definedToken.Kind())).ToArray()
                                         : operatorDeclaration.ExpressionBody.DescendantTokens().Where(x => x.IsKind(definedToken.Kind())).ToArray();

            if (operatorUsages.Length == 0)
            {
                return;
            }

            var enclosingTypeNode = operatorDeclaration.GetEnclosingTypeNode();
            if (enclosingTypeNode == null)
            {
                return;
            }

            var enclosingSymbol = context.SemanticModel.GetDeclaredSymbol(enclosingTypeNode);
            if (enclosingSymbol == null)
            {
                return;
            }

            if (definedToken.IsKind(SyntaxKind.TrueKeyword) || definedToken.IsKind(SyntaxKind.FalseKeyword))
            {
                checkForTrueOrFalseKeyword();
                return;
            }

            foreach (var usage in operatorUsages)
            {
                var surroundingNode = hasBody ? operatorDeclaration.Body.FindNode(usage.FullSpan)
                                              : operatorDeclaration.ExpressionBody.FindNode(usage.FullSpan);
                if (surroundingNode == null)
                {
                    continue;
                }

                if (surroundingNode is not ExpressionSyntax expression)
                {
                    continue;
                }

                switch (expression)
                {
                    case BinaryExpressionSyntax binaryExpression:
                        var hasWarned = checkOperatorToken(binaryExpression.OperatorToken, binaryExpression.Left);
                        if (!hasWarned)
                        {
                            checkOperatorToken(binaryExpression.OperatorToken, binaryExpression.Right);
                        }

                        break;

                    case PrefixUnaryExpressionSyntax prefixUnaryExpression:
                        checkOperatorToken(prefixUnaryExpression.OperatorToken, prefixUnaryExpression.Operand);
                        break;

                    case PostfixUnaryExpressionSyntax postfixUnaryExpression:
                        checkOperatorToken(postfixUnaryExpression.OperatorToken, postfixUnaryExpression.Operand);
                        break;

                    default:
                        continue;
                }
            }

            bool checkOperatorToken(SyntaxToken token, ExpressionSyntax expression)
            {
                if (!token.IsKind(definedToken.Kind()))
                {
                    return false;
                }

                var usedType = context.SemanticModel.GetTypeInfo(expression).Type;
                if (usedType == null)
                {
                    return false;
                }

                if (!usedType.Equals(enclosingSymbol, SymbolEqualityComparer.Default))
                {
                    return false;
                }

                context.ReportDiagnostic(Diagnostic.Create(Rule, token.GetLocation()));
                return true;
            }

            void checkForTrueOrFalseKeyword()
            {
                if (hasBody)
                {
                    var ifConditions = operatorDeclaration.Body.DescendantNodes().OfType<IfStatementSyntax>().ToArray();
                    foreach (var ifCondition in ifConditions)
                    {
                        checkOperatorToken(definedToken, ifCondition.Condition);
                    }
                }
                else if (hasExpression)
                {
                    var conditionalExpressions = operatorDeclaration.ExpressionBody.Expression.DescendantNodesAndSelf().OfType<ConditionalExpressionSyntax>().ToArray();
                    foreach (var conditionalExpression in conditionalExpressions)
                    {
                        checkOperatorToken(definedToken, conditionalExpression.Condition);
                    }
                }
            }
        }
    }
}