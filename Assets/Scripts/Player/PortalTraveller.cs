using System.Collections.Generic;
using UnityEngine;

public class PortalTraveller : MonoBehaviour
{
	[SerializeField]
	private GameObject _graphicsObject;
	
	public GameObject GraphicsObject => _graphicsObject;
	public GameObject GraphicsClone { get; set; }

	private bool _hasGraphicsObject;
	public bool HasGraphicsObject
	{
		get
		{
			Initialize();
			
			return _hasGraphicsObject;
		}
	}

	public Vector3 PreviousOffsetFromPortal { get; set; }

	public Material[] OriginalMaterials { get; private set; }
	public Material[] CloneMaterials { get; private set; }

	private bool _initialized;
	
	// Shader uniforms
	private static readonly int SliceNormal = Shader.PropertyToID("sliceNormal");
	private static readonly int SliceOffsetDst = Shader.PropertyToID("sliceOffsetDst");

	public virtual void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
	{
		transform.position = pos;
		transform.rotation = rot;
	}

	private void Initialize()
	{
		if (_initialized) return;
		_initialized = true;

		_hasGraphicsObject = GraphicsObject;
			
		if (_hasGraphicsObject)
		{
			GraphicsClone = Instantiate(_graphicsObject, _graphicsObject.transform.parent);
			GraphicsClone.transform.localScale = _graphicsObject.transform.localScale;
			OriginalMaterials = GetMaterials(_graphicsObject);
			CloneMaterials = GetMaterials(GraphicsClone);
		}
	}

	// Called when first touches portal
	public virtual void EnterPortalThreshold()
	{
		if (HasGraphicsObject)
		{
			GraphicsClone.SetActive(true);
		}
	}

	// Called once no longer touching portal (excluding when teleporting)
	public virtual void ExitPortalThreshold()
	{
		if (HasGraphicsObject)
		{
			GraphicsClone.SetActive(false);
			
			// Disable slicing
			for (int i = 0; i < OriginalMaterials.Length; i++)
			{
				OriginalMaterials[i].SetVector(SliceNormal, Vector3.zero);
			}
		}
	}

	public void SetSliceOffsetDst(float dst, bool clone)
	{
		for (int i = 0; i < OriginalMaterials.Length; i++)
		{
			if (clone)
			{
				CloneMaterials[i].SetFloat(SliceOffsetDst, dst);
			}
			else
			{
				OriginalMaterials[i].SetFloat(SliceOffsetDst, dst);
			}
		}
	}

	private static Material[] GetMaterials(GameObject g)
	{
		var renderers = g.GetComponentsInChildren<MeshRenderer>();
		var matList = new List<Material>();
		foreach (var renderer in renderers)
		{
			foreach (var mat in renderer.materials)
			{
				matList.Add(mat);
			}
		}

		return matList.ToArray();
	}
}