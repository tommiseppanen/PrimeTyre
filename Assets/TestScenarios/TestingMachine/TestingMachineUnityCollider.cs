using UnityEngine;

public class TestingMachineUnityCollider : MonoBehaviour
{
    [SerializeField]
    private Rigidbody Rigid;

    [SerializeField]
    private WheelCollider Tyre; 

    private float _maxSteeringAngle = 30.0f;
    private float _maxBrakeTorque = 2000.0f;
    private float _maxMotorTorque = 1000.0f;
    private float _speed = 4.0f;

    void Update () {
        Tyre.brakeTorque = 0.0f;

        var horizontalValue = Input.GetAxis("Horizontal");
        Tyre.steerAngle = _maxSteeringAngle * horizontalValue;

        var verticalValue = Input.GetAxis("Vertical");
        if (verticalValue < 0.0f)
            Tyre.brakeTorque = -verticalValue * _maxBrakeTorque;
        else
            Tyre.motorTorque = verticalValue * _maxMotorTorque;

        if (Input.GetButton("Jump"))
            Tyre.brakeTorque = _maxBrakeTorque;
    }

    private void FixedUpdate()
    {
        Rigid.velocity = new Vector3(0, 0, _speed);
    }
}
