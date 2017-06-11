using Assets.Plugins.PrimeTyre;
using Assets.TestScenarios.StraightLine.Scripts;
using UnityEngine;

namespace Assets.TestScenarios.Cornering.Scripts
{
    public class PrimeTyreCarController : MonoBehaviour
    {

        [SerializeField]
        private PrimeTyre RearRightWheel;

        [SerializeField]
        private PrimeTyre RearLeftWheel;

        private float _maxEngineTorque = 1200.0f;
        private float _maxBrakeTorque = 2000.0f;

        void FixedUpdate()
        {
            var joystick = Input.GetAxis("Vertical");
            if (joystick < 0.0f)
            {
                RearRightWheel.MotorTorque = 0;
                RearLeftWheel.MotorTorque = 0;
                RearLeftWheel.BrakeTorque = -joystick * _maxBrakeTorque;
                RearRightWheel.BrakeTorque = -joystick * _maxBrakeTorque;
            }
            else
            {
                RearLeftWheel.BrakeTorque = 0.0f;
                RearRightWheel.BrakeTorque = 0.0f;
                RearRightWheel.MotorTorque = joystick * _maxEngineTorque;
                RearLeftWheel.MotorTorque = joystick * _maxEngineTorque;
            }
        }
    }
}

