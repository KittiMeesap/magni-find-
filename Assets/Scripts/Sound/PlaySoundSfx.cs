using UnityEngine;

public class PlaySoundSfx : MonoBehaviour
{
    public static PlaySoundSfx Instance { get; private set; }

    public AudioClip SFX_Hand;
    public AudioClip SFX_Upsize;
    public AudioClip SFX_Smallsize;

    public int type;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //PlaySoundSfx.Instance.PlaySound(0);
    public void PlaySound(int Type)
    {
        type = Type;
        if (Type == 0) { SoundManager.Instance.PlaySFX(SFX_Hand); }
        else if(Type == 1) { SoundManager.Instance.PlaySFX(SFX_Upsize); }
        else if (Type == 2) { SoundManager.Instance.PlaySFX(SFX_Smallsize); }
    }
}
