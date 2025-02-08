using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource; // ✅ สำหรับเล่น BGM
    [SerializeField] private AudioSource sfxSource; // ✅ สำหรับเล่น SFX (Effect)

    private float bgmVolume = 1f;
    private float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ✅ ทำให้ SoundManager คงอยู่ทุก Scene
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadVolumeSettings(); // ✅ โหลดค่าการตั้งค่าเสียงที่บันทึกไว้
    }

    public void PlayBGM(AudioClip bgmClip = null)
    {
        if (bgmClip == null) bgmClip = GameController.Instance.DefaultBGM;
        if (bgmSource.clip == bgmClip && bgmSource.isPlaying) return; // ✅ ไม่เล่นซ้ำถ้าเล่นอยู่แล้ว

        bgmSource.clip = bgmClip;
        bgmSource.loop = true; // ✅ ให้เล่นวนซ้ำไปเรื่อยๆ
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
    }

    public void PauseBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Pause(); // ✅ หยุดเพลงชั่วคราว
        }
    }

    public void ResumeBGM()
    {
        if (!bgmSource.isPlaying)
        {
            bgmSource.UnPause(); // ✅ เล่นต่อจากที่หยุดไว้
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop(); // ✅ หยุดและรีเซ็ตไปตำแหน่งเริ่มต้น
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume); // ✅ เล่น Effect เสียงแบบซ้อนกันได้
        }
    }

    public bool IsPlayingSFX(AudioClip clip)
    {
        return sfxSource.isPlaying && sfxSource.clip == clip;
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public float GetBGMVolume() => bgmVolume;
    public float GetSFXVolume() => sfxVolume;

    private void LoadVolumeSettings()
    {
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        bgmSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;
    }
}
