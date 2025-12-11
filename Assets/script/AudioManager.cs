using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#BGM")]
    public AudioClip[] bgmClip;
    public float bgmVolume = 1f;
    public int channelBGM = 1;
    AudioSource[] bgmPlayer;
    int channelBGMIndex;
    public enum Bgm { Title, YRoom, RRoom, Boss, GameOver, GameClear }

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume = 1f;
    public int channelSFX = 5;
    AudioSource[] sfxPlayer;
    int channelSFXIndex;

    public enum Sfx { Blue, Attack, Skill, Jump, button, Potal, Red, Black, Normal, Mask, EnemyHit }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            LoadSetting();   // 1) 저장된 설정 불러오기
            Init();          // 2) AudioSource 생성
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // -------------------------
    // 🔹 설정 불러오기
    // -------------------------
    public void LoadSetting()
    {
        if (PlayerPrefs.HasKey("BGMVolume"))
            bgmVolume = PlayerPrefs.GetFloat("BGMVolume");

        if (PlayerPrefs.HasKey("SFXVolume"))
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
    }

    // -------------------------
    // 🔹 설정 저장하기
    // -------------------------
    public void SaveSetting()
    {
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    // -------------------------
    // 🔹 AudioSource 초기화
    // -------------------------
    void Init()
    {
        // BGM
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = new AudioSource[channelBGM];

        for (int i = 0; i < channelBGM; i++)
        {
            bgmPlayer[i] = bgmObject.AddComponent<AudioSource>();
            bgmPlayer[i].playOnAwake = false;
            bgmPlayer[i].loop = true;
            bgmPlayer[i].volume = bgmVolume;
        }

        // SFX
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayer = new AudioSource[channelSFX];

        for (int i = 0; i < channelSFX; i++)
        {
            sfxPlayer[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayer[i].playOnAwake = false;
            sfxPlayer[i].volume = sfxVolume;
        }
    }

    // -------------------------
    // 🔹 효과음 재생
    // -------------------------
    public void PlaySfx(Sfx sfx)
    {
        if (sfxPlayer == null || sfxClips == null)
            return;

        AudioClip clip = sfxClips[(int)sfx];
        if (clip == null)
            return;

        for (int i = 0; i < sfxPlayer.Length; i++)
        {
            int index = (i + channelSFXIndex) % sfxPlayer.Length;

            if (!sfxPlayer[index].isPlaying)
            {
                channelSFXIndex = index;
                sfxPlayer[index].clip = clip;
                sfxPlayer[index].Play();
                break;
            }
        }
    }

    // -------------------------
    // 🔹 배경음 재생
    // -------------------------
    public void PlayBgm(Bgm bgm)
    {
        if (bgmPlayer == null)
            return;

        foreach (var player in bgmPlayer)
        {
            player.Stop();
            player.clip = null;
        }

        bgmPlayer[0].clip = bgmClip[(int)bgm];
        bgmPlayer[0].volume = bgmVolume;

        // 클리어 BGM은 1번만 재생
        bgmPlayer[0].loop = (bgm != Bgm.GameClear);
        bgmPlayer[0].Play();

        channelBGMIndex = 0;
    }

    // -------------------------
    // 🔹 볼륨 적용 함수
    // -------------------------
    public void SetBgmVolume(float volume)
    {
        bgmVolume = volume;
        foreach (var p in bgmPlayer)
            p.volume = volume;

        SaveSetting();
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = volume;
        foreach (var p in sfxPlayer)
            p.volume = volume;

        SaveSetting();
    }
}
