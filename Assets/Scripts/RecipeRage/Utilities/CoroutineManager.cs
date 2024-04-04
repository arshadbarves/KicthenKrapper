using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecipeRage.Utilities
{
    public class CoroutineManager
    {
        private static CoroutineManager _instance;
        private readonly MonoBehaviour _coroutineExecutor;

        private readonly Dictionary<IEnumerator, Coroutine> _runningCoroutines = new Dictionary<IEnumerator, Coroutine>();

        public static CoroutineManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CoroutineManager();
                }
                return _instance;
            }
        }

        public CoroutineManager()
        {
            GameObject obj = new GameObject("CoroutineExecutor");
            _coroutineExecutor = obj.AddComponent<CoroutineExecutor>();
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            Coroutine coroutine = _coroutineExecutor.StartCoroutine(routine);
            _runningCoroutines.Add(routine, coroutine);
            return coroutine;
        }

        public void StopCoroutine(IEnumerator routine)
        {
            if (_runningCoroutines.ContainsKey(routine))
            {
                _coroutineExecutor.StopCoroutine(_runningCoroutines[routine]);
                _runningCoroutines.Remove(routine);
            }
        }

        public void StopAllCoroutines()
        {
            foreach (var coroutine in _runningCoroutines.Values)
            {
                _coroutineExecutor.StopCoroutine(coroutine);
            }
            _runningCoroutines.Clear();
        }
    }

    public class CoroutineExecutor : MonoBehaviour { }
}