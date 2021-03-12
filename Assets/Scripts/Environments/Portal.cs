using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
	[Header("Main Settings")]
	[SerializeField]
	private Portal _linkedPortal;

	[SerializeField]
	private MeshRenderer _screen;

	[SerializeField]
	private int _recursionLimit = 5;

	[Header("Advanced Settings")]
	[SerializeField]
	private float _nearClipOffset = 0.05f;

	[SerializeField]
	private float _nearClipLimit = 0.2f;

	// Private variables
	private RenderTexture _viewTexture;
	private Camera _portalCam;
	private Camera _playerCam;
	private Material _firstRecursionMat;
	private List<PortalTraveller> _trackedTravellers;
	private MeshFilter _screenMeshFilter;

	// Shader uniforms
	private static readonly int DisplayMask = Shader.PropertyToID("displayMask");
	private static readonly int MainTex = Shader.PropertyToID("_MainTex");
	private static readonly int SliceCentre = Shader.PropertyToID("sliceCentre");
	private static readonly int SliceNormal = Shader.PropertyToID("sliceNormal");
	private static readonly int SliceOffsetDst = Shader.PropertyToID("sliceOffsetDst");

	private void Awake()
	{
		_playerCam = Camera.main;
		_portalCam = GetComponentInChildren<Camera>();
		_portalCam.enabled = false;
		_trackedTravellers = new List<PortalTraveller>();
		_screenMeshFilter = _screen.GetComponent<MeshFilter>();
		_screen.material.SetInt(DisplayMask, 1);
	}

	private void LateUpdate()
	{
		HandleTravellers();
	}

	private void HandleTravellers()
	{
		for (int i = 0; i < _trackedTravellers.Count; i++)
		{
			PortalTraveller traveller = _trackedTravellers[i];
			Transform travellerT = traveller.transform;

			Vector3 offsetFromPortal = travellerT.position - transform.position;
			int portalSide = System.Math.Sign(Vector3.Dot(offsetFromPortal, transform.forward));
			int portalSideOld = System.Math.Sign(Vector3.Dot(traveller.PreviousOffsetFromPortal, transform.forward));
			// Teleport the traveller if it has crossed from one side of the portal to the other
			if (portalSide != portalSideOld)
			{
				var positionOld = travellerT.position;
				var rotOld = travellerT.rotation;

				Matrix4x4 m = _linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * travellerT.localToWorldMatrix;

				traveller.Teleport(transform, _linkedPortal.transform, m.GetColumn(3), m.rotation);
				if (traveller.HasGraphicsObject)
				{
					traveller.GraphicsClone.transform.SetPositionAndRotation(positionOld, rotOld);
				}
				// Can't rely on OnTriggerEnter/Exit to be called next frame since it depends on when FixedUpdate runs
				_linkedPortal.OnTravellerEnterPortal(traveller);
				_trackedTravellers.RemoveAt(i);
				i--;
			}
			else
			{
				if (traveller.HasGraphicsObject)
				{
					Matrix4x4 m = _linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * traveller.GraphicsObject.transform.localToWorldMatrix;
					traveller.GraphicsClone.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
				}

				//UpdateSliceParams (traveller);
				traveller.PreviousOffsetFromPortal = offsetFromPortal;
			}
		}
	}

	// Called before any portal cameras are rendered for the current frame
	public void PrePortalRender()
	{
		foreach (var traveller in _trackedTravellers)
		{
			if (traveller.HasGraphicsObject)
			{
				UpdateSliceParams(traveller);
			}
		}
	}

	// Manually render the camera attached to this portal
	// Called after PrePortalRender, and before PostPortalRender
	public void Render()
	{
		// Skip rendering the view from this portal if player is not looking at the linked portal
		if (!CameraUtility.VisibleFromCamera(_linkedPortal._screen, _playerCam))
		{
			return;
		}

		CreateViewTexture();

		var localToWorldMatrix = _playerCam.transform.localToWorldMatrix;
		var renderPositions = new Vector3[_recursionLimit];
		var renderRotations = new Quaternion[_recursionLimit];

		int startIndex = 0;
		_portalCam.projectionMatrix = _playerCam.projectionMatrix;
		for (int i = 0; i < _recursionLimit; i++)
		{
			if (i > 0)
			{
				// No need for recursive rendering if linked portal is not visible through this portal
				if (!CameraUtility.BoundsOverlap(_screenMeshFilter, _linkedPortal._screenMeshFilter, _portalCam))
				{
					break;
				}
			}

			localToWorldMatrix = transform.localToWorldMatrix * _linkedPortal.transform.worldToLocalMatrix * localToWorldMatrix;
			int renderOrderIndex = _recursionLimit - i - 1;
			renderPositions[renderOrderIndex] = localToWorldMatrix.GetColumn(3);
			renderRotations[renderOrderIndex] = localToWorldMatrix.rotation;

			_portalCam.transform.SetPositionAndRotation(renderPositions[renderOrderIndex], renderRotations[renderOrderIndex]);
			startIndex = renderOrderIndex;
		}

		// Hide screen so that camera can see through portal
		_screen.enabled = false;
		_linkedPortal._screen.material.SetInt(DisplayMask, 0);

		for (int i = startIndex; i < _recursionLimit; i++)
		{
			_portalCam.transform.SetPositionAndRotation(renderPositions[i], renderRotations[i]);
			SetNearClipPlane();
			HandleClipping();
			_portalCam.Render();

			if (i == startIndex)
			{
				_linkedPortal._screen.material.SetInt(DisplayMask, 1);
			}
		}

		// Unhide objects hidden at start of render
		_screen.enabled = true;
	}

	private void HandleClipping()
	{
		// There are two main graphical issues when slicing travellers
		// 1. Tiny sliver of mesh drawn on backside of portal
		//    Ideally the oblique clip plane would sort this out, but even with 0 offset, tiny sliver still visible
		// 2. Tiny seam between the sliced mesh, and the rest of the model drawn onto the portal screen
		// This function tries to address these issues by modifying the slice parameters when rendering the view from the portal
		// Would be great if this could be fixed more elegantly, but this is the best I can figure out for now
		const float HIDE_DST = -1000;
		const float SHOW_DST = 1000;
		float screenThickness = _linkedPortal.ProtectScreenFromClipping(_portalCam.transform.position);

		foreach (var traveller in _trackedTravellers)
		{
			if (!traveller.HasGraphicsObject) continue;
			
			if (SameSideOfPortal(traveller.transform.position, PortalCamPos))
			{
				// Addresses issue 1
				traveller.SetSliceOffsetDst(HIDE_DST, false);
			}
			else
			{
				// Addresses issue 2
				traveller.SetSliceOffsetDst(SHOW_DST, false);
			}

			// Ensure clone is properly sliced, in case it's visible through this portal:
			int cloneSideOfLinkedPortal = -SideOfPortal(traveller.transform.position);
			bool camSameSideAsClone = _linkedPortal.SideOfPortal(PortalCamPos) == cloneSideOfLinkedPortal;
			if (camSameSideAsClone)
			{
				traveller.SetSliceOffsetDst(screenThickness, true);
			}
			else
			{
				traveller.SetSliceOffsetDst(-screenThickness, true);
			}
		}

		var offsetFromPortalToCam = PortalCamPos - transform.position;
		foreach (var linkedTraveller in _linkedPortal._trackedTravellers)
		{
			if (!linkedTraveller.HasGraphicsObject) continue;
			
			var travellerPos = linkedTraveller.GraphicsObject.transform.position;
			var clonePos = linkedTraveller.GraphicsClone.transform.position;
			// Handle clone of linked portal coming through this portal:
			bool cloneOnSameSideAsCam = _linkedPortal.SideOfPortal(travellerPos) != SideOfPortal(PortalCamPos);
			if (cloneOnSameSideAsCam)
			{
				// Addresses issue 1
				linkedTraveller.SetSliceOffsetDst(HIDE_DST, true);
			}
			else
			{
				// Addresses issue 2
				linkedTraveller.SetSliceOffsetDst(SHOW_DST, true);
			}

			// Ensure traveller of linked portal is properly sliced, in case it's visible through this portal:
			bool camSameSideAsTraveller = _linkedPortal.SameSideOfPortal(linkedTraveller.transform.position, PortalCamPos);
			if (camSameSideAsTraveller)
			{
				linkedTraveller.SetSliceOffsetDst(screenThickness, false);
			}
			else
			{
				linkedTraveller.SetSliceOffsetDst(-screenThickness, false);
			}
		}
	}

	// Called once all portals have been rendered, but before the player camera renders
	public void PostPortalRender()
	{
		foreach (var traveller in _trackedTravellers)
		{
			if (traveller.HasGraphicsObject)
			{
				UpdateSliceParams(traveller);
			}
		}

		ProtectScreenFromClipping(_playerCam.transform.position);
	}

	private void CreateViewTexture()
	{
		if (_viewTexture == null || _viewTexture.width != Screen.width || _viewTexture.height != Screen.height)
		{
			if (_viewTexture != null)
			{
				_viewTexture.Release();
			}

			_viewTexture = new RenderTexture(Screen.width, Screen.height, 0);
			// Render the view from the portal camera to the view texture
			_portalCam.targetTexture = _viewTexture;
			// Display the view texture on the screen of the linked portal
			_linkedPortal._screen.material.SetTexture(MainTex, _viewTexture);
		}
	}

	// Sets the thickness of the portal screen so as not to clip with camera near plane when player goes through
	private float ProtectScreenFromClipping(Vector3 viewPoint)
	{
		float halfHeight = _playerCam.nearClipPlane * Mathf.Tan(_playerCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
		float halfWidth = halfHeight * _playerCam.aspect;
		float dstToNearClipPlaneCorner = new Vector3(halfWidth, halfHeight, _playerCam.nearClipPlane).magnitude;
		float screenThickness = dstToNearClipPlaneCorner;

		Transform screenT = _screen.transform;
		bool camFacingSameDirAsPortal = Vector3.Dot(transform.forward, transform.position - viewPoint) > 0;
		screenT.localScale = new Vector3(screenT.localScale.x, screenT.localScale.y, screenThickness);
		screenT.localPosition = Vector3.forward * screenThickness * ((camFacingSameDirAsPortal) ? 0.5f : -0.5f);
		return screenThickness;
	}

	private void UpdateSliceParams(PortalTraveller traveller)
	{
		// Calculate slice normal
		int side = SideOfPortal(traveller.transform.position);
		Vector3 sliceNormal = transform.forward * -side;
		Vector3 cloneSliceNormal = _linkedPortal.transform.forward * side;

		// Calculate slice centre
		Vector3 slicePos = transform.position;
		Vector3 cloneSlicePos = _linkedPortal.transform.position;

		// Adjust slice offset so that when player standing on other side of portal to the object, the slice doesn't clip through
		float sliceOffsetDst = 0;
		float cloneSliceOffsetDst = 0;
		float screenThickness = _screen.transform.localScale.z;

		bool playerSameSideAsTraveller = SameSideOfPortal(_playerCam.transform.position, traveller.transform.position);
		if (!playerSameSideAsTraveller)
		{
			sliceOffsetDst = -screenThickness;
		}

		bool playerSameSideAsCloneAppearing = side != _linkedPortal.SideOfPortal(_playerCam.transform.position);
		if (!playerSameSideAsCloneAppearing)
		{
			cloneSliceOffsetDst = -screenThickness;
		}

		// Apply parameters
		for (int i = 0; i < traveller.OriginalMaterials.Length; i++)
		{
			traveller.OriginalMaterials[i].SetVector(SliceCentre, slicePos);
			traveller.OriginalMaterials[i].SetVector(SliceNormal, sliceNormal);
			traveller.OriginalMaterials[i].SetFloat(SliceOffsetDst, sliceOffsetDst);

			traveller.CloneMaterials[i].SetVector(SliceCentre, cloneSlicePos);
			traveller.CloneMaterials[i].SetVector(SliceNormal, cloneSliceNormal);
			traveller.CloneMaterials[i].SetFloat(SliceOffsetDst, cloneSliceOffsetDst);
		}
	}

	// Use custom projection matrix to align portal camera's near clip plane with the surface of the portal
	// Note that this affects precision of the depth buffer, which can cause issues with effects like screenspace AO
	private void SetNearClipPlane()
	{
		// Learning resource:
		// http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
		Transform clipPlane = transform;
		int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, transform.position - _portalCam.transform.position));

		Vector3 camSpacePos = _portalCam.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
		Vector3 camSpaceNormal = _portalCam.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
		float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) + _nearClipOffset;

		// Don't use oblique clip plane if very close to portal as it seems this can cause some visual artifacts
		if (Mathf.Abs(camSpaceDst) > _nearClipLimit)
		{
			Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

			// Update projection based on new clip plane
			// Calculate matrix with player cam so that player camera settings (fov, etc) are used
			_portalCam.projectionMatrix = _playerCam.CalculateObliqueMatrix(clipPlaneCameraSpace);
		}
		else
		{
			_portalCam.projectionMatrix = _playerCam.projectionMatrix;
		}
	}

	private void OnTravellerEnterPortal(PortalTraveller traveller)
	{
		if (!_trackedTravellers.Contains(traveller))
		{
			traveller.EnterPortalThreshold();
			traveller.PreviousOffsetFromPortal = traveller.transform.position - transform.position;
			_trackedTravellers.Add(traveller);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		var traveller = other.GetComponent<PortalTraveller>();
		if (traveller)
		{
			OnTravellerEnterPortal(traveller);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		var traveller = other.GetComponent<PortalTraveller>();
		if (traveller && _trackedTravellers.Contains(traveller))
		{
			traveller.ExitPortalThreshold();
			_trackedTravellers.Remove(traveller);
		}
	}

	/*
	 ** Some helper/convenience stuff:
	 */

	private int SideOfPortal(Vector3 pos)
	{
		return System.Math.Sign(Vector3.Dot(pos - transform.position, transform.forward));
	}

	private bool SameSideOfPortal(Vector3 posA, Vector3 posB)
	{
		return SideOfPortal(posA) == SideOfPortal(posB);
	}

	private Vector3 PortalCamPos => _portalCam.transform.position;

	private void OnValidate()
	{
		if (_linkedPortal != null)
		{
			_linkedPortal._linkedPortal = this;
		}
	}
}