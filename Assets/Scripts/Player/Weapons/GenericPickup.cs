using System.Collections;
using System.Collections.Generic;
using Enderlook.Unity.AudioManager;
using Game.Level;
using Game.Player;
using Game.Player.Weapons;
using Game.Utility;
using UnityEngine;
using UnityEngine.Events;

public class GenericPickup : MonoBehaviour, IPickup, IInteractable
{
    [SerializeField, Tooltip("Audio played on pickup.")]
    private AudioUnit audioOnPickup;
    
    [SerializeField]
    private UnityEvent onPickup;

    private OutLineModifier outline;

    private float nextAudioWhenFullAt;

    private void Awake() => outline = GetComponent<OutLineModifier>();

    void IInteractable.Interact() => Pickup();

    public void Pickup()
    {
        onPickup.Invoke();
        Destroy(gameObject);

        Try.PlayOneShoot(transform.position, audioOnPickup, "Pickup");
    }

    void IInteractable.Highlight() => outline.Highlight();

    void IInteractable.Unhighlight() => outline.Unhighlight();

    void IInteractable.InSight() => outline.InSight();

    void IInteractable.OutOfSight() => outline.OutOfSight();
}