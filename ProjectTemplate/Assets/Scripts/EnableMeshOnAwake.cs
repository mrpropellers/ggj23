using UnityEngine;

public class EnableMeshOnAwake : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer m_Renderer;

    private void Awake()
    {
        m_Renderer.enabled = true;
    }
}
