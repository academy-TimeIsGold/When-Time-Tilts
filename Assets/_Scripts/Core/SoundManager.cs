using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(SoundManager))]
public class SoundManager : MonoBehaviour
{
    //싱글톤
    public static SoundManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Initialize();
        }

        else Destroy(gameObject);
    }

    //사운드 옵션 창 ON/OFF
    [Header("Sound Panel(소리 설정창)")]
    public GameManager soundPanel;

    //볼륨 조절
    [Header("Audio Volume")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float bgmVolume = 0.7f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.7f;

    //오디오 소스 컴포넌트
    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    //해당 Scene 사운드 설정
    [Header("Audio Clips")]

    [Header("Title")]
    public AudioClip[] TITLE_BGM;
    public AudioClip[] TITLE_SFX;

    [Header("Intro")]
    public AudioClip[] INTRO_BGM;
    public AudioClip[] INTRO_SFX;

    [Header("Game")]
    public AudioClip[] GAME_BGM;
    public AudioClip[] GAME_SFX;

    [Header("UI")]
    public AudioClip[] UI_BGM;
    public AudioClip[] UI_SFX;    

    //사운드 클립 저장
    Dictionary<string, AudioClip[]> bgmDictionary = new Dictionary<string, AudioClip[]>();
    Dictionary<string, AudioClip[]> sfxDictionary = new Dictionary<string, AudioClip[]>();

    public void Initialize()
    {
        //해당 Scene BGM 생성
        AddBGM("Title", TITLE_BGM);
        AddBGM("Intro", INTRO_BGM);
        AddBGM("Game", GAME_BGM);       
        AddBGM("UI", UI_BGM);

        //해당 Scene SFX 생성
        AddSFX("Title", TITLE_SFX);
        AddSFX("Intro", INTRO_SFX);
        AddSFX("Game", GAME_SFX);
        AddSFX("UI", UI_SFX);

        //초기 볼륨 적용
        ApplyAllVolumes();
    }

    //BGM 재생
    public void PlayBGM(string name, int index)
    {
        if (!bgmDictionary.TryGetValue(name, out AudioClip[] clips)) return;
        if (clips == null) return;
        if (index < 0 || index >= clips.Length) return;

        //BGM 재생
        bgmSource.clip = clips[index];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    //BGM 정지
    public void StopBGM()
    {
        if(bgmSource == null) return;

        //BGM 재생 중이라면 정지
        if(bgmSource != null && bgmSource.isPlaying) bgmSource.Stop();
    }

    //BGM 재생 여부 확인
    public bool isPlaying()
    {
        if (bgmSource == null) return false;

        //BGM이 재생 중이면 true 반환
        if (bgmSource.isPlaying) return true;
        else return false;
    }

    //SFX 재생
    public void PlaySFX(string name, int index)
    {
        if (!sfxDictionary.TryGetValue(name, out AudioClip[] clips)) return;
        if (clips == null) return;
        if (index < 0 || index >= clips.Length) return;

        //SFX 재생
        sfxSource.PlayOneShot(clips[index]);
    }

    //BGM 추가
    void AddBGM(string name, AudioClip[] clips)
    {
        if (bgmDictionary.ContainsKey(name)) bgmDictionary[name] = clips;
        else bgmDictionary.Add(name, clips);
    }

    //SFX 추가
    void AddSFX(string name, AudioClip[] clips)
    {
        if (sfxDictionary.ContainsKey(name)) sfxDictionary[name] = clips;
        else sfxDictionary.Add(name, clips);
    }

    //마스터 볼륨 조절
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyAllVolumes();
    }

    //배경음악 볼륨 조절
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        ApplyAllVolumes();
    }

    //효과음 볼륨 조절
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        ApplyAllVolumes();
    }

    //모든 오디오 파일 볼륨 설정
    private void ApplyAllVolumes()
    {
        //BGM 볼륨
        if (bgmSource != null) bgmSource.volume = masterVolume * bgmSource.volume;
        if (sfxSource != null) sfxSource.volume = masterVolume * sfxSource.volume;
    }
}
