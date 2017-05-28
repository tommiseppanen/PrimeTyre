using UnityEngine;

namespace Assets.TestScenarios.StraightLine.Scripts
{
    public class NormalWheelController : MonoBehaviour
    {
        [SerializeField]
        private WheelCollider wheelCollider;

        [SerializeField]
        private Transform visualWheel;

        private void Update()
        {
            UpdateVisualWheel();
        }

        private void UpdateVisualWheel()
        {
            Vector3 position;
            Quaternion rotation;
            wheelCollider.GetWorldPose(out position, out rotation);
            visualWheel.transform.position = position;
            visualWheel.transform.rotation = rotation;
            visualWheel.transform.Rotate(new Vector3(0,0,90));
        }
    }
}
 