using FMOD;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GGJ23.Audio
{
    public static class FmodHelper
    {
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
    }
}
