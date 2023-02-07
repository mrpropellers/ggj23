using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AnimateHealth : MonoBehaviour
{
    [SerializeField]
    private Sprite[] m_HealthyFrames;

    [SerializeField]
    private Color m_HealthyColor;

    [SerializeField]
    private Color m_ScaredColor;

    [SerializeField]
    private Sprite m_DeadFrame;

    [SerializeField]
    private Sprite m_EscapedFrame;

    [SerializeField]
    private float m_TimeBetweenHealthyFrames;

    [SerializeField]
    private float m_TimeBetweenDangerFrames;

    private Image m_Image;
    private float m_Fear;
    private bool m_Dead;
    private bool m_Escaped;

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
                m_Image.color = Color.white;
                yield break;
            }

            if (m_Escaped)
            {
                m_Image.sprite = m_EscapedFrame;
                m_Image.color = Color.white;
                yield break;
            }

            m_Image.sprite = m_HealthyFrames[frame];
            m_Image.color = Color.Lerp(m_HealthyColor, m_ScaredColor, m_Fear);
            frame = (frame + 1) % m_HealthyFrames.Length;
            yield return new WaitForSeconds(Mathf.Lerp(m_TimeBetweenHealthyFrames, m_TimeBetweenDangerFrames, m_Fear));
        }
    }

    public void Kill()
    {
        m_Dead = true;
    }

    public void Escaped()
    {
        m_Escaped = true;
    }

    public void SetFear(float normalizedFear)
    {
        m_Fear = normalizedFear;
    }
}
