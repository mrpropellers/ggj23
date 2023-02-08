using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using GGJ23.Audio;
using Humans;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private readonly Dictionary<int, string> k_Nums = new()
    {
        { 0, "ZERO" },
        { 1, "ONE" },
        { 2, "TWO" },
        { 3, "THREE" }
    };

    struct HintEntry
    {
        internal string Hint;
        internal float Duration;
    }

    public Stopwatch GameTimeStopwatch { get; private set; }

    [SerializeField]
    private Volume m_Volume;

    [SerializeField, Range(1f, 100f)]
    private float m_FearMeterSmoothSpeed = 10f;

    [SerializeField]
    StudioEventEmitter m_NewspaperStingEmitter;

    public bool IsAnimatingMainMenu => m_Animation.isPlaying;

    bool m_IsShowingHint;
    Queue<HintEntry> m_HintQueue;
    private Animation m_Animation;
    private Animation m_HintAnimation;
    private Vignette m_Vignette;
    private Image m_FearMeter;
    private TextMeshProUGUI m_Hint;
    private TextMeshProUGUI m_TimeLeft;

    public bool IsShowingAHint => m_IsShowingHint;
    public bool HasShownHintTooMuchFear { get; private set; }
    public bool HasShownHintChooseAHaunt { get; private set; }
    public bool HasShownHintKillHaunt { get; private set; }
    public bool m_ShownHauntablePrompt { get; set; }
    public bool m_ShownHauntCompletedPrompt { get; set; }
    public bool m_ShownSpreadRootsPrompt { get; set; }

    private float m_DefaultVignetteIntensity;

    private Coroutine m_HintRoutine;

    private void Awake()
    {
        Instance = this;
        m_HintQueue = new Queue<HintEntry>();

        VolumeProfile profile = m_Volume.sharedProfile;
        if (profile.TryGet(out Vignette v))
        {
            m_Vignette = v;
            m_DefaultVignetteIntensity = m_Vignette.intensity.value;
        }
        else
        {
            Debug.LogWarning("Your SkyAndFogVolume has no vignette!");
        }

        m_FearMeter = transform.Find("HUD/FearMeter/FearMeterBar").GetComponent<Image>();
        m_TimeLeft = transform.Find("HUD/TimeRemaining").GetComponent<TextMeshProUGUI>();
        m_Hint = transform.Find("HUD/HintText").GetComponent<TextMeshProUGUI>();
        m_Animation = GetComponent<Animation>();
        m_HintAnimation = transform.Find("HUD/HintText").GetComponent<Animation>();
        GameTimeStopwatch = GetComponent<Stopwatch>();
    }

    private void Update()
    {
        m_FearMeter.fillAmount = Mathf.SmoothStep(m_FearMeter.fillAmount, GameplayManager.Instance.FearEnergyNormalized,
            Time.deltaTime * m_FearMeterSmoothSpeed);

        int timeLeft = GameplayManager.Instance.GameLength - GameTimeStopwatch.GetSeconds();
        if (timeLeft < 0) timeLeft = 0;

        TimeSpan time = TimeSpan.FromSeconds(timeLeft);
        m_TimeLeft.SetText(time.ToString(@"mm\:ss"));
    }

    private void OnDisable()
    {
        m_Vignette.intensity.value = 0.43f;
    }

    public void ShowNewspaper(int killed, int escaped)
    {
        string headline = "";
        string subheading = "When Will the Murders End?";
        string body = "";

        if (killed == 0)
        {
            // All escaped
            headline = "TEENS NARROWLY ESCAPE FOREST DEMON";
            subheading = "Suspect At Large";
            body = "For years, the woods of Rootville have been haunted by a mysterious force that " +
                   "has claimed the lives of hundreds of teenagers. In a cabin deep in the woods, 3 teens were " +
                   "chased around by what they described as an evil tree stump. These claims are yet to be " +
                   "verified. The investigation is still ongoing and lorem ipsum dolor!";
        }
        else if (escaped == 0)
        {
            // None escaped
            headline = "FOREST DEMON KILLS THREE TEENS";
            body = "For years, the woods of Rootville have been haunted by a mysterious force that " +
                       "has claimed the lives of hundreds of teenagers. In a cabin deep in the woods, a search " +
                       $"party recovered the bodies of 3 teenagers who appeared to suffer gruesome deaths, similar " +
                       "to ones the town has seen before. The investigation is still ongoing and ";
        }
        else
        {
            // Some escaped, some killed
            headline = $"FOREST DEMON KILLS {k_Nums[killed]}, WOUNDS {k_Nums[escaped]}";
            if (killed == 1)
            {
                body = "For years, the woods of Rootville have been haunted by a mysterious force that " +
                       "has claimed the lives of hundreds of teenagers. In a cabin deep in the woods, a search " +
                       $"party recovered the bodies of 1 teenager who appeared to suffer a gruesome death, similar " +
                       "to ones the town has seen before. The investigation is still ongoing and ";
            }
            else
            {
                body = "For years, the woods of Rootville have been haunted by a mysterious force that " +
                       "has claimed the lives of hundreds of teenagers. In a cabin deep in the woods, a search " +
                       $"party recovered the bodies of {killed} teenagers who appeared to suffer gruesome deaths, similar " +
                       "to ones the town has seen before. The investigation is still ongoing and ";
            }
        }

        transform.Find("Newspaper/Headline").GetComponent<TextMeshProUGUI>().SetText(headline);
        transform.Find("Newspaper/Subheading").GetComponent<TextMeshProUGUI>().SetText(subheading);
        transform.Find("Newspaper/ArticleBody").GetComponent<TextMeshProUGUI>().SetText(body);


        m_Animation.Play("NewspaperIn");
        m_NewspaperStingEmitter.Play();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }


    public void KillHuman(HumanName humanName)
    {
        var portrait = transform.Find($"HUD/{humanName}Stats/Portrait");
        var portraitComponent = portrait.GetComponent<HumanPortrait>();
        portraitComponent.Kill();

        var cardiogram = transform.Find($"HUD/{humanName}Stats/Cardiogram");
        var animateHealth = cardiogram.GetComponent<AnimateHealth>();
        animateHealth.Kill();

        var thought = transform.Find($"HUD/{humanName}Stats/ThoughtBubble");
        var thoughts = thought.GetComponent<Thoughts>();
        thoughts.HeadEmpty();
    }

    public void HumanEscaped(HumanName humanName)
    {
        var portrait = transform.Find($"HUD/{humanName}Stats/Portrait");
        var portraitComponent = portrait.GetComponent<HumanPortrait>();
        portraitComponent.Escape();

        var cardiogram = transform.Find($"HUD/{humanName}Stats/Cardiogram");
        var animateHealth = cardiogram.GetComponent<AnimateHealth>();
        animateHealth.Escaped();
    }

    public void SetCardiogramFear(HumanName humanName, float fear)
    {
        var cardiogram = transform.Find($"HUD/{humanName}Stats/Cardiogram");
        var animateHealth = cardiogram.GetComponent<AnimateHealth>();
        animateHealth.SetFear(fear);
    }

    public void HumanThought(HumanName humanName, HumanNeed need, bool escape)
    {
        var thought = transform.Find($"HUD/{humanName}Stats/ThoughtBubble");
        var thoughts = thought.GetComponent<Thoughts>();
        thoughts.HaveNewThought(need, escape);
    }

    public void StatsTransitions(bool inAnim)
    {
        m_Animation.Play(inAnim ? "StatsIn" : "StatsOut");
    }

    public void MenuTransitions(bool shouldTransitionIn)
    {
        var animationName = shouldTransitionIn ? "MainMenuIn" : "MainMenuOut";
        m_Animation.Play(animationName);

        if (m_IsShowingHint)
        {
            StopCoroutine(m_HintRoutine);
            m_IsShowingHint = false;
            m_HintAnimation.Stop();
            m_Hint.SetText("");
        }
    }

    public void ShowHint(string hint, float duration, bool forceNewHint = true)
    {
        if (m_IsShowingHint)
        {
            if (forceNewHint)
            {
                StopCoroutine(m_HintRoutine);
                m_HintAnimation.Stop();
                m_HintQueue.Clear();
                m_Hint.SetText("");
            }
            else
            {
                m_HintQueue.Enqueue(new HintEntry()
                {
                    Hint = hint,
                    Duration = duration
                });
                return;
            }
        }
        m_IsShowingHint = true;
        m_HintRoutine = StartCoroutine(ShowHintAnim(hint, duration));
    }

    public void ShowHintTooMuchFear()
    {
        HasShownHintTooMuchFear = true;
        ShowHint("a person scared too often may flee...", 3f, false);
        ShowHint("pay attention to the heart monitors; hunt the calm ones.", 4f, false);
    }

    public void ShowHintKillHaunt()
    {
        HasShownHintKillHaunt = true;
        var diceRoll = UnityEngine.Random.value;
        if (diceRoll > 0.9f)
        {
            ShowHint("*chanting softly* roots, roots, roots, roots...", 3f);
            ShowHint("*banging on table* ROOTS, ROOTS, ROOTS, ROOTS!!!", 3f, false);
        }
        else if (diceRoll > 0.85f)
            ShowHint("what! are you doing!! in my swamp!?!?!", 3f);
        else
            ShowHint("maim them... mAnGlE tHeM..... KILL THEM!!", 4f);
    }

    public void ShowHintChooseAHaunt()
    {
        HasShownHintChooseAHaunt = true;
        ShowHint(
            "use [a] and [d] to choose your haunt...", 4f, true);
    }

    private IEnumerator ShowHintAnim(string hint, float duration)
    {
        if (m_HintAnimation.isPlaying)
        {
            Debug.LogWarning("Previous hint animation wasn't stopped before new one started!");
            m_HintAnimation.Stop();
        }

        // Pack the first hint up so it looks the same as hints coming out of the queue
        var currentHint = new HintEntry()
        {
            Hint = hint,
            Duration = duration
        };

        do
        {
            m_Hint.SetText(currentHint.Hint);
            m_HintAnimation.Play("HintIn");
            yield return new WaitForSeconds(currentHint.Duration);
            m_HintAnimation.Play("HintOut");
            yield return new WaitUntil(() => !m_HintAnimation.isPlaying);
        } while (m_HintQueue.TryDequeue(out currentHint));
        m_IsShowingHint = false;
    }

    public void SetVignetteIntensity(float intensity, float dur, bool fadeToOriginal=false)
    {
        StartCoroutine(SetVignetteIntensityOverTime(fadeToOriginal ? m_DefaultVignetteIntensity : intensity, dur));
    }

    private IEnumerator SetVignetteIntensityOverTime(float intensity, float dur)
    {
        float original = m_Vignette.intensity.value;

        float time = 0f;
        while (time < dur)
        {
            m_Vignette.intensity.value = Mathf.Lerp(original, intensity, time / dur);
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }

        m_Vignette.intensity.value = intensity;
    }
}
