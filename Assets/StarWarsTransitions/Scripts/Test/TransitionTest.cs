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

        private TransitionType[] types = {
            TransitionType.VerticalSplit,
            TransitionType.VerticalWipe
        };

        private bool dropdownOpen = false;
        
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
            if (GUI.Button(new Rect(25, 85, 300, 50), "Switch Camera (reverse)")) {
                if (!locked)
                    Transition(true);
            }
            
            if(GUI.Button(new Rect(25, 145, 300, 50), $"Transition: {tc.CurrentTransition}")) {
                dropdownOpen = !dropdownOpen;
            }
            if(dropdownOpen) {
                for(int i = 0; i < types.Length; i++) {
                    if (GUI.Button(new Rect(25, 205 + (55 * i), 120, 50), types[i].ToString())) {
                        dropdownOpen = false;
                        tc.SetTransitionType(types[i]);
                    }
                }
            }
        }
    }
}
