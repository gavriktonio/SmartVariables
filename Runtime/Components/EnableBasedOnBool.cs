using UnityEngine;

namespace SmartVariables
{
    public class EnableBasedOnBool : MonoBehaviour
    {
        public BoolReference boolReference;

        public GameObject[] enableWhenTrue;
        public GameObject[] enableWhenFalse;

        public Component[] enableComponentsWhenTrue = new Component[0];
        public Component[] enableComponentsWhenFalse = new Component[0];

        public BoolReference[] trueWhenTrue;
        public BoolReference[] trueWhenFalse;

        void OnEnable()
        {
            SetupListener();
        }

        private void OnDisable()
        {
            RemoveListener();
        }

        private void OnValidate()
        {
            ValidateComponentTargets(enableComponentsWhenTrue);
            ValidateComponentTargets(enableComponentsWhenFalse);
        }

        void SetupListener()
        {
            boolReference.AddListener(OnBoolChanged);
            OnBoolChanged(false, boolReference.Value);
        }

        void RemoveListener()
        {
            boolReference.RemoveListener(OnBoolChanged);
        }

        void OnBoolChanged(bool oldBool, bool newBool)
        {
            foreach (GameObject i in enableWhenTrue)
            {
                i.SetActive(newBool);
            }

            foreach (GameObject i in enableWhenFalse)
            {
                i.SetActive(!newBool);
            }

            ApplyComponentTargets(enableComponentsWhenTrue, newBool);
            ApplyComponentTargets(enableComponentsWhenFalse, !newBool);

            foreach (BoolReference i in trueWhenTrue)
            {
                i.Value = newBool;
            }

            foreach (BoolReference i in trueWhenFalse)
            {
                i.Value = !newBool;
            }
        }

        private void ValidateComponentTargets(Component[] targets)
        {
            if (targets == null)
                return;

            foreach (Component target in targets)
            {
                if (target != null && !(target is Behaviour) && !(target is Renderer)
                    && !(target is Collider) && !(target is Collider2D))
                    ReportUnsupportedTarget(target);
            }
        }

        private void ApplyComponentTargets(Component[] targets, bool enabled)
        {
            if (targets == null)
                return;

            foreach (Component target in targets)
            {
                if (target == null)
                    continue;

                if (target is Behaviour behaviour)
                    behaviour.enabled = enabled;
                else if (target is Renderer renderer)
                    renderer.enabled = enabled;
                else if (target is Collider collider)
                    collider.enabled = enabled;
                else if (target is Collider2D collider2D)
                    collider2D.enabled = enabled;
                else
                    ReportUnsupportedTarget(target);
            }
        }

        private void ReportUnsupportedTarget(Component target)
        {
            Debug.LogError($"Enable Based on Bool cannot toggle {target.GetType().Name}. Assign a Behaviour, Renderer, Collider, or Collider2D.", this);
        }
    }
}