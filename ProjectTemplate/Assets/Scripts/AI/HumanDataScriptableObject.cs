using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Humans
{
    public enum HumanNeed
    {
        Hunger, Bathroom, Sleep, Curious, Haunted, Error
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
        public HumanNeed FirstNeed;
        public const float MaxFear = 100f;
        public float CurrentFear;
        public float FearDecrementHealthy = 0.001f;
        public float FearDecrementDanger = 0.002f;
        private HumanNeed m_LastHaunted;
        private Dictionary<HauntType, float> m_FearPerRoom;
        public static Dictionary<HauntType, HumanNeed> HauntToNeed = new()
        {
            { HauntType.Bathroom, HumanNeed.Bathroom }, { HauntType.Bedroom, HumanNeed.Sleep },
            { HauntType.Kitchen, HumanNeed.Hunger }, { HauntType.LivingRoom, HumanNeed.Curious },
        };

        public List<Need> NeedStatus;
        public PriorityQueue<HumanNeed, float> TaskList = new();

        public void OnAfterDeserialize()
        {
            // Reset values
            CurrentFear = 0;
            m_LastHaunted = HumanNeed.Error;
            m_FearPerRoom = new()
            {
                { HauntType.Bathroom, 0 }, { HauntType.Bedroom, 0 },
                { HauntType.Kitchen, 0 }, { HauntType.LivingRoom, 0 },
            };
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
        public HumanNeed GetCurrentNeed(bool onStart, bool wasHaunted, StateType state)
        {
            if (onStart)
            {
                return FirstNeed;
            }

            // TEMP
            TaskList.Clear();
            foreach (var need in NeedStatus)
            {
                TaskList.Enqueue(need.NeedType, need.CurrentValue);
            }

            if (m_LastHaunted != HumanNeed.Error)
            {
                while (TaskList.Peek() == m_LastHaunted)
                {
                    TaskList.Dequeue();
                }
                m_LastHaunted = HumanNeed.Error;
            }

            HumanManager.NextFreeTask(TaskList);
            return TaskList.Peek();
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

            if (CurrentFear > 0)
            {
                CurrentFear -= CurrentFear < 70 ? FearDecrementHealthy : FearDecrementDanger;
            }
        }

        /// <returns>True if fear is maxed!</returns>
        public bool GetHaunted(float amount, HauntType haunt)
        {
            m_LastHaunted = HauntToNeed[haunt];
            CurrentFear += amount;
            m_FearPerRoom[haunt] += amount;

            // TODO: slow down rates?
            //var need = NeedStatus.Find(x => x.NeedType == HauntToNeed[haunt]);
            //need.CurrentRate *= 0.5f;

            if (CurrentFear >= MaxFear)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Finished task; refill need bar
        /// </summary>
        public void RefillNeed(HumanNeed need)
        {
            if (need != HumanNeed.Haunted)
            {
                var req = NeedStatus.Find(x => x.NeedType == need);
                req.CurrentValue = req.MaxValue;
            }
        }

        /// <summary>
        /// Stop decrementing the given need
        /// </summary>
        public void PauseNeed(HumanNeed need, bool paused)
        {
            if (need != HumanNeed.Haunted)
            {
                var req = NeedStatus.Find(x => x.NeedType == need);
                req.PauseDecrement = paused;
            }
        }

        public void PauseAllNeeds(bool paused)
        {
            foreach (var need in NeedStatus)
            {
                if (need.NeedType != HumanNeed.Haunted)
                {
                    need.PauseDecrement = paused;
                }
            }
        }
    }

}
