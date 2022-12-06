using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class RumbleShaderController : MonoBehaviour
{
    private RumblePPSSettings hurtShader;

    [SerializeField, Tooltip("The center position where rumble will reach maximun apperture.")]
    private Transform boilerRoomPosition;

    [SerializeField, Tooltip("The range that boiler room rumble reaches.")]
    private float range;

    [SerializeField, Tooltip("How fast explosion rumble will grow down.")]
    private float growDownExplosionSpeed;

    [Tooltip("The max rumble apperture.")]
    public float apperture;

    [SerializeField]
    private PostProcessVolume ppVolume;

    private void Awake()
    {
        ppVolume.profile.TryGetSettings(out hurtShader);
    }

    void Update()
    {
        ResetExplosionRumbleApperture();

        hurtShader._Rumble_Aprerture_Natural.value = apperture * Mathf.Clamp(1 - Vector3.Distance(boilerRoomPosition.position, transform.position) / range, 0, 1);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(boilerRoomPosition.position, range);
    }

    public void Explosion(float value)
    {
        hurtShader._Rumble_Aperture_Explosion.value += value;
    }

    private void ResetExplosionRumbleApperture()
    {
        if (hurtShader._Rumble_Aperture_Explosion.value > 0)
            hurtShader._Rumble_Aperture_Explosion.value -= Time.deltaTime * growDownExplosionSpeed;
        else if ((hurtShader._Rumble_Aperture_Explosion.value < 0))
            hurtShader._Rumble_Aperture_Explosion.value = 0;
    }
}
