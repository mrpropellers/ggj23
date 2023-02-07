using System.Collections;
using System.Collections.Generic;
using Humans;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

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

    [SerializeField]
    private Volume m_Volume;

    [SerializeField, Range(1f, 100f)]
    private float m_FearMeterSmoothSpeed = 10f;

    private Animation m_Animation;
    private Vignette m_Vignette;
    private Image m_FearMeter;

    private float m_DefaultVignetteIntensity;

    private void Awake()
    {
        Instance = this;

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
        m_Animation = GetComponent<Animation>();
    }

    private void Update()
    {
        Debug.Log(GameplayManager.Instance.FearEnergyNormalized);
        m_FearMeter.fillAmount = Mathf.SmoothStep(m_FearMeter.fillAmount, GameplayManager.Instance.FearEnergyNormalized,
            Time.deltaTime * m_FearMeterSmoothSpeed);
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
    }

    public void KillHuman(HumanName humanName)
    {
        var portrait = transform.Find($"HUD/{humanName}Stats/Portrait");
        var portraitComponent = portrait.GetComponent<HumanPortrait>();
        portraitComponent.Kill();

        var cardiogram = transform.Find($"HUD/{humanName}Stats/Cardiogram");
        var animateHealth = cardiogram.GetComponent<AnimateHealth>();
        animateHealth.Kill();
    }

    public void HumanInDanger(HumanName humanName)
    {
        var cardiogram = transform.Find($"HUD/{humanName}Stats/Cardiogram");
        var animateHealth = cardiogram.GetComponent<AnimateHealth>();
        animateHealth.Danger();
    }

    public void HumanHealthy(HumanName humanName)
    {
        var cardiogram = transform.Find($"HUD/{humanName}Stats/Cardiogram");
        var animateHealth = cardiogram.GetComponent<AnimateHealth>();
        animateHealth.Healthy();
    }

    public void HumanThought(HumanName humanName, HumanNeed need)
    {
        var thought = transform.Find($"HUD/{humanName}Stats/ThoughtBubble");
        var thoughts = thought.GetComponent<Thoughts>();
        thoughts.HaveNewThought(need);
    }

    public void StatsTransitions(bool inAnim)
    {
        m_Animation.Play(inAnim ? "StatsIn" : "StatsOut");
    }

    public void MenuTransitions(bool inAnim)
    {
        m_Animation.Play(inAnim ? "MainMenuIn" : "MainMenuOut");
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
