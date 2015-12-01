using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

	public Texture2D mapImage;
	public GameObject tile;
	public int tileSize;
	
	public GameObject root;
	public GameObject environment;
	public GameObject paths;

	struct Path {
		public byte id;
		public List<Vector3> route;
		public Path(byte id, List<Vector3> route) {
			this.id = id;
			this.route = route;
		}
	}

	public void GenerateMap() {
//		Color[] temp = mapImage.GetPixels(0,0,mapImage.width,mapImage.height );
		Color32[] temp = mapImage.GetPixels32();
//		foreach (Color v in temp) {
//			Debug.Log (v);
//		}

		short count = 0;
		int length = Mathf.RoundToInt(Mathf.Sqrt(temp.GetLength(0)));
		Vector3 start = new Vector3 (-length*tileSize/2, 0, -length*tileSize/2);

		List<Path> pathList = new List<Path>();


//		for (int w = 0; w < mapImage.width; w++) {
//			for(int h = 0; h < mapImage.height; h++){
//				mapImage.get
//			}
//		}
//
//
//
		foreach(Color32 value in temp){
//			GameObject node = Instantiate(tile, start + new Vector3((count % length)*tileSize, 0, (count++ / length)*tileSize), Quaternion.identity) as GameObject;
//			
//			if(value.b < 1f){
//				node.GetComponent<Tile>().SwitchTileType(Tile.TileType.Road);
//				node.GetComponent<MeshRenderer>().material.color = Color.gray;
//			
//			}
//			else{
//				node.GetComponent<Tile>().SwitchTileType(Tile.TileType.Pathway);
//				node.GetComponent<MeshRenderer>().material.color = Color.white;
//			}

			if(value.r > 0 && value.r < 255) {
				bool check = false;
				foreach(Path p in pathList) {
					if (p.id == value.r) {
						p.route.Add (start + new Vector3((count % length) * tileSize, 0, (count / length) * tileSize) );
						check = true;
						break;
					}
				}
				if(!check) {
					List<Vector3> tempList = new List<Vector3>();
					tempList.Add( start + new Vector3((count % length) * tileSize, 0, (count / length) * tileSize) );
					Path newPath = new Path(value.r, tempList);
					pathList.Add(newPath);
				}
//				GameObject path = null;
//				Transform find = paths.transform.Find(value.r.ToString());
//				if(find == null) {
//					path = new GameObject(value.r.ToString());
//					path.transform.SetParent(paths.transform);
//					pathList.Add (path);
//				} else {
//					path = find.gameObject;
//				}

//				var pathNode = new GameObject((path.transform.childCount + 1).ToString());
//				pathNode.transform.SetParent(path.transform);
//				pathNode.transform.position = start + new Vector3((count % length) * tileSize + tileSize/2, 0, (count / length) * tileSize + tileSize/2);
//
//				if(pathNode.transform.name == "0") {
//					var lastNode = new GameObject("999");
//					lastNode.transform.SetParent(path.transform);
//					lastNode.transform.position = start + new Vector3((count % length) * tileSize + tileSize/2, 0, (count / length) * tileSize + tileSize/2);
//				}
			}

			//			node.transform.SetParent(environment.transform);

			count++;
		}

		
		foreach(Path p in pathList) {
			GameObject path = null;
			path =  new GameObject(p.id.ToString());
			path.transform.SetParent(paths.transform);

			if(p.route.Count <= 0) continue;
			Vector3 initialPos = p.route.ToArray()[0];
			Vector3 firstPos = new Vector3(initialPos.x, initialPos.y, initialPos.z);

			GameObject initialNode = new GameObject("0");
			initialNode.transform.SetParent(path.transform);
			initialNode.transform.position = initialPos;

			p.route.RemoveAt(0);

			int nodeCount = p.route.Count;
			for(int i = 0; i < nodeCount; i++) {
				
				float leastDist = float.MaxValue;
				Vector3 selectedVector = new Vector3();

				foreach(Vector3 other in p.route) {
					float dist = Vector3.Distance(initialPos, other);
					
					if(dist < leastDist) {
						selectedVector = other;
						leastDist = dist;
					}
				}

				GameObject newNode = new GameObject((i+1).ToString());
				newNode.transform.SetParent(path.transform);
				newNode.transform.position = selectedVector;
				bool test = p.route.Remove(selectedVector);
				initialPos = selectedVector;
			}
			
			GameObject lastNode = new GameObject((nodeCount+1).ToString());
			lastNode.transform.SetParent(path.transform);
			lastNode.transform.position = firstPos;


			if(p.id % 100 == 0) {
				int rev = path.transform.childCount - 1;
				foreach(Transform child in path.transform) {
					child.name = rev--.ToString();
				}
			}
		}

//		Debug.Log (count);
//		Debug.Log (temp.GetLength(0));
//		foreach(GameObject g in pathList) {
////			if(int.Parse(g.transform.name) % 2 == 1) {
////				var ctr = g.transform.childCount;
////				
////			}
//			g.
//			foreach(Transform child in g.transform) {
//
//			}
//		}
	}
}
