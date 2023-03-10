using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private Animator m_animator = null;

    private int m_npcCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AI"))
        {
            if (m_npcCount == 0){
                m_animator.Play("DoorOpen", 0, 0.0f);
            }
            
            m_npcCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("AI"))
        {
            if (m_npcCount <= 1){
                m_animator.Play("DoorClose", 0, 0.0f);
            }

            m_npcCount--;
        }
    }
}
