using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [System.Serializable]
    public class SceneMusic
    {
        public string sceneName;
        public AudioClip musicClip;
        public bool loop = true;
        [Range(0f, 1f)] public float volume = 0.7f;
    }

    public SceneMusic[] sceneMusicSettings;
    public AudioSource audioSource;

    private string currentSceneName;

    void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize audio source if not set
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;
        UpdateMusicForCurrentScene();
    }

    void UpdateMusicForCurrentScene()
    {
        // Find music settings for current scene
        SceneMusic settings = null;
        foreach (var sm in sceneMusicSettings)
        {
            if (sm.sceneName == currentSceneName)
            {
                settings = sm;
                break;
            }
        }

        // Stop music if no settings found for this scene
        if (settings == null)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            return;
        }

        // Change music if needed
        if (audioSource.clip != settings.musicClip || !audioSource.isPlaying)
        {
            audioSource.clip = settings.musicClip;
            audioSource.loop = settings.loop;
            audioSource.volume = settings.volume;
            audioSource.Play();
        }
    }

    // Optional: Public method to manually change music
    public void ChangeMusic(AudioClip newClip, bool loop = true, float volume = 0.7f)
    {
        audioSource.clip = newClip;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.Play();
    }

    // Optional: Public method to stop music
    public void StopMusic()
    {
        audioSource.Stop();
    }
}