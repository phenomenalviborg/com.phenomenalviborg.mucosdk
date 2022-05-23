using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace PhenomenalViborg.MUCOSDK
{
    public class MUCOEditorUtilities : MonoBehaviour
    {
        public static void AddSceneToBuild(SceneReference sceneReference)
        {
            string sceneAssetPath = AssetDatabase.GUIDToAssetPath(sceneReference.guid);
            if (EditorBuildSettings.scenes.Select(x => x.path).Contains(sceneAssetPath)) return;

            EditorBuildSettingsScene[] originalBuildScenes = EditorBuildSettings.scenes;
            EditorBuildSettingsScene[] newBuildScenes = new EditorBuildSettingsScene[originalBuildScenes.Length + 1];
            System.Array.Copy(originalBuildScenes, newBuildScenes, originalBuildScenes.Length);
            newBuildScenes[newBuildScenes.Length - 1] = new EditorBuildSettingsScene(sceneAssetPath, true);
            EditorBuildSettings.scenes = newBuildScenes;
        }

    }
}