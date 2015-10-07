using UnityEngine;
using System.Collections;
namespace UnitySteer.Behaviors{
	public class WanderingScript : MonoBehaviour {
		
		public int decayMultiplier = 2;
		public int changeChance = 16;
		public float rate = 1f;

		private float currTime;
		private int decay;
		private SteerForWander wanderUS;
		private SteerForSphericalObstacles avoidUS;

		private void Awake () {
			currTime = 0f;
			wanderUS = GetComponent<SteerForWander> ();
			avoidUS = GetComponent<SteerForSphericalObstacles> ();
		}

		private void OnEnable () {
			decay = int.MaxValue;
			wanderUS.enabled = true;
			avoidUS.enabled = true;
		}

		private void OnDisable() {
			decay = int.MaxValue;
			wanderUS.enabled = false;
			avoidUS.enabled = false;
		}

		private void FixedUpdate () {
			DecayToSearch ();
		}

		private void DecayToSearch() {
			currTime += Time.deltaTime;
			if (currTime >= rate) {
				//Decay to Search
				decay /= decayMultiplier;
//				Debug.Log (decay);
				if (UnityEngine.Random.Range (0, decay) < changeChance) {
					GetComponent<PedestrianController>().changeState(PedestrianController.PedestrianState.Searching);
				}
				
				currTime = 0f;
			}
		}
	}
}