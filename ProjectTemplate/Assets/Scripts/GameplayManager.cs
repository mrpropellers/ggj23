using System.Collections;
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
        StartCoroutine(TestFearMeter());
    }

    private IEnumerator TestFearMeter()
    {
        yield return new WaitForSeconds(2f);
        m_FearEnergy = 50f;
        yield return new WaitForSeconds(2f);
        m_FearEnergy = 0f;
        yield return new WaitForSeconds(2f);
        m_FearEnergy = 100f;
        yield return new WaitForSeconds(2f);
        m_FearEnergy = 0f;
    }
}
