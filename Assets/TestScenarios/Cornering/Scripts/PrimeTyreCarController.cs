using Assets.Plugins.PrimeTyre;
using UnityEngine;

namespace Assets.TestScenarios.Cornering.Scripts
{
    public class PrimeTyreCarController : MonoBehaviour
    {
        [SerializeField]
        private PrimeTyre FrontRightWheel;

        [SerializeField]
        private PrimeTyre FrontLeftWheel;

        [SerializeField]
        private PrimeTyre RearRightWheel;

        [SerializeField]
        private PrimeTyre RearLeftWheel;

        private float _maxEngineTorque = 1600.0f;
        private float _maxBrakeTorque = 2500.0f;

        private float _maxSteeringAngle = 30.0f;

        private void FixedUpdate()
        {
            SetSteerAngles();
            SetTorques();
        }

        private void SetSteerAngles()
        {
            var joystickValue = Input.GetAxis("Horizontal");
            FrontRightWheel.SteeringAngle = _maxSteeringAngle * joystickValue;
            FrontLeftWheel.SteeringAngle = _maxSteeringAngle * joystickValue;
        }

        private void SetTorques()
        {
            var joystickValue = Input.GetAxis("Vertical");
            if (joystickValue < 0.0f)
            {
                RearRightWheel.MotorTorque = 0;
                RearLeftWheel.MotorTorque = 0;
                RearLeftWheel.BrakeTorque = -joystickValue * _maxBrakeTorque;
                RearRightWheel.BrakeTorque = -joystickValue * _maxBrakeTorque;
                FrontRightWheel.BrakeTorque = -joystickValue * _maxBrakeTorque;
                FrontLeftWheel.BrakeTorque = -joystickValue * _maxBrakeTorque;
            }
            else
            {
                RearLeftWheel.BrakeTorque = 0.0f;
                RearRightWheel.BrakeTorque = 0.0f;
                FrontRightWheel.BrakeTorque = 0.0f;
                FrontLeftWheel.BrakeTorque = 0.0f;
                RearRightWheel.MotorTorque = joystickValue * _maxEngineTorque;
                RearLeftWheel.MotorTorque = joystickValue * _maxEngineTorque;
            }

            //Handbrake
            if (Input.GetButton("Jump"))
            {
                RearLeftWheel.BrakeTorque = _maxBrakeTorque;
                RearRightWheel.BrakeTorque = _maxBrakeTorque;
            }

            //Front handbrake just for testing
            if (Input.GetButton("Fire1"))
            {
                FrontLeftWheel.BrakeTorque = _maxBrakeTorque;
                FrontRightWheel.BrakeTorque = _maxBrakeTorque;
            }
        }
    }
}