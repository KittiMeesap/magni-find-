using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource mainBGMSource; // ✅ สำหรับเล่น BGM
    [SerializeField] private AudioSource sfxSource; // ✅ สำหรับเล่น SFX (Effect)
    [SerializeField] private AudioSource minigameBGMSource;


    [Header("Audio Clip")]
    public AudioClip sfx_Hand;
    public AudioClip sfx_Upsize;
    public AudioClip sfx_Smallsize;

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
        if (mainBGMSource.clip == bgmClip && mainBGMSource.isPlaying) return; // ✅ ไม่เล่นซ้ำถ้าเล่นอยู่แล้ว

        mainBGMSource.clip = bgmClip;
        mainBGMSource.loop = true; // ✅ ให้เล่นวนซ้ำไปเรื่อยๆ
        mainBGMSource.volume = bgmVolume;
        mainBGMSource.Play();
    }

    public void PauseBGM()
    {
        if (mainBGMSource.isPlaying)
        {
            mainBGMSource.Pause(); // ✅ หยุดเพลงชั่วคราว
        }
    }

    public void ResumeBGM()
    {
        if (!mainBGMSource.isPlaying)
        {
            mainBGMSource.UnPause(); // ✅ เล่นต่อจากที่หยุดไว้
        }
    }

    public void StopBGM()
    {
        mainBGMSource.Stop(); // ✅ หยุดและรีเซ็ตไปตำแหน่งเริ่มต้น
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
        mainBGMSource.volume = bgmVolume;
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
        mainBGMSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;
    }

    public void PlayMinigameBGM(AudioClip minigameClip ,float volumeMultiplier = 1f)
    {
        if (mainBGMSource.isPlaying)
        {
            bgmVolume = mainBGMSource.volume;
            mainBGMSource.Pause();
        }

        minigameBGMSource.clip = minigameClip;
        minigameBGMSource.volume = volumeMultiplier; // เริ่มต้นที่เสียงดังสุด
        minigameBGMSource.loop = true;
        minigameBGMSource.Play();
    }

    public void StopMinigameBGM()
    {
        minigameBGMSource.Stop();
        if (mainBGMSource.clip != null)
        {
            mainBGMSource.volume = bgmVolume;
            mainBGMSource.UnPause();
        }
    }

    public void SetMinigameVolume(float volume)
    {
        minigameBGMSource.volume = Mathf.Clamp(volume, 0f, 1f);
    }
}
