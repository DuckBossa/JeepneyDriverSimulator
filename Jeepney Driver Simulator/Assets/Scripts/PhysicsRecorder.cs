using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System;

public class PhysicsRecorder : MonoBehaviour {
	Rigidbody rb;
	XmlTextWriter xtw;

//	string filename = "test.xml";

	void OnEnable(){
		World_Manager.EndGame+= End;
	}

	void OnDisable(){
		World_Manager.EndGame-= End;
	}

	void Start () {
		rb = GetComponent<Rigidbody>();
		if(!Directory.Exists(Application.dataPath + "/driving_behavior/")){
			Directory.CreateDirectory(Application.dataPath + "/driving_behavior/");
		}
		xtw = new XmlTextWriter(Application.dataPath + "/driving_behavior/" + DateTime.Now.ToString("yyyyMMdd_HH_mm_ss") + ".xml",System.Text.Encoding.UTF8); 

		xtw.Formatting = Formatting.Indented;
		xtw.Settings.Indent = true;
		xtw.Settings.IndentChars = ("\t");
		xtw.Settings.OmitXmlDeclaration = true;
		xtw.WriteStartElement("Physics Recorder");
		xtw.WriteStartElement("User");
	}

	void FixedUpdate(){
		AddData();
	}

	void AddData(){
		xtw.WriteElementString("Frame Number",Time.frameCount.ToString());
		xtw.WriteElementString("Position",rb.position.ToString());
		xtw.WriteElementString("Velocity",rb.velocity.ToString());
	}

	void End(){
		Debug.Log("ASDSADASDADGDFGDFGDFGDFGSDAD");
		xtw.WriteEndElement();
		xtw.WriteEndElement();
		xtw.Close();
		this.enabled = false;
	}
}
