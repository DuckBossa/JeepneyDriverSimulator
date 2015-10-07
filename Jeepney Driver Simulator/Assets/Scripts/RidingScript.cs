using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RidingScript : MonoBehaviour {
	public List<GameObject> positions =  new List<GameObject>();
	public Transform pedestrian_parent;
	public Transform pedestrian_drop;
	public GameObject test;
	private Stack<GameObject> seated = new Stack<GameObject>();
	PaymentSystem ps;
	DistanceAccumulator da;
	void Start () {

		ps = GetComponent<PaymentSystem>();
		da = GetComponent<DistanceAccumulator>();
		AddPassenger(test);
	}

	public void AddPassenger(GameObject passenger){
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
			//WANDER
		}
	}



	public void DropPassenger(){
		if(seated.Count > 0){
			GameObject passenger = seated.Pop ();
			passenger.gameObject.AddComponent<Rigidbody>();
			passenger.gameObject.GetComponent<CapsuleCollider>().enabled = true;
			passenger.gameObject.transform.SetParent(pedestrian_parent);
			passenger.gameObject.transform.position = pedestrian_drop.position;
		}
	}
	
}
