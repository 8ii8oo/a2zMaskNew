using UnityEngine;
using UnityEngine.UI;

public class AudioSettingUI : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        if (AudioManager.instance != null)
        {
            bgmSlider.value = AudioManager.instance.bgmVolume;
            sfxSlider.value = AudioManager.instance.sfxVolume;
        }

        bgmSlider.onValueChanged.AddListener(SetBgmVolume);
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);
    }

    void SetBgmVolume(float value)
    {
        AudioManager.instance.SetBgmVolume(value);
    }

    void SetSfxVolume(float value)
    {
        AudioManager.instance.SetSfxVolume(value);
    }
}
