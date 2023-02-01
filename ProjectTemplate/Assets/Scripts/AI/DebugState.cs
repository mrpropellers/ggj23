using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugState : MonoBehaviour
{
    private Human m_Human;
    private Dictionary<string, Color> m_StateToColor = new Dictionary<string, Color>()
    {
        { "Idle", Color.gray }, { "Moving", Color.blue }, { "Eat", Color.yellow }
    };

    // Start is called before the first frame update
    void Start()
    {
        m_Human = GetComponent<Human>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (m_Human != null && m_Human.m_CurrentState != null)
        {
            Gizmos.color = m_StateToColor[m_Human.m_CurrentState.StateName];
            Gizmos.DrawCube(transform.position + new Vector3(0, 1.5f, 0), Vector3.one);
        }
    }
}
