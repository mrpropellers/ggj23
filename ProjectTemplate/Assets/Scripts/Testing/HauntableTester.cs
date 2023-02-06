using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GGJ23.Testing;
using UnityEngine;
using UnityEngine.EventSystems;

public class HauntableTester : MonoBehaviour
{
    [Serializable]
    class TestableHaunt
    {
        public bool TestMe;
        public Hauntable ToTest;
    }

    [SerializeField]
    List<TestableHaunt> m_Hauntables;

    // Start is called before the first frame update
    void Start()
    {
        var targets = GetComponentsInChildren<Hauntable>();
        m_Hauntables = new List<TestableHaunt>();
        var numDisabled = 0;
        foreach (var t in targets)
        {
            if (!t.gameObject.activeInHierarchy)
            {
                numDisabled++;
                continue;
            }
            m_Hauntables.Add(new TestableHaunt()
            {
                TestMe = false,
                ToTest = t,
            });
        }
        Debug.Log($"{m_Hauntables.Count} hauntables found for testing -- " +
            $"disabled {numDisabled} so they are all disabled by default (for testing)");
    }

    void OnValidate()
    {
        foreach (var testable in m_Hauntables.Where(h => h.TestMe))
        {
            testable.TestMe = false;
            testable.ToTest.TestHaunt();
        }
    }
}

