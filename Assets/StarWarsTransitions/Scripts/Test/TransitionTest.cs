using System;
using UnityEngine;

namespace SinaC.SWT
{
    public class TransitionTest : MonoBehaviour
    {
        [SerializeField] private Camera A;
        [SerializeField] private Camera B;
        [SerializeField] private TransitionController tc;

        private Camera src, dest;
        private bool locked;
        
        private void Start() {
            A.enabled = true;
            B.enabled = false;

            src = A;
            dest = B;

            tc.OnStart += () => locked = true;
            tc.OnComplete += () => locked = false;
        }

        private void Transition(bool reverse) {
            tc.Transition(dest, src, reverse);

            Camera temp = src;
            src = dest;
            dest = temp;
        }

        private void OnGUI() {
            if (GUI.Button(new Rect(25, 25, 300, 50), "Switch Camera")) {
                if (!locked)
                    Transition(false);
            }
            if (GUI.Button(new Rect(25, 100, 300, 50), "Switch Camera (reverse)")) {
                if (!locked)
                    Transition(true);
            }
        }
    }
}
