using UnityEngine;
using System.Collections;
using System.IO;
using System;


public class RazerHydraInput : MonoBehaviour 
{
	public static event Action<float> GetChange; //hydra

	SixenseHand m_hand;
	
	Vector3 m_baseOffset;
	float m_sensitivity = 0.001f; // Sixense units are in mm
	bool m_bInitialized;
	bool get_change;
	
	// Use this for initialization
	void Start () 
	{
		get_change = false;
		m_hand = GetComponent<SixenseHand>();
	}
	
	
	
	// Update is called once per frame
	void Update () 
	{
		bool bResetHandPosition = false;
		if ( IsControllerActive( m_hand.m_controller ) && m_hand.m_controller.GetButtonDown( SixenseButtons.START ) )
			{
				bResetHandPosition = true;
			}
			if ( m_bInitialized )
			{

				UpdateHand( m_hand );
			}
		if ( bResetHandPosition )
		{
			m_bInitialized = true;
			
			m_baseOffset = Vector3.zero;
			
			// Get the base offset assuming forward facing down the z axis of the base
			m_baseOffset += m_hand.m_controller.Position;
			m_baseOffset /= 2;
		}
		if(IsControllerActive(m_hand.m_controller) && m_hand.m_controller.GetButtonDown(SixenseButtons.TRIGGER))
		{
			Debug.Log ("get ready");
			get_change = true;	
		}
		
	}

	void FixedUpdate(){
		if(get_change){
			Collider[] possible = Physics.OverlapSphere(transform.position + GetComponent<SphereCollider>().center,GetComponent<SphereCollider>().radius, LayerMask.GetMask("Money"));		
			get_change = false;
			if(GetChange!= null && possible.Length > 0){
				Debug.Log(possible[0].name);
				GetChange(possible[0].gameObject.GetComponent<Money>().value);
			}

		}
	}
	
	
	/** Updates hand position and rotation */
	void UpdateHand( SixenseHand hand )
	{
		bool bControllerActive = IsControllerActive( hand.m_controller );
		
		if ( bControllerActive )
		{
			hand.transform.localPosition = ( hand.m_controller.Position - m_baseOffset ) * m_sensitivity;
			hand.transform.localRotation = hand.m_controller.Rotation * hand.InitialRotation;
		}
		
		else
		{
			// use the inital position and orientation because the controller is not active
			hand.transform.localPosition = hand.InitialPosition;
			hand.transform.localRotation  = hand.InitialRotation;
		}
	}
	

	void OnGUI()
	{
		if ( !m_bInitialized )
		{
			GUI.Box( new Rect( Screen.width / 2 - 50, Screen.height - 40, 100, 30 ),  "Press Start" );
		}
	}
	
	
	/** returns true if a controller is enabled and not docked */
	bool IsControllerActive( SixenseInput.Controller controller )
	{
		return ( controller != null && controller.Enabled && !controller.Docked );
	}
}
