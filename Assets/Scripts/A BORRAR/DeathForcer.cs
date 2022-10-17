using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DeathForcer : MonoBehaviour
{
    public float blurValue, bloodValue1, bloodValue2, bloodValue3;

    [SerializeField]
    private PostProcessVolume ppVolume;

    private FatiguePPSSettings fatigueShader;

    private HurtShaderPPSSettings hurtShader;

    [SerializeField]
    private Animator _anim;

    private void Awake()
    {
        ppVolume.profile.TryGetSettings(out fatigueShader);
        ppVolume.profile.TryGetSettings(out hurtShader);
    }

    private void Update()
    {
        fatigueShader._GlobalIntensity.value = blurValue;
        hurtShader._Splats1.value = bloodValue1;
        hurtShader._Splats2.value = bloodValue2;
        hurtShader._Splats3.value = bloodValue3;

        if (Input.GetKeyDown(KeyCode.B))
        {
            _anim.SetBool("Dead", true);
        }
    }
}