using System.Collections.Generic;
using System.Linq;
using Humans;
using UnityEngine;

public class Room : MonoBehaviour
{
    public bool LookingInRoom { get; set; }

    [SerializeField, Tooltip("All hauntable objects, in visual order from left to right")]
    private Hauntable[] m_Hauntables;

    // TODO: killanim
    [SerializeField]
    private KillAnimationHelper m_KillHelper;

    [SerializeField]
    private HauntType m_HauntType;

    // All hauntables, but in order that you should unlock them
    private Hauntable[] m_HauntablesUnlockOrder;

    // The list of hauntables currently unlocked, in left to right visual order
    private List<Hauntable> m_UnlockedHauntables = new List<Hauntable>();

    private List<GameObject> m_HauntableHumans = new();

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

    public void BeginHaunt(float amount)
    {
        foreach (var human in m_HauntableHumans)
        {
            human.GetComponent<Human>().BeginHaunt(amount, m_HauntType);
        }
    }

    public void BeginKillMoveHaunt(Transform hauntable)
    {
        float minDistance = Mathf.Infinity;
        GameObject closestHuman = null;

        foreach (var human in m_HauntableHumans)
        {
            float dist = Vector3.Distance(hauntable.position, human.transform.position);

            if (dist <= minDistance)
            {
                minDistance = dist;
                closestHuman = human;
            }
        }

        if (closestHuman != null)
        {
            m_KillHelper.TriggerKill();
            closestHuman.GetComponent<Human>().Kill();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AI"))
        {
            m_HauntableHumans.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("AI"))
        {
            m_HauntableHumans.Remove(other.gameObject);
        }
    }
}
