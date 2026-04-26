using VideoEditorBoilerplateGen.Models;
using VideoEditorBoilerplateGen.Templates;

namespace VideoEditorBoilerplateGen.Generators;

public class ProjectGenerator
{
    private readonly ProjectConfig _config;

    public ProjectGenerator(ProjectConfig config) => _config = config;

    public void Generate()
    {
        var p = _config.ProjectName;
        Console.WriteLine($"\n🎬  Generating {p} — Clean Architecture\n");

        var root = Path.Combine(_config.OutputPath, p);
        CreateDirectories(root);

        var files = BuildManifest(root, p);
        WriteFiles(files, root);
        
        var swiftFiles = files.Keys.Where(k => k.EndsWith(".swift"));
        new PbxProjGenerator(p, _config.BundleId, _config.TeamName, swiftFiles).Generate(root);

        PrintSummary(root, files.Count);
    }

    private void CreateDirectories(string root)
    {
        var dirs = new[]
        {
            "Domain/Entities", "Domain/UseCases", "Domain/Repositories",
            "Data/Repositories", "Data/DataSources", "Data/DTOs",
            "Presentation/Features/ProjectList",
            "Presentation/Features/Editor",
            "Presentation/Features/Editor/Components",
            "Presentation/Components",
            "Core/DI", "Core/Extensions", "Core/Utils",
            "App",
            "Assets.xcassets",
            "Assets.xcassets/AppIcon.appiconset",
            "Assets.xcassets/AccentColor.colorset"
        };

        foreach (var d in dirs)
        {
            var full = Path.Combine(root, d);
            Directory.CreateDirectory(full);
            Console.WriteLine($"  📁  {d}");
        }
        Console.WriteLine();
    }

    private Dictionary<string, string> BuildManifest(string root, string p)
    {
        string R(string t) => t.Render(p); // token replacement

        return new Dictionary<string, string>
        {
            // App
            [Path.Combine(root, "App", $"{p}App.swift")] = R(SwiftTemplates.AppEntry),
            
            // Assets
            [Path.Combine(root, "Assets.xcassets", "Contents.json")] = SwiftTemplates.AssetsContents,
            [Path.Combine(root, "Assets.xcassets/AppIcon.appiconset", "Contents.json")] = SwiftTemplates.AppIconContents,
            [Path.Combine(root, "Assets.xcassets/AccentColor.colorset", "Contents.json")] = SwiftTemplates.AccentColorContents,

            // Domain — Entities
            [Path.Combine(root, "Domain/Entities", "VideoProject.swift")]    = R(SwiftTemplates.VideoProject),
            [Path.Combine(root, "Domain/Entities", "Timeline.swift")]        = R(SwiftTemplates.Timeline),
            [Path.Combine(root, "Domain/Entities", "VideoTrack.swift")]      = R(SwiftTemplates.VideoTrack),
            [Path.Combine(root, "Domain/Entities", "VideoClip.swift")]       = R(SwiftTemplates.VideoClip),
            [Path.Combine(root, "Domain/Entities", "AudioTrack.swift")]      = R(SwiftTemplates.AudioTrack),
            [Path.Combine(root, "Domain/Entities", "Effect.swift")]          = R(SwiftTemplates.Effect),
            [Path.Combine(root, "Domain/Entities", "ExportSettings.swift")]  = R(SwiftTemplates.ExportSettings),

            // Domain — Protocols & Use Cases
            [Path.Combine(root, "Domain/Repositories", "VideoProjectRepositoryProtocol.swift")] =
                R(SwiftTemplates.VideoProjectRepositoryProtocol),
            [Path.Combine(root, "Domain/UseCases", "UseCaseProtocols.swift")] =
                R(SwiftTemplates.UseCaseProtocols),

            // Data
            [Path.Combine(root, "Data/Repositories",  "VideoProjectRepository.swift")]    = R(SwiftTemplates.VideoProjectRepository),
            [Path.Combine(root, "Data/DataSources",   "VideoProjectLocalDataSource.swift")] = R(SwiftTemplates.VideoProjectLocalDataSource),
            [Path.Combine(root, "Data/DTOs",          "VideoProjectDTO.swift")]            = R(SwiftTemplates.VideoProjectDTO),

            // Core
            [Path.Combine(root, "Core/DI", "DIContainer.swift")] = R(SwiftTemplates.DIContainer),

            // Presentation
            [Path.Combine(root, "Presentation/Features/ProjectList", "ProjectListViewModel.swift")] = R(SwiftTemplates.ProjectListViewModel),
            [Path.Combine(root, "Presentation/Features/ProjectList", "ProjectListView.swift")]      = R(SwiftTemplates.ProjectListView),
            [Path.Combine(root, "Presentation/Features/Editor",      "EditorViewModel.swift")]      = R(SwiftTemplates.EditorViewModel),
            [Path.Combine(root, "Presentation/Features/Editor",      "EditorView.swift")]           = R(SwiftTemplates.EditorView),
            [Path.Combine(root, "Presentation/Features/Editor/Components", "TimelineView.swift")]   = R(SwiftTemplates.TimelineView),
            [Path.Combine(root, "Presentation/Components", "StubViews.swift")]                      = R(SwiftTemplates.StubViews),
            [Path.Combine(root, "Presentation/Components", "AppTheme.swift")]                       = R(SwiftTemplates.AppTheme),
        };
    }

    private void WriteFiles(Dictionary<string, string> files, string root)
    {
        foreach (var (path, content) in files)
        {
            if (File.Exists(path))
            {
                Console.WriteLine($"   [SKIP] {Path.GetRelativePath(root, path)} (Already exists)");
            }
            else
            {
                File.WriteAllText(path, content);
                Console.WriteLine($"   [NEW]  {Path.GetRelativePath(root, path)}");
            }
        }
    }

    private void PrintSummary(string root, int count)
    {
        Console.WriteLine($"""

        ────────────────────────────────────────────────────
        ✅  Done — {count} files generated
        📂  {root}
        ────────────────────────────────────────────────────

        Next steps:
          1. Open {root}/{_config.ProjectName}.xcodeproj in Xcode
          2. Link AVFoundation framework to your target
          3. Build and Run! 🚀

        """);
    }
}
