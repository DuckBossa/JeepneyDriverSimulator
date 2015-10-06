using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	public enum TileType{
		Road, Pathway
	}

	public TileType tt;

	public void Start(){
		SwitchTileType (TileType.Pathway);
	}

	public void SwitchTileType(TileType x){
		tt = x;
		switch(tt){
		case TileType.Pathway:
//			gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
			break;
		case TileType.Road:
//			gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
			break;
		}
		gameObject.layer = LayerMask.NameToLayer(tt.ToString());
	}

}
