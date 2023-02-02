using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Humans
{
    public enum HumanNeed
    {
        Hunger, Bathroom, Sleep
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

        // TODO: switch to something more searchable and serializable?
        public List<Need> NeedStatus;
        public PriorityQueue<Need, int> TaskList;

        public void OnAfterDeserialize()
        {
            // Reset values
            foreach (var need in NeedStatus)
            {
                need.CurrentValue = need.MaxValue;
                need.CurrentRate = need.InitialRate;
                need.PauseDecrement = false;
            }
        }

        public void OnBeforeSerialize() { }

        /// <summary>
        /// Grabs lowest value
        /// TODO: given a threshold per each need, prioritize that need
        /// </summary>
        public HumanNeed GetCurrentNeed()
        {
            return NeedStatus.OrderBy(i => i.CurrentValue).First().NeedType;
        }

        /// <summary>
        /// Decrement all needs
        /// </summary>
        public void UpdateNeeds()
        {
            foreach (var need in NeedStatus)
            {
                if (!need.PauseDecrement)
                {
                    need.CurrentValue -= need.CurrentRate;
                }
            }
        }

        /// <summary>
        /// Finished task; refill need bar
        /// </summary>
        public void RefillNeed(HumanNeed need)
        {
            var req = NeedStatus.Find(x => x.NeedType == need);
            req.CurrentValue = req.MaxValue;
            //req.CurrentValue += amount;
        }

        /// <summary>
        /// Stop decrementing the given need
        /// </summary>
        public void PauseNeed(HumanNeed need, bool paused)
        {
            var req = NeedStatus.Find(x => x.NeedType == need);
            req.PauseDecrement = paused;
        }
    }

}
