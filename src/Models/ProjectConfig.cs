namespace VideoEditorBoilerplateGen.Models;

public record ProjectConfig(
    string ProjectName,
    string OutputPath,
    string BundleId,
    string TeamName
);

public record FeatureDefinition(
    string Name,
    string[] Entities,
    string[] UseCases,
    string[] RepositoryMethods
);
