using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAudioManagerSFX : MonoBehaviour
{
    public SFXs sfx;

    public void PlaySFX()
    {
        KamAudioManager.instance.PlaySFX(KamAssetDatabase.i.GetSFX(sfx));
    }
}
