using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

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
            MetadataReference.CreateFromFile(string.Format(runtimePath, "mscorlib")),
            MetadataReference.CreateFromFile(string.Format(runtimePath, "System")),
            MetadataReference.CreateFromFile(string.Format(runtimePath, "System.Core")),
            MetadataReference.CreateFromFile("C:\\Program Files\\dotnet\\sdk\\8.0.102\\Microsoft\\Microsoft.NET.Build.Extensions\\net461\\lib\\System.Runtime.dll"),
            MetadataReference.CreateFromFile("C:\\Users\\AvdueshGautam\\Desktop\\CoreApp\\bin\\Debug\\net6.0\\CoreApp.dll"),
            MetadataReference.CreateFromFile("C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\netstandard.dll"),
        };

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
        var assembly = GetAssembly();
        ValidateAssembly(assembly);
    }

    public static Assembly GetAssembly()
    {
        var fileToCompile = @"C:\Users\AvdueshGautam\Desktop\UsingDotNet\Program.cs";
        var source = File.ReadAllText(fileToCompile);

        var parsedSyntaxTree = Parse(source, "", CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7));

        var compilation = CSharpCompilation.Create("Test.dll", new SyntaxTree[] { parsedSyntaxTree }, DefaultReferences, DefaultCompilationOptions);

        try
        {
            var ms = new MemoryStream();
            var emitResult = compilation.Emit(ms);

            if (!emitResult.Success)
            {
                var messageBuilder = new StringBuilder();
                foreach (var item in emitResult.Diagnostics)
                {
                    messageBuilder.AppendLine(item.GetMessage());
                }
                var message = messageBuilder.ToString();

                throw new Exception(message);
            }

            var bytes = ms.ToArray();
            var assembly = Assembly.Load(bytes);

            return assembly;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        Console.Read();
        return default;
    }

    public static void ValidateAssembly(Assembly assembly)
    {
        if (assembly == null)
        {
            throw new Exception("Assembly is null.");
        }

        // Manually load the dependencies
        string coreAppPath = "C:\\Users\\AvdueshGautam\\Desktop\\CoreApp\\bin\\Debug\\net6.0\\CoreApp.dll";
        if (File.Exists(coreAppPath))
        {
            Assembly.LoadFrom(coreAppPath);
        }
        else
        {
            throw new FileNotFoundException($"Dependency not found: {coreAppPath}");
        }

        // Validate that the assembly contains expected types
        Type programType = assembly.GetType("UsingDotNet.Program");
        if (programType == null)
        {
            throw new Exception("Type 'UsingDotNet.Program' not found in assembly.");
        }

        // Validate that the type contains expected methods
        MethodInfo mainMethod = programType.GetMethod("Main", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (mainMethod == null)
        {
            throw new Exception("Method 'Main' not found in type 'UsingDotNet.Program'.");
        }

        // Optionally, invoke the method to test its functionality
        try
        {
            mainMethod.Invoke(null, new object[] { new string[0] });
            Console.WriteLine("Method 'Main' executed successfully.");
        }
        catch (Exception ex)
        {
            throw new Exception("Method 'Main' execution failed.", ex);
        }

        Console.WriteLine("Assembly validation completed successfully.");
    }
}
