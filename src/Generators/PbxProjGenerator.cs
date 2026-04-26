using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace VideoEditorBoilerplateGen.Generators;

public class PbxProjGenerator
{
    private readonly string _projectName;
    private readonly string _bundleId;
    private readonly string _teamName;
    private readonly IEnumerable<string> _swiftFiles; 

    public PbxProjGenerator(string projectName, string bundleId, string teamName, IEnumerable<string> swiftFiles)
    {
        _projectName = projectName;
        _bundleId = bundleId;
        _teamName = teamName;
        _swiftFiles = swiftFiles;
    }

    private string Hash(string input)
    {
        using var md5 = MD5.Create();
        var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(bytes).Replace("-", "").Substring(0, 24);
    }

    class FolderNode
    {
        public string Name { get; set; } = string.Empty;
        public string Uuid { get; set; } = string.Empty;
        public Dictionary<string, FolderNode> SubFolders { get; set; } = new();
        public List<string> FileRels { get; set; } = new();
    }

    public void Generate(string outputRoot)
    {
        string pbxRoot = Path.Combine(outputRoot, $"{_projectName}.xcodeproj");
        Directory.CreateDirectory(pbxRoot);

        string projectUuid = Hash("Project");
        string targetUuid = Hash("Target");
        string mainGroupUuid = Hash("MainGroup");
        string appGroupUuid = Hash("AppGroup");
        string productsGroupUuid = Hash("ProductsGroup");
        string sourcesPhaseUuid = Hash("SourcesPhase");
        string resourcesPhaseUuid = Hash("ResourcesPhase");
        string frameworksPhaseUuid = Hash("FrameworksPhase");
        string configListProjectUuid = Hash("ConfigListProject");
        string configListTargetUuid = Hash("ConfigListTarget");
        string debugConfigProjectUuid = Hash("DebugConfigProject");
        string releaseConfigProjectUuid = Hash("ReleaseConfigProject");
        string debugConfigTargetUuid = Hash("DebugConfigTarget");
        string releaseConfigTargetUuid = Hash("ReleaseConfigTarget");
        string appProductRef = Hash("AppProduct");

        var fileRefs = new Dictionary<string, string>();
        var buildFiles = new Dictionary<string, string>();

        var rootFolder = new FolderNode { Name = _projectName, Uuid = appGroupUuid };

        foreach (var file in _swiftFiles.OrderBy(f => f))
        {
            fileRefs[file] = Hash("FileRef_" + file);
            buildFiles[file] = Hash("BuildFile_" + file);

            var parts = file.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var current = rootFolder;

            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (!current.SubFolders.TryGetValue(parts[i], out var subFolder))
                {
                    subFolder = new FolderNode { Name = parts[i], Uuid = Hash("Folder_" + string.Join("/", parts.Take(i + 1))) };
                    current.SubFolders[parts[i]] = subFolder;
                }
                current = subFolder;
            }
            current.FileRels.Add(file);
        }

        string assetsRef = Hash("AssetsRef");
        string assetsBuild = Hash("AssetsBuild");

        var sb = new StringBuilder();
        sb.AppendLine("// !$*UTF8*$!");
        sb.AppendLine("{");
        sb.AppendLine("\tarchiveVersion = 1;");
        sb.AppendLine("\tclasses = {");
        sb.AppendLine("\t};");
        sb.AppendLine("\tobjectVersion = 56;");
        sb.AppendLine("\tobjects = {");
        sb.AppendLine();

        // PBXBuildFile
        sb.AppendLine("/* Begin PBXBuildFile section */");
        foreach (var rel in buildFiles.Keys)
        {
            sb.AppendLine($"\t\t{buildFiles[rel]} /* {Path.GetFileName(rel)} in Sources */ = {{isa = PBXBuildFile; fileRef = {fileRefs[rel]} /* {Path.GetFileName(rel)} */; }};");
        }
        sb.AppendLine($"\t\t{assetsBuild} /* Assets.xcassets in Resources */ = {{isa = PBXBuildFile; fileRef = {assetsRef} /* Assets.xcassets */; }};");
        sb.AppendLine("/* End PBXBuildFile section */");
        sb.AppendLine();

