using UnityEngine;
using System.Collections;


public class MapGenerator : MonoBehaviour {

	public Texture2D mapImage;
	public GameObject road;
	public GameObject sidewalk;
	public int tileSize;

	public Transform root;

	public void GenerateMap() {
		byte[] temp = mapImage.GetRawTextureData ();
		root = GetComponent<Transform>;
		short count = 0;
		int length = Mathf.RoundToInt(Mathf.Sqrt(temp.GetLength(0)));
		Vector3 start = new Vector3 (-length*tileSize/2, 0, length*tileSize/2);
		foreach(byte value in temp){
			GameObject prefab = null;
			if(value == 255) prefab = road;
			else prefab = sidewalk;
			GameObject node = Instantiate(prefab, start + new Vector3((count++ % length)*tileSize, 0, (count / length)*tileSize), Quaternion.identity);
			Transform x = node.GetComponent<Transform>;
			x.SetParent(root);
		}
	}
}
