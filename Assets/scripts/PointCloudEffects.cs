using UnityEngine;
using System.Collections;

public class PointCloudEffects : MonoBehaviour {


	GameObject[] clustersGameObjects;

	// Use this for initialization
	void Start () {
	
		clustersGameObjects = GameObject.FindGameObjectsWithTag("Cluster");
	}
	
	// Update is called once per frame
	void Update () {
	

	}


	void OnTriggerEnter (Collider col)
	{
		Debug.Log (col.gameObject.name);
	}

	void DrawLines(){
	
		
	}

	void ChangeColor(){
		
		/*List<Mesh> clusterTemp = meshes[clusterID];
			foreach(Mesh m in clusterTemp){
				Vector3[] vertices = m.vertices;
				Color[] colors = new Color[vertices.Length];
				for(int v = 0; v < vertices.Length; v++)
					colors[0] = new Color(clusterID * 10, 255, clusterID * 20);

				m.colors = colors;
			}*/
	}
}