        // PBXFileReference
        sb.AppendLine("/* Begin PBXFileReference section */");
        foreach (var rel in fileRefs.Keys)
        {
            sb.AppendLine($"\t\t{fileRefs[rel]} /* {Path.GetFileName(rel)} */ = {{isa = PBXFileReference; lastKnownFileType = sourcecode.swift; path = \"{Path.GetFileName(rel)}\"; sourceTree = \"<group>\"; }};");
        }
        sb.AppendLine($"\t\t{assetsRef} /* Assets.xcassets */ = {{isa = PBXFileReference; lastKnownFileType = folder.assetcatalog; path = Assets.xcassets; sourceTree = \"<group>\"; }};");
        sb.AppendLine($"\t\t{appProductRef} /* {_projectName}.app */ = {{isa = PBXFileReference; explicitFileType = wrapper.application; includeInIndex = 0; path = \"{_projectName}.app\"; sourceTree = BUILT_PRODUCTS_DIR; }};");
        sb.AppendLine("/* End PBXFileReference section */");
        sb.AppendLine();

        // PBXFrameworksBuildPhase
        sb.AppendLine("/* Begin PBXFrameworksBuildPhase section */");
        sb.AppendLine($"\t\t{frameworksPhaseUuid} /* Frameworks */ = {{");
        sb.AppendLine("\t\t\tisa = PBXFrameworksBuildPhase;");
        sb.AppendLine("\t\t\tbuildActionMask = 2147483647;");
        sb.AppendLine("\t\t\tfiles = (");
        sb.AppendLine("\t\t\t);");
        sb.AppendLine("\t\t\trunOnlyForDeploymentPostprocessing = 0;");
        sb.AppendLine("\t\t};");
        sb.AppendLine("/* End PBXFrameworksBuildPhase section */");
        sb.AppendLine();

        // PBXGroup
        sb.AppendLine("/* Begin PBXGroup section */");
        
        void WriteGroup(FolderNode node, string pathStr = "")
        {
            sb.AppendLine($"\t\t{node.Uuid} /* {node.Name} */ = {{");
            sb.AppendLine("\t\t\tisa = PBXGroup;");
            sb.AppendLine("\t\t\tchildren = (");
            
            if (node == rootFolder)
            {
                sb.AppendLine($"\t\t\t\t{assetsRef} /* Assets.xcassets */,");
            }
            
            foreach (var sf in node.SubFolders.Values.OrderBy(s => s.Name))
            {
                sb.AppendLine($"\t\t\t\t{sf.Uuid} /* {sf.Name} */,");
            }
            foreach (var f in node.FileRels.OrderBy(x => x))
            {
                sb.AppendLine($"\t\t\t\t{fileRefs[f]} /* {Path.GetFileName(f)} */,");
            }
            sb.AppendLine("\t\t\t);");
            if (pathStr != "")
            {
                sb.AppendLine($"\t\t\tpath = \"{node.Name}\";");
            }
            sb.AppendLine("\t\t\tsourceTree = \"<group>\";");
            sb.AppendLine("\t\t};");

            foreach (var sf in node.SubFolders.Values)
            {
                WriteGroup(sf, node.Name);
            }
        }
        
        WriteGroup(rootFolder);

        sb.AppendLine($"\t\t{mainGroupUuid} = {{");
        sb.AppendLine("\t\t\tisa = PBXGroup;");
        sb.AppendLine("\t\t\tchildren = (");
        sb.AppendLine($"\t\t\t\t{appGroupUuid} /* {_projectName} */,");
        sb.AppendLine($"\t\t\t\t{productsGroupUuid} /* Products */,");
        sb.AppendLine("\t\t\t);");
        sb.AppendLine("\t\t\tsourceTree = \"<group>\";");
        sb.AppendLine("\t\t};");
        
        sb.AppendLine($"\t\t{productsGroupUuid} /* Products */ = {{");
        sb.AppendLine("\t\t\tisa = PBXGroup;");
        sb.AppendLine("\t\t\tchildren = (");
        sb.AppendLine($"\t\t\t\t{appProductRef} /* {_projectName}.app */,");
        sb.AppendLine("\t\t\t);");
        sb.AppendLine("\t\t\tname = Products;");
        sb.AppendLine("\t\t\tsourceTree = \"<group>\";");
        sb.AppendLine("\t\t};");

        sb.AppendLine("/* End PBXGroup section */");
        sb.AppendLine();

