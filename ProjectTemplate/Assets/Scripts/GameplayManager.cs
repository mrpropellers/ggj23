using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    private static float k_FearEnergyTotal = 100f;

    [field: SerializeField]
    public float GrowRootSpeed { get; private set; } = 1f;

    [field: SerializeField]
    public GameObject NPCFollowCam { get; private set; }

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
        m_FearEnergy += 0.2f;
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

    public void GameOver(int killed, int escaped)
    {
        Debug.Log($"GAME OVER LOSER!!!! {killed} humans killed, {escaped} humans escaped");
    }
}
