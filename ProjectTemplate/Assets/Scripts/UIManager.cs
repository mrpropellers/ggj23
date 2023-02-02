using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField]
    private Volume m_Volume;

    [SerializeField, Range(1f, 100f)]
    private float m_FearMeterSmoothSpeed = 10f;

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

        m_FearMeter = transform.Find("FearMeter/FearMeterBar").GetComponent<Image>();
    }

    private void Update()
    {
        m_FearMeter.fillAmount = Mathf.SmoothStep(m_FearMeter.fillAmount, GameplayManager.Instance.FearEnergyNormalized,
            Time.deltaTime * m_FearMeterSmoothSpeed);
    }

    private void OnDisable()
    {
        m_Vignette.intensity.value = 0.43f;
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
