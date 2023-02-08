using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using GGJ23.Audio;
using Humans;
using UnityEngine;

public class Room : MonoBehaviour
{
    public bool LookingInRoom { get; set; }
    public bool HasHauntableHumans => m_HauntableHumans.Count > 0;
    public bool HasUnlockableHaunts => !m_HauntablesUnlockOrder[^1].Unlocked;
    public bool HasAtLeastTwoUsableHaunts => m_Hauntables.Count(h => h.Unlocked && !h.HauntCompleted) > 1;

    [SerializeField, Tooltip("All hauntable objects, in visual order from left to right")]
    private Hauntable[] m_Hauntables;

    [SerializeField]
    private HauntType m_HauntType;

    // linked list of m_Hauntables in visual order from left to right
    private LinkedList<Hauntable> m_HauntablesLinkedList;

    // All hauntables, but in order that you should unlock them
    private Hauntable[] m_HauntablesUnlockOrder;

    private List<GameObject> m_HauntableHumans = new();

    private bool m_Growing;
    private LinkedListNode<Hauntable> m_SelectedHauntable = null;

    public static Dictionary<HauntType, string> k_HauntCharacterAnim = new()
    {
        { HauntType.Bathroom,"toilet_kill" }, { HauntType.Bedroom, "bed_kill" }
    };

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
        if (!LookingInRoom || !m_Growing) return;
        if (!HasUnlockableHaunts)
        {
            Debug.Log($"No more Haunts to unlock in {name}, not growing more roots.");
            StopGrowingRoots();
            return;
        }

        if (GameplayManager.Instance.SpendFearJuice(GameplayManager.Instance.GrowRootCost))
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
        if (m_HauntablesUnlockOrder[^1].Unlocked)
        {
            Debug.Log($"All hauntables in {name} are unlocked, not growing any more roots.");
            return;
        }
        FmodHelper.PlayRootGrowingSound();
        m_Growing = true;
    }

    public void StopGrowingRoots()
    {
        FmodHelper.StopRootGrowingSound();
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
        if (!unlocked.Unlocked)
        {
            Debug.LogWarning($"{unlocked.name} should already be unlocked when passed here! " +
                $"May create extra root sounds.");
        }

        if (!UIManager.Instance.HasShownHintChooseAHaunt && HasAtLeastTwoUsableHaunts)
        {
            // NOTE: If they unlock a lot of haunts in one room we may queue up multiple hints, but
            //       presumably only one will win the race, the rest should quietly exit
            if (UIManager.Instance.IsShowingAHint)
                StartCoroutine(WaitToShowMultiHauntHint());
            else
                UIManager.Instance.ShowHintChooseAHaunt();
        }
        else if (!UIManager.Instance.HasShownHintKillHaunt && !HasUnlockableHaunts)
            UIManager.Instance.ShowHintKillHaunt();


        if (m_SelectedHauntable == null){
            m_SelectedHauntable = m_HauntablesLinkedList.Find(unlocked);
        }

        if (m_Growing && HasUnlockableHaunts)
        {
            // TODO: We should attenuate out the previous emitter, if it's still active
            FmodHelper.PlayRootGrowingSound();
        }
    }

    IEnumerator WaitToShowMultiHauntHint()
    {
        yield return new WaitUntil(() => !LookingInRoom ||
            (!UIManager.Instance.IsShowingAHint && !InputHandler.Instance.FreezeControls));
        if (!LookingInRoom || UIManager.Instance.HasShownHintChooseAHaunt || !HasAtLeastTwoUsableHaunts)
            Debug.Log("Missed our chance to show the hint! Hopefully they got it somewhere else...");
        else
            UIManager.Instance.ShowHintChooseAHaunt();
    }


    public virtual void RemoveHauntable(Hauntable completed)
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

    public Human PrepareKillMoveHaunt(Transform hauntable)
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
            var humanToKill = closestHuman.GetComponent<Human>();
            if (!k_HauntCharacterAnim.ContainsKey(m_HauntType))
            {
                humanToKill.Animator.SetTrigger("other_kill");
            }

            humanToKill.PrepareKill(m_HauntType);

            return humanToKill;
        }
        return null;
    }

    public void BeginKillMoveHaunt(Hauntable activeHaunt, Human humanToKill, float hauntLength)
    {
        if (m_HauntType == HauntType.Kitchen || m_HauntType == HauntType.LivingRoom)
            activeHaunt.SetDeathFlingType(m_HauntType);
        // Call human death anim
        if (k_HauntCharacterAnim.TryGetValue(m_HauntType, out _))
        {
            humanToKill.Animator.SetTrigger(k_HauntCharacterAnim[m_HauntType]);
        }
        humanToKill.Kill(hauntLength, m_HauntType, activeHaunt);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AI"))
        {
            var human = other.GetComponent<Human>();
            if (human.Hauntable)
            {
                m_HauntableHumans.Add(other.gameObject);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("AI"))
        {
            var human = other.GetComponent<Human>();
            if (!human.Hauntable)
            {
                if (m_HauntableHumans.Contains(other.gameObject))
                {
                    m_HauntableHumans.Remove(other.gameObject);
                }
            }
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
