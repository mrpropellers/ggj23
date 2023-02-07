using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField]
    private float m_Speed;

    private void FixedUpdate()
    {
        transform.rotation *= Quaternion.AngleAxis(m_Speed, Vector3.up);
    }
}
