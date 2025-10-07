using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlipSFX : MonoBehaviour
{
    [SerializeField] private AudioClip onClickClip;
    [SerializeField] private AudioClip onHoverClip;

    private void Awake()
    {
        Button button = GetComponentInParent<Button>();
        if (button != null)
        {
            Assert.IsNotNull(onClickClip);
            Assert.IsNotNull(onHoverClip);
            button.onClick.AddListener(PlayOnClick);
            if (button.TryGetComponent(out OnHover handler))
                handler.onHover.AddListener(PlayOnHover);
        }
    }

    private void PlayOnClick()
    {
        Play(onClickClip);
    }

    private void PlayOnHover()
    {
        Play(onHoverClip);
    }

    private void Play(AudioClip clip)
    {
        GameObject sfxObject = new("BlipSFX");
        DontDestroyOnLoad(sfxObject);
        AudioSource source = sfxObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();
        Destroy(sfxObject, clip.length);
    }
}
