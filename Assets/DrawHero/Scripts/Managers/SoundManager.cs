using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private const string MUTE_SFX_KEY = "mute_sfx";
    private const string MUTE_MUSIC_KEY = "mute_music";

    private const int POOL_SIZE = 6;
    private List<AudioSource> sfxPool = new List<AudioSource>();
    private int poolIndex;

    private AudioSource musicSource;

    private Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

    public bool SfxMuted { get; private set; }
    public bool MusicMuted { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SfxMuted = PlayerPrefs.GetInt(MUTE_SFX_KEY, 0) == 1;
        MusicMuted = PlayerPrefs.GetInt(MUTE_MUSIC_KEY, 0) == 1;

        BuildPool();
        BuildClips();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void BuildPool()
    {
        for (int i = 0; i < POOL_SIZE; i++)
        {
            GameObject go = new GameObject("SFXSource_" + i);
            go.transform.SetParent(transform);
            AudioSource src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            sfxPool.Add(src);
        }

        GameObject musicGo = new GameObject("MusicSource");
        musicGo.transform.SetParent(transform);
        musicSource = musicGo.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
    }

    private void BuildClips()
    {
        clips["click"] = GenerateTone(660f, 0.08f, 0.4f);
        clips["back"] = GenerateTone(440f, 0.08f, 0.4f);
        clips["transition"] = GenerateTone(330f, 0.12f, 0.3f);
        clips["draw"] = GenerateTone(520f, 0.1f, 0.35f);
        clips["pop"] = GenerateTone(780f, 0.12f, 0.45f);
    }

    private AudioClip GenerateTone(float frequency, float duration, float volume)
    {
        int sampleRate = 44100;
        int sampleCount = (int)(sampleRate * duration);
        AudioClip clip = AudioClip.Create("tone_" + frequency, sampleCount, 1, sampleRate, false);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = Mathf.Clamp01(1f - (t / duration));
            samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * volume * envelope;
        }

        clip.SetData(samples, 0);
        return clip;
    }

    public void PlaySfx(string clipName, float pitch = 1f)
    {
        if (SfxMuted) return;

        if (!clips.ContainsKey(clipName))
        {
            Debug.LogWarning("SFX clip not found: " + clipName);
            return;
        }

        AudioSource src = sfxPool[poolIndex];
        poolIndex = (poolIndex + 1) % sfxPool.Count;

        src.clip = clips[clipName];
        src.pitch = pitch;
        src.Play();
    }

    public void ToggleSfxMute()
    {
        SfxMuted = !SfxMuted;
        PlayerPrefs.SetInt(MUTE_SFX_KEY, SfxMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleMusicMute()
    {
        MusicMuted = !MusicMuted;
        PlayerPrefs.SetInt(MUTE_MUSIC_KEY, MusicMuted ? 1 : 0);
        PlayerPrefs.Save();
        musicSource.mute = MusicMuted;
    }
}
