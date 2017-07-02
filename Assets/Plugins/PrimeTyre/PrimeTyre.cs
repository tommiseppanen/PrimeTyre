using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Assets.Plugins.PrimeTyre
{
    public class PrimeTyre : MonoBehaviour
    {

        [SerializeField]
        private float _wheelRadius = 0.353f;
        [SerializeField]
        private float _inertia = 3f;

        //TODO: create automatic look up for parent rigidbody
        [SerializeField]
        private Rigidbody Rigid;

        [SerializeField]
        private float _suspensionTravel = 0.3f;

        [SerializeField]
        private float _spring = 35000.0f;

        [SerializeField]
        private float _damper = 4500.0f;

        [SerializeField]
        private float _rollingResistance = 0.001f;

        [SerializeField]
        private FrictionCurve _forwardFriction = new FrictionCurve
        {
            ExtremumSlip = 0.4f,
            ExtremumValue = 1.0f,
            AsymptoteSlip = 0.8f,
            AsymptoteValue = 0.5f,
            Stiffness = 1.0f
        };

        [SerializeField]
        private FrictionCurve _sidewaysFriction = new FrictionCurve
        {
            ExtremumSlip = 15.0f,
            ExtremumValue = 1.0f,
            AsymptoteSlip = 30.0f,
            AsymptoteValue = 0.75f,
            Stiffness = 1.0f
        };

        public float MotorTorque { get; set; }

        private float _brakeTorque;
        public float BrakeTorque
        {
            get
            {
                return _brakeTorque;
            }
            set
            {
                _brakeTorque = Mathf.Abs(value);
            }
        }

        public float SteeringAngle { get; set; }

        public bool IsGrounded { get; private set; }

        private float _angularSpeed;
        private float _rotation;

        private float _differentialSlipRatio; //percentage
        private float _differentialTanOfSlipAngle;
        private float _slipAngle; //degrees
        private Vector3 _totalForce;
        private Vector3 _position;
        private float _previousSuspensionDistance;
        private float _normalForce;

        private const float RelaxationLengthLongitudinal = 0.103f;
        private const float RelaxationLengthLateral = 0.303f;

        void Start()
        {
            _previousSuspensionDistance = _suspensionTravel;
            _position = GetTyrePosition();
        }

        private void FixedUpdate()
        {
            SetStreeringRotation();
            var newPosition = GetTyrePosition();
            var velocity = (newPosition - _position) / Time.fixedDeltaTime;
            _position = newPosition;
            _normalForce = GetVerticalForce(_position);

            var longitudinalSpeed = Vector3.Dot(velocity, transform.forward);
            var lateralSpeed = Vector3.Dot(velocity, -transform.right);

            var longitudinalForce = GetLongitudinalForce(_normalForce, longitudinalSpeed);
            var lateralForce = GetLateralForce(_normalForce, longitudinalSpeed, lateralSpeed);
            _totalForce = _normalForce * Rigid.transform.up + 
                Rigid.transform.forward * longitudinalForce +
                Rigid.transform.right * lateralForce;
            
            if (IsGrounded)
                Rigid.AddForceAtPosition(_totalForce, _position);

            UpdateAngularSpeed(longitudinalForce);
        }

        private void SetStreeringRotation()
        {
            var steeringRotation = Quaternion.AngleAxis(SteeringAngle, Rigid.transform.up);
            transform.rotation = steeringRotation * Rigid.rotation;
        }

        private Vector3 GetTyrePosition()
        {
            RaycastHit hit;
            IsGrounded = Physics.Raycast(new Ray(transform.position, -Rigid.transform.up), out hit,
                _wheelRadius + _suspensionTravel);
            if (IsGrounded)
                return hit.point + Rigid.transform.up * _wheelRadius;
            return transform.position - Rigid.transform.up * _suspensionTravel;
        }

        private float GetVerticalForce(Vector3 tyrePosition)
        {
            var distance = Vector3.Distance(transform.position - Rigid.transform.up * _suspensionTravel, tyrePosition);
            var springForce = _spring * distance;
            var damperForce = _damper * ((distance - _previousSuspensionDistance) / Time.fixedDeltaTime);
            _previousSuspensionDistance = distance;
            return springForce + damperForce;
        }

        private float GetLongitudinalForce(float normalForce, float longitudinalSpeed)
        {
            var delta = CalculateSlipDelta(_differentialSlipRatio, longitudinalSpeed);
            _differentialSlipRatio +=  delta * Time.fixedDeltaTime;
            return Mathf.Sign(DampenForLowSpeeds(_differentialSlipRatio, delta, longitudinalSpeed, 0.02f)) 
                * normalForce * _forwardFriction.CalculateCoefficient(_differentialSlipRatio);           
        }

        //tau = oscillation period (experimental)
        private float DampenForLowSpeeds(float value, float delta, float speed, float tau)
        {
            if (speed > 0.15f)
                tau = 0.0f;
            return value + tau * delta;
        }

        private float CalculateSlipDelta(float differentialSlipRatio, float longitudinalSpeed)
        {
            var longitudinalAngularSpeed = _angularSpeed * _wheelRadius;
            var slipdelta = (longitudinalAngularSpeed - longitudinalSpeed) -
                            Mathf.Abs(longitudinalSpeed) * differentialSlipRatio;
            return slipdelta / RelaxationLengthLongitudinal;
        }

        private float GetLateralForce(float normalForce, float longitudinalSpeed, float lateralSpeed)
        {
            _slipAngle = CalculateSlipAngle(longitudinalSpeed, lateralSpeed);
            var coefficient = _sidewaysFriction.CalculateCoefficient(_slipAngle);
            //Limit lateral friction so that we won't go over traction circle budget
            coefficient *= Mathf.Sqrt(1 - Mathf.Pow(_forwardFriction.CalculateCoefficient(_differentialSlipRatio) / _forwardFriction.ExtremumValue, 2));
            return Mathf.Sign(_slipAngle) * coefficient * normalForce;
        }

        private float CalculateSlipAngle(float longitudinalSpeed, float lateralSpeed)
        {
            float delta = lateralSpeed - Mathf.Abs(longitudinalSpeed) * _differentialTanOfSlipAngle;
            delta /= RelaxationLengthLateral;
            _differentialTanOfSlipAngle += delta * Time.fixedDeltaTime;
            return Mathf.Atan(DampenForLowSpeeds(_differentialTanOfSlipAngle, delta, lateralSpeed, 0.1f)) * Mathf.Rad2Deg;
        }

        private void UpdateAngularSpeed(float longitudinalForce)
        {
            var rollingResistanceForce = _angularSpeed * _wheelRadius * _rollingResistance * _normalForce;
            var torqueFromTyreForces = (rollingResistanceForce + longitudinalForce) * _wheelRadius;
            var angularAcceleration = (MotorTorque - Mathf.Sign(_angularSpeed) * _brakeTorque -
                 torqueFromTyreForces) / _inertia;
            if (WillBrakesLock(angularAcceleration, torqueFromTyreForces))
            {
                _angularSpeed = 0.0f;
                return;
            }
            _angularSpeed += angularAcceleration * Time.fixedDeltaTime;
            _rotation = (_rotation + _angularSpeed * Time.fixedDeltaTime) % (2 * Mathf.PI);
        }

        private bool WillBrakesLock(float angularAcceleration, float torqueFromTyreForces)
        {
            if (_brakeTorque < Mathf.Abs(MotorTorque - torqueFromTyreForces))
                return false;

            if (_brakeTorque > 0.0f && Mathf.Sign(_angularSpeed) != 
                Mathf.Sign(_angularSpeed + angularAcceleration * Time.fixedDeltaTime))
                return true;
            return false;
        }

        public void GetWorldPose(out Vector3 position, out Quaternion rotation)
        {
            var tyreRotation = Quaternion.Euler(_rotation * Mathf.Rad2Deg, 0, 0);
            position = GetTyrePosition();
            rotation = transform.rotation * tyreRotation;
        }

        public bool GetGroundHit(out TyreHit hit)
        {
            hit = new TyreHit();
            if (IsGrounded)
            {
                hit.ForwardSlip = _differentialSlipRatio;
                hit.SidewaysSlip = _slipAngle;
                hit.Force = _totalForce;
            }
            return IsGrounded;
        }

        public void OnDrawGizmosSelected()
        {
            #if UNITY_EDITOR
            Gizmos.color = Color.yellow;
            Handles.color = Color.green;
            Vector3 position;
            Quaternion rotation;
            GetWorldPose(out position, out rotation);

            //Draw tyre
            Handles.DrawWireDisc(position, transform.right, _wheelRadius);

            //Draw suspension
            Gizmos.DrawLine(transform.position,
                transform.position - Rigid.transform.up * _suspensionTravel);

            var force = (_totalForce - _normalForce*transform.up) / 1000.0f;
            Gizmos.DrawLine(transform.position, transform.position + force);
            #endif
        }

    }
}