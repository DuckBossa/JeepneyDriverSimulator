using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	public enum TileType{
		Road, Pathway
	}

	public TileType tt;


	public void SwitchTileType(TileType x){
		tt = x;
		gameObject.layer = LayerMask.NameToLayer(tt.ToString());
	}

}
