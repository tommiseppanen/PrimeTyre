using Assets.Plugins.PrimeTyre;
using System.Collections.Generic;
using UnityEngine;

public class Acceleration : MonoBehaviour
{
    [SerializeField]
    private Rigidbody Rigid;

    [SerializeField]
    private LineRenderer Line;

    [SerializeField]
    private PrimeTyre Tyre; 

    private float _maxSteeringAngle = 30.0f;
    private float _maxBrakeTorque = 2000.0f;
    private float _maxMotorTorque = 1000.0f;
    private float _speed = 4.0f;

    private List<Vector3> _linePoints = new List<Vector3>();

    void Update () {
        Tyre.MotorTorque = 20.0f;
        TyreHit hit;
        Tyre.GetGroundHit(out hit);
        _linePoints.Add(new Vector3(0, hit.Force.z - 39.0f, Rigid.transform.position.z));
        if (_linePoints.Count > 100)
            _linePoints.RemoveAt(0);

        Line.SetPositions(_linePoints.ToArray());
    }
}