        // PBXNativeTarget
        sb.AppendLine("/* Begin PBXNativeTarget section */");
        sb.AppendLine($"\t\t{targetUuid} /* {_projectName} */ = {{");
        sb.AppendLine("\t\t\tisa = PBXNativeTarget;");
        sb.AppendLine($"\t\t\tbuildConfigurationList = {configListTargetUuid} /* Build configuration list for PBXNativeTarget \"{_projectName}\" */;");
        sb.AppendLine("\t\t\tbuildPhases = (");
        sb.AppendLine($"\t\t\t\t{sourcesPhaseUuid} /* Sources */,");
        sb.AppendLine($"\t\t\t\t{frameworksPhaseUuid} /* Frameworks */,");
        sb.AppendLine($"\t\t\t\t{resourcesPhaseUuid} /* Resources */,");
        sb.AppendLine("\t\t\t);");
        sb.AppendLine("\t\t\tbuildRules = (");
        sb.AppendLine("\t\t\t);");
        sb.AppendLine("\t\t\tdependencies = (");
        sb.AppendLine("\t\t\t);");
        sb.AppendLine($"\t\t\tname = \"{_projectName}\";");
        sb.AppendLine($"\t\t\tproductName = \"{_projectName}\";");
        sb.AppendLine($"\t\t\tproductReference = {appProductRef} /* {_projectName}.app */;");
        sb.AppendLine("\t\t\tproductType = \"com.apple.product-type.application\";");
        sb.AppendLine("\t\t};");
        sb.AppendLine("/* End PBXNativeTarget section */");
        sb.AppendLine();

        // PBXProject
        sb.AppendLine("/* Begin PBXProject section */");
        sb.AppendLine($"\t\t{projectUuid} /* Project object */ = {{");
        sb.AppendLine("\t\t\tisa = PBXProject;");
        sb.AppendLine("\t\t\tattributes = {");
        sb.AppendLine("\t\t\t\tLastUpgradeCheck = 1500;");
        sb.AppendLine("\t\t\t\tTargetAttributes = {");
        sb.AppendLine($"\t\t\t\t\t{targetUuid} = {{");
        sb.AppendLine("\t\t\t\t\t\tCreatedOnToolsVersion = 15.0;");
        sb.AppendLine("\t\t\t\t\t};");
        sb.AppendLine("\t\t\t\t};");
        sb.AppendLine("\t\t\t};");
        sb.AppendLine($"\t\t\tbuildConfigurationList = {configListProjectUuid} /* Build configuration list for PBXProject \"{_projectName}\" */;");
        sb.AppendLine("\t\t\tcompatibilityVersion = \"Xcode 14.0\";");
        sb.AppendLine($"\t\t\tdevelopmentRegion = en;");
        sb.AppendLine("\t\t\thasScannedForEncodings = 0;");
        sb.AppendLine("\t\t\tknownRegions = (");
        sb.AppendLine("\t\t\t\ten,");
        sb.AppendLine("\t\t\t\tBase,");
        sb.AppendLine("\t\t\t);");
        sb.AppendLine($"\t\t\tmainGroup = {mainGroupUuid};");
        sb.AppendLine($"\t\t\tproductRefGroup = {productsGroupUuid} /* Products */;");
        sb.AppendLine($"\t\t\tprojectDirPath = \"\";");
        sb.AppendLine($"\t\t\tprojectRoot = \"\";");
        sb.AppendLine("\t\t\ttargets = (");
        sb.AppendLine($"\t\t\t\t{targetUuid} /* {_projectName} */,");
        sb.AppendLine("\t\t\t);");
        sb.AppendLine("\t\t};");
        sb.AppendLine("/* End PBXProject section */");
        sb.AppendLine();

        // PBXResourcesBuildPhase
        sb.AppendLine("/* Begin PBXResourcesBuildPhase section */");
        sb.AppendLine($"\t\t{resourcesPhaseUuid} /* Resources */ = {{");
        sb.AppendLine("\t\t\tisa = PBXResourcesBuildPhase;");
        sb.AppendLine("\t\t\tbuildActionMask = 2147483647;");
        sb.AppendLine("\t\t\tfiles = (");
        sb.AppendLine($"\t\t\t\t{assetsBuild} /* Assets.xcassets in Resources */,");
        sb.AppendLine("\t\t\t);");
        sb.AppendLine("\t\t\trunOnlyForDeploymentPostprocessing = 0;");
        sb.AppendLine("\t\t};");
        sb.AppendLine("/* End PBXResourcesBuildPhase section */");
        sb.AppendLine();

