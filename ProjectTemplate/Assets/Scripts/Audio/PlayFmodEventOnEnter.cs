using FMODUnity;
using FMOD;
using UnityEngine;

namespace GGJ23.Audio
{
    public class PlayFmodEventOnEnter : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<StudioEventEmitter>().Play();
        }

    }
}
