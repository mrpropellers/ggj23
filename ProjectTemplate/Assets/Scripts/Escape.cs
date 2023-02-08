using Humans;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Escape : MonoBehaviour
{
    const float k_EscapeTimeBeforeGameOver = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AI"))
        {
            // TODO:
            Debug.Log($"{other.transform.parent.name} escaped!");
            other.GetComponent<Human>().Escaped = true;
            UIManager.Instance.HumanEscaped(other.GetComponent<Human>().NameOfHuman);
            var currentTime = Time.unscaledTime;
            HumanManager.Instance.CheckGameOver(() => Time.unscaledTime - currentTime > k_EscapeTimeBeforeGameOver);
            other.transform.parent.gameObject.SetActive(false);
        }
    }
}
