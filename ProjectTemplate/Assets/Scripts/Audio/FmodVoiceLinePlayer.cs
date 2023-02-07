using System;
using FMODUnity;
using UnityEngine;

namespace GGJ23.Audio
{
    // TODO: Doing this a simple way because I don't have time to implement an audio table based approach
    //       This would scale better if we followed a specific file-naming pattern and injected the character names
    //       into an audio table key passed back into FMOD:
    //       https://www.fmod.com/docs/2.02/unity/examples-programmer-sounds.html
    public class FmodVoiceLinePlayer : MonoBehaviour
    {
        const string k_VoicePath = "event:/NPCNoises/Voices/";
        string m_HumanName;
        // IMPORTANT: This is very delicate and only works because the GameObject names match the names of the
        //            characters inside the FMOD events; if either changes this will break
        void Awake() => m_HumanName = transform.parent.name;

        void PlayVoiceLine(string lineName) =>
            RuntimeManager.PlayOneShotAttached($"{k_VoicePath}{m_HumanName}/{m_HumanName}_{lineName}", gameObject);

        public void PlayVoiceCurious() => PlayVoiceLine("Curious");
        public void PlayVoiceEat() => PlayVoiceLine("Eat");
        public void PlayVoiceScream() => PlayVoiceLine("Scream");
        public void PlayVoiceSleep() => PlayVoiceLine("Sleep");
        public void PlayVoiceToilet() => PlayVoiceLine("Toilet");
    }
}
