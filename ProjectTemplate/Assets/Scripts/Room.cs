using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField]
    private GrowingRoot[] m_Roots;

    private float m_RootsFilled;

    private bool m_Growing;

    private void FixedUpdate()
    {
        if (m_Growing && GameplayManager.Instance.SpendFearJuice(GameplayManager.Instance.GrowRootSpeed))
        {
            m_RootsFilled += GameplayManager.Instance.GrowRootSpeed;
            foreach (GrowingRoot root in m_Roots)
            {
                root.m_GrowthAmount = m_RootsFilled / 100f;
            }

            Debug.Log($"Roots filled {m_RootsFilled}% !!");
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
