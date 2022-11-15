using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enderlook.Unity.AudioManager;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using AudioType = Enderlook.Unity.AudioManager.AudioType;

public class KamAudioManager : MonoBehaviour 
{
	private static KamAudioManager i;
	public static KamAudioManager instance
	{
		get
		{
			/*
			if (i == null)
			{
				GameObject temp = Resources.Load("AudioManager") as GameObject;
				i = Instantiate(temp).GetComponent<AudioManager>();
				Initialize();
			}
			*/

			return i;
		}
	}
	public static MUSIC activeSong = null;
	public static List<MUSIC> allSongs = new List<MUSIC>();
	
	public AudioMixerGroup sfxMixerGroup;
	public AudioMixerGroup musicMixerGroup;
	public AudioMixerGroup voiceMixerGroup;


	public float songTransitionSpeed = 2f;
	public bool songSmoothTransitions = true;

	 void Awake()
	{
		if (i == null)
		{
			i = this;
			Initialize();
		}
		else
		{
			DestroyImmediate(gameObject);
		}
	}

	void Initialize()
	{
		transform.parent = null;
		DontDestroyOnLoad(i);
	}
	public void PlaySFX(AudioClip effect, float volume = 1f, float pitch = 1f, Transform position = null, AudioMixerGroup mixerGroup = null)
	{
		PlaySFX_Vector3(effect, volume, pitch, position == null ? Vector3.zero : position.position, mixerGroup);
	}
	
	public void PlaySFX_Vector3(AudioClip effect, float volume = 1f, float pitch = 1f, Vector3 position = default, AudioMixerGroup mixerGroup = null)
	{
		AudioMixerGroup resultMixerGroup = mixerGroup == null ? sfxMixerGroup : mixerGroup;
		AudioSource source = position == Vector3.zero ? 
			CreateNewSource(string.Format("SFX [{0}]", effect.name), resultMixerGroup) 
			: CreateNewSource(string.Format("SFX [{0}]", effect.name), position, resultMixerGroup);
		source.clip = effect;
		source.volume = volume;
		source.pitch = pitch;
		source.Play();

		Destroy(source.gameObject, effect.length);
	}
	
	public void PlaySFX_AudioUnit(AudioUnit audio, Vector3 position = default, bool loop = false, AudioMixerGroup mixerGroup = null)
	{
		AudioMixerGroup resultMixerGroup = sfxMixerGroup;
		
		switch (audio.audioType)
		{
			case AudioType.Music:
				resultMixerGroup = musicMixerGroup;
				break;
			case AudioType.Sound:
				resultMixerGroup = sfxMixerGroup;
				break;
			default:
				Debug.Assert(false, "Impossible state.");
				break;
		}

		if (mixerGroup != null)
		{
			resultMixerGroup = mixerGroup;
		}
		
		AudioSource source = position == Vector3.zero ? 
			CreateNewSource(string.Format("SFX [{0}]", audio.name), resultMixerGroup) 
			: CreateNewSource(string.Format("SFX [{0}]", audio.name), position, resultMixerGroup);
		
		source.clip = audio.audioClip;
		source.priority = audio.priority;
		source.volume = audio.volume.Value;
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

		if (loop)
		{
			source.loop = true;
		}
		else
		{
			Destroy(source.gameObject, audio.audioClip.length);
		}
		
	}

	public void PlayMusic(AudioClip music, float maxVolume = 1f, float pitch = 1f, float startingVolume = 0f, bool playOnStart = true, bool loop = true)
	{
		if (music != null)
		{
			for(int i = 0; i < allSongs.Count; i++)
			{
				MUSIC s = allSongs[i];
				if (s.clip == music)
				{
					activeSong = s;
					break;
				}
			}
			if (activeSong == null || activeSong.clip != music)
				activeSong = new MUSIC(music, maxVolume, pitch, startingVolume, playOnStart, loop);
		}
		else 
			activeSong = null;

		StopAllCoroutines();
		StartCoroutine(VolumeLeveling());
	}

	IEnumerator VolumeLeveling()
	{
		while(TransitionSongs())
			yield return new WaitForEndOfFrame();
	}

	bool TransitionSongs()
	{
		bool anyValueChanged = false;

		float speed = songTransitionSpeed * Time.deltaTime;
		for (int i = allSongs.Count - 1; i >= 0; i--) 
		{
			MUSIC song = allSongs [i];
			if (song == activeSong) 
			{
				if (song.volume < song.maxVolume) 
				{
					song.volume = songSmoothTransitions ? Mathf.Lerp (song.volume, song.maxVolume, speed) : Mathf.MoveTowards (song.volume, song.maxVolume, speed);
					anyValueChanged = true;
				}
			} 
			else 
			{
				if (song.volume > 0) 
				{
					song.volume = songSmoothTransitions ? Mathf.Lerp (song.volume, 0f, speed) : Mathf.MoveTowards (song.volume, 0f, speed);
					anyValueChanged = true;
				}
				else
				{
					allSongs.RemoveAt (i);
					song.DestroySong();
					continue;
				}
			}
		}

		return anyValueChanged;
	}

	public static AudioSource CreateNewSource(string _name, Vector3 position, AudioMixerGroup group = null)
	{
		AudioSource newSource = CreateNewSource(_name, group);
		newSource.transform.position = position;
		return newSource;
	}
	
	static AudioSource CreateNewSource(string _name, AudioMixerGroup group = null)
	{
		AudioSource newSource = new GameObject(_name).AddComponent<AudioSource>();
		if(group != null)
		{
			newSource.outputAudioMixerGroup = group;
		}
		newSource.transform.SetParent(instance.transform);
		return newSource;
	}

	

	[System.Serializable]
	public class MUSIC
	{
		public AudioSource source;
		public AudioClip clip {get{return source.clip;} set{source.clip = value;}}
		public float maxVolume = 1f;

		public MUSIC(AudioClip clip, float _maxVolume, float pitch, float startingVolume, bool playOnStart, bool loop)
		{
			source = CreateNewSource(string.Format("SONG [{0}]", clip.name), instance.musicMixerGroup);
			source.clip = clip;
			source.volume = startingVolume;
			maxVolume = _maxVolume;
			source.pitch = pitch;
			source.loop = loop;

			allSongs.Add(this);

			if (playOnStart)
				source.Play();
		}

		public float volume { get{ return source.volume;} set{source.volume = value;}}
		public float pitch {get{return source.pitch;} set{source.pitch = value;}}

		public void Play()
		{
			source.Play();
		}

		public void Stop()
		{
			source.Stop();
		}

		public void Pause()
		{
			source.Pause();
		}

		public void UnPause()
		{
			source.UnPause();
		}

		public void DestroySong()
		{
			allSongs.Remove(this);
			DestroyImmediate(source.gameObject);
		}
	}
}
