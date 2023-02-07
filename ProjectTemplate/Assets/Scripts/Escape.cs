using Humans;
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
            other.GetComponent<Human>().Escaped = true;
            UIManager.Instance.HumanEscaped(other.GetComponent<Human>().NameOfHuman);
            HumanManager.Instance.CheckGameOver();
            other.transform.parent.gameObject.SetActive(false);
        }
    }
}
