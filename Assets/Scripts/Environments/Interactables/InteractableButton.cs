using UnityEngine;

public class InteractableButton : Interactable
{
    [SerializeField] private Activable linkedObject;

    protected override void Awake()
    {
        base.Awake();
        OnPlayerInteract.AddListener(ActionOnUse);
    }

    private void ActionOnUse()
    {
        linkedObject.onStateChange.Invoke();
    }
}
