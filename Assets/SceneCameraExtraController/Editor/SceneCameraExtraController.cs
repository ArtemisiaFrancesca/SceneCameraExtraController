using UnityEngine;
using UnityEditor;


[InitializeOnLoadAttribute]
public static class SceneCameraExtraController
{
	const string COPY_GAMECAMERA_FROM_SCENECAMERA = "Edit/SceneView Camera/SceneView Camera -> Game MainCamera";
	const string COPY_SCENECAMERA_FROM_GAMECAMERA = "Edit/SceneView Camera/Game MainCamera -> SceneView Camera";
	const string SYNC_GAMECAMERA_WITH_SCENECAMERA = "Edit/SceneView Camera/Sync GameCamera with SceneCamera";

	static GUIStyle LabelStyle;
	static Rect GUILayoutRect;

	static SceneCameraExtraController()
	{
		LabelStyle = new GUIStyle();
		LabelStyle.normal.textColor = Color.white;
		LabelStyle.alignment = TextAnchor.UpperLeft;
		GUILayoutRect = new Rect(0, 0, 200, 200);
		SceneView.duringSceneGui += OnSceneViewGUI;
	}

	static void OnSceneViewGUI(SceneView targetView)
	{
		if (EditorApplication.isPlaying)
		{
			Menu.SetChecked(SYNC_GAMECAMERA_WITH_SCENECAMERA, false);
			return;
		}

		if (Menu.GetChecked(SYNC_GAMECAMERA_WITH_SCENECAMERA))
		{
			CopyGameCameraFromSceneCamera();

			// SceneView GUI
			Handles.BeginGUI();
			GUILayout.BeginArea(GUILayoutRect);
			GUILayout.Label("Sync GameCamera", LabelStyle);
			GUILayout.EndArea();
			Handles.EndGUI();
			return;
		}
	}

	/// <summary>
	/// シーンビューカメラからゲームビューメインカメラへ設定をコピーする
	/// </summary>
	[MenuItem(COPY_GAMECAMERA_FROM_SCENECAMERA)]
	static void CopyGameCameraFromSceneCamera()
	{
		var sceneView = GetLastActiveSceneView();
		var gameMainCamera = GetGameViewMainCamera();

		if (sceneView == null || gameMainCamera == null)
		{
			Menu.SetChecked(SYNC_GAMECAMERA_WITH_SCENECAMERA, false);
			return;
		}

		CopyCameraSettings(sceneView, ref gameMainCamera);
	}

	/// <summary>
	/// シーンビューカメラからゲームビューメインカメラへ設定をコピーする
	/// </summary>
	[MenuItem(COPY_SCENECAMERA_FROM_GAMECAMERA)]
	static void CopySceneCameraFromGameCamera()
	{
		var sceneView = GetLastActiveSceneView();
		var gameMainCamera = GetGameViewMainCamera();


		if (sceneView == null || gameMainCamera == null)
		{
			Menu.SetChecked(SYNC_GAMECAMERA_WITH_SCENECAMERA, false);
			return;
		}

		CopyCameraSettings(gameMainCamera, ref sceneView);
	}

	/// <summary>
	/// シーンビューカメラとゲームビューメインカメラの設定を同期させるトグル
	/// </summary>
	[MenuItem(SYNC_GAMECAMERA_WITH_SCENECAMERA)]
	static void SyncToggle()
	{
		if (EditorApplication.isPlaying)
		{
			Menu.SetChecked(SYNC_GAMECAMERA_WITH_SCENECAMERA, false);
			return;
		}

		var isOn = Menu.GetChecked(SYNC_GAMECAMERA_WITH_SCENECAMERA);
		Menu.SetChecked(SYNC_GAMECAMERA_WITH_SCENECAMERA, !isOn);
	}


	#region Camera 情報取得
	/// <summary>
	/// シーンビューを取得
	/// </summary>
	/// <returns>SceneView.見つからない場合はnull</returns>
	static SceneView GetLastActiveSceneView()
	{
		var sceneView = SceneView.lastActiveSceneView;
		if (sceneView == null)
		{
			Debug.LogWarning($"SceneViewが見つかりません。:{COPY_GAMECAMERA_FROM_SCENECAMERA}");
		}
		return sceneView;
	}

	/// <summary>
	/// ゲーム中のメインカメラを取得
	/// </summary>
	/// <returns>Camera.見つからない場合はnull</returns>
	static Camera GetGameViewMainCamera()
	{
		var gameMainCamera = Camera.main;
		if (gameMainCamera == null)
		{
			Debug.LogWarning($"Scene中にMainCameraが見つかりません:{COPY_GAMECAMERA_FROM_SCENECAMERA}");
		}
		return gameMainCamera;
	}
	#endregion


	#region カメラ情報操作
	/// <summary>
	/// シーンビューからカメラへ設定をコピー
	/// * 位置
	/// * 回転
	/// * focalLength
	/// * 平行投影
	/// * near / far
	/// </summary>
	/// <param name="srcSceneView">コピー元シーンビュー</param>
	/// <param name="dstCamera">コピー先カメラ</param>
	static void CopyCameraSettings(SceneView srcSceneView, ref Camera dstCamera)
	{
		dstCamera.transform.position = srcSceneView.camera.transform.position;
		dstCamera.transform.rotation = srcSceneView.camera.transform.rotation;
		dstCamera.fieldOfView = srcSceneView.camera.fieldOfView;
		dstCamera.orthographic = srcSceneView.camera.orthographic;
		dstCamera.orthographicSize = srcSceneView.camera.orthographicSize;
		dstCamera.nearClipPlane = srcSceneView.camera.nearClipPlane;
		dstCamera.farClipPlane = srcSceneView.camera.farClipPlane;
	}

	/// <summary>
	/// カメラからシーンビューカメラへ設定をコピー
	/// * 位置
	/// * 回転
	/// * focalLength
	/// * 平行投影
	/// * near / far
	/// </summary>
	/// <param name="srcSceneView">コピー元カメラ</param>
	/// <param name="dstCamera">コピー先シーンビュー</param>
	static void CopyCameraSettings(Camera srcCamera, ref SceneView dstSceneView)
	{
		if (srcCamera.orthographic)
		{
			Debug.LogWarning("MainCameraがOrthographic Modeの場合はコピーできません");
		}

		dstSceneView.rotation = srcCamera.transform.rotation;
		dstSceneView.pivot = (srcCamera.transform.forward * dstSceneView.cameraDistance) + srcCamera.transform.position;
		var cameraSettings = dstSceneView.cameraSettings;
		cameraSettings.fieldOfView = srcCamera.fieldOfView;
		dstSceneView.cameraSettings = cameraSettings;

	}
	#endregion
}

