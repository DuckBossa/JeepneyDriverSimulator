using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System;


public class PaymentSystem : MonoBehaviour {

	public Text CM;
	public Text CP;
	public Text CC;
	public Text TT;

	
	public enum PaymentState{
		NoPayment, ReceivePayment, GiveChange
	}

	public float INITIAL_MONEY = 50f;
	public float JEEPNEY_FARE = 8f;
	public GameObject MoneyBoxes;
	public int MAX_CAPACITY = 8;
	private int currCapacity;
	private PaymentState currState;
	private float currMoney;
	private float currChange;
	private float excessChange;
	private float currPayment;
	private Queue<float> transactions = new Queue<float>(); // only contains the amount of change needed to give back
	private bool gettingChange;
	public void Awake(){
		gettingChange = false;
		currMoney = INITIAL_MONEY;
		currChange = currPayment = excessChange = 0;
		currCapacity = 0;
		currState = PaymentState.NoPayment;
	}


	public void OnEnable(){
		RazerHydraInput.GetChange += GetChange;
		Input_Manager.EmbarkPassenger+= AddPayment;
	}

	public void OnDisable(){
		RazerHydraInput.GetChange -= GetChange;
		Input_Manager.EmbarkPassenger -= AddPayment;
	}

	public void Update(){
		CM.text = "CM: " + currMoney;
		CP.text = "CP: " + currPayment;
		CC.text = "CC: " + currChange;
		TT.text = "TT: " + transactions.Count;
//		Debug.Log (currState + " current money: " + currMoney + " current payment: " + currPayment + " current change : " + currChange + "number of transactions : "  + transactions.Count);
	}


	public bool isFull(){
		return currCapacity == MAX_CAPACITY;
	}

	public void AddPayment(){
		currCapacity++;
		int randompayment = 0;
		for(int i = 0; i < 6; i++){
			randompayment += UnityEngine.Random.Range(1,50);
		}
		randompayment /= 6;
		float amount = JEEPNEY_FARE + randompayment;
		Debug.Log (amount + "pesos");
		transactions.Enqueue(amount);
		if(currState == PaymentState.GiveChange){}
		else currState = PaymentState.ReceivePayment;
	}

	private void GetChange(float amount){
		if(currState == PaymentState.GiveChange){
			if(currMoney - amount < 0){
//				Debug.LogWarning("Poor Game Over");
			}
			else{
				currChange += amount;
				currMoney -= amount;
//				Debug.Log(currChange + " is currently your change");
			}
				
		}
	}
	
	private void UndoChange (float amount){
		if(currState == PaymentState.GiveChange){
			if(currChange < amount ){
				currMoney += amount - currChange;
				currChange = 0;
			}
			else{
				currMoney += amount;
				currChange -= amount;
			}
		}
	}


	public bool CanDisembark(){
		return currState == PaymentState.NoPayment;
//		return true;
	}
	


	private void OnTriggerEnter(Collider other){
		switch(currState){
			case PaymentState.ReceivePayment:
//				Debug.Log("Receive Payment");
				currPayment = transactions.Dequeue();
				currMoney += currPayment;
				if(currPayment == JEEPNEY_FARE){
					if(transactions.Count == 0){
						currPayment = 0;
						currState = PaymentState.NoPayment;
					}
				}
				else{
					currPayment -= JEEPNEY_FARE;
					MoneyBoxes.gameObject.SetActive(true);
					currState = PaymentState.GiveChange;
				}
				break;
			case PaymentState.GiveChange:
//				Debug.Log ("Give Change");
				if(currChange < currPayment){
//					Debug.Log("Kulang Po");
				}
				else{
					if (currChange > currPayment){
//						Debug.Log("Sobra ng " + (currChange - currPayment));
						currMoney += (currChange - currPayment);
						excessChange += (currChange - currPayment);
					}
					if(transactions.Count == 0){
						currState = PaymentState.NoPayment;
					
					}
					else{
						currState = PaymentState.ReceivePayment;
					}
					currChange = 0;
					currPayment = 0;
				}
				break;
		}
	} 




}
