using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hauntable : MonoBehaviour
{
    [field: SerializeField, Range(0.01f, 100.00f)]
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

    [SerializeField]
    private bool m_IsKillMove;

    [SerializeField]
    private bool m_ApplyScreenShake;

    [SerializeField]
    private Material[] m_MatsToSwap;

    [SerializeField]
    private Renderer[] m_RenderersToSwapMatsOn;

    private KillAnimationHelper m_KillHelper;

    private Animation m_Animation;

    private List<Material> m_Mats = new List<Material>();
    private bool m_Hovering;

    private void Awake()
    {
        TryGetComponent<KillAnimationHelper>(out m_KillHelper);
    }

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
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameplayManager.Instance.ScreenShake();
        }

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

    public void SwapMats()
    {
        for (int i = 0; i < m_MatsToSwap.Length; i++)
        {
            m_RenderersToSwapMatsOn[i].material = m_MatsToSwap[i];
        }
    }

    private IEnumerator EnableFramingCamForDuration(float dur)
    {
        m_FramingCam.SetActive(true);
        InputHandler.Instance.FreezeControls = true;

        // Wait for camera to get to its place
        yield return new WaitForSeconds(1f);

        if (m_IsKillMove)
        {
            InputHandler.Instance.CurrentWindow.Room.BeginKillMoveHaunt(transform);
            // TODO: check bed/toilet 
            m_KillHelper.TriggerKill();
        }
        else
        {
            InputHandler.Instance.CurrentWindow.Room.BeginHaunt(Spookiness);
            m_Animation.Play();
        }

        if (m_ApplyScreenShake)
        {
            yield return new WaitForSeconds(2f);
            GameplayManager.Instance.ScreenShake();
        }

        yield return new WaitForSeconds(dur);

        m_FramingCam.SetActive(false);
        InputHandler.Instance.FreezeControls = false;
    }
}
