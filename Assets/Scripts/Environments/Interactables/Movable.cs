using UnityEngine;
using UnityEngine.Events;

public class Movable : Lookable
{
    public UnityEvent OnPlayerStartMoving { get; } = new UnityEvent();
    public UnityEvent OnPlayerStopMoving { get; } = new UnityEvent();
    
    public bool Moving { get; private set; }

    public Rigidbody Rigidbody { get; private set; }
    public Collider Collider { get; private set; }

    public float MinBoundingRadius { get; protected set; } = 2;

    private int _objectHeldLayer;
    private int _defaultLayer;
    
    private static PhysicMaterial _noFrictionMaterial;
    private static PhysicMaterial NoFrictionMaterial => _noFrictionMaterial ??= new PhysicMaterial
    {
        dynamicFriction = 0,
        staticFriction = 0,
        frictionCombine = PhysicMaterialCombine.Minimum
    };

    protected override void Awake()
    {
        base.Awake();
        
        _objectHeldLayer = LayerMask.NameToLayer("ObjectHeld");
        _defaultLayer = LayerMask.NameToLayer("Default");
        
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<Collider>();
        
        // these two listeners must be called first
        OnPlayerStartMoving.AddListener(() => Moving = true);
        OnPlayerStopMoving.AddListener(() => Moving = false);
        
        OnPlayerStartMoving.AddListener(UpdateHighlightState);
        OnPlayerStartLooking.AddListener(UpdateHighlightState);
        OnPlayerStopMoving.AddListener(UpdateHighlightState);
        OnPlayerStopLooking.AddListener(UpdateHighlightState);
        
        OnPlayerStartMoving.AddListener(StartMoving);
        OnPlayerStopMoving.AddListener(StopMoving);
    }

    private void StartMoving()
    {
        Rigidbody.useGravity = false;
        Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        gameObject.layer = _objectHeldLayer;
        Collider.sharedMaterial = NoFrictionMaterial;
    }

    private void StopMoving()
    {
        Rigidbody.useGravity = true;
        Rigidbody.interpolation = RigidbodyInterpolation.None;
        gameObject.layer = _defaultLayer;
        Collider.sharedMaterial = null;
    }

    private void UpdateHighlightState()
    {
        if (LookedAt)
        {
            if (Moving)
            {
                PlayerInteraction.Instance.GrabInputHint.SetActive(false);
                PlayerInteraction.Instance.DropInputHint.SetActive(true);
            }
            else
            {
                PlayerInteraction.Instance.GrabInputHint.SetActive(true);
                PlayerInteraction.Instance.DropInputHint.SetActive(false);
            }
        }
        else
        {
            if (Moving)
            {
                PlayerInteraction.Instance.GrabInputHint.SetActive(false);
                PlayerInteraction.Instance.DropInputHint.SetActive(true);
            }
            else
            {
                PlayerInteraction.Instance.GrabInputHint.SetActive(false);
                PlayerInteraction.Instance.DropInputHint.SetActive(false);
            }
        }
    }
}
