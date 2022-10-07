using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager instance;

    private AudioSource audioSource;

    [SerializeField]
    private AudioDataLockpicking lockpickingAudio;

    [SerializeField]
    private float introClipsDelay;
    private WaitForSeconds waitForIntroDelay;

    [Tooltip("Chance to play audio on action.")]
    [SerializeField]
    private float audioPlayFrequency; // [0, 100]

    public AudioManager() {
        if (AudioManager.instance != null && AudioManager.instance != this) {
            Destroy(this);
        } else {
            AudioManager.instance = this;
        }
    }

    public void Awake() {
        audioSource = this.gameObject.GetComponent<AudioSource>();

        waitForIntroDelay = new WaitForSeconds(introClipsDelay);
    }


    public void PlayIntroAudio() {
        StartCoroutine(IntroCoroutine());
    }

    // [ TODO ] This may be unnecessary
    public void PlayTumblerChangeAudio() {
        float chanceToPlay = UnityEngine.Random.Range(0, 100);
        if (chanceToPlay < audioPlayFrequency) {
            audioSource.Stop();
            audioSource.PlayOneShot(lockpickingAudio.GetAudioClip(1, 0, true));
        }
    }

    private IEnumerator IntroCoroutine() {
        yield return waitForIntroDelay;
        audioSource.PlayOneShot(lockpickingAudio.GetAudioClip(0, 0));
        audioSource.PlayOneShot(lockpickingAudio.GetAudioClip(0, 1));
    }
}
