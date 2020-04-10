using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpSource.Utilities;

namespace SharpSource.Diagnostics.AccessingTaskResultWithoutAwait
{
    [ExportCodeFixProvider(DiagnosticId.AccessingTaskResultWithoutAwait + "CF", LanguageNames.CSharp), Shared]
    public class AccessingTaskResultWithoutAwaitCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(AccessingTaskResultWithoutAwaitAnalyzer.Rule.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var taskResultExpression = root.FindToken(diagnosticSpan.Start)
                .Parent
                .AncestorsAndSelf()
                .OfType<MemberAccessExpressionSyntax>()
                .SingleOrDefault(x => x.Name.Identifier.ValueText == "Result");

            context.RegisterCodeFix(
                CodeAction.Create(Resources.AccessingTaskResultWithoutAwaitCodeFixTitle,
                    x => UseAwait(context.Document, taskResultExpression, root, x),
                    AccessingTaskResultWithoutAwaitAnalyzer.Rule.Id),
                diagnostic);
        }

        private Task<Document> UseAwait(Document document, MemberAccessExpressionSyntax memberAccessExpression, SyntaxNode root, CancellationToken x)
        {
            if (memberAccessExpression == null)
            {
                return Task.FromResult(document);
            }
            var newExpression = SyntaxFactory.AwaitExpression(memberAccessExpression.Expression);
            var newRoot = root.ReplaceNode(memberAccessExpression, newExpression);
            var newDocument = document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument);
        }
    }
}
