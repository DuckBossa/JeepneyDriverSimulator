using UnityEngine;
using System.Collections;
namespace UnitySteer.Behaviors{
	public class SearchingScript : MonoBehaviour {
		
		public int decayMultiplier = 2;
		public int changeChance = 16;
		public float rate = 1f;
		public float searchRadius = 15f;
		public float stopDistance = 10f;

		private float currTime;
		private int decay;
		private SteerForPursuit pursuitUS;
		private GameObject target;
		
		private void Awake () {
			currTime = 0f;
			pursuitUS = GetComponent<SteerForPursuit> ();
		}
		
		private void OnEnable () {
			target = null;
			decay = int.MaxValue;
			pursuitUS.enabled = true;
		}
		
		private void OnDisable() {
			decay = int.MaxValue;
			pursuitUS.enabled = false;
		}
		
		private void FixedUpdate () {

			if(target != null) WalkingToJeep();
			else SearchJeepney();

			DecayToWander ();
		}

		private void SearchJeepney(){
			Collider[] collided = Physics.OverlapSphere (transform.position, searchRadius, LayerMask.GetMask("Vehicle"));
			if (collided.Length > 0) {
				target = collided[0].gameObject;
				pursuitUS.Quarry = collided[0].GetComponentInParent<DetectableObject> ();
				pursuitUS.enabled = true;
			}
		}

		private void WalkingToJeep() {
			if((target.transform.position - gameObject.transform.position).magnitude < stopDistance)
				target = null;
				GetComponent<PedestrianController> ().changeState (PedestrianController.PedestrianState.Riding);
		}

		private void DecayToWander() {
			currTime += Time.deltaTime;
			if (currTime >= rate) {
				decay /= decayMultiplier;
				Debug.Log (decay);
				if (UnityEngine.Random.Range (0, decay) < changeChance) {
					GetComponent<PedestrianController>().changeState(PedestrianController.PedestrianState.Wandering);
				}
				currTime = 0f;
			}
		}
	}
}