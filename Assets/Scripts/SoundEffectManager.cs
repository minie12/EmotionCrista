using Fungus;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SoundEffectName
{
    DefaultClickPositive, // 기본 클릭음 긍정 (팝업창 열 때 등)
    DefaultClickNegative, // 기본 클릭음 부정 (팝업창 닫을 때 등)
    DefaultBeep, // 경고음
    MiniRotateLeft, // 미니게임 젬 왼쪽 회전
    MiniRotateRight, // 미니게임 젬 오른쪽 회전
    MiniGemCrush, // 미니게임 젬 크러쉬
    MiniGemCrushFever, // 미니게임 젬 크러쉬 (피버)
    MiniGemClick, // 미니게임 젬 클릭
}

[System.Serializable]
public struct SoundEffectInfo
{
    public string name;
    public AudioClip audioClip;
}

public class SoundEffectManager : MonoBehaviour
{
    private static SoundEffectManager _instance; // 싱글톤 객체
    public bool soundEffectOn = true; // true 이면 효과음 재생 

    private AudioSource audioSource;

    // audio clip 들 초기화. 실제 audio clip은 inspector 창에서 적용
    [SerializeField]
    private List<SoundEffectInfo> soundEffectClips = System.Enum.GetValues(typeof(SoundEffectName))
                               .Cast<SoundEffectName>()
                               .Select(name => new SoundEffectInfo { name = name.ToString(), audioClip = null })
                               .ToList();

    private void Awake()
    {
        if (_instance == null)
        {
            audioSource = GetComponent<AudioSource>();

            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static SoundEffectManager Instance
    {
        get
        {
            if (_instance == null)
            {
                return null;
            }
            return _instance;
        }
    }

    public void Play(int audioClipNum)
    {
        if (!soundEffectOn || soundEffectClips[audioClipNum].audioClip == null)
        {
            return;
        }
        
        _instance.audioSource.PlayOneShot(soundEffectClips[audioClipNum].audioClip, SystemManager.Get().GetSFXVolume());
    }
}
