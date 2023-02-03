using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Humans
{
    public enum HumanNeed
    {
        Hunger, Bathroom, Sleep, Error
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
        // TODO: just everything is public
        public string Name;
        public float MaxFear = 100f;
        public float CurrentFear;
        public Dictionary<HauntType, float> FearPerRoom = new();

        // TODO: switch to something more searchable and serializable?
        public List<Need> NeedStatus;
        public PriorityQueue<HumanNeed, float> TaskList = new();

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
        /// </summary>
        public HumanNeed GetCurrentNeed(bool onStart, StateType state)
        {
            if (onStart)
            {
                // TODO: shuffle this for each AI
                return (HumanNeed)UnityEngine.Random.Range(0, Enum.GetNames(typeof(HumanNeed)).Length);
            }

            //bool skipState = false;
            //var skipNeed = HumanNeed.Error;

            //// TODO: if IDLE or MOVING, recalculate ALL tasks; if TASK, recalculate OTHER tasks
            //switch (state)
            //{
            //    case StateType.Idle:
            //    case StateType.Moving:
            //        TaskList.Clear();
            //        break;
            //    case StateType.Task:
            //        skipState = true;
            //        // TODO: recalculate ALL BUT CURRENT
            //        if (TaskList.TryPeek(out var need, out var pri))
            //        {
            //            TaskList.Clear();
            //            TaskList.Enqueue(need, 0);
            //        }
            //        break;
            //    default:
            //        break;         
            //}
            //foreach (var need in NeedStatus)
            //{
            //    if (skipState && need.NeedType == skipNeed)
            //    {
            //        continue;
            //    }
            //    TaskList.Enqueue(need.NeedType, need.CurrentValue);
            //}

            // TEMP
            TaskList.Clear();
            foreach (var need in NeedStatus)
            {
                TaskList.Enqueue(need.NeedType, need.CurrentValue);
            }

            return TaskList.Peek();
            //return NeedStatus.OrderBy(i => i.CurrentValue).First().NeedType;
        }

        //public void QueueTasks(HumanNeed current)
        //{
        //    // TODO: ignore the current task? clear ?? what am i doing
        //    var ordered = NeedStatus.OrderBy(i => i.CurrentValue).ToList();
        //    foreach (var need in ordered)
        //    {
        //        if (need.NeedType != current)
        //        {
        //            TaskList.Enqueue(need, 10);
        //        }
        //    }
        //}

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

        public void GetHaunted(float amount, HauntType haunt)
        {
            CurrentFear += amount;
            FearPerRoom[haunt] += 1;

            if (CurrentFear >= MaxFear)
            {
                // TODO:
                Debug.Log($"{Name} is fully spooked!");
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
