using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Humans;
using Utils;

[Serializable]
public class WaypointsDict
{
    public Transform HallwayWaypoint;
    public Transform BathroomWaypoint;
    public Transform KitchenWaypoint;
    public Transform BedWaypoint;
    public Transform LivingRoomWaypoint;
    public Transform[] EscapeWaypoints;
}

public class HumanManager : MonoBehaviour
{
    public static Dictionary<HumanNeed, bool> NeedOccupancy = new()
    {
        { HumanNeed.Bathroom, false }, { HumanNeed.Curious, false },
        { HumanNeed.Hunger, false }, { HumanNeed.Sleep, false },
        { HumanNeed.Haunted, false }, { HumanNeed.Error, false }
    };

    public static HumanManager Instance { get; private set; }

    [SerializeField]
    private WaypointsDict m_Waypoints;

    private Human[] m_Humans;

    private bool m_InitiatedGameOver;

    void Awake()
    {
        Instance = this;

        m_Humans = transform.GetComponentsInChildren<Human>();
        foreach (var child in m_Humans)
        {
            child.Initialize(m_Waypoints);
        }
    }

    private void Update()
    {
        if (!m_InitiatedGameOver && UIManager.Instance.GameTimeStopwatch.GetSeconds() > GameplayManager.Instance.GameLength)
        {
            m_InitiatedGameOver = true;
            TimeUp();
        }
    }

    public static HumanNeed NextFreeTask(PriorityQueue<HumanNeed, float> queue)
    {
        var peek = queue.Peek();
        while (NeedOccupancy[peek])
        {
            queue.Dequeue();
            peek = queue.Peek();
        }

        return peek;
    }

    public static void UpdateOccupancy(HumanNeed need, bool occupied)
    {
        if (need != HumanNeed.Haunted || need != HumanNeed.Error)
        {
            NeedOccupancy[need] = occupied;
        }
    }

    public void CheckGameOver()
    {
        int killed = 0;
        int escaped = 0;

        foreach (Human human in m_Humans)
        {
            if (human.Killed)
            {
                killed++;
            }
            else if (human.Escaped)
            {
                escaped++;
            }
        }

        if (killed + escaped == m_Humans.Length)
            GameplayManager.Instance.GameOver(killed, escaped);
    }

    public void TimeUp()
    {
        foreach (Human human in m_Humans)
        {
            if (!human.Killed && !human.Escaped)
                human.TimeUpRunAway();
        }
    }
}
