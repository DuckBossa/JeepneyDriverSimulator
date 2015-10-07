using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {

	public Texture2D mapImage;
	public GameObject tile;
	public int tileSize;

	public GameObject environment;

	public void GenerateMap() {
//		Color[] temp = mapImage.GetPixels(0,0,mapImage.width,mapImage.height );
		Color[] temp = mapImage.GetPixels ();
//		foreach (Color v in temp) {
//			Debug.Log (v);
//		}

		short count = 0;
		int length = Mathf.RoundToInt(Mathf.Sqrt(temp.GetLength(0)));
		Vector3 start = new Vector3 (-length*tileSize/2, 0, -length*tileSize/2);


//		for (int w = 0; w < mapImage.width; w++) {
//			for(int h = 0; h < mapImage.height; h++){
//				mapImage.get
//			}
//		}
//
//
//
		foreach(Color value in temp){
			GameObject node = Instantiate(tile, start + new Vector3((count % length)*tileSize, 0, (count++ / length)*tileSize), Quaternion.identity) as GameObject;

			if(value.r == 0f){
				node.GetComponent<Tile>().SwitchTileType(Tile.TileType.Road);
				node.GetComponent<MeshRenderer>().material.color = Color.gray;
			
			}
			else{
				node.GetComponent<Tile>().SwitchTileType(Tile.TileType.Pathway);
				node.GetComponent<MeshRenderer>().material.color = Color.white;
			}

			node.transform.SetParent(environment.transform);	
		}
//		Debug.Log (count);
//		Debug.Log (temp.GetLength(0));
	}
}
