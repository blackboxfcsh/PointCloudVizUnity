using UnityEngine;
using System.Collections;

public class PointCloudEffects : MonoBehaviour {


	GameObject[] clustersGameObjects;
	[SerializeField] private float fadePerSecond = 0.05f;

	// Use this for initialization
	void Start () {
	
		clustersGameObjects = GameObject.FindGameObjectsWithTag("Cluster");
	}
	
	// Update is called once per frame
	void Update () {
	
		/*var material = GetComponent<Renderer>().material;
		var color = material.color;
		
		material.color = new Color(color.r, color.g, color.b, color.a - (fadePerSecond * Time.deltaTime));*/

		//Renderer clusterRenderer = gameObject.GetComponent<Renderer> ();
		//mat.EnableKeyword ("_Emission");
		//DynamicGI.SetEmissive(clusterRenderer, Color.red);


		
		// Another thing to note is that Unity 5 uses the concept of shader keywords extensively.
		// So if your material is initially configured to be without emission, then in order to enable emission, you need to enable // the keyword.
		//materials[0].EnableKeyword ("_EMISSION");
	}


	void OnTriggerEnter (Collider col)
	{
		Debug.Log (col.gameObject.name);
		//ChangeColor (col.gameObject);
		//DrawLines (col.gameObject);
	}

	IEnumerator Fade() {

		Renderer renderer = GetComponent<Renderer> ();

		for (float f = 1f; f >= 0; f -= 0.1f) {
			Color c = renderer.material.color;
			c.a = f;
			renderer.material.color = c;
			yield return null;
		}
	}

	void SpreadMeshVertices(){
	
		Debug.Log ("spreadVertices");
		/*
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;
		for(int i = 0; i < vertices.Length; i++) {

			Vector3 V = transform.TransformPoint(vertices[i]);
			V.x += Random.Range(100, 200);
			V.z += Random.Range(100, 200);
			mesh.vertices[i] = transform.InverseTransformPoint(V);
		}*/
		//mesh.vertices = vertices;
	}

	void DrawLines(GameObject cluster){
	
		Vector3 start = this.gameObject.GetComponent<BoxCollider> ().bounds.center;
		Vector3 end = cluster.GetComponent<BoxCollider>().bounds.center;

		GameObject newLine = new GameObject("LineRenderer");
		LineRenderer lineRenderer = newLine.AddComponent<LineRenderer> ();
		lineRenderer.SetVertexCount (2);
		lineRenderer.SetPosition (0, start);
		lineRenderer.SetPosition (1, end);
		lineRenderer.SetWidth (0.025f, 0.025f);
		lineRenderer.SetColors (Color.blue, Color.red);

		
	}

	void ChangeColor(GameObject cluster){

		Debug.Log ("change color");

		Material[] materials = GetComponent<Renderer>().materials;
		foreach(Material mat in materials) {
			mat.SetColor ("_EmissionColor", Color.blue);
			mat.color = Color.red;
		}

		//Renderer clusterRenderer = cluster.GetComponent<Renderer> ();
		//clusterRenderer.material.color = new Color(239, 48, 36); 

		/*Mesh clusterMesh = cluster.GetComponent<MeshFilter>().mesh;
		Color[] colours = clusterMesh.colors;
		for(int i = 0; i < colours.Length; i++)
		{
			colours[i] = new Color(2 * 10, 255, 2 * 20);
		}

		Vector3[] vertices = clusterMesh.vertices;
		Color[] colors = new Color[vertices.Length];
		for(int v = 0; v < vertices.Length; v++)
		colors[0] = new Color(2 * 10, 255, 2 * 20);
		clusterMesh.colors = colors;*/
	}


}
