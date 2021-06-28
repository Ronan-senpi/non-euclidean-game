using UnityEngine.Events;

public class Interactable : Lookable
{
    public UnityEvent OnPlayerInteract { get; } = new UnityEvent();

    protected override void Awake()
    {
        base.Awake();
        
        OnPlayerStartLooking.AddListener(() => PlayerInteraction.Instance.InteractInputHint.SetActive(true));
        OnPlayerStopLooking.AddListener(() => PlayerInteraction.Instance.InteractInputHint.SetActive(false));
    }
}
