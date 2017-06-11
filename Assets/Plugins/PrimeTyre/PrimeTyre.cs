using UnityEngine;

namespace Assets.Plugins.PrimeTyre
{
    public class PrimeTyre : MonoBehaviour
    {

        [SerializeField]
        private float WheelRadius = 0.3f;
        [SerializeField]
        private float Inertia = 1.3f;

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
        private FrictionCurve ForwardFriction = new FrictionCurve
        {
            ExtremumSlip = 0.4f,
            ExtremumValue = 1.0f,
            AsymptoteSlip = 0.8f,
            AsymptoteValue = 0.5f,
            Stiffness = 1.0f
        };

        [SerializeField]
        private FrictionCurve SidewaysFriction = new FrictionCurve
        {
            ExtremumSlip = 0.2f,
            ExtremumValue = 1.0f,
            AsymptoteSlip = 0.5f,
            AsymptoteValue = 0.75f,
            Stiffness = 1.0f
        };

        public float MotorTorque { get; set; }
        public float BrakeTorque { get; set; }
        public float SteeringAngle { get; set; }

        public bool IsGrounded { get; private set; }

        private float _angularSpeed;
        private float _rotation;

        private float _differentialSlipRatio;
        private Vector3 _position;
        private float _previousSuspensionDistance;
        private float _normalForce;

        private const float RelaxationLenght = 0.0914f;

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
            var totalForce = _normalForce * Rigid.transform.up + 
                Rigid.transform.forward * longitudinalForce +
                Rigid.transform.right * lateralForce;
            
            if (IsGrounded)
                Rigid.AddForceAtPosition(totalForce, _position);

            UpdateAngularSpeed(longitudinalForce);
        }

        private void SetStreeringRotation()
        {
            var steeringRotation = Quaternion.AngleAxis(SteeringAngle, Rigid.transform.up);
            transform.rotation = Rigid.rotation * steeringRotation;
        }

        private Vector3 GetTyrePosition()
        {
            RaycastHit hit;
            IsGrounded = Physics.Raycast(new Ray(transform.position, -Rigid.transform.up), out hit,
                WheelRadius + _suspensionTravel);
            if (IsGrounded)
                return hit.point + Rigid.transform.up * WheelRadius;
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
            //TODO: substract rolling resistance
            _differentialSlipRatio += CalculateSlipDelta(_differentialSlipRatio, longitudinalSpeed) * Time.fixedDeltaTime;
            return Mathf.Sign(_differentialSlipRatio) * normalForce * ForwardFriction.CalculateCoefficient(_differentialSlipRatio);
        }

        private float CalculateSlipDelta(float differentialSlipRatio, float longitudinalSpeed)
        {
            var longitudinalAngularSpeed = _angularSpeed * WheelRadius;
            var slipdelta = (longitudinalAngularSpeed - longitudinalSpeed) -
                            Mathf.Abs(longitudinalSpeed) * differentialSlipRatio;
            return slipdelta / RelaxationLenght;
        }

        private float GetLateralForce(float normalForce, float longitudinalSpeed, float lateralSpeed)
        {
            var slipAngle = CalculateSlipAngle(longitudinalSpeed, lateralSpeed);
            var coefficient = SidewaysFriction.CalculateCoefficient(slipAngle);
            return Mathf.Sign(slipAngle) * coefficient * normalForce;
        }

        private float CalculateSlipAngle(float longitudinalSpeed, float lateralSpeed)
        {
            return Mathf.Atan2(lateralSpeed, Mathf.Abs(longitudinalSpeed));
        }

        private void UpdateAngularSpeed(float longitudinalForce)
        {
            //TODO: apply rolling resistance and longitudinalForce only if grounded
            var rollingResistanceForce = _angularSpeed * WheelRadius * _rollingResistance * _normalForce;
            var angularAcceleration = (MotorTorque - Mathf.Sign(_angularSpeed)* BrakeTorque - 
                (rollingResistanceForce + longitudinalForce) * WheelRadius) / Inertia;
            _angularSpeed += angularAcceleration * Time.fixedDeltaTime;
            _rotation = (_rotation + _angularSpeed * Time.fixedDeltaTime) % (2 * Mathf.PI);
        }

        public void GetWorldPose(out Vector3 position, out Quaternion rotation)
        {
            var tyreRotation = Quaternion.Euler((_rotation * 180.0f) / Mathf.PI, 0, 0);
            position = GetTyrePosition();
            rotation = transform.rotation * tyreRotation;
        }

        public bool GetGroundHit(out TyreHit hit)
        {
            hit = new TyreHit();
            if (IsGrounded)
                hit.ForwardSlip = _differentialSlipRatio;
            return IsGrounded;
        }
    }
}