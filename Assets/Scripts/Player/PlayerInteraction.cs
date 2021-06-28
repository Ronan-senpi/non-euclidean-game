using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private static PlayerInteraction instance;
    public static PlayerInteraction Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerInteraction>();
            }
            return instance;
        }
    }
    
    [SerializeField]
    private GameObject interactInputHint;
    public GameObject InteractInputHint => interactInputHint;
    [SerializeField]
    private GameObject grabInputHint;
    public GameObject GrabInputHint => grabInputHint;
    [SerializeField]
    private GameObject dropInputHint;
    public GameObject DropInputHint => dropInputHint;

    [SerializeField]
    [Range(0f, 1f)]
    private float _movedObjectCenteringSpeed = 0.1f;
    [SerializeField]
    [Range(0f, 1f)]
    private float _movedObjectAlignmentSpeed = 0.1f;

    [SerializeField] private float maxDistance;
    private Transform cam;
    private Ray camRay;
    private Lookable objectBeingLookedAt;
    private Movable objectInHand;

    private int _portalScreenLayerMask;
    
    private void Awake()
    {
        _portalScreenLayerMask = 1 << LayerMask.NameToLayer("PortalScreen");
        
        cam = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        UpdateObjectBeingLookedAt();
        UpdateObjectbeingMoved();
    }

    private void UpdateObjectBeingLookedAt()
    {
        camRay = new Ray(cam.position, cam.forward);

        Lookable objectCurrentlyBeingLookedAt = null;
        if (Physics.Raycast(camRay, out RaycastHit hit, maxDistance) && hit.collider.gameObject.CompareTag("Interactable"))
        {
            objectCurrentlyBeingLookedAt = hit.collider.gameObject.GetComponent<Lookable>();
        }
        
        // if both Interactable are the same, no need to check anything else
        if (ReferenceEquals(objectBeingLookedAt, objectCurrentlyBeingLookedAt))
            return;

        // if objectBeingLookedAt != null and we are here, then it is no longer being looked at
        if (!ReferenceEquals(objectBeingLookedAt, null))
        {
            objectBeingLookedAt.OnPlayerStopLooking.Invoke();
            objectBeingLookedAt = null;
        }
        
        // if objectCurrentlyBeingLookedAt != null and we are here, then it is the new Interactable being looked at
        if (!ReferenceEquals(objectCurrentlyBeingLookedAt, null))
        {
            objectBeingLookedAt = objectCurrentlyBeingLookedAt;
            objectBeingLookedAt.OnPlayerStartLooking.Invoke();
        }
    }

    private void UpdateObjectbeingMoved()
    {
        if (objectInHand)
        {
            // Centering
            Vector3 currentPosition = objectInHand.transform.position;
            Vector3 handHeight = cam.position - cam.up * 0.5f;

            Ray ray = new Ray(handHeight, cam.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, objectInHand.MinBoundingRadius, _portalScreenLayerMask))
            {
                GameObject portalScreen = hit.collider.gameObject;
                Portal portal = portalScreen.transform.parent.GetComponent<Portal>();
                ray = portal.TransformRay(ray);
            }
            
            Vector3 targetPosition = ray.origin + ray.direction * objectInHand.MinBoundingRadius;
            Vector3 velocity = (targetPosition - currentPosition) * _movedObjectCenteringSpeed;

            objectInHand.Rigidbody.velocity = velocity / Time.fixedDeltaTime;

            // Alignment
            Quaternion currentRotation = objectInHand.Rigidbody.rotation;
            Quaternion targetRotation = cam.rotation;
            Quaternion deltaRotation = targetRotation * Quaternion.Inverse(currentRotation);
            Quaternion scaledDeltaRotation = Quaternion.Slerp(Quaternion.identity, deltaRotation, _movedObjectAlignmentSpeed);
            
            Vector3 euler = scaledDeltaRotation.eulerAngles;
            euler.x = euler.x > 180 ? euler.x - 360 : euler.x;
            euler.y = euler.y > 180 ? euler.y - 360 : euler.y;
            euler.z = euler.z > 180 ? euler.z - 360 : euler.z;
            
            objectInHand.Rigidbody.angularVelocity = euler;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            if (objectInHand)
            {
                objectInHand.OnPlayerStopMoving.Invoke();
                objectInHand = null;
            }
            else if (objectBeingLookedAt)
            {
                switch (objectBeingLookedAt)
                {
                    case Interactable interactable:
                        interactable.OnPlayerInteract.Invoke();
                        break;
                    case Movable movable:
                        objectInHand = movable;
                        objectInHand.OnPlayerStartMoving.Invoke();
                        break;
                }
            }
            
        }
    }
}
