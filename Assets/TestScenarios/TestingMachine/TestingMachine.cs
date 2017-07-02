using Assets.Plugins.PrimeTyre;
using UnityEngine;

public class TestingMachine : MonoBehaviour
{
    [SerializeField]
    private Rigidbody Rigid;

    [SerializeField]
    private PrimeTyre Tyre; 

    private float _maxSteeringAngle = 30.0f;
    private float _maxBrakeTorque = 6000.0f;
    private float _speed = 4.0f;

    void Update () {
        Rigid.velocity = new Vector3(0, 0, _speed);

        Tyre.BrakeTorque = 0.0f;
        if (Input.GetButton("Jump"))
        {
            Tyre.BrakeTorque = _maxBrakeTorque;
        }

        var joystickValue = Input.GetAxis("Horizontal");
        Tyre.SteeringAngle = _maxSteeringAngle * joystickValue;
    }
}
