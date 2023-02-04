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

    [SerializeField]
    private WaypointsDict m_Waypoints;

    void Awake()
    {
        var humans = transform.GetComponentsInChildren<Human>();
        foreach (var child in humans)
        {
            child.Initialize(m_Waypoints);
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
}
