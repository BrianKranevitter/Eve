using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBracerColor : MonoBehaviour
{
    PlayerBody player;

    Material leftArmColor, rightArmColor;

    public GameObject leftArm, rightArm;

    public float frecuency;

    public Color healthyColor, damagedColor;
    private void Awake()
    {
        player = GetComponent<PlayerBody>();
        leftArmColor = leftArm.GetComponent<Renderer>().material;
        rightArmColor = rightArm.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.Paused) return;
        
        if(player.currentHp>0)
        {
        leftArmColor.SetColor("_EmissionColor", Color.Lerp(damagedColor, healthyColor, player.currentHp / 100));
        rightArmColor.SetColor("_EmissionColor", Color.Lerp(damagedColor, healthyColor, player.currentHp / 100));
        }
        else
        {
            leftArmColor.SetColor("_EmissionColor", Color.black);
            rightArmColor.SetColor("_EmissionColor", Color.black);
        }
    }
}
