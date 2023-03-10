﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Unity.Entities.SourceGen.Common
{
    public static class TypeCreationHelpers
    {
        /// <summary>
        /// Line to replace with on generated source.
        /// </summary>
        public static string GeneratedLineTriviaToGeneratedSource => "// __generatedline__";

        /// <summary>
        /// Generates the SourceText from a given list of root nodes using the SyntaxTree's path.
        /// Includes the existing SyntaxTree's root using statements,
        /// and adds correct line directives.
        /// </summary>
        /// <param name="generatorName">Name to base filepath on.</param>
        /// <param name="generatorExecutionContext">Context of the generator executed.</param>
        /// <param name="originalSyntaxTree">Original SyntaxTree to base SourceText location and usings on.</param>
        /// <param name="rootNodes">Root nodes to add to compilation unit.</param>
        /// <returns>The SourceText based on nodes and SyntaxTree filepath.</returns>
        public static SourceText GenerateSourceTextForRootNodes(
            string generatorName,
            GeneratorExecutionContext generatorExecutionContext,
            SyntaxTree originalSyntaxTree,
            IEnumerable<MemberDeclarationSyntax> rootNodes)
        {
            // Create compilation unit
            var existingUsings =
                originalSyntaxTree
                    .GetCompilationUnitRoot(generatorExecutionContext.CancellationToken)
                    .WithoutPreprocessorTrivia().Usings;

            var compilationUnit = SyntaxFactory.CompilationUnit()
                .AddMembers(rootNodes.ToArray())
                .WithoutPreprocessorTrivia()
                .WithUsings(existingUsings)
                .NormalizeWhitespace();

            var generatedSourceFilePath = originalSyntaxTree.GetGeneratedSourceFilePath(generatorExecutionContext.Compilation.Assembly, generatorName);

            // Output as source
            var sourceTextForNewClass =
                compilationUnit.GetText(Encoding.UTF8)
                    .WithInitialLineDirectiveToGeneratedSource(generatedSourceFilePath)
                    .WithIgnoreUnassignedVariableWarning();

            // Add line directives for lines with `GeneratedLineTriviaToGeneratedSource` or #line
            var textChanges = new List<TextChange>();
            foreach (var line in sourceTextForNewClass.Lines)
            {
                var lineText = line.ToString();
                if (lineText.Contains(GeneratedLineTriviaToGeneratedSource))
                {
                    textChanges.Add(new TextChange(line.Span,
                        lineText.Replace(GeneratedLineTriviaToGeneratedSource, $"#line {line.LineNumber + 2} \"{generatedSourceFilePath}\"")));
                }
                else if (lineText.Contains("#line") && lineText.TrimStart().IndexOf("#line", StringComparison.Ordinal) != 0)
                {
                    var indexOfLineDirective = lineText.IndexOf("#line", StringComparison.Ordinal);
                    textChanges.Add(new TextChange(line.Span,
                        lineText.Substring(0, indexOfLineDirective - 1) + Environment.NewLine +
                        lineText.Substring(indexOfLineDirective)));
                }
            }

            return sourceTextForNewClass.WithChanges(textChanges);
        }

        /// <summary>
        /// Get root nodes of a replaced SyntaxTree.
        /// Where all the original nodes in dictionary is replaced with new nodes.
        /// </summary>
        /// <param name="syntaxTree">SyntaxTree to look through.</param>
        /// <param name="originalToReplaced">Dictionary containing keys of original nodes, and values of replacements.</param>
        /// <returns>Root nodes of replaced SyntaxTrees.</returns>
        public static List<MemberDeclarationSyntax> GetReplacedRootNodes(SyntaxTree syntaxTree, IDictionary<TypeDeclarationSyntax, TypeDeclarationSyntax> originalToReplaced)
        {
            var newRootNodes = new List<MemberDeclarationSyntax>();
            var originalToPartialSyntaxDictionary = originalToReplaced.ToDictionary(entry => (SyntaxNode) entry.Key, entry => (SyntaxNode) entry.Value);
            var allOriginalNodesAlsoInReplacedTree = originalToReplaced.Keys.SelectMany(node => node.AncestorsAndSelf()).ToImmutableHashSet();

            foreach (var childNode in syntaxTree.GetRoot().ChildNodes())
            {
                switch (childNode)
                {
                    case NamespaceDeclarationSyntax _:
                    case ClassDeclarationSyntax _:
                    case StructDeclarationSyntax _:
                    {
                        var newRootNode = ConstructReplacedTree(childNode, originalToPartialSyntaxDictionary, allOriginalNodesAlsoInReplacedTree);
                        if (newRootNode != null)
                            newRootNodes.Add(newRootNode);
                        break;
                    }
                }
            }

            return newRootNodes;
        }

        /// <summary>
        /// Constructs a replaced tree based on a root note.
        /// Uses originalToReplacedNode to replace.
        /// Filtered based on replacementNodeCandidates.
        /// </summary>
        /// <param name="currentNode">Root to replace downwards from.</param>
        /// <param name="originalToReplacedNode">Dictionary containing keys of original nodes, and values of replacements.</param>
        /// <param name="replacementNodeCandidates">A list of nodes to look through. (ie. only these nodes will be replaced.)</param>
        /// <returns>Top member of replaced tree.</returns>
        /// <exception cref="InvalidOperationException">
        /// Happens if currentNode is not a class, namespace or struct. (and is contained in replacementNodeCandidates.)
        /// </exception>
        /// <remarks> Uses Downwards Recursion. </remarks>
        static MemberDeclarationSyntax ConstructReplacedTree(SyntaxNode currentNode,
            IDictionary<SyntaxNode, SyntaxNode> originalToReplacedNode,
            ImmutableHashSet<SyntaxNode> replacementNodeCandidates)
        {
            // If this node shouldn't exist in replaced tree, early out
            if (!replacementNodeCandidates.Contains(currentNode))
                return null;

            // Otherwise, check for replaced children by recursing
            var replacedChildren =
                currentNode
                    .ChildNodes()
                    .Select(childNode => ConstructReplacedTree(childNode, originalToReplacedNode, replacementNodeCandidates))
                    .Where(child => child != null).ToArray();

            // Either get the replaced node for this level - or create one - and add the replaced children
            // No node found, need to create a new one to represent this node in the hierarchy
            return currentNode switch
            {
                NamespaceDeclarationSyntax namespaceNode =>
                    SyntaxFactory.NamespaceDeclaration(namespaceNode.Name)
                        .AddMembers(replacedChildren)
                        .WithModifiers(namespaceNode.Modifiers)
                        .WithUsings(namespaceNode.Usings),

                TypeDeclarationSyntax typeNode when originalToReplacedNode.ContainsKey(currentNode) =>
                    (originalToReplacedNode[currentNode] as TypeDeclarationSyntax)?
                        .AddMembers(replacedChildren),

                ClassDeclarationSyntax classNode =>
                    SyntaxFactory.ClassDeclaration(classNode.Identifier)
                        .AddMembers(replacedChildren)
                        .WithBaseList(classNode.BaseList)
                        .WithModifiers(classNode.Modifiers),

                StructDeclarationSyntax structNode =>
                    SyntaxFactory.StructDeclaration(structNode.Identifier)
                        .AddMembers(replacedChildren)
                        .WithBaseList(structNode.BaseList)
                        .WithModifiers(structNode.Modifiers),

                _ => throw new InvalidOperationException($"Expecting class or namespace declaration in syntax tree for {currentNode} but found {currentNode.Kind()}")
            };
        }
    }
}
