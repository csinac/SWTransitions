using System;
using UnityEngine;
using UnityEngine.UI;

namespace SinaC.SWT
{
    public class TransitionController : MonoBehaviour
    {
        [SerializeField] private Canvas transitionCanvas;
        [SerializeField] private RawImage transitionOverlay;
        [SerializeField] private float transitionTime = 1f;

        private float t = float.MaxValue;
        private Camera currentCam = null;
        private RenderTexture rt;
        private Camera target;

        public Action OnStart;
        public Action OnComplete;
        
        private static readonly int ProgressPropID = Shader.PropertyToID("_Progress");

        private void Start() {
            transitionCanvas.enabled = false;
        }
        
        public void Transition(Camera to) {
            OnStart?.Invoke();
            target = to;
            rt = new RenderTexture(Screen.width, Screen.height, 32);
            transitionCanvas.enabled = true;

            currentCam = Camera.current;
            currentCam.targetTexture = rt;
            currentCam.tag = "Untagged";
            transitionOverlay.texture = rt;

            target.enabled = true;
            target.tag = "MainCamera";
            target.targetTexture = null;
            t = 0;
        }

        private void Update() {
            if (t < 1) {
                t += Time.deltaTime / transitionTime;
                transitionOverlay.material.SetFloat(ProgressPropID, Mathf.Clamp(t, 0, 1));
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
    }
}