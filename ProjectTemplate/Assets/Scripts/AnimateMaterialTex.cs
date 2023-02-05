using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class AnimateMaterialTex : MonoBehaviour
{
    [SerializeField]
    private Texture[] m_Textures;

    [SerializeField]
    private float m_TimeBetweenFrames;

    private Material m_Mat;
    private int m_Frame = 0;

    private void Start()
    {
        m_Mat = GetComponent<Renderer>().material;
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        while (true)
        {
            m_Frame = (m_Frame + 1) % m_Textures.Length;
            m_Mat.mainTexture = m_Textures[m_Frame];
            yield return new WaitForSeconds(m_TimeBetweenFrames);
        }
    }
}
