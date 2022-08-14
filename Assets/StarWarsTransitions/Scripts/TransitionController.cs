using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SinaC.SWT
{
    public class TransitionController : MonoBehaviour
    {
        [SerializeField] private Canvas transitionCanvas;
        [SerializeField] private RawImage transitionOverlay;
        [SerializeField] private float transitionTime = 1f;
        [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private TransitionType defaultTransition = TransitionType.VerticalWipe;
        [SerializeField] private TransitionEntry[] transitions;

        private float t = float.MaxValue;
        private Camera currentCam = null;
        private RenderTexture rt;
        private Camera target;
        private TransitionType currentTransition;

        public Action OnStart;
        public Action OnComplete;
        
        private static readonly int ProgressPropID = Shader.PropertyToID("_Progress");
        private static readonly int ReversePropID = Shader.PropertyToID("_Reverse");

        private Dictionary<TransitionType, Material> transitionDictionary;

        public TransitionType CurrentTransition => currentTransition;

        private void Start() {
            transitionCanvas.enabled = false;
            transitionDictionary = new Dictionary<TransitionType, Material>();
            foreach (var entry in transitions) {
                transitionDictionary.Add(entry.Type, Instantiate(entry.Material));
            }
            
            SetTransitionType(defaultTransition);
        }

        public void SetTransitionType(TransitionType type) {
            if (transitionDictionary.ContainsKey(type)) {
                transitionOverlay.material = transitionDictionary[type];
                currentTransition = type;
            }
        }
        
        public void Transition(Camera to, Camera from = null, bool reverseDirection = false) {
            OnStart?.Invoke();
            target = to;
            rt = new RenderTexture(Screen.width, Screen.height, 32);
            transitionCanvas.enabled = true;

            currentCam = from ?? Camera.main;
            currentCam.targetTexture = rt;
            currentCam.tag = "Untagged";
            transitionOverlay.texture = rt;
            transitionOverlay.material.SetInt(ReversePropID, reverseDirection ? 1 : 0);

            target.enabled = true;
            target.tag = "MainCamera";
            target.targetTexture = null;
            t = 0;
        }

        private void Update() {
            if (t < 1) {
                t += Time.deltaTime / transitionTime;
                transitionOverlay.material.SetFloat(ProgressPropID, transitionCurve.Evaluate(Mathf.Clamp(t, 0, 1)));
                CheckComplete();
            }
        }

        private void CheckComplete() {
            if (t >= 1) {
                transitionCanvas.enabled = false;
                currentCam.enabled = false;
                OnComplete?.Invoke();
            }
        }

        [Serializable]
        public struct TransitionEntry
        {
            public TransitionType Type;
            public Material Material;
        }
    }
}