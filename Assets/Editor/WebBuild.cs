using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>One-click Web (WebGL) build into Builds/WebGL, with settings that itch.io accepts.</summary>
public static class WebBuild
{
    private const string OutputFolder = "Builds/WebGL";

    [MenuItem("Map Conquest/Build For Web (itch.io)")]
    public static void Build()
    {
        // Uncompressed files load on itch.io without any server setup.
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
        PlayerSettings.WebGL.decompressionFallback = false;

        var options = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/Main.unity" },
            locationPathName = OutputFolder,
            target = BuildTarget.WebGL,
            options = BuildOptions.None,
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Web build done: {OutputFolder} ({report.summary.totalSize / (1024 * 1024)} MB). Zip the folder's contents and upload to itch.io.");
            EditorUtility.RevealInFinder(OutputFolder + "/index.html");
        }
        else
        {
            Debug.LogError($"Web build failed: {report.summary.result}. See the Console for errors.");
        }
    }
}