        // PBXSourcesBuildPhase
        sb.AppendLine("/* Begin PBXSourcesBuildPhase section */");
        sb.AppendLine($"\t\t{sourcesPhaseUuid} /* Sources */ = {{");
        sb.AppendLine("\t\t\tisa = PBXSourcesBuildPhase;");
        sb.AppendLine("\t\t\tbuildActionMask = 2147483647;");
        sb.AppendLine("\t\t\tfiles = (");
        foreach (var buildId in buildFiles.Values)
        {
            sb.AppendLine($"\t\t\t\t{buildId} /* in Sources */,");
        }
        sb.AppendLine("\t\t\t);");
        sb.AppendLine("\t\t\trunOnlyForDeploymentPostprocessing = 0;");
        sb.AppendLine("\t\t};");
        sb.AppendLine("/* End PBXSourcesBuildPhase section */");
        sb.AppendLine();

        // XCBuildConfiguration
        sb.AppendLine("/* Begin XCBuildConfiguration section */");
        sb.AppendLine($"\t\t{debugConfigProjectUuid} /* Debug */ = {{");
        sb.AppendLine("\t\t\tisa = XCBuildConfiguration;");
        sb.AppendLine("\t\t\tbuildSettings = {");
        sb.AppendLine("\t\t\t\tALWAYS_SEARCH_USER_PATHS = NO;");
        sb.AppendLine("\t\t\t\tCLANG_ENABLE_OBJC_ARC = YES;");
        sb.AppendLine("\t\t\t\tCLANG_ENABLE_MODULES = YES;");
        sb.AppendLine("\t\t\t\tENABLE_STRICT_OBJC_MSGSEND = YES;");
        sb.AppendLine("\t\t\t\tSWIFT_OPTIMIZATION_LEVEL = \"-Onone\";");
        sb.AppendLine("\t\t\t\tSWIFT_ACTIVE_COMPILATION_CONDITIONS = DEBUG;");
        sb.AppendLine("\t\t\t\tGCC_OPTIMIZATION_LEVEL = 0;");
        sb.AppendLine("\t\t\t\tIPHONEOS_DEPLOYMENT_TARGET = 16.0;");
        sb.AppendLine("\t\t\t\tSDKROOT = iphoneos;");
        sb.AppendLine("\t\t\t};");
        sb.AppendLine("\t\t\tname = Debug;");
        sb.AppendLine("\t\t};");
        sb.AppendLine($"\t\t{releaseConfigProjectUuid} /* Release */ = {{");
        sb.AppendLine("\t\t\tisa = XCBuildConfiguration;");
        sb.AppendLine("\t\t\tbuildSettings = {");
        sb.AppendLine("\t\t\t\tALWAYS_SEARCH_USER_PATHS = NO;");
        sb.AppendLine("\t\t\t\tCLANG_ENABLE_OBJC_ARC = YES;");
        sb.AppendLine("\t\t\t\tCLANG_ENABLE_MODULES = YES;");
        sb.AppendLine("\t\t\t\tSWIFT_OPTIMIZATION_LEVEL = \"-O\";");
        sb.AppendLine("\t\t\t\tIPHONEOS_DEPLOYMENT_TARGET = 16.0;");
        sb.AppendLine("\t\t\t\tSDKROOT = iphoneos;");
        sb.AppendLine("\t\t\t};");
        sb.AppendLine("\t\t\tname = Release;");
        sb.AppendLine("\t\t};");
        sb.AppendLine($"\t\t{debugConfigTargetUuid} /* Debug */ = {{");
        sb.AppendLine("\t\t\tisa = XCBuildConfiguration;");
        sb.AppendLine("\t\t\tbuildSettings = {");
        sb.AppendLine("\t\t\t\tASSETCATALOG_COMPILER_APPICON_NAME = AppIcon;");
        sb.AppendLine("\t\t\t\tASSETCATALOG_COMPILER_GLOBAL_ACCENT_COLOR_NAME = AccentColor;");
        sb.AppendLine($"\t\t\t\tPRODUCT_BUNDLE_IDENTIFIER = {_bundleId};");
        sb.AppendLine($"\t\t\t\tPRODUCT_NAME = \"$(TARGET_NAME)\";");
        sb.AppendLine("\t\t\t\tSWIFT_VERSION = 5.0;");
        sb.AppendLine("\t\t\t\tTARGETED_DEVICE_FAMILY = \"1,2\";");
        sb.AppendLine($"\t\t\t\tINFOPLIST_KEY_UIApplicationSceneManifest_Generation = YES;");
        sb.AppendLine($"\t\t\t\tINFOPLIST_KEY_UILaunchScreen_Generation = YES;");
        sb.AppendLine($"\t\t\t\tDEVELOPMENT_TEAM = \"{_teamName}\";");
        sb.AppendLine("\t\t\t};");
        sb.AppendLine("\t\t\tname = Debug;");
        sb.AppendLine("\t\t};");
        sb.AppendLine($"\t\t{releaseConfigTargetUuid} /* Release */ = {{");
        sb.AppendLine("\t\t\tisa = XCBuildConfiguration;");
        sb.AppendLine("\t\t\tbuildSettings = {");
        sb.AppendLine("\t\t\t\tASSETCATALOG_COMPILER_APPICON_NAME = AppIcon;");
        sb.AppendLine("\t\t\t\tASSETCATALOG_COMPILER_GLOBAL_ACCENT_COLOR_NAME = AccentColor;");
        sb.AppendLine($"\t\t\t\tPRODUCT_BUNDLE_IDENTIFIER = {_bundleId};");
        sb.AppendLine($"\t\t\t\tPRODUCT_NAME = \"$(TARGET_NAME)\";");
        sb.AppendLine("\t\t\t\tSWIFT_VERSION = 5.0;");
        sb.AppendLine("\t\t\t\tTARGETED_DEVICE_FAMILY = \"1,2\";");
        sb.AppendLine($"\t\t\t\tINFOPLIST_KEY_UIApplicationSceneManifest_Generation = YES;");
        sb.AppendLine($"\t\t\t\tINFOPLIST_KEY_UILaunchScreen_Generation = YES;");
        sb.AppendLine($"\t\t\t\tDEVELOPMENT_TEAM = \"{_teamName}\";");
        sb.AppendLine("\t\t\t};");
        sb.AppendLine("\t\t\tname = Release;");
        sb.AppendLine("\t\t};");
        sb.AppendLine("/* End XCBuildConfiguration section */");
        sb.AppendLine();

