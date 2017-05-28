using UnityEngine;

namespace Assets.TestScenarios.StraightLine.Scripts
{
    public class NormalWheelController : MonoBehaviour
    {
        [SerializeField]
        private WheelCollider wheelCollider;

        [SerializeField]
        private bool driveWheel;

        [SerializeField]
        private Transform visualWheel;

        [SerializeField]
        private float slipLimit;

        private float torque = 200.0f;

        private void Update()
        {
            UpdateVisualWheel();
        }

        void FixedUpdate()
        {
            if (!driveWheel)
                return;

            WheelHit hit;
            wheelCollider.GetGroundHit(out hit);
            bool tractionControl;
            torque = StraigtLineTestLogic.GetTorque(torque, hit.forwardSlip, slipLimit, out tractionControl);
            wheelCollider.motorTorque = tractionControl ? 0 : torque;
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
 