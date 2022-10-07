using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AudioDataLockipicking")]
public class AudioDataLockpicking : ScriptableObject {
    // [x] - sound type, present as argument in function GetAudioClip
    [SerializeField] // [1] lockpicking start sounds (insert tools into lock)
    private List<AudioClip> intro;
    [SerializeField] // [2]
    private List<AudioClip> unlock;
    [SerializeField] // [3]
    private List<AudioClip> tumblerChange;
    [SerializeField] // [4] pin pushing sounds
    private List<AudioClip> pinMove;
    [SerializeField] // [5]
    private List<AudioClip> pinReset;
    [SerializeField] // [6]
    private List<AudioClip> pinSet;
    [SerializeField] // [7]
    private List<AudioClip> wrenchPressure;
    [SerializeField] // [8]
    private List<AudioClip> wrenchRelease;

    /// <summary>
    /// Returns an audio clip from the list of clips of type 'type'. If 'rand' is
    /// set to true, a random clip is selected, otherwise the value of the parameter
    /// 'index' will be used to select a clip.
    /// </summary>
    /// <param name="type"> Type of audio clip: 1 - lockpick start, 2 - unlock
    /// 3- tumbler change, 4 - pin move, 5 - pin reset, 6 - pin set, 7 - wrench pressure,
    /// 8 - wrench release.</param>
    /// <param name="index"> Index of audio clip. </param>
    /// <param name="rand"> Select random clip if set to true. </param>
    /// <returns></returns>
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
