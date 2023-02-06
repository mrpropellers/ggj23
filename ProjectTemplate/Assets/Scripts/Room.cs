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

    // linked list of m_Hauntables in visual order from left to right
    private LinkedList<Hauntable> m_HauntablesLinkedList;

    // All hauntables, but in order that you should unlock them
    private Hauntable[] m_HauntablesUnlockOrder;

    private List<GameObject> m_HauntableHumans = new();

    private bool m_Growing;
    private LinkedListNode<Hauntable> m_SelectedHauntable = null;

    private void Start()
    {
        m_HauntablesLinkedList = new LinkedList<Hauntable>(m_Hauntables);

        // Find hauntable unlock order based on spookiness of haunts
        m_HauntablesUnlockOrder = m_Hauntables.OrderBy(h => h.Spookiness).ToArray();
    }

    private void Update()
    {
        if (!LookingInRoom) return;

        InputHandler.Instance.CurrentHauntableObject = m_SelectedHauntable?.Value;

        if (InputHandler.Instance.CurrentHauntableObject != null)
        {
            InputHandler.Instance.CurrentHauntableObject.Hover();
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
        if (m_SelectedHauntable == null){
            return;
        }

        var currHauntable = m_SelectedHauntable;
        var nextHauntable = m_SelectedHauntable.Next ?? m_SelectedHauntable.List.First; // check for null to loop back to first node in list

        while (nextHauntable != currHauntable && (!nextHauntable.Value.Unlocked || nextHauntable.Value.HauntCompleted)){
            nextHauntable = nextHauntable.Next ?? nextHauntable.List.First;
        }

        m_SelectedHauntable = nextHauntable;
    }

    public void SelectNextHauntableLeft()
    {
        if (m_SelectedHauntable == null){
            return;
        }

        var currHauntable = m_SelectedHauntable;
        var prevHauntable = m_SelectedHauntable.Previous ?? m_SelectedHauntable.List.Last; // check for null to loop back to last node in list

        while (prevHauntable != currHauntable && (!prevHauntable.Value.Unlocked || prevHauntable.Value.HauntCompleted)){
            prevHauntable = prevHauntable.Previous ?? prevHauntable.List.Last; 
        }

        m_SelectedHauntable = prevHauntable;
    }

    public void UnlockHauntable(Hauntable unlocked)
    {
        if (m_SelectedHauntable == null){
            m_SelectedHauntable = m_HauntablesLinkedList.Find(unlocked);
        }
    }

    public void RemoveHauntable(Hauntable completed)
    {
        // if the completed is the current selected hauntable, try to find the next one to the right
        if (m_SelectedHauntable.Value == completed)
        {
            var currHauntable = m_SelectedHauntable;
            this.SelectNextHauntableRight();

            // didnt a valid next one to the right, set selected hauntable to null
            if (currHauntable == m_SelectedHauntable)
            {
                m_SelectedHauntable = null;
            }
        }
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
