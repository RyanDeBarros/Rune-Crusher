using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class BKGMusic : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private float fullVolume = 1f;
    [SerializeField] private float dimmedVolume = 0.5f;
    [SerializeField] private float fadeDuration = 0.5f;

    private AudioSource audioSource;

    private static BKGMusic instance;
    private Coroutine fadeCoroutine;

    private int playbackPosition = 0;
    private bool atFullVolume = true;

    public static BKGMusic Instance
    {
        get
        {
            Assert.IsNotNull(instance);
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        Assert.IsNotNull(clip);
        DontDestroyOnLoad(gameObject);
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
        OnLoopRestart();
    }

    private void Update()
    {
        int currentPlaybackPosition = audioSource.timeSamples;
        if (currentPlaybackPosition < playbackPosition)
            OnLoopRestart();
        playbackPosition = currentPlaybackPosition;
    }

    public void PlayAtFullVolume()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeVolume(fullVolume));
        atFullVolume = true;
    }

    public void PlayAtDimmedVolume()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeVolume(dimmedVolume));
        atFullVolume = false;
    }

    private void OnLoopRestart()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        audioSource.volume = 0f;
        fadeCoroutine = StartCoroutine(FadeVolume(atFullVolume ? fullVolume : dimmedVolume));
    }

    private IEnumerator FadeVolume(float target)
    {
        float start = audioSource.volume;
        if (start == target)
            yield break;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, target, Mathf.Clamp01(elapsed / fadeDuration));
            yield return null;
        }
        audioSource.volume = target;
    }
}
