using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildAutomationTool
{
    [MenuItem("Tools/Build StandaloneWindows/2 Players")]
    private static void Build_StandaloneWindows_2Players()
    {
        Build_StandaloneWindows(2);
    }

    [MenuItem("Tools/Build StandaloneWindows/3 Players")]
    private static void Build_StandaloneWindows_3Players()
    {
        Build_StandaloneWindows(3);
    }

    [MenuItem("Tools/Build StandaloneWindows/4 Players")]
    private static void Build_StandaloneWindows_4Players()
    {
        Build_StandaloneWindows(4);
    }

    private static void Build_StandaloneWindows(int numPlayer = 1)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);

        for (int i = 1; i <= numPlayer; i++)
        {
            string locationPathName = "Builds/Windows64/" + ProjectName + i + "/" + ProjectName + i + ".exe";
            BuildPipeline.BuildPlayer(Levels(), locationPathName, BuildTarget.StandaloneWindows, BuildOptions.AutoRunPlayer);
        }
    }

    private static readonly string ProjectName = Application.dataPath.Split('/')[^2];

    private static string[] Levels()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;  // Build Settings의 Scenes in Build에 있는 씬들의 이름
        }

        return scenes;
    }
}
