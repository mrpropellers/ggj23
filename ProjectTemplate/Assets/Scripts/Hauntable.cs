using System.Collections;
using UnityEngine;

public class Hauntable : MonoBehaviour
{
    public bool Unlocked { get; private set; }

    public float RootGrowthAmount { get; private set; }

    [SerializeField]
    private GrowingRoot[] m_Roots;

    [SerializeField]
    private GameObject m_FramingCam;

    [SerializeField]
    private float m_FramingCamHauntDuration;

    private Animation m_Animation;

    private Material m_Mat;
    private bool m_Hovering;

    private void Start()
    {
        m_Mat = GetComponentInChildren<Renderer>().material;
        m_Animation = GetComponent<Animation>();
    }

    private void FixedUpdate()
    {
        if (m_Hovering && Unlocked)
        {
            m_Mat.SetColor("_EmissiveColor", Color.red * 8);
        }
        else
        {
            m_Mat.SetColor("_EmissiveColor", Unlocked ? Color.white * 8f : Color.black);
        }

        m_Hovering = false;
    }

    public void GrowRoots(float amount)
    {
        RootGrowthAmount = Mathf.Clamp(RootGrowthAmount + amount, 0f, 100f);
        if (RootGrowthAmount >= 100f)
        {
            Unlocked = true;
            m_Mat.SetColor("_EmissiveColor", Color.white * 8);
        }
        else
        {
            foreach (var root in m_Roots)
            {
                root.m_GrowthAmount = RootGrowthAmount / 100f;
            }
        }
    }

    public void Hover()
    {
        m_Hovering = true;
    }

    public void Haunt()
    {
        m_Animation.Play();
        StartCoroutine(EnableFramingCamForDuration(m_FramingCamHauntDuration));
    }

    private IEnumerator EnableFramingCamForDuration(float dur)
    {
        m_FramingCam.SetActive(true);
        yield return new WaitForSeconds(dur);
        m_FramingCam.SetActive(false);
    }
}
