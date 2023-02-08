using System.Collections;
using System.Collections.Generic;
using GGJ23.Audio;
using Humans;
using UnityEngine;

public class Hauntable : MonoBehaviour
{
    public const float k_ScreenShakeWaitTime = 5f;

    [field: SerializeField, Range(0.01f, 100.00f)]
    public float Spookiness { get; private set; }

    public bool Unlocked { get; private set; }

    public float RootGrowthAmount { get; private set; }

    public bool HauntCompleted { get; internal set; }
    public bool HauntActuallyCompletedForReal { get; private set; }

    [SerializeField]
    private GrowingRoot[] m_Roots;

    [SerializeField]
    internal float m_HauntWindupTime = 1f;

    [SerializeField]
    internal GameObject m_FramingCam;

    [SerializeField]
    internal float m_FramingCamHauntDuration;

    [SerializeField]
    private bool m_IsKillMove;

    [SerializeField]
    private Material[] m_MatsToSwap;

    [SerializeField]
    private Renderer[] m_RenderersToSwapMatsOn;

    [SerializeField]
    private GameObject m_LightToEnable;

    internal Animation m_Animation;
    internal FmodEventCueAdvancer m_FmodPlayer;
    private KillAnimationHelper m_KillHelper;

    private List<Material> m_Mats = new List<Material>();
    private bool m_Hovering;

    public bool IsKill => m_IsKillMove;

    HauntType m_DeathFlingType;
    Human m_HumanBeingKilled;

    private void Awake()
    {
        TryGetComponent<KillAnimationHelper>(out m_KillHelper);
    }

    private void Start()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            if (!r.material.name.Contains("Root"))
                m_Mats.Add(r.material);
        }
        m_Animation = GetComponent<Animation>();
        m_FmodPlayer = GetComponent<FmodEventCueAdvancer>();
    }

    private void Update()
    {
        if (m_Hovering && Unlocked)
        {
            if (!UIManager.Instance.m_ShownHauntablePrompt)
            {
                UIManager.Instance.ShowHint("[click] to haunt them...", 6);
                UIManager.Instance.m_ShownHauntablePrompt = true;
            }

            foreach (Material mat in m_Mats)
            {
                mat.SetColor("_EmissiveColor", Color.red * 6);
            }
        }
        else
        {
            foreach (Material mat in m_Mats)
            {
                mat.SetColor("_EmissiveColor", Unlocked && !HauntCompleted ? Color.white * 2f : Color.black);
            }
        }

        m_Hovering = false;
    }

    // Call from the Animation timeline, not code
    public void DoScreenShake() => GameplayManager.Instance.ScreenShake();

    // NOTE: This is very fragile, lol. If anything changes about order of operations in kill move code, this will
    //       probably break.
    public void SetDeathFlingType(HauntType hauntType) => m_DeathFlingType = hauntType;
    // Now we have a public method to give to the animator, though...
    public void PerformDeathFling() => m_HumanBeingKilled.DeathFling(m_DeathFlingType);

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

        Humans.Human human = null;
        if (m_IsKillMove)
        {
            human = InputHandler.Instance.CurrentWindow.Room.PrepareKillMoveHaunt(transform);
            m_HumanBeingKilled = human;
        }

        // Wait for camera to get to its place
        yield return new WaitForSeconds(m_HauntWindupTime);

        if (m_IsKillMove)
        {
            InputHandler.Instance.CurrentWindow.Room.BeginKillMoveHaunt(this, human, m_FramingCamHauntDuration);
            if (m_KillHelper != null) // if bed or toilet kill
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

        // BUG? Should you subtract previous wait durations from this number?
        yield return new WaitForSeconds(dur);

        m_FramingCam.SetActive(false);
        InputHandler.Instance.FreezeControls = false;
        GameplayManager.Instance.AddFearJuice(Spookiness);
        if (m_LightToEnable != null)
            m_LightToEnable.SetActive(true);

        if (!UIManager.Instance.m_ShownHauntCompletedPrompt)
        {
            UIManager.Instance.m_ShownHauntCompletedPrompt = true;
            UIManager.Instance.ShowHint("you scared them, earning you some of their precious fear juice.", 4);
            UIManager.Instance.ShowHint("use fear juice to grow more roots, and so on...", 3, false);
            UIManager.Instance.ShowHint("when you're done here, [right click] to leave.", 4, false);
        }

        HauntActuallyCompletedForReal = true;
    }
}
