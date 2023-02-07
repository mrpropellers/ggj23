using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace GGJ23.Audio
{
    public class FmodEventCueAdvancer : MonoBehaviour
    {
        EventInstance m_EventInstance;
        bool m_IsBroken;

        [SerializeField]
        bool m_UseContinueParameter;
        [SerializeField]
        Transform m_AttenuationObject;
        [SerializeField]
        EventReference m_FmodEvent;

        void Start()
        {
            m_EventInstance = RuntimeManager.CreateInstance(m_FmodEvent);
            RuntimeManager.AttachInstanceToGameObject(m_EventInstance,
                m_AttenuationObject == null ? transform : m_AttenuationObject);
        }

        public void PlayNextFmodCue()
        {
            if (m_IsBroken)
                return;
            var ret = m_EventInstance.getPlaybackState(out var playbackState);
            if (!this.CheckFmodResult("get playback state", ret))
                return;

            if (playbackState == PLAYBACK_STATE.STOPPED)
            {
                if (m_UseContinueParameter)
                {
                    this.CheckFmodResult("set continue parameter",
                        m_EventInstance.setParameterByName(FmodHelper.PARAM_CONTINUE, 0f));
                }

                this.CheckFmodResult("start event", m_EventInstance.start());
            }
            else if (m_UseContinueParameter)
            {
                if (!this.CheckFmodResult("get continue value",
                        m_EventInstance.getParameterByName(FmodHelper.PARAM_CONTINUE, out var value)))
                    return;
                value = 1f - value;

                this.CheckFmodResult($"set Continue value to {value}",
                    m_EventInstance.setParameterByName(FmodHelper.PARAM_CONTINUE, value));
            }
            else
                this.CheckFmodResult("key off next cue", m_EventInstance.keyOff());
        }
    }
}
