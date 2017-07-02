using Assets.Plugins.PrimeTyre;
using UnityEngine;

public class TestingMachine : MonoBehaviour
{
    [SerializeField]
    private Rigidbody Rigid;

    [SerializeField]
    private PrimeTyre Tyre; 

    private float _maxSteeringAngle = 30.0f;
    private float _maxBrakeTorque = 2000.0f;
    private float _maxMotorTorque = 1000.0f;
    private float _speed = 4.0f;

    void Update () {
        Tyre.BrakeTorque = 0.0f;

        var horizontalValue = Input.GetAxis("Horizontal");
        Tyre.SteeringAngle = _maxSteeringAngle * horizontalValue;

        var verticalValue = Input.GetAxis("Vertical");
        if (verticalValue < 0.0f)
            Tyre.BrakeTorque = -verticalValue * _maxBrakeTorque;
        else
            Tyre.MotorTorque = verticalValue * _maxMotorTorque;

        if (Input.GetButton("Jump"))
            Tyre.BrakeTorque = _maxBrakeTorque;
    }

    private void FixedUpdate()
    {
        Rigid.velocity = new Vector3(0, 0, _speed);
    }
}
