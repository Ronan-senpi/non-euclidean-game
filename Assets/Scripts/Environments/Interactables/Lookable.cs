using UnityEngine;
using UnityEngine.Events;

public class Lookable : MonoBehaviour
{
	public UnityEvent OnPlayerStartLooking { get; } = new UnityEvent();
	public UnityEvent OnPlayerStopLooking { get; } = new UnityEvent();
	
	private Outline _outline;

	public bool LookedAt { get; private set; }

	protected virtual void Awake()
	{
		_outline = GetComponent<Outline>();
		
		// these two listeners must be called first
		OnPlayerStartLooking.AddListener(() => LookedAt = true);
		OnPlayerStopLooking.AddListener(() => LookedAt = false);
		
		OnPlayerStartLooking.AddListener(ShowHighlight);
		OnPlayerStopLooking.AddListener(HideHighlight);
	}

	private void ShowHighlight()
	{
		if (_outline)
			_outline.enabled = true;
	}

	private void HideHighlight()
	{
		if (_outline != null)
			_outline.enabled = false;
	}
}