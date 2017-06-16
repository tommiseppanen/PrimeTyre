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
                return string.Format("{0,-10:F2}\t{1,-10:F2}\t{2,-10:F0}",
                    hit.ForwardSlip, hit.SidewaysSlip, hit.Force.y);
            });
            _text.text = info.Aggregate(string.Empty, (combined, next) => combined + "\n" + next);
        }
    }
}
