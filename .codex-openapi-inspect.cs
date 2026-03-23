using System;
using System.Linq;
using Microsoft.OpenApi;

Console.WriteLine(typeof(OpenApiPathItem).GetProperty("Operations")!.PropertyType.FullName);
Console.WriteLine(string.Join(Environment.NewLine, typeof(OpenApiPathItem).GetMethods().Where(m => m.Name.Contains("Operation") || m.Name.Contains("Add")).Select(m => m.ToString())));
