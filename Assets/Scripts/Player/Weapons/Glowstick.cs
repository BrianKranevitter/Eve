using System;
using System.Collections;
using System.Collections.Generic;
using Enderlook.Unity.AudioManager;
using Game.Level;
using Game.Player;
using Game.Player.Weapons;
using Game.Utility;
using UnityEngine;

[RequireComponent(typeof(OutLineModifier))]
public sealed class Glowstick : MonoBehaviour, IPickup, IInteractable
{
    public static List<Glowstick> activeGlowsticks = new List<Glowstick>();
    
    
    [SerializeField, Tooltip("Audio played on pickup.")]
    private AudioUnit audioOnPickup;
    
    [Min(0), Tooltip("Minimum Range at which the light starts interacting with things, such as enemies, at CLOSE range.")]
    public float interactionRange_Close;
    [Min(0), Tooltip("Maximum range at which the light starts interacting with things, such as enemies, at the FAR range.")]
    public float interactionRange_Far;

    private OutLineModifier outline;

    private float nextAudioWhenFullAt;

    private void Awake() => outline = GetComponent<OutLineModifier>();

    private void Start()
    {
        activeGlowsticks.Add(this);
    }

    void IInteractable.Interact() => Pickup();

    public void Pickup()
    {
        Try.PlayOneShoot(transform.position, audioOnPickup, "on pickup");

        Destroy(gameObject);
    }

    void IInteractable.Highlight() => outline.Highlight();

    void IInteractable.Unhighlight() => outline.Unhighlight();

    void IInteractable.InSight() => outline.InSight();

    void IInteractable.OutOfSight() => outline.OutOfSight();

    private void OnDestroy()
    {
        activeGlowsticks.Remove(this);
    }
}
