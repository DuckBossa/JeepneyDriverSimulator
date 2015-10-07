using UnityEngine;
using System.Collections;
namespace UnitySteer.Behaviors{
	public class SearchingScript : MonoBehaviour {
		
		public int decayMultiplier = 2;
		public int changeChance = 16;
		public float rate = 1f;
		public float searchRadius = 15f;

		private float currTime;
		private int decay;
		private SteerForPursuit pursuitUS;
		
		private void Awake () {
			currTime = 0f;
			pursuitUS = GetComponent<SteerForPursuit> ();
		}
		
		private void Start () {
			decay = int.MaxValue;
			pursuitUS.enabled = true;
		}
		
		private void OnDisable() {
			pursuitUS.enabled = false;
		}
		
		private void FixedUpdate () {

			SearchJeepney ();
			DecayToWander ();
		}

		private void SearchJeepney(){
			Collider[] collided = Physics.OverlapSphere (transform.position, searchRadius, LayerMask.GetMask("Vehicle"));
			if (collided.Length > 0) {
				foreach( Collider v in collided){
					Debug.Log (v.gameObject.name);
				}
				pursuitUS.Quarry = collided[0].GetComponentInParent<DetectableObject> ();
				pursuitUS.enabled = true;
				GetComponent<PedestrianController> ().changeState (PedestrianController.PedestrianState.Riding);
			} else {
				Debug.Log("NO JEEP");
			}
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