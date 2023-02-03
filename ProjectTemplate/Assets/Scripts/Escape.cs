using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Escape : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AI"))
        {
            // TODO:
            Debug.Log($"{other.transform.parent.name} escaped!");
            other.transform.parent.gameObject.SetActive(false);
        }
    }
}
