using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum HumanNeed
{
    Hunger, Bathroom
}

[Serializable]
public class Need
{
    public HumanNeed NeedType;
    public float MaxValue;
    public float CurrentValue;
    public float InitialRate;
    public float CurrentRate;
    public bool PauseDecrement;
}

[CreateAssetMenu(fileName = "Human", menuName = "ScriptableObjects/HumanDataScriptableObject")]
public class HumanDataScriptableObject : ScriptableObject, ISerializationCallbackReceiver
{
    public string Name;
    public float InitialHealth;
    public float Health;

    // TODO: switch to serializable dict?
    public List<Need> Needs;

    //public List<HumanTask> TaskList;

    public void OnAfterDeserialize()
    {
        foreach (var need in Needs)
        {
            need.CurrentValue = need.MaxValue;
            need.CurrentRate = need.InitialRate;
            need.PauseDecrement = false;
        }
    }

    public void OnBeforeSerialize()
    {
        //throw new System.NotImplementedException();
    }

    public HumanNeed GetCurrentNeed()
    {
        return Needs.OrderBy(i => i.CurrentValue).First().NeedType;
    }

    public void UpdateNeeds()
    {
        foreach (var need in Needs)
        {
            if (!need.PauseDecrement)
            {
                need.CurrentValue -= need.CurrentRate;
            }
        }
    }

    public void RefillNeed(HumanNeed need)
    {
        var req = Needs.Find(x => x.NeedType == need);
        req.CurrentValue = req.MaxValue;
        //req.CurrentValue += amount;
    }

    public void PauseNeed(HumanNeed need, bool paused)
    {
        var req = Needs.Find(x => x.NeedType == need);
        req.PauseDecrement = paused;
    }
}
