using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSettingUI : MonoBehaviour
{
    [Header("BGM")]
    public Slider bgmSlider;
    public TMP_InputField bgmInput;

    [Header("SFX")]
    public Slider sfxSlider;
    public TMP_InputField sfxInput;

    void Start()
    {
        if (AudioManager.instance != null)
        {
            // 초기값
            bgmSlider.value = AudioManager.instance.bgmVolume;
            sfxSlider.value = AudioManager.instance.sfxVolume;

            bgmInput.text = Mathf.RoundToInt(AudioManager.instance.bgmVolume * 100).ToString();
            sfxInput.text = Mathf.RoundToInt(AudioManager.instance.sfxVolume * 100).ToString();
        }

        // 슬라이더 > 숫자
        bgmSlider.onValueChanged.AddListener(OnBgmSlider);
        sfxSlider.onValueChanged.AddListener(OnSfxSlider);

        // 숫자> 슬라이더
        bgmInput.onEndEdit.AddListener(OnBgmInput);
        sfxInput.onEndEdit.AddListener(OnSfxInput);
    }

    void OnBgmSlider(float value)
    {
        AudioManager.instance.SetBgmVolume(value);
        bgmInput.text = Mathf.RoundToInt(value * 100).ToString("00");
    }

    void OnBgmInput(string text)
    {
        if (!int.TryParse(text, out int value))
            value = 0;

        value = Mathf.Clamp(value, 0, 100);

        float volume = value / 100f;
        bgmSlider.value = volume;
        AudioManager.instance.SetBgmVolume(volume);

        bgmInput.text = value.ToString("00");
    }

    void OnSfxSlider(float value)
    {
        AudioManager.instance.SetSfxVolume(value);
        sfxInput.text = Mathf.RoundToInt(value * 100).ToString("00");
    }

    void OnSfxInput(string text)
    {
        if (!int.TryParse(text, out int value))
            value = 0;

        value = Mathf.Clamp(value, 0, 100);

        float volume = value / 100f;
        sfxSlider.value = volume;
        AudioManager.instance.SetSfxVolume(volume);

        sfxInput.text = value.ToString("00");
    }
}
