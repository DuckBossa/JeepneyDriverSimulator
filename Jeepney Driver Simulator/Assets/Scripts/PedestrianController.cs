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
		changeState (PedestrianState.Wandering);
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
			GetComponent<MeshRenderer>().material.color = Color.blue;
			break;
		case PedestrianState.Searching:
			GetComponent<MeshRenderer>().material.color = Color.red;
			searchingScript.enabled = true;
			break;
		case PedestrianState.Riding:
			GetComponent<MeshRenderer>().material.color = Color.green;
			ridingScript.enabled = true;
			break;
		case PedestrianState.Departing:
			GetComponent<MeshRenderer>().material.color = Color.grey;
			departingScript.enabled = true;
			break;
			
		}
	}
}