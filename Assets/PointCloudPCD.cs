using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PointCloudPCD : MonoBehaviour {

    public string meshDirPath;
    public string cameraFilePath;
    public int firstIndex = 0;
    public int endIndex = 0;
    int maxclouds = 0;
    public int currentCloud = 0;
    public bool playing;
    Dictionary<int, List<Mesh>> meshes;

    List<Vector3> points = new List<Vector3>();
    private ParticleSystem.Particle[] cloud;
    public float max = 1.0f;
    public Color backgroundRGBA = Color.white / 4;
    public float particleSize = 0.004f;
    public bool updatePoints = false;

	// Use this for initialization
	void Start () {

        // load point cloud cluster data
        string[] clusterFilesPath = {""};
        for (int idx = firstIndex; idx <= endIndex; idx++) {
            Debug.Log("current idx = " + idx);
            clusterFilesPath = getClusterFilesPath(idx);
            currentCloud = idx;
        }

        // load point cloud cluster data in Mesh objects
        foreach (string f in clusterFilesPath) {
            List<Mesh> thecloud = readFileNoColor(f);
            meshes.Add(currentCloud, thecloud);
        }

        // create game objects
     /*   Material mat = Resources.Load("cloudmat") as Material;
        List<Mesh> first = meshes[firstIndex];
        for (int i = 0; i < maxclouds; i++)
        {
            GameObject a = new GameObject();
            a.name = "OutputCloud" + firstIndex;
            a.AddComponent<MeshFilter>();
            MeshRenderer mr = a.AddComponent<MeshRenderer>();
            mr.material = mat;
            a.transform.parent = this.gameObject.transform;
        }
        setCloudToRender(first, true);
        setInitialPositionIni();*/
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

       
        List<int> ind = new List<int>();
        List<Mesh> clouds = new List<Mesh>();

        string line = "";
        Mesh m = new Mesh();
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


    // Set particle positions according to point coordinates using two arguments. Color points according to inputted Color[] array.
    public void SetPoints()
    {
        cloud = new ParticleSystem.Particle[points.Count];

        // Find the max value of any coordinate and use it for normalizing the automatic coloring of the points.
       // max = Max(p);

        for (int i = 0; i < points.Count; ++i)
        {
            // Set position of particles to match those of the 3-D points.
            cloud[i].position = points[i];
            // Color points according to Color[] array.
            cloud[i].color = Color.white;
            // Static size.
            cloud[i].size = particleSize;
            //print("{ x: " + cloud[i].position.x + " // y: " + cloud[i].position.y + " // z: " + cloud[i].position.z + " }");
        }

        // Every time the points are set, redraw them.
        updatePoints = true;
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

        if (updatePoints)
        {
            // RETRIEVE UPDATED POINTS HERE IF YOU WANT TO

            // Fix all negative points and force them into the unit cube.
            //normalizePoints();
            // Create new particles and set at new positions.
            SetPoints();
            // Redraw the points.
            
            // Don't redraw the points until SetPoints() is called again.
            updatePoints = false;
        }
	}
}
