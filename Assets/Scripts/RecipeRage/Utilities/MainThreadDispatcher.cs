using System;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeRage.Utilities
{
    public class MainThreadDispatcher : MonoSingleton<MainThreadDispatcher>
    {
        private static readonly Queue<Action> ExecutionQueue = new Queue<Action>();

        private void Update()
        {
            lock (ExecutionQueue)
            {
                while (ExecutionQueue.Count > 0)
                {
                    ExecutionQueue.Dequeue().Invoke();
                }
            }
        }

        public void Enqueue(Action action)
        {
            lock (ExecutionQueue)
            {
                ExecutionQueue.Enqueue(action);
            }
        }
    }
}