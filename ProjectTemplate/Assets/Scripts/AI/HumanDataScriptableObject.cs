using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Human", menuName = "ScriptableObjects/HumanDataScriptableObject")]
public class HumanDataScriptableObject : ScriptableObject
{
    public int InitialHealth;
    public int Health;

    public float InitialHungerRate;
    public float HungerRate;
    public int Hunger;

    public List<HumanTask> TaskList;
}
