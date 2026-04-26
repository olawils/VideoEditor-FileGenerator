using VideoEditorBoilerplateGen.Generators;
using VideoEditorBoilerplateGen.Models;

if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
{
    Console.WriteLine("""
        vegen — Video Editor Swift boilerplate generator (Clean Architecture)

        Usage:
          vegen --name <ProjectName> [options]    (Generates full boilerplate)
          vegen <type> <FileName> [options]       (Generates specific file)

        Types:
          view, model, usecase, repository, file

        Options:
          -n, --name    <name>    Project name, e.g. CapFlow    (required for full project)
          -o, --output  <path>    Output directory  (default: current dir)
          -b, --bundle  <id>      Bundle ID prefix  (default: com.dev)
          -t, --team    <name>    Developer/team name

        Examples:
          vegen --name CapFlow --output ~/Projects
          vegen view TimelineView
          vegen model UserProfile
        """);
    return 0;
}

// Single file generator
if (args.Length > 0 && !args[0].StartsWith("-"))
{
    string type = args[0].ToLower();
    if (args.Length < 2)
    {
        Console.Error.WriteLine($"Error: Missing name for {type}. Example: vegen {type} ItemName");
        return 1;
    }
    string resourceName = args[1];
    string outputDir = GetArg(args, "--output", "-o") ?? Directory.GetCurrentDirectory();
    
    try
    {
        ResourceGenerator.Generate(type, resourceName, outputDir);
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        return 1;
    }
}

string? name = GetArg(args, "--name", "-n");
string output = GetArg(args, "--output", "-o") ?? Directory.GetCurrentDirectory();
string bundle = GetArg(args, "--bundle", "-b") ?? "com.dev";
string team   = GetArg(args, "--team",   "-t") ?? "Developer";

if (name is null)
{
    Console.Error.WriteLine("Error: --name is required. Run with --help for usage.");
    return 1;
}

var sanitized = string.Concat(
    name.Split(' ', '-', '_')
        .Where(w => w.Length > 0)
        .Select(w => char.ToUpper(w[0]) + w[1..])
);

var config = new ProjectConfig(
    ProjectName: sanitized,
    OutputPath:  Path.GetFullPath(output),
    BundleId:    $"{bundle}.{sanitized.ToLower()}",
    TeamName:    team
);

try
{
    new ProjectGenerator(config).Generate();
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}

static string? GetArg(string[] args, params string[] flags)
{
    for (int i = 0; i < args.Length - 1; i++)
        if (flags.Contains(args[i]))
            return args[i + 1];
    return null;
}
