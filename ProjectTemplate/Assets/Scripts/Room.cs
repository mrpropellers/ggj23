using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    public bool LookingInRoom { get; set; }

    [SerializeField, Tooltip("All hauntable objects, in visual order from left to right")]
    private Hauntable[] m_Hauntables;

    // All hauntables, but in order that you should unlock them
    private Hauntable[] m_HauntablesUnlockOrder;

    // The list of hauntables currently unlocked, in left to right visual order
    private List<Hauntable> m_UnlockedHauntables = new List<Hauntable>();

    private bool m_Growing;
    private int m_SelectedHauntable;

    private void Start()
    {
        // Find hauntable unlock order based on spookiness of haunts
        m_HauntablesUnlockOrder = m_Hauntables.OrderBy(h => h.Spookiness).ToArray();
    }

    private void Update()
    {
        if (!LookingInRoom) return;
        if (m_Hauntables.Length != 0 && m_UnlockedHauntables.Count != 0)
        {
            InputHandler.Instance.CurrentHauntableObject = m_UnlockedHauntables[m_SelectedHauntable];
            InputHandler.Instance.CurrentHauntableObject.Hover();
        }
        else if (m_UnlockedHauntables.Count == 0)
        {
            InputHandler.Instance.CurrentHauntableObject = null;
        }
    }

    private void FixedUpdate()
    {
        if (!LookingInRoom) return;
        if (m_Growing && GameplayManager.Instance.SpendFearJuice(GameplayManager.Instance.GrowRootSpeed) && !m_HauntablesUnlockOrder[^1].Unlocked)
        {
            for (int i = 0; i < m_HauntablesUnlockOrder.Length; i++)
            {
                if (i == 0 || m_HauntablesUnlockOrder[i - 1].RootGrowthAmount > 75f)
                {
                    m_HauntablesUnlockOrder[i].GrowRoots(GameplayManager.Instance.GrowRootSpeed);
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

    public void SelectNextHauntableRight()
    {
        int nextHauntable = (m_SelectedHauntable + 1) % m_UnlockedHauntables.Count;
        m_SelectedHauntable = m_UnlockedHauntables[nextHauntable].Unlocked ? nextHauntable : 0;
    }

    public void SelectNextHauntableLeft()
    {
        int lastUnlockedHauntable = m_UnlockedHauntables.Count - 1;
        int nextHauntable = m_SelectedHauntable - 1 == -1 ? lastUnlockedHauntable : m_SelectedHauntable - 1;
        m_SelectedHauntable = m_UnlockedHauntables[nextHauntable].Unlocked ? nextHauntable : lastUnlockedHauntable;
    }

    public void UnlockHauntable(Hauntable unlocked)
    {
        m_UnlockedHauntables.Add(unlocked);
    }

    public void RemoveHauntable(Hauntable completed)
    {
        m_UnlockedHauntables.Remove(completed);
        m_SelectedHauntable--;
        if (m_SelectedHauntable < 0)
            m_SelectedHauntable = 0;
    }
}
