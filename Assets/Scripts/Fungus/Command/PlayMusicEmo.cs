using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Plays looping game music. If any game music is already playing, it is stopped. Game music will continue playing across scene loads.
    /// </summary>
    [CommandInfo("Audio",
                 "Play Music Emo",
                 "Plays looping game music (Intro + Loop). If any game music is already playing, it can either stop or blend. Game music will continue playing across scene loads.")]
    [AddComponentMenu("")]
    public class PlayMusicEmo : Command
    {
        [Tooltip("Music sound clip to play")]
        [SerializeField] protected AudioClip IntroClip;

        [Tooltip("Music sound clip to play")]
        [SerializeField] protected AudioClip LoopClip;

        [Tooltip("Time to begin playing in seconds. If the audio file is compressed, the time index may be inaccurate.")]
        [SerializeField] protected float atTime;

        [Tooltip("The music will start playing again at end.")]
        [SerializeField] protected bool loop = true;

        [Tooltip("Length of time to fade out previous playing music.")]
        [SerializeField] protected float fadeDuration = 1f;

        [Tooltip("Fade In while previous music is fading out to blend")]
        [SerializeField] protected bool bFadeIn = false;

        #region Public members

        public override void OnEnter()
        {
            var musicManager = FungusManager.Instance.MusicManager;

            float startTime = Mathf.Max(0, atTime);
            startTime = Mathf.Min(atTime, IntroClip.length);

            musicManager.PlayMusicEmo(IntroClip, LoopClip, loop, fadeDuration, startTime, bFadeIn);

            Continue();
        }

        public override string GetSummary()
        {
            if ((IntroClip == null) || (LoopClip == null))
            {
                return "Error: No music clip selected";
            }

            return LoopClip.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }
}