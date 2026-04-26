using System;
using System.IO;
using VideoEditorBoilerplateGen.Templates;

namespace VideoEditorBoilerplateGen.Generators;

public static class ResourceGenerator
{
    public static void Generate(string type, string name, string outputPath)
    {
        string content;
        
        switch (type.ToLower())
        {
            case "view":
                content = SwiftTemplates.GenericView.Replace("__NAME__", name);
                break;
            case "model":
                content = SwiftTemplates.GenericModel.Replace("__NAME__", name);
                break;
            case "theme":
                content = SwiftTemplates.AppTheme.Replace("__NAME__", name);
                break;
            default:
                content = SwiftTemplates.GenericFile.Replace("__NAME__", name);
                break;
        }

        Directory.CreateDirectory(outputPath);
        string fullPath = Path.Combine(outputPath, $"{name}.swift");
        
        if (File.Exists(fullPath))
        {
            Console.WriteLine($"\n  [SKIP] {type.ToUpper()}: {fullPath} (Already exists)\n");
        }
        else
        {
            File.WriteAllText(fullPath, content);
            Console.WriteLine($"\n  [NEW]  {type.ToUpper()}: {fullPath}\n");
        }
    }
}
