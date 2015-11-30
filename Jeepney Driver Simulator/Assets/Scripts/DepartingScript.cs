using UnityEngine;
using System.Collections;

namespace UnitySteer.Behaviors{
	public class DepartingScript : MonoBehaviour {
		public float searchRadius = 50f;
		public float stopDistance = 10f;

		private SteerForPursuit pursuitUS;
		private GameObject target;

		// Use this for initialization
		private void Awake () {
			pursuitUS = GetComponent<SteerForPursuit> ();
		}

		private void OnEnable () {
			target = Physics.OverlapSphere (transform.position, searchRadius, LayerMask.GetMask("Pathway"))[0].gameObject;
			pursuitUS.Quarry = target.GetComponent<DetectableObject>();
			pursuitUS.enabled = true;
		}

		private void OnDisable() {
			pursuitUS.enabled = false;
		}

		// Update is called once per frame
		private void FixedUpdate () {
			if((target.transform.position - gameObject.transform.position).magnitude < stopDistance){
				GetComponent<PedestrianController> ().changeState (PedestrianController.PedestrianState.Wandering);
			}
		}
	}
}