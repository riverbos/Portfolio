using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class UnitySceneDirtyStateWriter
{
    private const double UpdateInterval = 0.25d;
    private static readonly string StateDirectory = Path.Combine(Directory.GetCurrentDirectory(), ".codex", "state");
    private static readonly string StatePath = Path.Combine(StateDirectory, "unity-scene-state.json");

    private static double nextUpdateTime;
    private static string lastDirtySignature;

    static UnitySceneDirtyStateWriter()
    {
        EditorApplication.update += UpdateState;
        EditorApplication.hierarchyChanged += WriteState;
        EditorSceneManager.sceneOpened += OnSceneOpened;
        EditorSceneManager.sceneClosed += OnSceneClosed;
        EditorSceneManager.sceneSaved += OnSceneSaved;
        WriteState();
    }

    private static void UpdateState()
    {
        if (EditorApplication.timeSinceStartup < nextUpdateTime)
            return;

        nextUpdateTime = EditorApplication.timeSinceStartup + UpdateInterval;
        WriteState();
    }

    private static void WriteState()
    {
        List<DirtyScene> dirtyScenes = GetDirtyScenes();
        string dirtySignature = string.Join("|", dirtyScenes.ConvertAll(scene => $"{scene.name}:{scene.path}"));
        if (dirtySignature == lastDirtySignature)
            return;

        SceneState state = new SceneState
        {
            updatedAtUtc = DateTime.UtcNow.ToString("O"),
            dirtyScenes = dirtyScenes
        };

        string json = JsonUtility.ToJson(state, true);
        Directory.CreateDirectory(StateDirectory);
        File.WriteAllText(StatePath, json);
        lastDirtySignature = dirtySignature;
    }

    private static List<DirtyScene> GetDirtyScenes()
    {
        List<DirtyScene> dirtyScenes = new List<DirtyScene>();

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (!scene.isDirty)
                continue;

            dirtyScenes.Add(new DirtyScene
            {
                name = scene.name,
                path = scene.path
            });
        }

        return dirtyScenes;
    }

    private static void OnSceneOpened(Scene scene, OpenSceneMode mode) => WriteState();
    private static void OnSceneClosed(Scene scene) => WriteState();
    private static void OnSceneSaved(Scene scene) => WriteState();

    [Serializable]
    private class SceneState
    {
        public string updatedAtUtc;
        public List<DirtyScene> dirtyScenes;
    }

    [Serializable]
    private class DirtyScene
    {
        public string name;
        public string path;
    }
}
