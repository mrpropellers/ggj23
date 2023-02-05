using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAnimationHelper : MonoBehaviour
{
    private Animation[] m_Animations;

    private void Awake()
    {
        m_Animations = GetComponentsInChildren<Animation>();    
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerKill()
    {
        foreach (var anim in m_Animations)
        {
            anim.Play();
        }
    }
}
