using System;
using System.Collections;
using FMOD;
using FMODUnity;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GGJ23.Audio
{
    public static class FmodHelper
    {
        public const string PARAM_CONTINUE = "Continue";
        public const string PARAM_RADIO_MUTE = "RadioMute";
        public const string PARAM_IS_SNEAKING = "IsSneaking";
        // This adjusts the compressor to make space for foreground noises
        public const string PARAM_BG_ATTENUATE = "BgmAttenuation";
        // This straight up kills the music
        public const string PARAM_BGM_VOLUME = "BgmVolume";
        public const string PARAM_SFX_VOLUME = "SfxVolume";
        public const string PARAM_INGAME_BGM_VOLUME = "InGameBgmVolume";

        static float s_CurrentAttenuateTarget;

        static GameObject s_FmodManager;
        static StudioEventEmitter s_RootGrowthEmitter;

        static bool TryGetManager(out GameObject manager)
        {
            if (s_FmodManager == null)
            {
                var bankLoader = GameObject.FindObjectOfType<StudioBankLoader>();
                if (bankLoader == null)
                {
                    manager = null;
                    return false;
                }

                s_FmodManager = bankLoader.gameObject;
            }

            manager = s_FmodManager;
            return true;
        }

        static bool TryGetRootEventEmitter(out StudioEventEmitter emitter)
        {
            if (!TryGetManager(out var manager))
            {
                emitter = null;
                return false;
            }

            var hasEmitter = s_RootGrowthEmitter != null;
            if (!hasEmitter)
            {
                hasEmitter = manager.TryGetComponent(out s_RootGrowthEmitter);
            }

            emitter = s_RootGrowthEmitter;
            return hasEmitter;
        }

        public static bool CheckFmodResult(string thingAttempted, RESULT result, bool isError = true)
        {
            if (result == RESULT.OK)
                return true;

            var failMessage = $"Failed to {thingAttempted} -- result: {result}";
            if (isError)
                Debug.LogError(failMessage);
            else
                Debug.LogWarning(failMessage);
            return false;
        }

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

        public static void TurnOnInGameMusic() =>
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName(PARAM_INGAME_BGM_VOLUME, 1f);

        // 0 - no attenuation
        // 1 - max attenuation (NOTE: this doesn't fully mute, just makes it very quiet)
        public static IEnumerator AttenuateBgmTo(float target, float time)
        {
            if (target is < 0f or > 1f)
            {
                Debug.LogError($"Target {target} is not a valid attenuation target");
                yield break;
            }
            if (!CheckFmodResult($"get {PARAM_BG_ATTENUATE} value",
                    FMODUnity.RuntimeManager.StudioSystem.getParameterByName(PARAM_BG_ATTENUATE,
                        out var startingValue)))
                yield break;
            s_CurrentAttenuateTarget = target;
            var timeLeft = time;

            while (timeLeft > 0f)
            {
                yield return null;
                if (!Mathf.Approximately(target, s_CurrentAttenuateTarget))
                {
                    Debug.LogWarning($"Attenuation target changed from {target} to {s_CurrentAttenuateTarget} --" +
                        $"aborting this coroutine.");
                    yield break;
                }
                timeLeft -= Time.deltaTime;
                var t = Mathf.Clamp01(timeLeft / time);
                var val = Mathf.Lerp(target, startingValue, t);
                if (!CheckFmodResult($"set {PARAM_BG_ATTENUATE} to {val}",
                        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(PARAM_BG_ATTENUATE, val)))
                    yield break;
            }
        }

        public static void PlayRootGrowingSound()
        {
            if (!TryGetRootEventEmitter(out var emitter))
            {
                Debug.LogWarning("Couldn't find the emitter for the Root sound.");
                return;
            }

            emitter.Play();
        }

        public static void StopRootGrowingSound()
        {
            if (!TryGetRootEventEmitter(out var emitter))
            {
                Debug.LogWarning("Couldn't find the emitter for the Root sound.");
                return;
            }

            emitter.Stop();
        }
    }
}
