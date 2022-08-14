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

        private void Transition() {
            tc.Transition(dest);

            Camera temp = src;
            src = dest;
            dest = temp;
        }

        [ContextMenu("TestTransition")]
        private void TestTransition() {
            if (locked)
                return;
            
            Invoke(nameof(Transition), 1);
        }
    }
}
