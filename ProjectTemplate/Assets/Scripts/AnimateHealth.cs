using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AnimateHealth : MonoBehaviour
{
    [SerializeField]
    private Sprite[] m_HealthyFrames;

    [SerializeField]
    private Sprite[] m_DangerFrames;

    [SerializeField]
    private Sprite m_DeadFrame;

    [SerializeField]
    private float m_TimeBetweenHealthyFrames;

    [SerializeField]
    private float m_TimeBetweenDangerFrames;

    private Image m_Image;
    private bool m_Healthy = true;
    private bool m_Dead;

    private void OnEnable()
    {
        m_Image = GetComponent<Image>();
        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        int frame = 0;

        while (true)
        {
            if (m_Dead)
            {
                m_Image.sprite = m_DeadFrame;
                yield break;
            }

            if (m_Healthy)
            {
                m_Image.sprite = m_HealthyFrames[frame];
                frame = (frame + 1) % m_HealthyFrames.Length;
                yield return new WaitForSeconds(m_TimeBetweenHealthyFrames);
            }
            else
            {
                m_Image.sprite = m_DangerFrames[frame];
                frame = (frame + 1) % m_DangerFrames.Length;
                yield return new WaitForSeconds(m_TimeBetweenDangerFrames);
            }
        }
    }

    public void Kill()
    {
        m_Dead = true;
    }

    public void Danger()
    {
        m_Healthy = false;
    }

    public void Healthy()
    {
        m_Healthy = true;
    }
}
