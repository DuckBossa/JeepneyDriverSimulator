using UnityEngine;
using System.Collections;

public class PedestrianController : MonoBehaviour {

	public MonoBehaviour wanderingScript;
	public MonoBehaviour searchingScript;
	public MonoBehaviour ridingScript;
	public MonoBehaviour departingScript;
	public Animator a;
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
			a.SetBool("Wandering",true);
			a.SetBool("Sitting",false);
			a.SetBool("Searching",false);
			//GetComponent<MeshRenderer>().material.color = Color.blue;
			//change animation
			break;
		case PedestrianState.Searching:
			//GetComponent<MeshRenderer>().material.color = Color.red;
			searchingScript.enabled = true;
			a.SetBool("Searching",true);
			a.SetBool("Wandering",false);
			a.SetBool("Sitting",false);


			break;
		case PedestrianState.Riding:
			//GetComponent<MeshRenderer>().material.color = Color.green;
			ridingScript.enabled = true;
			a.SetBool("Sitting",true);
			a.SetBool("Searching",false);
			a.SetBool("Searching",false);
			break;
		case PedestrianState.Departing:
			//GetComponent<MeshRenderer>().material.color = Color.grey;
			departingScript.enabled = true;
			a.SetBool("Wandering",true);
			a.SetBool("Sitting",false);
			a.SetBool("Searching",false);
			break;
		}
	}
}