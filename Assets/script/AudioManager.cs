using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private float[] sfxLastPlayTime = new float[20];
    public float sfxMinInterval = 0.1f; 

    
    [Header("#BGM")]
    public AudioClip[] bgmClip;
    public float bgmVolume;
    public int channelBGM;
    AudioSource[] bgmPlayer;
    int channelBGMIndex;
    public enum Bgm {Title, YRoom, RRoom, Boss, GameOver, GameClear}
    

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channelSFX;
    AudioSource[] sfxPlayer;
    int channelSFXIndex;

    public enum Sfx {Blue, Attack, Skill, Jump, button, Potal, 
    Red, Black, Normal, Mask, EnemyHit}

    void Awake()
    {
        if(instance == null)
        {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        Init();
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Init()
    {
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = new AudioSource[channelBGM];

        for(int index=0; index < bgmPlayer.Length; index++)
        {
        bgmPlayer[index] = bgmObject.AddComponent<AudioSource>();
        bgmPlayer[index].playOnAwake = false;
        bgmPlayer[index].loop = true;
        bgmPlayer[index].volume = bgmVolume;
        //bgmPlayer[index].clip = bgmClip;
        }

        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayer = new AudioSource[channelSFX];

        for(int index=0; index < sfxPlayer.Length; index++)
        {
            sfxPlayer[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayer[index].playOnAwake = false;
            sfxPlayer[index].volume = sfxVolume;

            
        }

    }

    public void PlaySfx(Sfx sfx) 
    {
        for(int index=0; index < sfxPlayer.Length; index++) 
        { 
        
        int loopIndexSfx = (index + channelSFXIndex) % sfxPlayer.Length;
          
        if(sfxPlayer[loopIndexSfx].isPlaying)
        continue;

        channelSFXIndex = loopIndexSfx;
        sfxPlayer[loopIndexSfx].clip = sfxClips[(int)sfx];
        sfxPlayer[loopIndexSfx].Play(); 
        break;

        } 
    }

     public void PlayBgm(Bgm bgm) 
{
    for (int i = 0; i < bgmPlayer.Length; i++)
    {
        bgmPlayer[i].Stop();
        bgmPlayer[i].clip = null;
    }

   
    bgmPlayer[0].clip = bgmClip[(int)bgm];
    bgmPlayer[0].volume = bgmVolume;
    bgmPlayer[0].loop = true;
    bgmPlayer[0].Play();

    channelBGMIndex = 0; 
}
    
    


}
