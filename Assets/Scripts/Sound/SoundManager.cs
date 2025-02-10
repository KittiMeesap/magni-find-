using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    private string lastSceneName;
    private HashSet<string> continuousScenes = new HashSet<string> { "Win", "Credit" }; // ตั้งค่าชื่อ Scene ที่ต้องเล่นเพลงต่อกัน


    [Header("Audio Sources")]
    [SerializeField] private AudioSource mainBGMSource; // ✅ สำหรับเล่น BGM
    [SerializeField] private AudioSource sfxSource; // ✅ สำหรับเล่น SFX (Effect)
    [SerializeField] private AudioSource minigameBGMSource;


    [Header("Audio Clip")]
    [SerializeField] private AudioClip mainmenuBGM;
    [SerializeField] private AudioClip gameplayBGM;
    [SerializeField] private AudioClip storyBGM;
    [SerializeField] private AudioClip dialogueBGM;
    [SerializeField] private AudioClip creditBGM;
    [SerializeField] private AudioClip defaultBGM;
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
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // ✅ ฟัง event เมื่อเปลี่ยน Scene
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadVolumeSettings();
        //PlayBGM(defaultBGM);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (continuousScenes.Contains(scene.name) && continuousScenes.Contains(lastSceneName))
        {
            // ✅ ถ้า Scene ใหม่และ Scene ก่อนหน้าอยู่ในกลุ่มที่เล่นต่อกัน ให้ Resume BGM
            ResumeBGM();
        }
        else
        {
            // ✅ เปลี่ยนเพลงใหม่สำหรับ Scene อื่น ๆ
            PlayBGM(GetBGMForScene(scene.name));
        }

        lastSceneName = scene.name; // ✅ อัปเดต Scene ล่าสุด
    }

    private AudioClip GetBGMForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Tutorial": return mainmenuBGM;
            case "Gameplay": return gameplayBGM;
            case "Win": return creditBGM;
            case "Credit": return creditBGM;
            case "Story Scene": return storyBGM;
            case "Dialogue": return dialogueBGM;
            default: return defaultBGM;
        }
    }



    public void PlayBGM(AudioClip bgmClip = null)
    {
        if (bgmClip == null) bgmClip = defaultBGM;
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
        minigameBGMSource.pitch = Mathf.Clamp(volume, 0f, 1f);
    }
}
