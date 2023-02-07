using System.Collections;
using System.Collections.Generic;
using FMOD;
using GGJ23.Audio;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class RadioActivator : MonoBehaviour
{
    [SerializeField]
    FMODUnity.StudioEventEmitter m_SwitchEmitter;

    public void ActivateRadio()
    {
        Debug.Log("Activating all radios in scene.");
        m_SwitchEmitter.Play();
        this.CheckFmodResult("un-mute radio",
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName(FmodHelper.PARAM_RADIO_MUTE, 0f));
    }
}
