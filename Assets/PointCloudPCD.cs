using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class PointCloudPCD : MonoBehaviour {

    public string meshDirPath;
    public int skipFirstX = 0;
    public int totalNumberOfPointClouds;
	int maxNumberOfClusters = 0;
    int maxclouds = 0;
    public int currentCloud = 0;
    public bool playing;
	//Dictionary<int, List<Mesh>> meshes;
    Dictionary<int, Frame> framesByIndex;
	bool show = true;

	// Use this for initialization
	void Start () {

    	framesByIndex = new Dictionary<int, Frame>();
       
            // load point cloud cluster data
        string[] clusterFilesPath = {""};

        for (int idx = skipFirstX; idx <= totalNumberOfPointClouds; idx++)
        {
            clusterFilesPath = getClusterFilesPath(idx);
			if(clusterFilesPath.Length == 0)
				continue;
			else {
				// load point cloud cluster data in Mesh objects
                Frame frame = new Frame();
                int i = idx;
				foreach (string f in clusterFilesPath) {
                    Debug.Log("id = " + i + " - filename = " + f);
					List<Mesh> thecloud = readFileWithColor(f);
                    frame.AddCluster(i, thecloud);
                    i++;
				}
				Debug.Log("idx = " + idx);
				framesByIndex.Add(idx, frame);

                int numberOfClusters = framesByIndex[idx].GetClusterList().Count;
                if(maxNumberOfClusters < numberOfClusters)
                    maxNumberOfClusters = numberOfClusters;
                Debug.Log("frames number = " + idx + " number of clusters = " + numberOfClusters);
			}
        }

        // create game objects
    	Material mat = Resources.Load("cloudmat") as Material;
		for(int clusterID = 0; clusterID < maxNumberOfClusters; clusterID++){
			GameObject a = new GameObject();
			a.name = "Cluster" + clusterID;
			a.tag = "Cluster";
			a.AddComponent<MeshFilter>();
			MeshRenderer mr = a.AddComponent<MeshRenderer>();
			mr.material = mat;
			a.transform.parent = this.gameObject.transform;

			// change render mode to Fade
			mr.material.SetFloat("_Mode", 2);
			mr.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			mr.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			mr.material.SetInt("_ZWrite", 0);
			mr.material.DisableKeyword("_ALPHATEST_ON");
			mr.material.EnableKeyword("_ALPHABLEND_ON");
			mr.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			mr.material.renderQueue = 3000;
		
			// add script with effects
			a.AddComponent<PointCloudEffects>();

			// add rigidbody and boxcollider for collision detection
			Rigidbody rigidbody =  a.AddComponent<Rigidbody>();
			rigidbody.useGravity = false;

			BoxCollider boxCollider = a.AddComponent<BoxCollider>();
			boxCollider.isTrigger = true;


		}

	    setCloudToRender(framesByIndex[0].GetClusterList(), true);
		//setInitialPositionIni();
	}
	
    string[] getClusterFilesPath(int index) 
	{

        string[] clusterFilesPath = Directory.GetFiles(meshDirPath, "OutputCloud" + index + "_*.ply");

        foreach (string p in clusterFilesPath)
            Debug.Log("cluster file path = " + p);
        return clusterFilesPath;
    }

	private List<Mesh> readFileWithColor(string f)
	{
		FileStream fs = new FileStream(f, FileMode.Open);
		StreamReader sr = new StreamReader(fs);
		
		List<Vector3> points = new List<Vector3> ();
		List<int> ind = new List<int> ();
		List<Color> colors = new List<Color> ();
		List<Mesh> clouds = new List<Mesh> ();
		
		string line = "";
		int i = 0;
		Mesh m = new Mesh ();
		while (!sr.EndOfStream) {
			line = sr.ReadLine ();
			line = line.Replace(",",".");
			char[] sep = { ' ' };
			string[] lin = line.Split (sep);
			if (lin.Length == 10 && lin [0] != "" && lin [1] != "" && lin [2] != "") {
				float x = float.Parse (lin [0]);
				float y = float.Parse (lin [1]);
				float z = float.Parse (lin [2]);
				
				bool use = false;
				Color c = new Color ();
				if (lin [3] != "0" &&  lin [3] != "") {
					c.b = int.Parse (lin [5]) / 255.0f;
					c.g = int.Parse (lin [4]) / 255.0f;
					c.r = int.Parse (lin [3]) / 255.0f;
					use = true;
					
				}
				if (use) {
					points.Add(new Vector3(x,y,z));
					ind.Add(i);
					colors.Add (c);
					i++;
				}
			}
			if(i == 65000){
				m.vertices = points.ToArray ();
				m.colors = colors.ToArray ();
				m.SetIndices (ind.ToArray(), MeshTopology.Points, 0);
				clouds.Add(m);
				m = new Mesh();
				i = 0;
				points.Clear();
				colors.Clear();
				ind.Clear();
			}
		}
		m.vertices = points.ToArray ();
		m.colors = colors.ToArray ();
		m.SetIndices (ind.ToArray(), MeshTopology.Points, 0);
		clouds.Add(m);
		fs.Close ();
		return clouds;
	}

    private List<Mesh> readFileNoColor(string f)
    {
        FileStream fs = new FileStream(f, FileMode.Open);
        StreamReader sr = new StreamReader(fs);

		List<Vector3> points = new List<Vector3>();
       	List<int> ind = new List<int>();
        List<Mesh> clouds = new List<Mesh>();

        string line = "";
        Mesh m = new Mesh();
		int i = 0; //current number of points in the mesh
        while (!sr.EndOfStream)
        {
            line = sr.ReadLine();
            line = line.Replace(",", ".");
            char[] sep = { ' ' };
            string[] lin = line.Split(sep);

            if (lin.Length != 3)
                continue;
            else
            {
                float x = float.Parse(lin[0]);
                float y = float.Parse(lin[1]);
                float z = float.Parse(lin[2]);

                points.Add(new Vector3(x, y, z));
				ind.Add(i);
				i++;
			}
			if(points.Count == 65000) {
				
				m.vertices = points.ToArray();
				m.SetIndices(ind.ToArray(), MeshTopology.Points, 0);
				clouds.Add(m);
				m = new Mesh();
				i = 0;
				points.Clear();
				ind.Clear();
			}
        }
        m.vertices = points.ToArray();
        m.SetIndices(ind.ToArray(), MeshTopology.Points, 0);
        clouds.Add(m);
        fs.Close();
        return clouds;
    }
	
    private void setCloudToRender(List<Mesh> meshes, bool show)
    {
		if(meshes == null)
			return;
        Renderer[] r = GetComponentsInChildren<Renderer>();
        foreach (Renderer r1 in r) {
            if (show) {
                r1.enabled = true;
            }
            else {
                r1.enabled = false;
            }
        }
        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();
        int i = 0;
        foreach (MeshFilter mf in filters) {
			if (i < meshes.Count) {
                mf.sharedMesh = meshes[i++];
            }
            else {
				if(mf.sharedMesh != null)
                	mf.sharedMesh.Clear();
            }
        }

		GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Cluster");
		foreach (GameObject go in gameObjects) {
			Vector3 pos = go.transform.position;
			Bounds bounds = go.GetComponent<Renderer>().bounds;
			BoxCollider boxCollider = go.GetComponent<BoxCollider>();
			//BoxCollider[] boxCollider = go.GetComponentsInChildren<BoxCollider>();
			if(boxCollider != null) {
				boxCollider.center =  bounds.center - go.transform.position;
				boxCollider.size = bounds.size;
			}
	    }


	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Quaternion orient = transform.rotation;
        Quaternion cameraRot = Camera.main.transform.rotation;
        float ydiff = Mathf.Abs(orient.eulerAngles.y - cameraRot.eulerAngles.y);
    
        if (currentCloud == framesByIndex.Count)
            currentCloud = 0;
        setCloudToRender(framesByIndex[currentCloud].GetClusterList(), show);
        if (playing)
            currentCloud++;

	}
}

