using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAnimationHelper : MonoBehaviour
{
    [SerializeField]
    private bool m_UsesAnimators = false;

    private Animator[] m_Animators;

    private void Awake()
    {
        m_Animators = GetComponentsInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var anim in m_Animators)
        {
            anim.enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TriggerKill();
        }
    }

    public void TriggerKill()
    {
        foreach (var anim in m_Animators)
        {
            anim.enabled = true;
        }
    }
}
