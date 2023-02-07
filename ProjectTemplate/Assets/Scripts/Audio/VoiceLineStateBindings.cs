using UnityEngine;

namespace GGJ23.Audio
{
    public class VoiceLineStateBindings : StateMachineBehaviour
    {
        const string k_EatStateName = "eat";
        const string k_JumpStateName = "jump";
        const string k_RunStateName = "run";
        const string k_SleepStateName = "sleep_chill";
        const string k_ToiletStateName = "toilet_chill";

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!animator.TryGetComponent<FmodVoiceLinePlayer>(out var player))
            {
                Debug.LogError($"No {nameof(FmodVoiceLinePlayer)} on {animator.name}; can't play voice lines!",
                    this);
                return;
            }

            if (stateInfo.IsName(k_EatStateName))
                player.PlayVoiceEat();
            else if (stateInfo.IsName(k_RunStateName))
                player.PlayVoiceScream();
            else if (stateInfo.IsName(k_JumpStateName))
                player.PlayVoiceCurious();
            else if (stateInfo.IsName(k_SleepStateName))
                player.PlayVoiceSleep();
            else if (stateInfo.IsName(k_ToiletStateName))
                player.PlayVoiceToilet();
            else
                Debug.LogError($"No handling for this state: {stateInfo}");

        }

        // TODO: We could set some of these voice lines to loop and then use the OnStateExit to kill the loop,
        //       would need to add more logic to the FmodVoiceLinePlayer as well
    }
}
