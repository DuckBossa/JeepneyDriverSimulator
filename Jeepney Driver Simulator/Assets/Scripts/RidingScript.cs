using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RidingScript : MonoBehaviour {
	public List<GameObject> positions =  new List<GameObject>();
	public Transform pedestrian_parent;
	public Transform pedestrian_drop;
	private Stack<GameObject> seated = new Stack<GameObject>();
	PaymentSystem ps;
	DistanceAccumulator da;
	void Awake () {

		ps = GetComponent<PaymentSystem>();
		da = GetComponent<DistanceAccumulator>();
	}

	public void AddPassenger(GameObject passenger){
		Debug.Log(ps == null);
		if(!ps.isFull()){
			passenger.GetComponent<CapsuleCollider>().enabled = false;
			Destroy(passenger.GetComponent<Rigidbody>());
			passenger.transform.SetParent(positions[seated.Count].transform);
			passenger.transform.localPosition = Vector3.zero;
			passenger.transform.localRotation = Quaternion.identity;
			seated.Push(passenger);
			ps.AddPayment();
			da.AddPassenger();
		}
		else{
			passenger.GetComponent<PedestrianController>().changeState(PedestrianController.PedestrianState.Wandering);
		}
	}



	public void DropPassenger(){
		if(seated.Count > 0){
			Debug.Log("LOOOOOOOOOOL");
			GameObject passenger = seated.Pop ();
			Rigidbody pr = passenger.gameObject.AddComponent<Rigidbody>();
			pr.constraints =  RigidbodyConstraints.FreezeRotation;
			passenger.gameObject.GetComponent<CapsuleCollider>().enabled = true;
			passenger.gameObject.transform.SetParent(pedestrian_parent);
			passenger.gameObject.transform.position = pedestrian_drop.position;
			passenger.gameObject.transform.rotation = Quaternion.identity;
			passenger.GetComponent<PedestrianController>().changeState(PedestrianController.PedestrianState.Departing);
		}
	}
	
}
