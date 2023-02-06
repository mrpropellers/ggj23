using System;
using FMOD;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GGJ23.Audio
{
    public static class FmodHelper
    {
        public const string PARAM_CONTINUE = "Continue";
        public const string PARAM_RADIO_MUTE = "RadioMute";
        public const string PARAM_BG_ATTENUATE = "BgmAttenuation";
        public const string PARAM_IS_SNEAKING = "IsSneaking";
        public static bool CheckFmodResult(
            this Component owner, string thingAttempted, RESULT result, bool isError = true)
        {
            if (result == RESULT.OK)
                return true;

            var failMessage = $"{owner.name} failed to {thingAttempted} -- result: {result}";
            if (isError)
                Debug.LogError(failMessage, owner);
            else
                Debug.LogWarning(failMessage, owner);
            return false;
        }

        public static void SetPlayerIsSneaking(bool isSneaking)
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName(PARAM_IS_SNEAKING, isSneaking ? 1f : 0f);
        }

        // 0 - no attenuation
        // 1 - max attenuation (NOTE: this doesn't fully mute, just makes it very quiet)
        public static void SetBgmAttenuation(float ratio)
        {
            if (ratio is < 0f or > 1f)
            {
                Debug.LogError($"{ratio} is not an acceptable value; must be between 0-1");
                return;
            }

            FMODUnity.RuntimeManager.StudioSystem.setParameterByName(PARAM_BG_ATTENUATE, ratio);
        }
    }
}
