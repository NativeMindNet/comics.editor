using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using ComicsUnity.Models;
using Unity.EditorCoroutines.Editor;

namespace ComicsUnity.Audio
{
	/// <summary>
	/// Manages audio preview playback in the editor.
	/// </summary>
	public class EditorAudioManager : IDisposable
	{
		private readonly Dictionary<string, AudioClip> _clipCache = new Dictionary<string, AudioClip>();
		private readonly Dictionary<SoundModel, AudioSource> _sources = new Dictionary<SoundModel, AudioSource>();
		private GameObject _audioHost;
		private bool _enabled = true;
		private double _lastProcessTime;
		private const double DebounceMs = 50;
		private string _soundsFolderPath;

		/// <summary>
		/// Initialize the audio manager with the sounds folder path.
		/// </summary>
		public void Initialize(string soundsFolderPath)
		{
			_soundsFolderPath = soundsFolderPath;

			if (_audioHost == null)
			{
				_audioHost = new GameObject("EditorAudioHost");
				_audioHost.hideFlags = HideFlags.HideAndDontSave;
			}

			// Preload clips from folder
			if (Directory.Exists(soundsFolderPath))
			{
				foreach (var file in Directory.GetFiles(soundsFolderPath, "*.mp3"))
				{
					var fileName = Path.GetFileName(file);
					if (!_clipCache.ContainsKey(fileName))
					{
						EditorCoroutineUtility.StartCoroutineOwnerless(LoadClip(file, clip =>
						{
							if (clip != null)
								_clipCache[fileName] = clip;
						}));
					}
				}
			}
		}

		private IEnumerator LoadClip(string filePath, Action<AudioClip> onLoaded)
		{
			using (var request = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG))
			{
				yield return request.SendWebRequest();

				if (request.result == UnityWebRequest.Result.Success)
				{
					var clip = DownloadHandlerAudioClip.GetContent(request);
					clip.name = Path.GetFileName(filePath);
					onLoaded(clip);
				}
				else
				{
					Debug.LogWarning($"[EditorAudioManager] Failed to load: {filePath} - {request.error}");
					onLoaded(null);
				}
			}
		}

		/// <summary>
		/// Process scroll change and trigger/stop sounds as needed.
		/// </summary>
		public void ProcessScroll(IList<SoundModel> sounds, double prevScroll, double currentScroll)
		{
			if (!_enabled) return;

			var now = Time.realtimeSinceStartupAsDouble;

			foreach (var sound in sounds)
			{
				var anim = SoundAnim.FindCurrent(sound.Animations, prevScroll, currentScroll);
				var source = GetOrCreateSource(sound);

				if (source == null) continue;

				if (anim != null)
				{
					// Check debounce for play (not for stop)
					if (!source.isPlaying && now - _lastProcessTime >= DebounceMs / 1000.0)
					{
						source.loop = anim.Start != anim.End;
						source.Play();
					}
				}
				else if (source.isPlaying && source.loop)
				{
					// Immediate stop, no debounce
					source.Stop();
				}
			}

			_lastProcessTime = now;
		}

		private AudioSource GetOrCreateSource(SoundModel sound)
		{
			if (_audioHost == null) return null;

			if (_sources.TryGetValue(sound, out var source) && source != null)
				return source;

			// Get clip from cache
			if (string.IsNullOrEmpty(sound.File) || !_clipCache.TryGetValue(sound.File, out var clip))
			{
				// Try to load on demand
				if (!string.IsNullOrEmpty(sound.File) && !string.IsNullOrEmpty(_soundsFolderPath))
				{
					var filePath = Path.Combine(_soundsFolderPath, sound.File);
					if (File.Exists(filePath))
					{
						EditorCoroutineUtility.StartCoroutineOwnerless(LoadClip(filePath, loadedClip =>
						{
							if (loadedClip != null)
								_clipCache[sound.File] = loadedClip;
						}));
					}
				}
				return null;
			}

			// Create AudioSource
			source = _audioHost.AddComponent<AudioSource>();
			source.clip = clip;
			source.playOnAwake = false;
			source.spatialBlend = 0f; // 2D sound
			_sources[sound] = source;

			return source;
		}

		/// <summary>
		/// Enable or disable audio playback.
		/// </summary>
		public void SetEnabled(bool enabled)
		{
			if (!enabled && _enabled)
				StopAll();
			_enabled = enabled;
		}

		/// <summary>
		/// Stop all currently playing sounds.
		/// </summary>
		public void StopAll()
		{
			foreach (var source in _sources.Values)
			{
				if (source != null && source.isPlaying)
					source.Stop();
			}
		}

		/// <summary>
		/// Dispose and cleanup all resources.
		/// </summary>
		public void Dispose()
		{
			StopAll();

			// Destroy clips
			foreach (var clip in _clipCache.Values)
			{
				if (clip != null)
					UnityEngine.Object.DestroyImmediate(clip);
			}
			_clipCache.Clear();

			// Destroy audio host (includes all AudioSource components)
			if (_audioHost != null)
			{
				UnityEngine.Object.DestroyImmediate(_audioHost);
				_audioHost = null;
			}

			_sources.Clear();
		}
	}
}
