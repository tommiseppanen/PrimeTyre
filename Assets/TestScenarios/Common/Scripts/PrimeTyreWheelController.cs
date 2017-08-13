using Assets.Plugins.PrimeTyre;
using UnityEngine;

namespace Assets.TestScenarios.StraightLine.Scripts
{
    public class PrimeTyreWheelController : MonoBehaviour
    {
        [SerializeField]
        private PrimeTyre tyre;

        [SerializeField]
        private Transform visualWheel;

        void Update()
        {
            UpdateVisualWheel();
        }

        void UpdateVisualWheel()
        {
            Vector3 position;
            Quaternion rotation;
            tyre.GetWorldPose(out position, out rotation);
            visualWheel.transform.position = position;
            visualWheel.transform.rotation = rotation;
            visualWheel.transform.Rotate(new Vector3(0,0,90));
        }
    }
}