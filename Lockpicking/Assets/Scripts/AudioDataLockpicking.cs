using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AudioDataLockipicking")]
public class AudioDataLockpicking : ScriptableObject {
    [SerializeField]
    List<AudioClip> intro;
    [SerializeField]
    List<AudioClip> tumblerChange;

    public AudioClip GetAudioClip(int type, int index, bool rand = false) {
        List<AudioClip> requestedContainer;

        switch(type) {
            case 0:
                requestedContainer = intro;
                break;
            default:
                requestedContainer = tumblerChange;
                break;
        }

        if (!rand) {
            if (index < requestedContainer.Count) {
                return requestedContainer[index];
            } else {
                return null;
            }
        } else {
            return requestedContainer[UnityEngine.Random.Range(0, requestedContainer.Count)];
        }
    }

}
