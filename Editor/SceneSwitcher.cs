using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PhenomenalViborg.MUCOSDK
{
	static class ToolbarStyles
	{
		public static readonly GUIStyle commandButtonStyle;

		static ToolbarStyles()
		{
			commandButtonStyle = new GUIStyle("Command")
			{
				fontSize = 16,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageAbove,
				fontStyle = FontStyle.Bold
			};
		}
	}

	[InitializeOnLoad]
	public class SceneSwitchLeftButton
	{
		static SceneSwitchLeftButton()
		{
			UnityToolbarExtender.ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
		}

		static void OnToolbarGUI()
		{
			GUILayout.FlexibleSpace();
			
			
			GUIStyle style = ToolbarStyles.commandButtonStyle;
			style.fixedWidth = 0;
			style.padding.left = 16;
			style.padding.right = 16;

			GUI.enabled = !EditorApplication.isPlaying;
			if (GUILayout.Button(new GUIContent("Start from entry", "Start S_Entry"), style))
			{
				SceneHelper.StartScene("Packages/com.phenomenalviborg.mucosdk/Runtime/Framework/Shared/S_Entry.unity");
			}
		}
	}

	static class SceneHelper
	{
		static string sceneToOpen;

		public static void StartScene(string sceneName)
		{
			if(EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
			}

			sceneToOpen = sceneName;
			EditorApplication.update += OnUpdate;
		}

		static void OnUpdate()
		{
			if (sceneToOpen == null ||
			    EditorApplication.isPlaying || EditorApplication.isPaused ||
			    EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
			{
				return;
			}

			EditorApplication.update -= OnUpdate;

			if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				EditorSceneManager.OpenScene(sceneToOpen);
				EditorApplication.isPlaying = true;
			}
			sceneToOpen = null;
		}
	}
}