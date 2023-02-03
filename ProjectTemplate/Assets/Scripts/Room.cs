using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField]
    private Hauntable[] m_Hauntables;

    private bool m_Growing;

    private void FixedUpdate()
    {
        if (m_Growing && GameplayManager.Instance.SpendFearJuice(GameplayManager.Instance.GrowRootSpeed) && !m_Hauntables[^1].Unlocked)
        {
            for (int i = 0; i < m_Hauntables.Length; i++)
            {
                if (i == 0 || m_Hauntables[i - 1].RootGrowthAmount > 75f)
                {
                    m_Hauntables[i].GrowRoots(GameplayManager.Instance.GrowRootSpeed);
                }
            }
        }
    }

    public void GrowRoots()
    {
        // TODO: Start playing SFX
        m_Growing = true;
    }

    public void StopGrowingRoots()
    {
        // TODO: Stop playing SFX
        m_Growing = false;
    }
}
