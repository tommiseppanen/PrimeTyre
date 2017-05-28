using Assets.TestScenarios.StraightLine.Scripts;
using UnityEngine;

public class NormalCarController : MonoBehaviour {

    [SerializeField]
    private WheelCollider wheelCollider1;

    [SerializeField]
    private WheelCollider wheelCollider2;

    [SerializeField]
    private float slipLimit = 0.5f;

    private float torque = 200.0f;

    void FixedUpdate()
    {
        WheelHit hit;
        wheelCollider1.GetGroundHit(out hit);
        WheelHit hit2;
        wheelCollider2.GetGroundHit(out hit2);
        bool tractionControl;
        torque = StraigtLineTestLogic.GetTorque(torque, hit.forwardSlip, hit2.forwardSlip, slipLimit, out tractionControl);
        wheelCollider1.motorTorque = tractionControl ? 0 : torque;
        wheelCollider2.motorTorque = tractionControl ? 0 : torque;
    }
}
