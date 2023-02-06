using System.Collections;
using System.Collections.Generic;
using GGJ23.Audio;
using UnityEngine;

public class Hauntable : MonoBehaviour
{
    public const float k_HauntWindupTime = 1f;
    public const float k_ScreenShakeWaitTime = 5f;

    [field: SerializeField, Range(0.01f, 100.00f)]
    public float Spookiness { get; private set; }

    public bool Unlocked { get; private set; }

    public float RootGrowthAmount { get; private set; }

    public bool HauntCompleted { get; internal set; }

    [SerializeField]
    private GrowingRoot[] m_Roots;

    [SerializeField]
    internal GameObject m_FramingCam;

    [SerializeField]
    internal float m_FramingCamHauntDuration;

    [SerializeField]
    private bool m_IsKillMove;

    [SerializeField]
    internal bool m_ApplyScreenShake;

    [SerializeField]
    private Material[] m_MatsToSwap;

    [SerializeField]
    private Renderer[] m_RenderersToSwapMatsOn;

    internal Animation m_Animation;
    internal FmodEventCueAdvancer m_FmodPlayer;
    private KillAnimationHelper m_KillHelper;

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
        m_FmodPlayer = GetComponent<FmodEventCueAdvancer>();
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
        StartCoroutine(EnableFramingCamForDuration(m_FramingCamHauntDuration));
        InputHandler.Instance.CurrentWindow.Room.RemoveHauntable(this);
    }

    public void SwapMats()
    {
        for (int i = 0; i < m_MatsToSwap.Length; i++)
        {
            m_RenderersToSwapMatsOn[i].material = m_MatsToSwap[i];
        }
    }

    IEnumerator EnableFramingCamForDuration(float dur)
    {
        m_FramingCam.SetActive(true);
        if (m_FmodPlayer == null)
        {
            if (TryGetComponent<FMODUnity.StudioEventEmitter>(out var emitter))
            {
                Debug.Log("Playing sound effect from emitter.", this);
                emitter.Play();
            }
            else
                Debug.LogWarning($"{name} doesn't have any sound effects!", this);

        }
        else
            m_FmodPlayer.PlayNextFmodCue();
        InputHandler.Instance.FreezeControls = true;

        // Wait for camera to get to its place
        yield return new WaitForSeconds(k_HauntWindupTime);

        if (m_IsKillMove)
        {
            InputHandler.Instance.CurrentWindow.Room.BeginKillMoveHaunt(transform);
            // TODO: check bed/toilet
            if (m_KillHelper != null)
            {
                m_KillHelper.TriggerKill();
            }
            else if (m_Animation != null)
            {
                m_Animation.Play();
            }
        }
        else
        {
            InputHandler.Instance.CurrentWindow.Room.BeginHaunt(Spookiness);
            m_Animation.Play();
        }

        if (m_ApplyScreenShake)
        {
            yield return new WaitForSeconds(k_ScreenShakeWaitTime);
            GameplayManager.Instance.ScreenShake();
        }

        // BUG? Should you subtract previous wait durations from this number?
        yield return new WaitForSeconds(dur);

        m_FramingCam.SetActive(false);
        InputHandler.Instance.FreezeControls = false;
    }
}
