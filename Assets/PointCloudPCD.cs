﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PointCloudPCD : MonoBehaviour {

    public string meshDirPath;
    public string cameraFilePath;
    public int skipFirstX = 0;
    public int totalNumberOfPointClouds;
	int maxNumberOfClusters = 0;
    int maxclouds = 0;
    public int currentCloud = 0;
    public bool playing;
	//Dictionary<int, List<Mesh>> meshes;
    Dictionary<int, Frame> framesByIndex;

	// Use this for initialization
	void Start () {

       // Dictionary<int, List<Mesh>> meshes = new Dictionary<int, List<Mesh>>();

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
					List<Mesh> thecloud = readFileNoColor(f);
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
			a.AddComponent<MeshFilter>();
			MeshRenderer mr = a.AddComponent<MeshRenderer>();
			mr.material = mat;
			a.transform.parent = this.gameObject.transform;

			/*List<Mesh> clusterTemp = meshes[clusterID];
			foreach(Mesh m in clusterTemp){
				Vector3[] vertices = m.vertices;
				Color[] colors = new Color[vertices.Length];
				for(int v = 0; v < vertices.Length; v++)
					colors[0] = new Color(clusterID * 10, 255, clusterID * 20);

				m.colors = colors;
			}*/
		}
        setCloudToRender(framesByIndex[0].GetClusterList(), true);
		setInitialPositionIni();
	}


    string[] getClusterFilesPath(int index) {

        string[] clusterFilesPath = Directory.GetFiles(meshDirPath, "outputCloud" + index + "_*.pcd");

        foreach (string p in clusterFilesPath)
            Debug.Log("cluster file path = " + p);
        return clusterFilesPath;
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

    private void setInitialPositionIni()
    {
        FileStream fs = new FileStream(cameraFilePath, FileMode.Open);
        StreamReader sr = new StreamReader(fs);
        float[] values = new float[6];
        for (int i = 0; i < 6; i++)
        {
            char[] sep = { '=' };
            string pos = sr.ReadLine();
            string[] p = pos.Split(sep);
            values[i] = float.Parse(p[1]);

        }
        Vector3 position = new Vector3(values[0], values[1], values[2]);
        Vector3 rotation = new Vector3(values[3], values[4], values[5]);
        this.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        this.gameObject.transform.Translate(position);
        this.gameObject.transform.Rotate(rotation);

    }

    private void setCloudToRender(List<Mesh> meshes, bool show)
    {
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
                mf.sharedMesh.Clear();
            }
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        Quaternion orient = transform.rotation;
        Quaternion cameraRot = Camera.main.transform.rotation;
        float ydiff = Mathf.Abs(orient.eulerAngles.y - cameraRot.eulerAngles.y);
        bool show = true;
        if (ydiff > 90 || cameraRot.eulerAngles.x < -30 || cameraRot.eulerAngles.z < -30)
        {
            //	show = false;
        }

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