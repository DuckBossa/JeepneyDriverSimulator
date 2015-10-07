using UnityEngine;
using System.Collections;
namespace UnitySteer.Behaviors{

	public class PedestrianController : MonoBehaviour {
		
		public SteerForWander wanderScript;
		public SteerForSphericalObstacles avoidScript;
		public  ridingScript; // check unity steer component
		
		
		
		public void Start(){
			wanderScript = GetComponent<SteerForWander> ();
			avoidScript = GetComponent<SteerForSphericalObstacles> ();

		}
		
		
		public enum PedestrianState{
			Wander,
			Riding,
			Looking
		}
		
		public PedestrianState currState;
		
		
		private void changeState(PedestrianState x){
			currState = x;
			wanderScript.gameObject.SetActive (false);
			ridingScript.gameObject.SetActive (false);
			switch (currState) {
			case PedestrianState.Looking:
				ridingScript.gameObject.SetActive(true);
				break;
			case PedestrianState.Wander:
				wanderScript.gameObject.SetActive(true);
				break;
			case PedestrianState.Riding:
				Debug.Log("STOP");
				break;
				
			}
		}
		
	}



}
