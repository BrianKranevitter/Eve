using System.Collections;
using System.Collections.Generic;
using Enderlook.Unity.AudioManager;
using UnityEngine;
using UnityEngine.Audio;

public class KamLoopSound : MonoBehaviour
{
    public AudioSource source;
    public bool fadeIn;
    public float fadeInSpeed;
    public bool fadeOut;
    public float fadeOutSpeed;
    
    private float targetVolume;

    private bool fadingIn;
    private bool fadingOut;
    public void Play(AudioUnit audio)
    {
	    source.loop = audio.loopOnPlay;
        source.clip = audio.audioClip;
        source.priority = audio.priority;
        targetVolume = audio.volume.Value;
        source.volume = targetVolume;
        source.pitch = audio.pitch.Value;
        source.panStereo = audio.stereoSpan;
        
        if (audio.spatialBlend.length == 1)
        	source.spatialBlend = audio.spatialBlend[0].value;
        else
        	source.SetCustomCurve(AudioSourceCurveType.SpatialBlend, audio.spatialBlend);

        if (audio.reverbZoneMix.length == 1)
        	source.reverbZoneMix = audio.reverbZoneMix[0].value;
        else
        	source.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, audio.reverbZoneMix);

        if (audio.spread.length == 1)
        	source.spread = audio.spread[0].value;
        else
        	source.SetCustomCurve(AudioSourceCurveType.Spread, audio.spread);

        source.dopplerLevel = audio.dopplerLevel;
        source.minDistance = audio.minDistance;
        source.maxDistance = audio.maxDistance;
        source.rolloffMode = audio.volumeRolloff;

        if (audio.volumeRolloff == AudioRolloffMode.Custom)
        	source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, audio.customRolloffCurve);
        
        source.Play();

        if (fadeIn)
        {
	        if (fadingOut)
	        {
		        StopAllCoroutines();
	        }
	        
	        StartCoroutine(FadeIn());
        }
    }

    IEnumerator FadeIn()
    {
	    source.volume = 0;

	    while (source.volume != targetVolume)
	    {
		    source.volume = Mathf.Lerp(source.volume, targetVolume, fadeInSpeed * Time.deltaTime);
		    if (targetVolume - source.volume < 0.01f)
		    {
			    source.volume = targetVolume;
		    }
		    yield return null;
	    }
    }
    public void Stop()
    {
	    if (fadeOut)
	    {
		    if (fadingIn)
		    {
			    StopAllCoroutines();
		    }
	        
		    StartCoroutine(FadeOut());
	    }
    }
    
    IEnumerator FadeOut()
    {
	    while (source.volume != 0)
	    {
		    source.volume = Mathf.Lerp(source.volume, 0, fadeOutSpeed * Time.deltaTime);
		    if (source.volume < 0.01f)
		    {
			    source.volume = 0;
		    }
		    yield return null;
	    }
	    
	    source.Stop();
    }
}
