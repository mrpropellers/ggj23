using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField, Range(1f, 100f)]
    private float m_FearMeterSmoothSpeed = 10f;

    private Image m_FearMeter;

    private void Awake()
    {
        Instance = this;
        m_FearMeter = transform.Find("FearMeter/FearMeterBar").GetComponent<Image>();
    }

    private void Update()
    {
        m_FearMeter.fillAmount = Mathf.SmoothStep(m_FearMeter.fillAmount, GameplayManager.Instance.FearEnergyNormalized,
            Time.deltaTime * m_FearMeterSmoothSpeed);
    }
}
