using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Humans;
using Unity.VisualScripting;

namespace GGJ23.Testing
{
    [DefaultExecutionOrder(int.MinValue)]
    public class HumanAnimationTester : MonoBehaviour
    {
        [Serializable]
        class TestableState
        {
            [SerializeField]
            internal bool TestMe;

            public bool Eat;
            public bool Toilet;
            public bool Sleep;
            public bool Running;
        }

        [Serializable]
        struct TestableHumans
        {
            [SerializeField]
            internal Animator HumanAnimator;

            [SerializeField]
            internal TestableState TestableState;
        }

        [SerializeField]
        List<TestableHumans> m_HumansToTest;

        void OnValidate()
        {
            if (!m_HumansToTest.Any(h => h.TestableState.TestMe))
            {
                return;
            }

            var target = m_HumansToTest.First(h => h.TestableState.TestMe);
            target.TestableState.TestMe = false;
            var animator = target.HumanAnimator;
            var state = target.TestableState;
            animator.SetBool("eat", state.Eat);
            animator.SetBool("toilet", state.Toilet);
            animator.SetBool("sleep", state.Sleep);
            animator.SetBool("running", state.Running);
        }

        void Awake()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Awake called while not playing -- doing nothing " +
                    "(this could be a problem if domain reload is off.)");
                return;
            }
            PopulateList();
        }

        void PopulateList()
        {
            m_HumansToTest = new List<TestableHumans>(3);

            GetComponentInChildren<HumanManager>().enabled = false;
            var humans = GetComponentsInChildren<Humans.Human>();
            foreach (var human in humans)
            {
                var animator = human.GetComponent<Animator>();

                m_HumansToTest.Add(new TestableHumans()
                {
                    HumanAnimator = animator,
                });

                human.GetComponent<NavMeshAgent>().enabled = false;
                human.enabled = false;
            }

        }
    }
}
