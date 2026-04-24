using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip ambientMusic;

    [Header("SFX")]
    public AudioClip footstepSound;
    public AudioClip runSound;
    public AudioClip powerUpCollect;
    public AudioClip powerUpEnd;
    public AudioClip collectibleSound;

    [Header("Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 0.7f;

    private float footstepTimer = 0f;
    private float footstepInterval = 0.5f; // caminar cada 0.5 segundos
    private float runInterval = 0.3f;       // correr cada 0.3 segundos
    private float lastRunSoundTime = 0f;
    private float lastFootstepTime = 0f;
    public bool groundedRightNow = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (musicSource != null) musicSource.volume = musicVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {        
        if (scene.name == "Login" || scene.name == "Scores" || scene.name == "Menu")
        {
            PlayMenuMusic();
        }
        else if (scene.name == "Game" || scene.name == "Level1" || scene.name == "Juego")
        {
            PlayAmbientMusic();
        }
    }

    
    public void PlayMenuMusic()
    {
        if (musicSource != null && menuMusic != null && musicSource.clip != menuMusic)
        {
            musicSource.clip = menuMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayAmbientMusic()
    {
        if (musicSource != null && ambientMusic != null && musicSource.clip != ambientMusic)
        {
            musicSource.clip = ambientMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void TryPlayFootstep(bool isMoving, bool isRunning)
    {
        if (!isMoving || !groundedRightNow) return;

        float stepInterval = isRunning ? 0.3f : 0.5f;

        if (Time.time >= lastFootstepTime + stepInterval)
        {
            lastFootstepTime = Time.time;

            if (isRunning && runSound != null)
            {
                sfxSource.PlayOneShot(runSound, sfxVolume);
                Debug.Log("SONIDO CORRER");
            }
            else if (footstepSound != null)
            {
                sfxSource.PlayOneShot(footstepSound, sfxVolume);
                Debug.Log("SONIDO PASO");
            }
        }
    }

    public void PlayPowerUpCollect()
    {
        if (powerUpCollect != null)
            sfxSource.PlayOneShot(powerUpCollect, sfxVolume);
    }

    public void PlayPowerUpEnd()
    {
        if (powerUpEnd != null)
            sfxSource.PlayOneShot(powerUpEnd, sfxVolume);
    }

    public void PlayCollectible()
    {
        if (collectibleSound != null)
            sfxSource.PlayOneShot(collectibleSound, sfxVolume);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}