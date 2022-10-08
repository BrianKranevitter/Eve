using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamParticleStateSetter : MonoBehaviour
{
    Animator anim;
    [SerializeField, Tooltip("Determinates if the effect is Soft, Mid, or Aggresive")]
    public string leakStrenght;


    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();

        anim.SetBool(leakStrenght, true);
    }
}