public class Frame {

    Dictionary<int, List<Mesh>> clustersByIndex;
    List<Mesh> clusters;

    public List<Mesh> GetClusterListByIndex(int index) {
        if (clustersByIndex.ContainsKey(index))
            return clustersByIndex[index];
        else
            return null; // the key does not exist
    }

    public List<Mesh> GetClusterList() {
        if (clusters.Count == 0)
        {
            foreach (int index in clustersByIndex.Keys)
                clusters.AddRange(clustersByIndex[index]);
        }
        return clusters;
    }

    // constructor
    public Frame() {
        clustersByIndex = new Dictionary<int, List<Mesh>>();
        clusters = new List<Mesh>();
    }

    public void AddCluster(int index, List<Mesh> clusterMesh) {
        if (clustersByIndex.ContainsKey(index))
            clustersByIndex[index].AddRange(clusterMesh);
        else {
            List<Mesh> clusterList = new List<Mesh>();
            clusterList.AddRange(clusterMesh);
            clustersByIndex.Add(index, clusterList);
        }
    }

    // add anotation to cluster
}

public class Cluster
{
    List<Mesh> mesh;
  //  Color rgb;
   // Vector3 normal;
    int idx;
    

    public Cluster(List<Mesh> mesh, int idx)
    {
        this.mesh = mesh;
        this.idx = idx;
    }

    List<Mesh> GetMesh() { return mesh; }
    void SetMesh(List<Mesh> mesh) { this.mesh = mesh; }
}