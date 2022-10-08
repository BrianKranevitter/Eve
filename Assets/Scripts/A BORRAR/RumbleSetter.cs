using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RumbleSetter : MonoBehaviour
{
    private FatigueForcer fatigueShader;

    private RumbleShaderController rumbleShader;

    private void Awake()
    {
        fatigueShader = FindObjectOfType<FatigueForcer>();
        rumbleShader = FindObjectOfType<RumbleShaderController>();
    }

    public void SetExplosion(float force)
    {
        rumbleShader.Explosion(force/100);
        fatigueShader.Explosion(force);
    }
}
