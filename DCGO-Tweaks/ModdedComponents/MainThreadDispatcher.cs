using MelonLoader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DCGO_Tweaks.ModdedComponents
{
    [RegisterTypeInIl2Cpp]
    class MainThreadDispatcher : MonoBehaviour
    {
        public MainThreadDispatcher(IntPtr ptr) : base(ptr) { }

        public static MainThreadDispatcher Instance { get; private set;  }

        Queue<Action> _actions = new Queue<Action>();

        Queue<Action> _image_actions = new Queue<Action>();

        public static MainThreadDispatcher CreateDispatcher()
        {
            GameObject obj = new GameObject();
            DontDestroyOnLoad(obj);
            return obj.AddComponent<MainThreadDispatcher>();
        }

        public void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            else
            {
                Instance = this;
            }
        }

        public void Enqueue(Action action)
        {
            lock (_actions)
            {
                _actions.Enqueue(action);
            }
        }

        public void EnqueueImageLoad(Action action)
        {
            lock (_image_actions)
            {
                _image_actions.Enqueue(action);
            }
        }


        void Update()
        {
            lock (_actions)
            {
                while (_actions.Count > 0)
                {
                    Action action = _actions.Dequeue();
                    action.Invoke();
                }
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

            lock (_image_actions)
            {
                while (_image_actions.Count > 0 && stopwatch.Elapsed.TotalMilliseconds < 5)
                {
                    Action action = _image_actions.Dequeue();
                    action.Invoke();
                }
            }

            stopwatch.Stop();
        }
    }
}
