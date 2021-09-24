using RogueLike.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLike.Systems
{
    public class SchedulingSystem
    {
        private int _time;
        private readonly SortedDictionary<int, List<IScheduleable>> _scheduleables;

        public SchedulingSystem()
        {
            _time = 0;
            _scheduleables = new SortedDictionary<int, List<IScheduleable>>();
        }

        /// <summary>
        /// Adds a new object to the schedule and places it at the current time (turn) plus the object's Speed property.
        /// </summary>
        /// <param name="scheduleable">Object to add to the schedule.</param>
        public void Add(IScheduleable scheduleable)
        {
            int key = _time + scheduleable.Speed;
            if (!_scheduleables.ContainsKey(key))
                _scheduleables.Add(key, new List<IScheduleable>());
            _scheduleables[key].Add(scheduleable);
        }

        /// <summary>
        /// Remove a specific obejct from the schedule. Useful for when a monster is killed to remove it before its actrion comes up again.
        /// </summary>
        /// <param name="scheduleable">Object to remove.</param>
        public void Remove(IScheduleable scheduleable)
        {
            KeyValuePair<int, List<IScheduleable>> scheduleableListFound = new KeyValuePair<int, List<IScheduleable>>(-1, null);

            foreach (KeyValuePair<int, List<IScheduleable>> scheduleableList in _scheduleables)
            {
                if (scheduleableList.Value.Any(s => s == scheduleable))
                {
                    scheduleableListFound = scheduleableList;
                    break;
                }
            }
            if (scheduleableListFound.Value != null)
            {
                scheduleableListFound.Value.Remove(scheduleable);
                if (scheduleableListFound.Value.Count <= 0)
                    _scheduleables.Remove(scheduleableListFound.Key);
            }
        }

        /// <summary>
        /// Get the next object whose turn it is from the schedule. Advance time if necessary.
        /// </summary>
        /// <returns></returns>
        public IScheduleable Get()
        {
            KeyValuePair<int, List<IScheduleable>> firstScheduleableGroup = _scheduleables.First();
            IScheduleable firstScheduleable = firstScheduleableGroup.Value.First();
            Remove(firstScheduleable);
            _time = firstScheduleableGroup.Key;
            return firstScheduleable;
        }

        /// <summary>
        /// Gets the current time (turn) for the schedule.
        /// </summary>
        /// <returns></returns>
        public int GetTime()
        {
            return _time;
        }

        /// <summary>
        /// Reset the time and clear out the schedule.
        /// </summary>
        public void Clear()
        {
            _time = 0;
            _scheduleables.Clear();
        }

        public int Count()
        {
            return _scheduleables.Count;
        }
    }
}
