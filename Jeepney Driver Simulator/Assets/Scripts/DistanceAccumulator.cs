using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DistanceAccumulator : MonoBehaviour {

	public float MAX_DIST = 50f;
	public float MIN_DIST = 25f;
	public float stop_time = 2f;
	public float slow_vel = 5f;
	private RidingScript rs;
	private PaymentSystem ps;
	private List<float> dist_left;
	private Vector3 prev;
	private int num_para;
	private float timeEl;
	void Awake(){
		prev = transform.position;
		dist_left = new List<float>();
		rs = GetComponent<RidingScript>();
		ps = GetComponent<PaymentSystem>();
		num_para = 0;
		timeEl = 0;
	}


	public void AddPassenger(){
		dist_left.Add(Random.Range(MIN_DIST,MAX_DIST));
	}

	void FixedUpdate(){


		float dist_traveled = (transform.position - prev).magnitude;
		float vel = dist_traveled/Time.deltaTime;
		prev = transform.position;

		  

		for(int i = 0; i < dist_left.Count; i++){
			dist_left[i] -= dist_traveled;
//			Debug.Log("DIST LEFT " + i + " " + dist_left[i]); 
			if(dist_left[i] <= 0){
				dist_left.RemoveAt(i);
				num_para++;
			}
		}

		if(num_para > 0){
			if(vel < slow_vel){
				timeEl += Time.deltaTime;
				if(timeEl > stop_time){
					if(ps.CanDisembark()){
						while (num_para > 0){
							rs.DropPassenger();
							num_para--;
						}
					}
				}
			}
			else{
				timeEl = 0;
			}
		}
	}


}
