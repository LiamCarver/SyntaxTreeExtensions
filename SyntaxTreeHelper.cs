using CSharpClassHelper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace SyntaxTreeExtensions
{
    public static class SyntaxTreeHelper
    {
        public static IEnumerable<TypeDeclarationSyntax> GetTypeDeclarations(this SyntaxTree syntaxTree)
        {
            return syntaxTree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>();
        }

        public static IEnumerable<string> GetUsingDirectives(this SyntaxTree syntaxTree)
        {
            return syntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>().Select(x => x.ToString().Split(' ')[1].Replace(";", string.Empty)).ToList();
        }

        public static IEnumerable<SyntaxTree> GetSyntaxTreesWithTypeDeclarations(this Compilation compilation)
        {
            return compilation.SyntaxTrees.Where(x => x.GetTypeDeclarations().Any());
        }

        public static List<CSharpClassDefinition> GetCSharpClassDefinitions(this GeneratorExecutionContext context)
        {
            var classDefinitions = new List<CSharpClassDefinition>();
            var syntaxTreesWithTypeDeclarations = context.Compilation.GetSyntaxTreesWithTypeDeclarations();

            foreach (var syntaxTree in syntaxTreesWithTypeDeclarations)
            {
                var usingStatements = syntaxTree.GetUsingDirectives();
                var typeDeclarations = syntaxTree.GetTypeDeclarations();

                foreach (var delaration in typeDeclarations)
                {
                    classDefinitions.Add(CSharpClassDefinition.CreateFromTypeDeclarationSyntax(delaration, syntaxTree.GetUsingDirectives()));
                }
            }

            return classDefinitions;
        }

    }
}
