using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace GGJ23.Audio
{
    public class FmodEventCueAdvancer : MonoBehaviour
    {
        EventInstance m_EventInstance;

        [SerializeField]
        EventReference m_FmodEvent;

        void Start()
        {
            m_EventInstance = RuntimeManager.CreateInstance(m_FmodEvent);
        }

        public void PlayNext()
        {
            var ret = m_EventInstance.getPlaybackState(out var playbackState);
            if (!this.CheckFmodResult("get playback state", ret))
                return;

            if (playbackState == PLAYBACK_STATE.STOPPED)
                this.CheckFmodResult("start event", m_EventInstance.start());
            else
                this.CheckFmodResult("key off next cue", m_EventInstance.keyOff());
        }
    }
}
