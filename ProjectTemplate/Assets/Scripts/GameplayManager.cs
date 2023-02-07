using Cinemachine;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    private static float k_FearEnergyTotal = 100f;

    [field: SerializeField]
    public float GrowRootSpeed { get; private set; } = 1f;

    [field: SerializeField]
    public float GrowRootCost { get; private set; } = 0.5f;

    [field: SerializeField]
    public GameObject NPCFollowCam { get; private set; }

    [field: SerializeField]
    public int GameLength { get; private set; } = 240;

    [SerializeField]
    private float m_PassiveEnergyGain = 0.2f;

    [SerializeField]
    private CinemachineImpulseSource m_ImpulseSource;

    [SerializeField, Tooltip("The amount of energy you start with.")]
    private float m_FearEnergy = 20f;
    public float FearEnergyNormalized => m_FearEnergy / k_FearEnergyTotal;

    private void Awake()
    {
        Instance = this;
    }

    private void FixedUpdate()
    {
        m_FearEnergy = Mathf.Clamp(m_FearEnergy + m_PassiveEnergyGain, 0f, 100f);
    }

    public bool SpendFearJuice(float juiceToSpend)
    {
        if (m_FearEnergy - juiceToSpend >= 0)
        {
            m_FearEnergy -= juiceToSpend;
            return true;
        }

        return false;
    }

    public void AddFearJuice(float juiceToAdd)
    {
        m_FearEnergy = Mathf.Clamp(m_FearEnergy + juiceToAdd, 0f, 100f);
    }

    public void GameOver(int killed, int escaped)
    {
        UIManager.Instance.ShowNewspaper(killed, escaped);
        InputHandler.Instance.FreezeControls = true;
    }

    public void ScreenShake()
    {
        m_ImpulseSource.GenerateImpulse();
    }
}
