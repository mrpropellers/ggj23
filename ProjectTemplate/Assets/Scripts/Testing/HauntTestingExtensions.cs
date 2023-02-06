using System.Collections;
using UnityEngine;

namespace GGJ23.Testing
{
    public static class HauntTestingExtensions
    {
        public static void TestHaunt(this Hauntable self)
        {
            self.StartCoroutine(DoTestHaunt(self));
        }

        static IEnumerator DoTestHaunt(Hauntable self)
        {
            self.m_FramingCam.SetActive(true);
            if (self.m_FmodPlayer != null)
                self.m_FmodPlayer.PlayNextFmodCue();
            else if (self.TryGetComponent<FMODUnity.StudioEventEmitter>(out var emitter))
                emitter.Play();
            else if (self.TryGetComponent<KillAnimationHelper>(out var helper))
            {
                Debug.Log("Bed detected - god help us all.");
                yield break;
            }

            yield return new WaitForSeconds(Hauntable.k_HauntWindupTime);
            self.m_Animation.Play();
            if (self.m_ApplyScreenShake)
            {
                yield return new WaitForSeconds(Hauntable.k_ScreenShakeWaitTime);
            }

            yield return new WaitForSeconds(self.m_FramingCamHauntDuration);
            self.m_FramingCam.SetActive(false);
        }
    }

}
