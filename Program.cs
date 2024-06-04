using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DLLWithFramework
{
    public class Program
    {
        private static readonly IEnumerable<string> DefaultNamespaces =
            new[]
            {
                "System",
                "System.IO",
                "System.Net",
                "System.Linq",
                "System.Text",
                "System.Text.RegularExpressions",
                "System.Collections.Generic"
            };

        private static string runtimePath = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\{0}.dll";

        private static readonly IEnumerable<MetadataReference> DefaultReferences =
            new[]
            {
                // STEP 3 : Add References: Add any necessary references (such as assemblies or other projects) to the compilation. You can use the MetadataReference.CreateFromFile method to add references to external assemblies.
                MetadataReference.CreateFromFile(string.Format(runtimePath, "mscorlib")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "System")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "System.Core")),
                MetadataReference.CreateFromFile("C:\\Users\\AvdueshGautam\\Desktop\\AgeNuget\\bin\\Debug\\net8.0\\AgeCalculator.dll"),
                MetadataReference.CreateFromFile("C:\\Program Files\\dotnet\\sdk\\8.0.102\\Microsoft\\Microsoft.NET.Build.Extensions\\net461\\lib\\System.Runtime.dll"),
                MetadataReference.CreateFromFile("C:\\Users\\AvdueshGautam\\Desktop\\KubernetesClient.dll"),
            };

        // STEP 4 : Create Compilation Options: Set up compilation options, such as language version, optimization level, and platform target. You can use the CSharpCompilationOptions or VisualBasicCompilationOptions classes to configure these options.
        private static readonly CSharpCompilationOptions DefaultCompilationOptions =
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOverflowChecks(true).WithOptimizationLevel(OptimizationLevel.Release)
                    .WithUsings(DefaultNamespaces);

        public static SyntaxTree Parse(string text, string filename = "", CSharpParseOptions options = null)
        {
            var stringText = SourceText.From(text, Encoding.UTF8);
            return SyntaxFactory.ParseSyntaxTree(stringText, options, filename);
        }

        static void Main(string[] args)
        {
            var fileToCompile = @"C:\Users\AvdueshGautam\Desktop\AgeNuget\Program.cs";
            var source = File.ReadAllText(fileToCompile);

            // STEP 2 : Add Syntax Trees: Next, add your source code as syntax trees to the compilation. You can parse your code files into syntax trees using the SyntaxTree.ParseText method.
            var parsedSyntaxTree = Parse(source, "", CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7));

            // STEP 1 : Create a Compilation: First, create an instance of the Compilation class. A compilation represents a single invocation of the compiler and is an immutable representation of your code. You can add your source files, references, and other compilation options to it

            var compilation
                = CSharpCompilation.Create("Test.dll", new SyntaxTree[] { parsedSyntaxTree }, DefaultReferences, DefaultCompilationOptions);

            try
            {
                // STEP 5 : Emit the Assembly: Finally, emit the compiled assembly. You can use the Emit method on the compilation to generate the assembly in memory. If the compilation is successful, you’ll get an EmitResult with the assembly bytes.
                var result = compilation.Emit(@"C:\Temp\Test.dll");

                Console.WriteLine(result.Success ? "Sucess!!" : "Failed");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.Read();
        }
    }
}
