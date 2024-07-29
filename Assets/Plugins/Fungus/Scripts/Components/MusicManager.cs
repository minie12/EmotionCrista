// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Music manager which provides basic music and sound effect functionality.
    /// Music playback persists across scene loads.
    /// </summary>
    //[RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        protected AudioSource[] audioSourcesMusic = new AudioSource[2];
        protected AudioSource audioSourceAmbiance;
        protected AudioSource audioSourceSoundEffect;

        int currentMusicIndex = 0;
        int musicGUID = 0;

        void Reset()
        {
            int audioSourceCount = this.GetComponents<AudioSource>().Length;
            for (int i = 0; i < 4 - audioSourceCount; i++)
                gameObject.AddComponent<AudioSource>();

        }

        protected virtual void Awake()
        {
            Reset();
            AudioSource[] audioSources = GetComponents<AudioSource>();
            audioSourcesMusic[0] = audioSources[0];
            audioSourcesMusic[1] = audioSources[1];
            audioSourceAmbiance = audioSources[2];
            audioSourceSoundEffect = audioSources[3];
        }

        protected virtual void Start()
        {
            foreach (AudioSource audioSource in audioSourcesMusic)
            {
                audioSource.playOnAwake = false;
                audioSource.loop = true;
            }
        }

        #region Public members

        void IncreaseMusicGuid()
        {
            musicGUID = (musicGUID + 1) % 10000;
        }

        /// <summary>
        /// Plays game music using an audio clip.
        /// One music clip may be played at a time.
        /// </summary>
        public void PlayMusic(AudioClip musicClip, bool loop, float fadeDuration, float atTime)
        {
            if (audioSourcesMusic[currentMusicIndex] == null || audioSourcesMusic[currentMusicIndex].clip == musicClip)
            {
                return;
            }

            int cachedMusicGUID = musicGUID;

            if (Mathf.Approximately(fadeDuration, 0f))
            {
                audioSourcesMusic[currentMusicIndex].clip = musicClip;
                audioSourcesMusic[currentMusicIndex].loop = loop;
                audioSourcesMusic[currentMusicIndex].time = atTime;  // May be inaccurate if the audio source is compressed http://docs.unity3d.com/ScriptReference/AudioSource-time.html BK
                audioSourcesMusic[currentMusicIndex].Play();

                IncreaseMusicGuid();
            }
            else
            {
                float startVolume = audioSourcesMusic[currentMusicIndex].volume;

                LeanTween.value(gameObject, startVolume, 0f, fadeDuration)
                    .setOnUpdate((v) => {
                        // Fade out current music
                        audioSourcesMusic[currentMusicIndex].volume = v;
                    }).setOnComplete(() => {
                        if (cachedMusicGUID == musicGUID)
                        {
                            // Play new music
                            audioSourcesMusic[currentMusicIndex].volume = startVolume;
                            audioSourcesMusic[currentMusicIndex].clip = musicClip;
                            audioSourcesMusic[currentMusicIndex].loop = loop;
                            audioSourcesMusic[currentMusicIndex].time = atTime;  // May be inaccurate if the audio source is compressed http://docs.unity3d.com/ScriptReference/AudioSource-time.html BK
                            audioSourcesMusic[currentMusicIndex].Play();

                            IncreaseMusicGuid();
                        }
                    });
            }
        }


        /// <summary>
        /// Plays Intro, then Loop music using an audio clip.
        /// </summary>
        public void PlayMusicEmo(AudioClip introClip, AudioClip loopClip, bool bLoop, float fadeDuration, float atTime, bool bFadeIn)
        {
            if (audioSourcesMusic[currentMusicIndex] == null || audioSourcesMusic[currentMusicIndex].clip == introClip || audioSourcesMusic[currentMusicIndex].clip == loopClip )
            {
                return;
            }

            int cachedMusicIndex = currentMusicIndex;
            int cachedMusicGUID = musicGUID;

            if (Mathf.Approximately(fadeDuration, 0f))
            {
                audioSourcesMusic[currentMusicIndex].clip = introClip;
                audioSourcesMusic[currentMusicIndex].loop = bLoop;
                audioSourcesMusic[currentMusicIndex].time = atTime;  // May be inaccurate if the audio source is compressed http://docs.unity3d.com/ScriptReference/AudioSource-time.html BK
                audioSourcesMusic[currentMusicIndex].Play();

                IncreaseMusicGuid();
            }
            else
            {
                if (false == bFadeIn)
                {
                    float startVolume = audioSourcesMusic[currentMusicIndex].volume;

                    LeanTween.value(gameObject, startVolume, 0f, fadeDuration)
                        .setOnUpdate((v) =>
                        {
                            // Fade out current music
                            audioSourcesMusic[currentMusicIndex].volume = v;
                        }).setOnComplete(() =>
                        {
                            if ((cachedMusicGUID == musicGUID) && (cachedMusicIndex == currentMusicIndex))
                            {
                                // Play new music
                                audioSourcesMusic[currentMusicIndex].volume = startVolume;
                                audioSourcesMusic[currentMusicIndex].clip = introClip;
                                audioSourcesMusic[currentMusicIndex].loop = bLoop;
                                audioSourcesMusic[currentMusicIndex].time = atTime;  // May be inaccurate if the audio source is compressed http://docs.unity3d.com/ScriptReference/AudioSource-time.html BK
                                audioSourcesMusic[currentMusicIndex].Play();

                                IncreaseMusicGuid();

                                StartCoroutine(PlayConsective(loopClip));
                            }
                        });
                }
                else
                {
                    int previousMusicIndex = currentMusicIndex;
                    currentMusicIndex = (currentMusicIndex + 1) % audioSourcesMusic.Length;

                    float startVolume = audioSourcesMusic[previousMusicIndex].volume;

                    LeanTween.value(gameObject, startVolume, 0f, fadeDuration)
                        .setOnUpdate((v) =>
                        {
                            // Fade out current music
                            audioSourcesMusic[previousMusicIndex].volume = v;
                        }).setOnComplete(() =>
                        {
                            if (currentMusicIndex != previousMusicIndex)
                            {
                                audioSourcesMusic[previousMusicIndex].Stop();
                                audioSourcesMusic[previousMusicIndex].clip = null;
                            }
                        });

                    // Play new music
                    audioSourcesMusic[currentMusicIndex].volume = 0f;
                    audioSourcesMusic[currentMusicIndex].clip = introClip;
                    audioSourcesMusic[currentMusicIndex].loop = bLoop;
                    audioSourcesMusic[currentMusicIndex].time = atTime;  // May be inaccurate if the audio source is compressed http://docs.unity3d.com/ScriptReference/AudioSource-time.html BK
                    audioSourcesMusic[currentMusicIndex].Play();

                    IncreaseMusicGuid();

                    LeanTween.value(gameObject, 0f, startVolume, fadeDuration)
                        .setOnUpdate((v) =>
                        {
                            // Fade out current music
                            audioSourcesMusic[currentMusicIndex].volume = v;
                        });

                    StartCoroutine(PlayConsective(loopClip));
                }
            }
        }

        public IEnumerator PlayConsective(AudioClip loopClip) {
            int cachedMusicIndex = currentMusicIndex;
            int cachedMusicGUID = musicGUID;

            float clipLength = audioSourcesMusic[cachedMusicIndex].clip.length;

            Debug.Log("Audio length : " + clipLength);
            yield return new WaitForSeconds(clipLength);

            if ((currentMusicIndex == cachedMusicIndex) && (cachedMusicGUID == musicGUID))
            {
                audioSourcesMusic[currentMusicIndex].clip = loopClip;
                audioSourcesMusic[currentMusicIndex].Play();
            }
        }

        /// <summary>
        /// Plays a sound effect once, at the specified volume.
        /// </summary>
        /// <param name="soundClip">The sound effect clip to play.</param>
        /// <param name="volume">The volume level of the sound effect.</param>
        public virtual void PlaySound(AudioClip soundClip, float volume)
        {
            audioSourceSoundEffect.PlayOneShot(soundClip, volume);
        }

        /// <summary>
        /// Plays a sound effect with optional looping values, at the specified volume.
        /// </summary>
        /// <param name="soundClip">The sound effect clip to play.</param>
        /// <param name="loop">If the audioclip should loop or not.</param>
        /// <param name="volume">The volume level of the sound effect.</param>
        public virtual void PlayAmbianceSound(AudioClip soundClip, bool loop, float volume)
        {
            audioSourceAmbiance.loop = loop;
            audioSourceAmbiance.clip = soundClip;
            audioSourceAmbiance.volume = volume;
            audioSourceAmbiance.Play();
        }

        /// <summary>
        /// Shifts the game music pitch to required value over a period of time.
        /// </summary>
        /// <param name="pitch">The new music pitch value.</param>
        /// <param name="duration">The length of time in seconds needed to complete the pitch change.</param>
        /// <param name="onComplete">A delegate method to call when the pitch shift has completed.</param>
        public virtual void SetAudioPitch(float pitch, float duration, System.Action onComplete)
        {
            if (Mathf.Approximately(duration, 0f))
            {
                audioSourcesMusic[currentMusicIndex].pitch = pitch;
                audioSourceAmbiance.pitch = pitch;
                if (onComplete != null)
                {
                    onComplete();
                }
                return;
            }

            LeanTween.value(gameObject,
                audioSourcesMusic[currentMusicIndex].pitch,
                pitch,
                duration).setOnUpdate((p) =>
                {
                    audioSourcesMusic[currentMusicIndex].pitch = p;
                    audioSourceAmbiance.pitch = p;
                }).setOnComplete(() =>
                {
                    if (onComplete != null)
                    {
                        onComplete();
                    }
                });
        }

        /// <summary>
        /// Fades the game music volume to required level over a period of time.
        /// </summary>
        /// <param name="volume">The new music volume value [0..1]</param>
        /// <param name="duration">The length of time in seconds needed to complete the volume change.</param>
        /// <param name="onComplete">Delegate function to call when fade completes.</param>
        public virtual void SetAudioVolume(float volume, float duration, System.Action onComplete)
        {
            if (Mathf.Approximately(duration, 0f))
            {
                if (onComplete != null)
                {
                    onComplete();
                }
                audioSourcesMusic[currentMusicIndex].volume = volume;
                audioSourceAmbiance.volume = volume;
                return;
            }

            LeanTween.value(gameObject,
                audioSourcesMusic[currentMusicIndex].volume,
                volume,
                duration).setOnUpdate((v) => {
                    audioSourcesMusic[currentMusicIndex].volume = v;
                    audioSourceAmbiance.volume = v;
                }).setOnComplete(() => {
                    if (onComplete != null)
                    {
                        onComplete();
                    }
                });
        }

        /// <summary>
        /// Stops playing game music.
        /// </summary>
        public virtual void StopMusic()
        {
            audioSourcesMusic[currentMusicIndex].Stop();
            audioSourcesMusic[currentMusicIndex].clip = null;
        }

        /// <summary>
        /// Stops playing game ambiance.
        /// </summary>
        public virtual void StopAmbiance()
        {
            audioSourceAmbiance.Stop();
            audioSourceAmbiance.clip = null;
        }

        #endregion
    }
}