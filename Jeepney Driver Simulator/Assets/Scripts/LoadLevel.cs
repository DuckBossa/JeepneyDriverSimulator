using UnityEngine;
using System.Collections;

public class LoadLevel : MonoBehaviour {

	public string level_load;
	public void Load(){
		Application.LoadLevel(level_load);
	}
}
