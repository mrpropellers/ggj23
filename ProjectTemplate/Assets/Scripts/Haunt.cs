using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Humans;

[RequireComponent(typeof(Collider))]
public class Haunt : MonoBehaviour
{
    [SerializeField]
    private HauntType m_HauntType;

    private List<GameObject> m_HauntableHumans = new();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // TEMP
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"pushing space for haunt");
            BeginHaunt(10f);
        }
    }

    public void BeginHaunt(float amount)
    {
        foreach (var human in m_HauntableHumans)
        {
            human.GetComponent<Human>().BeginHaunt(amount, m_HauntType);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AI"))
        {
            m_HauntableHumans.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("AI"))
        {
            m_HauntableHumans.Remove(other.gameObject);
        }
    }
}
