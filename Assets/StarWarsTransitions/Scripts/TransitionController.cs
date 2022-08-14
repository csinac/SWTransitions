using System;
using UnityEngine;
using UnityEngine.UI;

namespace SinaC.SWT
{
    public class TransitionController : MonoBehaviour
    {
        [SerializeField] private Canvas transitionCanvas;
        [SerializeField] private Image transitionOverlay;
        [SerializeField] private float transitionTime = 1f;
        [Space]
        [SerializeField] private Camera target;

        private float t = float.MaxValue;
        private Camera main = null;
        
        private static readonly int ProgressPropID = Shader.PropertyToID("_Progress");

        private void Start() {
            transitionCanvas.enabled = false;
        }
        
        private void Transition() {
            t = 0;
            transitionCanvas.enabled = true;
        }
        
        [ContextMenu("TestTransition")]
        private void TestTransition() {
            Invoke(nameof(Transition), 1);
        }
        
        private void Update() {
            if (t < 1) {
                t += Time.deltaTime;
                transitionOverlay.material.SetFloat(ProgressPropID, Mathf.Clamp(t, 0, 1));
                CheckComplete();
            }
        }

        private void CheckComplete() {
            if (t >= 1) {
                transitionCanvas.enabled = false;
            }
        }
    }
}