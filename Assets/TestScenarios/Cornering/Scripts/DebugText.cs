using System.Collections.Generic;
using System.Linq;
using Assets.Plugins.PrimeTyre;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.TestScenarios.Cornering.Scripts
{
    public class DebugText : MonoBehaviour
    {
        [SerializeField]
        private List<PrimeTyre> _tyres;

        private Text _text;

        void Start ()
        {
            _text = GetComponent<Text>();
        }
	
        void Update ()
        {
            var info = _tyres.Select(t =>
            {
                TyreHit hit;
                t.GetGroundHit(out hit);
                return hit.ForwardSlip + " " + hit.SidewaysSlip + " " +  hit.Force.y;
            });
            _text.text = info.Aggregate(string.Empty, (combined, next) => combined + "\n" + next);
        }
    }
}
