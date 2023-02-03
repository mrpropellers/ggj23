using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Humans;

[Serializable]
public class WaypointsDict
{
    public Transform HallwayWaypoint;
    public Transform BathroomWaypoint;
    public Transform KitchenWaypoint;
    public Transform BedWaypoint;
}

public class HumanInitializer : MonoBehaviour
{
    [SerializeField]
    private WaypointsDict m_Waypoints;

    // Start is called before the first frame update
    void Start()
    {
        var children = transform.GetComponentsInChildren<Human>();
        foreach (var child in children)
        {
            child.Initialize(m_Waypoints);
        }
    }
}
