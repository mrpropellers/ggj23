using UnityEngine;
using Humans;

namespace GGJ23.Testing
{
    public class ToiletKillTester : MonoBehaviour
    {
        const string k_HumanKillTrigger = "toilet_kill";

        [SerializeField]
        KillAnimationHelper m_KillHelper;

        [SerializeField]
        bool m_DoKill;

        [SerializeField]
        Human m_Target;
        [SerializeField]
        Transform m_KillSpot;

        void Start()
        {
            m_DoKill = false;
        }

        void Update()
        {
            if (m_DoKill)
            {
                m_KillHelper.TriggerKill();
                m_Target.transform.SetPositionAndRotation(m_KillSpot.position, m_KillSpot.rotation);
                m_Target.Animator.SetTrigger(k_HumanKillTrigger);
                m_DoKill = false;
            }
        }
    }
}
