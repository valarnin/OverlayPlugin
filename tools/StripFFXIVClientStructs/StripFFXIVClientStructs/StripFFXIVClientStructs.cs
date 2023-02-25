using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StripFFXIVClientStructs
{
    class StripFFXIVClientStructs
    {
        private static SyntaxKind[] DiscardKinds = new SyntaxKind[] {
            SyntaxKind.PropertyDeclaration,
            SyntaxKind.MethodDeclaration,
            SyntaxKind.ConstructorDeclaration,
            SyntaxKind.AttributeTargetSpecifier,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.IncompleteMember,
            SyntaxKind.GlobalStatement,
            SyntaxKind.ConversionOperatorDeclaration,
            SyntaxKind.RecordDeclaration,
            SyntaxKind.IndexerDeclaration,
            SyntaxKind.OperatorDeclaration,
            SyntaxKind.DelegateDeclaration,
            SyntaxKind.ImplicitObjectCreationExpression,
        };
        private static string[] DiscardAttributes = new string[] {
            "DisableRuntimeMarshalling",
            "Flags",
            "",
            "Addon",
            "Agent",
            "Obsolete",
            "FixedArray",
            "FixedSizeArray",
            "AssemblyCompany",
            "AssemblyProduct",
            "AssemblyTitle",
            "AssemblyConfiguration",
            "InfoProxy",
        };

        private static string[] DiscardFiles = new string[] {
            @"\FFXIVClientStructs\GlobalUsings.cs",
            @"\FFXIVClientStructs\AssemblyAttributes.cs",
            @"\FFXIVClientStructs\Interop",
            @"\FFXIVClientStructs\Attributes",
            @"\FFXIVClientStructs.InteropSourceGenerators",
            @"\FFXIVClientStructs.ResolverTester",
            @"\FFXIVClientStructs\Havok",
            @"\FFXIVClientStructs\FFXIV\Client\Graphics\Kernel\CVector.cs",
            @"\FFXIVClientStructs\FFXIV\Component\GUI\AtkLinkedList.cs",
            @"\FFXIVClientStructs\STD\Deque.cs",
            @"\FFXIVClientStructs\STD\Map.cs",
            @"\FFXIVClientStructs\STD\Pair.cs",
            @"\FFXIVClientStructs\STD\Set.cs",
            @"\FFXIVClientStructs\STD\Vector.cs",
            @"\ida",
        };

        private static string[] GlobalUsings = new string[] {
            "System.Runtime.InteropServices",
            "FFXIVClientStructs.{0}.STD",
            "FFXIVClientStructs.{0}.FFXIV.Client.Graphics",
            "FFXIVClientStructs.{0}.FFXIV.Common.Math",
        };

        private static Dictionary<string, string> GenericMaps = new Dictionary<string, string>(){
            {
                "StdPair",
                @"
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public unsafe struct StdPair{0}{2}
    {{
        public {1} Item1;
        public {3} Item2;
    }}
"},
            {
                "StdSet",
                @"
    [StructLayout(LayoutKind.Sequential, Size = 0x10)]
    public unsafe struct StdSet{0}
    {{
        public Node* Head;
        public ulong Count;
        public ref struct Enumerator
        {{
            private readonly Node* _head;
            private Node* _current;
        }}

        [StructLayout(LayoutKind.Sequential)]
        public struct Node
        {{
            public Node* Left;
            public Node* Parent;
            public Node* Right;
            public byte Color;
            public bool IsNil;
            public byte _18;
            public byte _19;
            public {1} Key;
        }}
    }}
" },
            {
                "StdVector",
                @"
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct StdVector{0}
    {{
        public {1}* First;
        public {1}* Last;
        public {1}* End;
    }}
" },
            {
                "StdMap",
                @"
    [StructLayout(LayoutKind.Sequential, Size = 0x10)]
    public unsafe struct StdMap{0}{2}
    {{
        public Node* Head;
        public ulong Count;
        public ref struct Enumerator
        {{
            private readonly Node* _head;
            private Node* _current;
        }}

        [StructLayout(LayoutKind.Sequential)]
        public struct Node
        {{
            public Node* Left;
            public Node* Parent;
            public Node* Right;
            public byte Color;
            public bool IsNil;
            public byte _18;
            public byte _19;
            public StdPair<{1}, {3}> KeyValuePair;
        }}
    }}
" },
            {
                "StdDeque",
                @"
    [StructLayout(LayoutKind.Sequential, Size = 0x28)]
    public unsafe struct StdDeque{0}
    {{
        public void* ContainerBase; // iterator base nonsense
        public {1}* Map; // pointer to array of pointers (size MapSize) to arrays of T (size BlockSize)
        public ulong MapSize; // size of map
        public ulong MyOff; // offset of current first element
        public ulong MySize; // current length 
    }}
" },
            {
                "AtkLinkedList",
                @"
    [StructLayout(LayoutKind.Sequential, Size = 0x18)]
    public unsafe struct AtkLinkedList{0}
    {{
        [StructLayout(LayoutKind.Sequential)]
        public struct Node
        {{
            public {1} Value;
            public Node* Next;
            public Node* Previous;
        }}

        public Node* End;
        public Node* Start;
        public uint Count;
    }}
" },
            {
                "CVector",
                @"
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CVector{0}
    {{
        public void* vtbl;
        public StdVector<{1}> Vector;
    }}
" },
        };

        private static Regex GenericTypeRenamer = new Regex("[^a-zA-Z0-9]");

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Missing required namespace");
                return;
            }

            var ns = args[0];

            if (args.Length < 2)
            {
                Console.WriteLine("Missing required path");
                return;
            }

            var path = args[1];
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Path {path} does not exist");
                return;
            }
            path = Path.GetFullPath(path);

            string dest;
            if (args.Length > 2)
            {
                dest = Path.GetFullPath(args[2]);
                if (!Directory.Exists(dest))
                {
                    Directory.CreateDirectory(dest);
                }
            }
            else
            {
                dest = path;
            }

            foreach (var file in Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories))
            {
                var relFile = file.Replace(path, "");
                if (DiscardFiles.Any((discard) => relFile.StartsWith(discard)))
                {
                    if (path == dest)
                    {
                        File.Delete(file);
                    }
                    continue;
                }

                try {
                    var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
                    var root = tree.GetRoot();
                    var rewriter = new SyntaxRewriter(ns);
                    CompilationUnitSyntax outTree = (CompilationUnitSyntax)rewriter.Visit(root);

                    foreach (var usingValTemplate in GlobalUsings)
                    {
                        var usingVal = string.Format(usingValTemplate, ns);

                        if (!rewriter.usings.Contains(usingVal))
                        {
                            outTree = outTree.AddUsings(new [] {
                                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(usingVal))
                            });
                        }
                    }

                    var destFile = file.Replace(path, dest);
                    Directory.CreateDirectory(Directory.GetParent(destFile).FullName);
                    File.WriteAllText(destFile, outTree.NormalizeWhitespace().ToFullString());
                } catch (Exception e) {
                    Console.WriteLine(file);
                    throw e;
                }
            }
            return;
        }

        class SyntaxRewriter : CSharpSyntaxRewriter
        {
            public List<string> usings = new List<string>();
            private string structsNS;
            private Dictionary<string, SyntaxNode> PendingNodes = new Dictionary<string, SyntaxNode>();
            private StructDeclarationSyntax topStruct = null;
            private Dictionary<string, string> remapUsingAliases = new Dictionary<string, string>();

            public SyntaxRewriter(string ns)
            {
                structsNS = ns;
            }

            [return: NotNullIfNotNull("node")]
            public override SyntaxNode Visit(SyntaxNode node)
            {
                if (node == null)
                {
                    return base.Visit(node);
                }
#if true
                // For debugging/testing, since conditional breakpoints on `ToString`'d results don't work
                // And checking `rawText.Contains` in a conditional breakpoint is incredibly slow
                var rawText = node.ToString();
                if (rawText.Equals("CategoryMap"))
                {
                    var b = "";
                }
#endif
                if (DiscardKinds.Contains(node.Kind()))
                {
                    return base.Visit(null);
                }
                return base.Visit(node);
            }

            public override SyntaxNode VisitUsingDirective(UsingDirectiveSyntax node)
            {
                if (node.ToString().Contains("="))
                {
                    remapUsingAliases.Add(node.Alias.Name.Identifier.ValueText, node.Name.ToString());
                    return Visit(null);
                }
                if (node.Name.ToString().EndsWith("Havok"))
                {
                    return Visit(null);
                }
                var newNameString = node.Name.ToString();
                if (newNameString.StartsWith("FFXIVClientStructs") && !newNameString.StartsWith("FFXIVClientStructs." + structsNS))
                {
                    usings.Add(node.ChildNodes().First().ToString());
                    return SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("FFXIVClientStructs." + structsNS + newNameString.Substring(18))).NormalizeWhitespace();
                }
                usings.Add(node.ChildNodes().First().ToString());

                return base.VisitUsingDirective(node);
            }
            public override SyntaxNode VisitTypeParameter(TypeParameterSyntax node)
            {
                SyntaxNode lastNode = null;
                // Check to see if the next node in parent from this one contains " : unmanaged"
                if (node.Parent.ChildNodes().Any((node) => {
                    if (lastNode == node && node.ToString().Contains(" : unmanaged"))
                    {
                        return true;
                    }
                    lastNode = node;
                    return false;
                }
                ))
                {
                    // Add a new concrete type for this generic parameter instead
                    NamespaceDeclarationSyntax ns = (NamespaceDeclarationSyntax)node.Ancestors().First((node) => node.IsKind(SyntaxKind.NamespaceDeclaration));
                }
                return base.VisitTypeParameter(node);
            }
            public override SyntaxNode VisitAttribute(AttributeSyntax node)
            {
                var attributeName = node.ChildNodes().First().GetFirstToken().ToString();
                if (DiscardAttributes.Contains(attributeName))
                {
                    return Visit(null);
                }
                return base.VisitAttribute(node);
            }
            public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
            {
                var nodeString = node.ToString();
                if (nodeString.Contains(" = new("))
                {
                    return Visit(null);
                }
                if (nodeString.StartsWith("public const float Deg2Rad"))
                {
                    return Visit(null);
                }
                if (nodeString.StartsWith("public const float Rad2Deg"))
                {
                    return Visit(null);
                }
                return base.VisitFieldDeclaration(node);
            }
            public override SyntaxNode VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
            {
                var newName = node.Name;
                var newNameString = newName.ToString();
                if (newNameString.StartsWith("FFXIVClientStructs") && !newNameString.StartsWith("FFXIVClientStructs." + structsNS))
                {
                    newName = SyntaxFactory.ParseName("FFXIVClientStructs." + structsNS + newNameString.Substring(18));
                    return Visit(SyntaxFactory.NamespaceDeclaration(
                        node.AttributeLists, node.Modifiers, node.NamespaceKeyword,
                        newName, SyntaxFactory.Token(SyntaxKind.OpenBraceToken), node.Externs, node.Usings,
                        node.Members, SyntaxFactory.Token(SyntaxKind.CloseBraceToken), node.SemicolonToken));
                }
                return base.VisitNamespaceDeclaration(node);
            }
            public override SyntaxNode VisitFileScopedNamespaceDeclaration(FileScopedNamespaceDeclarationSyntax node)
            {
                var newName = node.Name;
                var newNameString = newName.ToString();
                if (newNameString.StartsWith("FFXIVClientStructs") && !newNameString.StartsWith("FFXIVClientStructs." + structsNS))
                {
                    newName = SyntaxFactory.ParseName("FFXIVClientStructs." + structsNS + newNameString.Substring(18));
                }
                return Visit(SyntaxFactory.NamespaceDeclaration(
                    node.AttributeLists, node.Modifiers, node.NamespaceKeyword,
                    newName, SyntaxFactory.Token(SyntaxKind.OpenBraceToken), node.Externs, node.Usings,
                    node.Members, SyntaxFactory.Token(SyntaxKind.CloseBraceToken), node.SemicolonToken));
            }
            public override SyntaxNode VisitAttributeList(AttributeListSyntax node)
            {
                var newNode = base.VisitAttributeList(node);
                if (newNode.ChildNodes().Count() == 0)
                {
                    return Visit(null);
                }
                return newNode;
            }
            public override SyntaxNode VisitFunctionPointerType(FunctionPointerTypeSyntax node)
            {
                if (node.ToString().Trim().StartsWith("delegate*"))
                {
                    return Visit(SyntaxFactory.PointerType(SyntaxFactory.ParseTypeName("void")));
                }
                return base.VisitFunctionPointerType(node);
            }
            public override SyntaxNode VisitGenericName(GenericNameSyntax node)
            {
                var gnIdentifier = node.Identifier.ToString();
                if (gnIdentifier == "Pointer")
                {
                    return Visit(SyntaxFactory.PointerType((TypeSyntax)node.ChildNodes().First().ChildNodes().First()));
                }

                if (GenericMaps.ContainsKey(gnIdentifier))
                {
                    var typeStringFormatParams = new List<string>();
                    var newName = gnIdentifier;
                    foreach (var t in node.TypeArgumentList.Arguments)
                    {
                        var strippedString = GenericTypeRenamer.Replace(t.ToString(), "");
                        typeStringFormatParams.Add(strippedString);
                        typeStringFormatParams.Add(t.ToString());
                        newName += strippedString;
                    }
                    if (!PendingNodes.ContainsKey(newName))
                    {
                        var typeString = string.Format(GenericMaps[gnIdentifier], typeStringFormatParams.ToArray());
                        PendingNodes.Add(newName, base.Visit(SyntaxFactory.ParseSyntaxTree(typeString).GetRoot().ChildNodes().First()));
                    }
                    return base.Visit(SyntaxFactory.ParseName(newName));
                }
                return base.VisitGenericName(node);
            }

            public override SyntaxNode VisitStructDeclaration(StructDeclarationSyntax node)
            {
                // For the topmost struct, visit all children first, so we can append the new subtypes if needed
                if (topStruct == null)
                {
                    topStruct = node;
                    StructDeclarationSyntax newNode = (StructDeclarationSyntax)base.VisitStructDeclaration(node);
                    if (PendingNodes.Count > 0 && !newNode.Ancestors().Any((ancestor) => ancestor.IsKind(SyntaxKind.StructDeclaration)))
                    {
                        newNode = newNode.InsertNodesAfter(newNode.ChildNodes().Last(), PendingNodes.Values.ToArray());
                        PendingNodes.Clear();
                    }
                    var newStruct = base.VisitStructDeclaration(newNode);
                    topStruct = null;
                    return newStruct;
                }
                return base.VisitStructDeclaration(node);
            }

            public override SyntaxNode VisitTypeArgumentList(TypeArgumentListSyntax node)
            {
                var newNode = base.VisitTypeArgumentList(node);
                if (newNode.ChildNodes().Count() == 0)
                {
                    return Visit(null);
                }
                return newNode;
            }

            public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
            {
                switch (node.Identifier.ValueText)
                {
                    case "nint":
                        return Visit(SyntaxFactory.ParseName("long"));
                    case "hkaSkeleton":
                    case "hkLoader":
                    case "hkaSkeletonMapper":
                        return Visit(SyntaxFactory.ParseName("void"));
                    case "MathF":
                        return Visit(SyntaxFactory.ParseName("System.Math"));
                }

                if (remapUsingAliases.ContainsKey(node.Identifier.ValueText))
                {
                    return Visit(SyntaxFactory.ParseName(remapUsingAliases[node.Identifier.ValueText]));
                }
                return base.VisitIdentifierName(node);
            }

            public override SyntaxNode VisitBaseList(BaseListSyntax node)
            {
                var types = node.Types.Where((type) => {
                    switch (type.ToString().Split("<")[0])
                    {
                        case "IEquatable":
                        case "IFormattable":
                            return false;
                    }
                    return true;
                }).ToArray();
                if (types.Length != node.Types.Count)
                {
                    return Visit(SyntaxFactory.BaseList(node.ColonToken, SyntaxFactory.SeparatedList(types)));
                }
                if (node.Types.Count == 0)
                {
                    return null;
                }
                return base.VisitBaseList(node);
            }
        }
    }
}
