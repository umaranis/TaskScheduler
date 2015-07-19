using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskSchedular
{
    class TaskCollection : IEnumerable<ITask>
    {
        private List<ITask> tasks;

        public TaskCollection()
        {
            tasks = new List<ITask>();
        }

        public void Add(ITask item)
        {
            int index = IndexOf(item);
            if (index > -1)
                tasks.Insert(index + 1, item);
            else
                tasks.Insert(~index, item);
        }

        public bool Remove(ITask item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                tasks.RemoveAt(index);
                return true;
            }
            else
            {
                return false;
            }

        }

        public ITask First()
        {
            if (tasks.Count > 0)
                return tasks[0];
            else
                return null;
        }

        /// <summary>
        /// Returns the index of <paramref name="item"/> using Binary Search
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(ITask item)
        {
            return tasks.BinarySearch(item, new TaskComparer());
        }

        public void RemoveAt(int index)
        {
            tasks.RemoveAt(index);
        }

        public void Clear()
        {
            tasks.Clear();
        }

        public bool Contains(ITask item)
        {
            return IndexOf(item) > 0;
        }

        public void CopyTo(ITask[] array, int arrayIndex)
        {
            tasks.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ITask> GetEnumerator()
        {
            return ((IEnumerable<ITask>)tasks).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ITask>)tasks).GetEnumerator();
        }

        public int Count
        {
            get
            {
                return tasks.Count;
            }
        }

        public ITask this[int index]
        {
            get
            {
                return tasks[index];
            }            
        }
    }
}
