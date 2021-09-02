using UnityEngine.Audio;
using System;
using UnityEngine;

// Reproduce el sonido indicado con Play().
// Es un singleton

namespace MazesAndMore
{
	public class AudioManager : MonoBehaviour
	{
		public static AudioManager Instance;

		public AudioMixerGroup mixerGroup;

		public Sound[] sounds;

		void Awake()
		{
			if (Instance != null)
				Destroy(gameObject);
			else
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}

			foreach (Sound s in sounds)
			{
				s.source = gameObject.AddComponent<AudioSource>();
				s.source.clip = s.clip;
				s.source.loop = s.loop;

				s.source.outputAudioMixerGroup = mixerGroup;
			}
		}

		public void Play(string sound)
		{
			Sound s = Array.Find(sounds, item => item.name == sound);
			if (s == null)
			{
				Debug.LogWarning("Sound: " + name + " not found!");
				return;
			}

			if (s.source == null)
				return;

			s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
			s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

			s.source.Play();
		}

		public void Stop(string sound)
		{
			Sound s = Array.Find(sounds, item => item.name == sound);
			if (s == null)
			{
				Debug.LogWarning("Sound: " + name + " not found!");
				return;
			}

			s.source.Stop();
		}

		public void StopAll()
		{
			foreach (Sound s in sounds)
			{
				s.source.Stop();
			}
		}

		public void StopAllExcept(string song)
		{
			foreach (Sound s in sounds)
			{
				if (s.name != song)
					s.source.Stop();
			}
		}

		public void Pause(string sound)
		{
			Sound s = Array.Find(sounds, item => item.name == sound);
			if (s == null)
			{
				Debug.LogWarning("Sound: " + name + " not found!");
				return;
			}

			s.source.Pause();
		}

		public void Resume(string sound)
		{
			Sound s = Array.Find(sounds, item => item.name == sound);
			if (s == null)
			{
				Debug.LogWarning("Sound: " + name + " not found!");
				return;
			}

			s.source.UnPause();
		}

		public bool IsPlaying(string sound)
		{
			Sound s = Array.Find(sounds, item => item.name == sound);
			if (s == null)
			{
				Debug.LogWarning("Sound: " + name + " not found!");
				return false;
			}

			return s.source.isPlaying;
		}

		public void setVolume(float volume)
		{
			mixerGroup.audioMixer.SetFloat("Volume", volume);
		}
	}
}