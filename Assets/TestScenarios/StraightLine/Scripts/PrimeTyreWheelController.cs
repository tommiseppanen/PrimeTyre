using Assets.Plugins.PrimeTyre;
using UnityEngine;

namespace Assets.TestScenarios.StraightLine.Scripts
{
    public class PrimeTyreWheelController : MonoBehaviour
    {
        [SerializeField]
        private PrimeTyre tyre;

        [SerializeField]
        private bool driveWheel;

        [SerializeField]
        private Transform visualWheel;

        [SerializeField]
        private float slipLimit;

        private float torque = 200.0f;

        void Update()
        {
            UpdateVisualWheel();
        }

        void FixedUpdate()
        {
            if (!driveWheel)
                return;

            TyreHit hit;
            tyre.GetGroundHit(out hit);
            bool tractionControl;
            torque = StraigtLineTestLogic.GetTorque(torque, hit.ForwardSlip, slipLimit, out tractionControl);
            tyre.MotorTorque = tractionControl ? 0 : torque;
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