using System;
using UnityEngine;
using Humans;

namespace GGJ23.Testing
{
    public class BedKillTester : MonoBehaviour
    {
        const string k_HumanKillTrigger = "bed_kill";

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
            m_KillHelper = GetComponentInChildren<KillAnimationHelper>();
        }

        void Update()
        {
            if (m_DoKill)
            {
                m_Target.transform.SetPositionAndRotation(m_KillSpot.position, m_KillSpot.rotation);
                m_KillHelper.TriggerKill();
                m_Target.Animator.SetTrigger(k_HumanKillTrigger);
                m_DoKill = false;
            }
        }
    }
}
