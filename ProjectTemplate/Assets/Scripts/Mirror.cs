using UnityEngine;

public class Mirror : MonoBehaviour
{
    [SerializeField]
    private Transform m_MainCamera;

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.LookRotation(m_MainCamera.position, Vector3.up);
    }
}
