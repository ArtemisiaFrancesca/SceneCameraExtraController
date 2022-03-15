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
	/// �V�[���r���[�J��������Q�[���r���[���C���J�����֐ݒ���R�s�[����
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
	/// �V�[���r���[�J��������Q�[���r���[���C���J�����֐ݒ���R�s�[����
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
	/// �V�[���r���[�J�����ƃQ�[���r���[���C���J�����̐ݒ�𓯊�������g�O��
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


	#region Camera ���擾
	/// <summary>
	/// �V�[���r���[���擾
	/// </summary>
	/// <returns>SceneView.������Ȃ��ꍇ��null</returns>
	static SceneView GetLastActiveSceneView()
	{
		var sceneView = SceneView.lastActiveSceneView;
		if (sceneView == null)
		{
			Debug.LogWarning($"SceneView��������܂���B:{COPY_GAMECAMERA_FROM_SCENECAMERA}");
		}
		return sceneView;
	}

	/// <summary>
	/// �Q�[�����̃��C���J�������擾
	/// </summary>
	/// <returns>Camera.������Ȃ��ꍇ��null</returns>
	static Camera GetGameViewMainCamera()
	{
		var gameMainCamera = Camera.main;
		if (gameMainCamera == null)
		{
			Debug.LogWarning($"Scene����MainCamera��������܂���:{COPY_GAMECAMERA_FROM_SCENECAMERA}");
		}
		return gameMainCamera;
	}
	#endregion


	#region �J������񑀍�
	/// <summary>
	/// �V�[���r���[����J�����֐ݒ���R�s�[
	/// * �ʒu
	/// * ��]
	/// * focalLength
	/// * ���s���e
	/// * near / far
	/// </summary>
	/// <param name="srcSceneView">�R�s�[���V�[���r���[</param>
	/// <param name="dstCamera">�R�s�[��J����</param>
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
	/// �J��������V�[���r���[�J�����֐ݒ���R�s�[
	/// * �ʒu
	/// * ��]
	/// * focalLength
	/// * ���s���e
	/// * near / far
	/// </summary>
	/// <param name="srcSceneView">�R�s�[���J����</param>
	/// <param name="dstCamera">�R�s�[��V�[���r���[</param>
	static void CopyCameraSettings(Camera srcCamera, ref SceneView dstSceneView)
	{
		if (srcCamera.orthographic)
		{
			Debug.LogWarning("MainCamera��Orthographic Mode�̏ꍇ�̓R�s�[�ł��܂���");
		}

		dstSceneView.rotation = srcCamera.transform.rotation;
		dstSceneView.pivot = (srcCamera.transform.forward * dstSceneView.cameraDistance) + srcCamera.transform.position;
		var cameraSettings = dstSceneView.cameraSettings;
		cameraSettings.fieldOfView = srcCamera.fieldOfView;
		dstSceneView.cameraSettings = cameraSettings;

	}
	#endregion
}

