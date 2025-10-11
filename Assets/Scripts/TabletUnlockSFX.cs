using UnityEngine;
using UnityEngine.Assertions;

public class TabletUnlockSFX : MonoBehaviour
{
    [SerializeField] private AudioClip unlockSFXClip;
    [SerializeField] private float replayBuffer = 0.5f;

    private AudioSource unlockSFX;
    private float cooldown = 0f;

    private void Awake()
    {
        Assert.IsNotNull(unlockSFXClip);
        unlockSFX = gameObject.AddComponent<AudioSource>();
        unlockSFX.clip = unlockSFXClip;
        unlockSFX.playOnAwake = false;
    }

    private void Update()
    {
        if (cooldown > 0f)
            cooldown -= Time.deltaTime;
    }

    public void Play()
    {
        if (cooldown <= 0f)
        {
            unlockSFX.Play();
            cooldown = replayBuffer;
        }
    }
}
