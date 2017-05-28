using Assets.Plugins.PrimeTyre;
using Assets.TestScenarios.StraightLine.Scripts;
using UnityEngine;

public class PrimeTyreCarController : MonoBehaviour {

    [SerializeField]
    private PrimeTyre wheelCollider1;

    [SerializeField]
    private PrimeTyre wheelCollider2;

    [SerializeField]
    private float slipLimit = 0.5f;

    private float torque = 200.0f;

    void FixedUpdate()
    {
        TyreHit hit;
        wheelCollider1.GetGroundHit(out hit);
        TyreHit hit2;
        wheelCollider2.GetGroundHit(out hit2);
        bool tractionControl;
        torque = StraigtLineTestLogic.GetTorque(torque, hit.ForwardSlip, hit2.ForwardSlip, slipLimit, out tractionControl);
        wheelCollider1.MotorTorque = tractionControl ? 0 : torque;
        wheelCollider2.MotorTorque = tractionControl ? 0 : torque;
    }
}
