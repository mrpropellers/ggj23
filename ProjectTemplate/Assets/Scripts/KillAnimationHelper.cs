using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAnimationHelper : MonoBehaviour
{
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

    // Update is called once per frame
    void Update()
    {
        // TODO: temp debug
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