        // XCConfigurationList
        sb.AppendLine("/* Begin XCConfigurationList section */");
        sb.AppendLine($"\t\t{configListProjectUuid} /* Build configuration list for PBXProject \"{_projectName}\" */ = {{");
        sb.AppendLine("\t\t\tisa = XCConfigurationList;");
        sb.AppendLine("\t\t\tbuildConfigurations = (");
        sb.AppendLine($"\t\t\t\t{debugConfigProjectUuid} /* Debug */,");
        sb.AppendLine($"\t\t\t\t{releaseConfigProjectUuid} /* Release */,");
        sb.AppendLine("\t\t\t);");
        sb.AppendLine("\t\t\tdefaultConfigurationIsVisible = 0;");
        sb.AppendLine("\t\t\tdefaultConfigurationName = Release;");
        sb.AppendLine("\t\t};");
        sb.AppendLine($"\t\t{configListTargetUuid} /* Build configuration list for PBXNativeTarget \"{_projectName}\" */ = {{");
        sb.AppendLine("\t\t\tisa = XCConfigurationList;");
        sb.AppendLine("\t\t\tbuildConfigurations = (");
        sb.AppendLine($"\t\t\t\t{debugConfigTargetUuid} /* Debug */,");
        sb.AppendLine($"\t\t\t\t{releaseConfigTargetUuid} /* Release */,");
        sb.AppendLine("\t\t\t);");
        sb.AppendLine("\t\t\tdefaultConfigurationIsVisible = 0;");
        sb.AppendLine("\t\t\tdefaultConfigurationName = Release;");
        sb.AppendLine("\t\t};");
        sb.AppendLine("/* End XCConfigurationList section */");
        
        sb.AppendLine("\t};");
        sb.AppendLine("\trootObject = " + projectUuid + " /* Project object */;");
        sb.AppendLine("}");

        File.WriteAllText(Path.Combine(pbxRoot, "project.pbxproj"), sb.ToString());
    }
}
