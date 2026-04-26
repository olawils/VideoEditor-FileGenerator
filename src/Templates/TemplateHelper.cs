namespace VideoEditorBoilerplateGen.Templates;

/// <summary>
/// All Swift source templates use __PROJECT__ as the project name token.
/// Call .Render(projectName) to get the final string.
/// </summary>
public static class T
{
    public static string Render(this string template, string projectName) =>
        template.Replace("__PROJECT__", projectName);
}
