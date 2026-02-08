// See https://aka.ms/new-console-template for more information

using System.Reflection;
using ShadcnBlazor.Cli;
using ShadcnBlazor.Cli.Models;
using ShadcnBlazor.ComponentDependencies;

Console.WriteLine("Hello, World!");

var executingAssembly = Assembly.GetExecutingAssembly();
var assemblyDir = Path.GetDirectoryName(executingAssembly.Location) ?? throw new NotImplementedException();
var componentsAssemblyPath = Path.Join(assemblyDir, "ShadcnBlazor.dll");
var assembly = Assembly.LoadFrom(componentsAssemblyPath);

var componentsWithMetadata = ComponentData.GetComponents(assembly);

foreach (var comp in componentsWithMetadata)
{
    Console.WriteLine($"Name: {comp.ComponentMetadata.Name}");
    Console.WriteLine($"Namespace: {comp.Namespace}");
    Console.WriteLine($"FullName: {comp.FullName}");
    Console.WriteLine($"Dependencies: {string.Join(", ", comp.ComponentMetadata.Dependencies)}");

    var componentDir = Path.Join(assemblyDir, "Components", comp.ComponentMetadata.Name);
    Console.WriteLine(componentDir);
    
    Console.WriteLine();
}
