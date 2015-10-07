using UnityEngine;
using System.Collections;

public class PedestrianController : MonoBehaviour {

	public MonoBehaviour wanderingScript;
	public MonoBehaviour searchingScript;
	public MonoBehaviour ridingScript;
	public MonoBehaviour departingScript;

	public enum PedestrianState{
		Wandering,
		Searching,
		Riding,
		Departing
	}
	
	public PedestrianState currState;

	public void Start (){
		changeState (PedestrianState.Searching);
	}
	
	public void changeState(PedestrianState x){
		currState = x;
		wanderingScript.enabled = false;
		searchingScript.enabled = false;
		ridingScript.enabled = false;
		departingScript.enabled = false;
		switch (currState) {
		case PedestrianState.Wandering:
			wanderingScript.enabled = true;
			break;
		case PedestrianState.Searching:
			searchingScript.enabled = true;
			break;
		case PedestrianState.Riding:
			ridingScript.enabled = true;
			break;
		case PedestrianState.Departing:
			departingScript.enabled = true;
			break;
			
		}
	}
}