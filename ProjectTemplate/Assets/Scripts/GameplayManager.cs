using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    private static float k_FearEnergyTotal = 100f;

    private float m_FearEnergy;
    public float FearEnergyNormalized => m_FearEnergy / k_FearEnergyTotal;

    private void Awake()
    {
        Instance = this;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        m_FearEnergy += 0.1f;
    }
}
