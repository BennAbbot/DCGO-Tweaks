using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace DCGO_Tweaks
{

    [RegisterTypeInIl2Cpp]
    public class FPSCounter : MonoBehaviour
    {
        public FPSCounter(IntPtr ptr) : base(ptr) { }

        private float deltaTime = 0.0f;
        private GUIStyle style = new GUIStyle();

        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = 24;
            style.normal.textColor = Color.green;
        }

        void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            float fps = 1.0f / deltaTime;
            float ms = deltaTime * 1000.0f;
            string text = string.Format("{0:0.} FPS ({1:0.0} ms)", fps, ms);

            // Change color based on FPS
            if (fps < 30)
                style.normal.textColor = Color.red;
            else if (fps < 60)
                style.normal.textColor = Color.yellow;
            else
                style.normal.textColor = Color.green;

            GUI.Label(new Rect(10, 10, 200, 30), text, style);
        }
    }
}
