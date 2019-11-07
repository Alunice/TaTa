using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class shadowCameraCtrl : MonoBehaviour{
	RenderTexture _Tex;

	public Material _ProjectorMaterialShadow;

	Matrix4x4 _ProjectorMatrix;
	Matrix4x4 _BiasMatrix;
	Matrix4x4 _ViewMatrix;
	Matrix4x4 _BPV;
	Matrix4x4 _ModelMatrix;
	Matrix4x4 _FinalMatrix;


	Camera _ProjectorCamera;

	List<MeshFilter> _ShadowProjectors;
	List<MeshFilter> _ShadowReceivers;


	void Awake(){
		Initialize();
	}

	void Initialize(){
		_ProjectorMaterialShadow = new Material(Shader.Find("Ouke/ShadowProjectorMultiply"));

		_ProjectorCamera = GetComponent<Camera>();
		_ProjectorCamera.clearFlags = CameraClearFlags.SolidColor;
		_ProjectorCamera.backgroundColor = new Color32(255,255,255,0);
		_ProjectorCamera.orthographic = true;
		_ProjectorCamera.aspect = 1.0f;
		_ProjectorCamera.depth = float.MinValue;
		_ProjectorCamera.allowMSAA = false;
		_ProjectorCamera.allowHDR = false;
		_ProjectorCamera.useOcclusionCulling = false;
		_ProjectorCamera.SetReplacementShader(Shader.Find("Ouke/Shadow"),"RenderType");

		_BiasMatrix = new Matrix4x4();
		_BiasMatrix.SetRow(0, new Vector4(0.5f,0.0f,0.0f,0.5f));
		_BiasMatrix.SetRow(1, new Vector4(0.0f,0.5f,0.0f,0.5f));
		_BiasMatrix.SetRow(2, new Vector4(0.0f,0.0f,0.5f,0.5f));
		_BiasMatrix.SetRow(3, new Vector4(0.0f,0.0f,0.0f,1.0f));

		_ProjectorMatrix = new Matrix4x4();

		_ShadowProjectors = new List<MeshFilter>();
		_ShadowReceivers = new List<MeshFilter>();

		_ProjectorCamera.enabled = false;

		CreateProjectorEyeTexture();
		RefreshMeshRender();

	}

	void CreateProjectorEyeTexture(){
		_Tex = new RenderTexture(1024,1024,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default);
		_Tex.Create();

		_Tex.anisoLevel = 0;
		_Tex.filterMode = FilterMode.Bilinear;

		_ProjectorCamera.targetTexture = _Tex;
		_ProjectorMaterialShadow.SetTexture("_ShadowTex", _Tex);
	}

	public void RefreshMeshRender(){
		GameObject[] projectors = GameObject.FindGameObjectsWithTag("shadowProjector");
		GameObject[] receivers = GameObject.FindGameObjectsWithTag("shadowReceiver");
		for(int i = 0; i< projectors.Length; i++){
			MeshFilter t_msf = projectors[i].GetComponent<MeshFilter>();
			AddProjector(t_msf);
		}

		for(int i = 0; i< receivers.Length; i++){
			MeshFilter t_msf = receivers[i].GetComponent<MeshFilter>();
			AddReceiver(t_msf);
		}
	}

	public void AddProjector(MeshFilter projector){
		if(_Tex == null){
			CreateProjectorEyeTexture();
		}
		if(!_ShadowProjectors.Contains(projector)){
			_ShadowProjectors.Add(projector);
			if(_ProjectorCamera.enabled == false){
				_ProjectorCamera.enabled = true;
			}
		}
	}

	public void RemoveProjector(MeshFilter projector){
		if(_ShadowProjectors.Contains(projector)){
			_ShadowProjectors.Remove(projector);
			if(_ShadowProjectors.Count == 0){
				_ProjectorCamera.enabled = false;
			}
		}
	}

	public void AddReceiver(MeshFilter receiver){
		if(!_ShadowReceivers.Contains(receiver)){
			_ShadowReceivers.Add(receiver);
		}
	}

	public void RemoveReceiver(MeshFilter receiver){
		if(_ShadowReceivers.Contains(receiver)){
			_ShadowReceivers.Remove(receiver);
		}
	}


	void LateUpdate(){
		if(!_Tex){
			return;
		}
		RefreshMeshRender();
		RenderProjectors();
	}

	void RenderProjectors(){
		if(!_Tex){
			return;
		}

		if(_ShadowProjectors.Count > 0 && _ShadowReceivers.Count > 0){
			float n = _ProjectorCamera.nearClipPlane;
			float f = _ProjectorCamera.farClipPlane;
			float r = _ProjectorCamera.orthographicSize;
			float t = _ProjectorCamera.orthographicSize;

			//the projector plane is a square has r = -l,t = -b,so the mclipmatrix is the value below
			_ProjectorMatrix.SetRow(0,new Vector4(1/r, 0.0f,0.0f,0.0f));
			_ProjectorMatrix.SetRow(1,new Vector4(0.0f, 1/t,0.0f,0.0f));
			_ProjectorMatrix.SetRow(2,new Vector4(0.0f, 0.0f,-2 / (f-n),0.0f));
			_ProjectorMatrix.SetRow(3,new Vector4(0.0f, 0.0f,0.0f,1));

			_ViewMatrix = _ProjectorCamera.transform.localToWorldMatrix.inverse;
			_BPV = _BiasMatrix * _ProjectorMatrix * _ViewMatrix;
			Render();
		}
	}

	void Render(){
		if(!_Tex){
			return;
		}
		MeshFilter receiver;
		for(int i = 0; i< _ShadowReceivers.Count; i++){
			// print(">????");
			receiver = _ShadowReceivers[i];
			_ModelMatrix = receiver.transform.localToWorldMatrix;
			_FinalMatrix = _BPV * _ModelMatrix;
			_ProjectorMaterialShadow.SetMatrix("_GlobalProjector", _FinalMatrix);

			Graphics.DrawMesh(receiver.sharedMesh, _ModelMatrix,_ProjectorMaterialShadow, LayerMask.NameToLayer("Scene"));

		}
	}

}