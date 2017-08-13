using System.Collections.Generic;
using System.Linq;
using Assets.Plugins.PrimeTyre;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.TestScenarios.Cornering.Scripts
{
    public class DebugTextUnityCollider : MonoBehaviour
    {
        [SerializeField]
        private List<WheelCollider> _tyres;

        [SerializeField]
        private Rigidbody _rigidbody;

        private Text _text;
        private float _previousVelocity = 0.0f;

        void Start ()
        {
            _text = GetComponent<Text>();
        }
	
        void Update ()
        {
            var info = _tyres.Select(t =>
            {
                WheelHit hit;
                t.GetGroundHit(out hit);
                return string.Format("{0,-10:F2} {1,-10:F2} {2,-10:F0}",
                    hit.forwardSlip, hit.sidewaysSlip, hit.force);
            });
            _text.text = info.Aggregate(string.Empty, (combined, next) => combined + "\n" + next);
            var velocity = _rigidbody.velocity.magnitude;
            _text.text += string.Format("\n{0:F2} {1:F2}", velocity, (velocity - _previousVelocity) / Time.deltaTime);
            _previousVelocity = velocity;
        }
    }
}
