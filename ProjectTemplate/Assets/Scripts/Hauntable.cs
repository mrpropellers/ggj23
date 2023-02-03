using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hauntable : MonoBehaviour
{
    [field: SerializeField, Range(0.01f, 1.00f)]
    public float Spookiness { get; private set; }

    public bool Unlocked { get; private set; }

    public float RootGrowthAmount { get; private set; }

    public bool HauntCompleted { get; private set; }

    [SerializeField]
    private GrowingRoot[] m_Roots;

    [SerializeField]
    private GameObject m_FramingCam;

    [SerializeField]
    private float m_FramingCamHauntDuration;

    private Animation m_Animation;

    private List<Material> m_Mats = new List<Material>();
    private bool m_Hovering;

    private void Start()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            m_Mats.Add(r.material);
        }
        m_Animation = GetComponent<Animation>();
    }

    private void Update()
    {
        if (m_Hovering && Unlocked)
        {
            foreach (Material mat in m_Mats)
            {
                mat.SetColor("_EmissiveColor", Color.red * 8);
            }
        }
        else
        {
            foreach (Material mat in m_Mats)
            {
                mat.SetColor("_EmissiveColor", Unlocked && !HauntCompleted ? Color.white * 8f : Color.black);
            }
        }

        m_Hovering = false;
    }

    public void GrowRoots(float amount)
    {
        RootGrowthAmount = Mathf.Clamp(RootGrowthAmount + amount, 0f, 100f);
        if (RootGrowthAmount >= 100f && !Unlocked)
        {
            Unlocked = true;
            InputHandler.Instance.CurrentWindow.Room.UnlockHauntable(this);
            foreach (Material mat in m_Mats)
            {
                mat.SetColor("_EmissiveColor", Color.white * 8);
            }
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
        HauntCompleted = true;
        InputHandler.Instance.CurrentWindow.Room.RemoveHauntable(this);
        StartCoroutine(EnableFramingCamForDuration(m_FramingCamHauntDuration));
    }

    private IEnumerator EnableFramingCamForDuration(float dur)
    {
        m_FramingCam.SetActive(true);
        InputHandler.Instance.FreezeControls = true;

        // Wait for camera to get to its place
        yield return new WaitForSeconds(1f);
        m_Animation.Play();

        yield return new WaitForSeconds(dur);

        m_FramingCam.SetActive(false);
        InputHandler.Instance.FreezeControls = false;
    }
}
