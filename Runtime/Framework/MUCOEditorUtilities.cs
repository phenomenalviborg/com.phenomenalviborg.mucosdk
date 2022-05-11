using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace PhenomenalViborg.MUCOSDK
{
    public class MUCOEditorUtilities : MonoBehaviour
    {
        public static void AddSceneToBuild(SceneAsset sceneAsset)
        {
            if (EditorBuildSettings.scenes.Select(x => x.path).Contains(AssetDatabase.GetAssetPath(sceneAsset))) return;

            EditorBuildSettingsScene[] originalBuildScenes = EditorBuildSettings.scenes;
            EditorBuildSettingsScene[] newBuildScenes = new EditorBuildSettingsScene[originalBuildScenes.Length + 1];
            System.Array.Copy(originalBuildScenes, newBuildScenes, originalBuildScenes.Length);
            newBuildScenes[newBuildScenes.Length - 1] = new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(sceneAsset), true);
            EditorBuildSettings.scenes = newBuildScenes;
        }

    }
}