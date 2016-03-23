using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class Annotations3D : MonoBehaviour {
	
	public string calibration1txt;
	public string calibration2txt;
	public string calibration3txt;
	
	Dictionary<string, Calibration> calibrations;
	Dictionary<int, Annotation> annotations; //it should be a annotation list
	public Dictionary<int, Annotation> Annotations {
		get {return annotations;}
	}
	
	Calibration calibration1;
	Calibration calibration2;
	Calibration calibration3;
	
	public string currentCalibration;
	int currentFrame = 0;
	int currentAnnotation = 0;
	
	public string xmlPath;
	XmlReaderWriter xmlReaderWriter;
	List<AnnotationText> textAnnotationList;
	List<AnnotationInk> inkAnnotationList;
	List<AnnotationLink> linkAnnotationList;
	List<AnnotationMark> markAnnotationList;
	List<int> annotationListOrderByID;
	
	List<GameObject> gameObjectsInTheScene;

	public List<GameObject> GameObjectsInTheScene {
		get {return gameObjectsInTheScene;}
		set {gameObjectsInTheScene =  value;}
	}

    List<GameObject> clusters;
	
	GameObject canvas;
	
	// Use this for initialization
	void Start () {
		
		
		LoadCalibrationData ();

        clusters = new List<GameObject>();

        clusters.AddRange(GameObject.FindGameObjectsWithTag("Cluster"));

		annotations = new Dictionary<int,Annotation>();
		gameObjectsInTheScene = new List<GameObject> ();
		canvas = GameObject.Find("/Canvas");
		
		xmlReaderWriter = new XmlReaderWriter (xmlPath);
		xmlReaderWriter.LoadFromFile ();
		Take take = xmlReaderWriter.Take;
		currentCalibration = take.Name;
		
		//organize annotations per time and duration
		// write text annotation
		Annotation value;
		textAnnotationList = take.TextAnnotationList;
		int idTextAnnoations = 0; // use this as id because the id on the xml might not be in sequence
		foreach (AnnotationText annotationText in textAnnotationList) {
			//if(!annotations.TryGetValue(annotationText.Begin, out value))
			annotations.Add(idTextAnnoations, annotationText);
			idTextAnnoations++;
			//else {
			//	int beginning = annotationText.Begin + 5;
			//	annotations.Add(beginning, annotationText);
			//}
			//WriteTextAnnotation(annotationText);
		}
		
		
		inkAnnotationList = take.InkAnnotationList;
		foreach (AnnotationInk annotationInk in inkAnnotationList) {
			//if(!annotations.TryGetValue(annotationInk.Begin, out value))
			annotations.Add(annotationInk.ID, annotationInk);
			//else
			//	annotations.Add(++annotationInk.Begin, annotationInk);
			//WriteInkAnnotation(annotationInk);
		}
		
		
		linkAnnotationList = take.LinkAnnotationList;
		foreach (AnnotationLink annotationLink in linkAnnotationList) {
			//if(!annotations.TryGetValue(annotationLink.Begin, out value))
			annotations.Add(annotationLink.ID, annotationLink);
			//else
			//	annotations.Add(++annotationLink.Begin, annotationLink);
			//WriteLinkAnnotation(annotationLink);
		}
		
		//TODO: ADD MARK ANNOTATIONS
		//markAnnotationList = take.MarkAnnotationList;
		
		annotationListOrderByID = annotations.Keys.ToList();
		
		//DebugAnnotationSorting ();
	}
	
	void DebugAnnotationSorting(){
		foreach (int annotationID in annotationListOrderByID) {
			Debug.Log ("Annotation = " + annotations [annotationID]);
		}
	}
	
	
	// Update is called once per frame
	void Update () {
		//Debug.Log("Frame number : " + currentFrame);
		currentFrame++;
	}
	
	void LoadCalibrationData () {
		
		calibrations = new Dictionary<string, Calibration> ();
		
		TextAsset calibrationData1 = (TextAsset)Resources.Load (calibration1txt);
		TextAsset calibrationData2 = (TextAsset)Resources.Load (calibration2txt);
		TextAsset calibrationData3 = (TextAsset)Resources.Load (calibration3txt);
		
		calibration1 = new Calibration();
		calibration2 = new Calibration();
		calibration3 = new Calibration();
		
		CreateCalibrationClasses (calibrationData1.text, calibration1);
		CreateCalibrationClasses (calibrationData2.text, calibration2);
		CreateCalibrationClasses (calibrationData3.text, calibration3);
		
		calibrations.Add(calibration1txt, calibration1);
		calibrations.Add(calibration2txt, calibration2);
		calibrations.Add(calibration3txt, calibration3);
	}
	
	void CreateCalibrationClasses(string calibrationData, Calibration calibration){
		
		string[] stringLines = calibrationData.Split ('\n');
		Debug.Log (stringLines.Length);
		Vector3 position = new Vector3();
		for (int i = 0; i < stringLines.Length; i++) {
			
			string[] stringLeftRightSide = stringLines[i].Split('=');
			switch(stringLeftRightSide[0]){
				
			case "posx":
				position.x = float.Parse(stringLeftRightSide[1]);
				break;
			case "posy":
				position.y = float.Parse(stringLeftRightSide[1]);
				break;
			case "posz":
				position.z = float.Parse(stringLeftRightSide[1]);
				break;
			case "rotx":
				calibration.RotationX = float.Parse(stringLeftRightSide[1]);
				break;
			case "roty":
				calibration.RotationY = float.Parse(stringLeftRightSide[1]);
				break;
			case "rotz":
				calibration.RotationZ = float.Parse(stringLeftRightSide[1]);
				break;
			default:
				break;
			}
			
			calibration.Position = position;
			
		}
	}
	
	public Annotation WriteNextAnnotation(){
		
		Annotation annotation = null;
		if (currentAnnotation < annotationListOrderByID.Count) {
			annotation = annotations[annotationListOrderByID[currentAnnotation]];
			currentAnnotation++;
			
			Debug.Log ("TYPE = " + annotation.Type);
			
			switch (annotation.Type) {
				
			case "text":
				Debug.Log ("WRITE TEXT ANNOTATION");
				if(((AnnotationText)annotation).TextValue.Length > 0)
					WriteTextAnnotation ((AnnotationText)annotation);
				break;
				
			case "link":
				Debug.Log ("WRITE LINK ANNOTATION");
				if(((AnnotationLink)annotation).Link.Length > 0)
					WriteLinkAnnotation ((AnnotationLink)annotation);
				break;
			default:
				Debug.Log ("DEFAULT");
				break;
			}
		}
		return annotation;
	}
	
	public Annotation WritePreviousAnnotation(){
		
		//delete all objects in the scene
		
		Annotation annotation = null;
		if (currentAnnotation > 0) {
			currentAnnotation--;
			
			GameObject tmpObject = gameObjectsInTheScene [currentAnnotation];
			Destroy (tmpObject);
			gameObjectsInTheScene.Remove (tmpObject);
			
			if (currentAnnotation > annotationListOrderByID.Count) {
				currentAnnotation--;
				annotation = annotations [annotationListOrderByID [currentAnnotation]];
				
				switch (annotation.Type) {
					
				case "text":
					Debug.Log ("WRITE TEXT ANNOTATION");
					if(((AnnotationText)annotation).TextValue.Length > 0)
						WriteTextAnnotation ((AnnotationText)annotation);
					break;
					
				case "link":
					Debug.Log ("WRITE LINK ANNOTATION");
					if(((AnnotationLink)annotation).Link.Length > 0)
						WriteLinkAnnotation ((AnnotationLink)annotation);
					break;
					
				default:
					Debug.Log ("DEFAULT");
					break;
				}
			}
		}
		return annotation;
	}

    GameObject FindClosestCluster(Vector3 annotationPos)
    {
        float minDistance = -1.0f;
        GameObject result = null;
        foreach (GameObject cluster in clusters)
        {
            float distance = Vector3.Distance(annotationPos, cluster.transform.position);
            if (minDistance == -1.0f)
            {
                minDistance = distance;
                result = cluster;
            }
                
             if (distance < minDistance)
             {
                 distance = minDistance;
                 result = cluster;
             }
               
        }
        return result;
    }
	
	void WriteTextAnnotation(AnnotationText annotationText){
		
		GameObject textObject = new GameObject ();
        TextMesh textMesh = textObject.AddComponent<TextMesh>();
       // GameObject textMeshGameObject = Resources.Load("TextMeshPrefab") as GameObject;

       // TextMesh textMesh = textMeshGameObject.GetComponent<TextMesh>();

        textMesh.characterSize = 0.1f;
        textMesh.text = annotationText.TextValue;
        textMesh.color = new Color(annotationText.FormattingText.Color.r, annotationText.FormattingText.Color.g,
                                      annotationText.FormattingText.Color.b);

        GameObject cluster = FindClosestCluster(annotationText.PositionKin);
        if (cluster == null)
        {
            Debug.Log("Error: Could find cluster to add annotation!");
            return;
        }

		textObject.AddComponent<GameObjectFollowBoxCollider>();
        textMesh.transform.parent = cluster.transform;
       
      //  TextMesh annotationMesh = cluster.AddComponent<TextMesh>();
       // annotationMesh = textMesh;


        /*TextMeshPro textMeshPro = textObject.AddComponent<TextMeshPro> ();
        textMeshPro.gameObject.AddComponent <CameraFacingBillboard>();
		
        textMeshPro.fontSize = 1;
        textMeshPro.text = annotationText.TextValue;
        textMeshPro.color = new Color(annotationText.FormattingText.Color.r, annotationText.FormattingText.Color.g,
                                      annotationText.FormattingText.Color.b);
        textMeshPro.transform.position = annotationText.PositionKin;
		
        // CALIBRATION 
        Calibration calibration = calibrations[currentCalibration];
		
        textMeshPro.transform.Rotate (calibration.RotationX,
                                   calibration.RotationY, calibration.RotationZ);
		
        textMeshPro.transform.Translate (calibration.Position.x, 
                                      calibration.Position.y, calibration.Position.z);*/

        gameObjectsInTheScene.Add (textMesh.gameObject);
	}
	
	void WriteInkAnnotation(AnnotationInk annotationInk){
		
		List<Vector2> pointList = annotationInk.Paths;
		
		GameObject inkAnnotation3D = new GameObject ();
		LineRenderer lineRenderer = inkAnnotation3D.AddComponent<LineRenderer>();
		lineRenderer.SetVertexCount (pointList.Count);
		lineRenderer.material = new Material (Shader.Find("Particles/Additive")); // TODO
		lineRenderer.SetWidth((float) annotationInk.FormattingInk.Thickness, 
		                      (float)annotationInk.FormattingInk.Thickness);
		lineRenderer.material.color = annotationInk.FormattingInk.Color;
		lineRenderer.transform.position = annotationInk.PositionKin;
		
		int i = 0; 
		foreach (Vector2 pos in pointList) {
			
			lineRenderer.SetPosition(i, new Vector3(pos.x, pos.y, 0));
			i++;
		}
		
		// CALIBRATION 
		Calibration calibration = calibrations[currentCalibration];
		
		lineRenderer.transform.Rotate (calibration.RotationX,
		                               calibration.RotationY, calibration.RotationZ);
		
		lineRenderer.transform.Translate (calibration.Position.x, 
		                                  calibration.Position.y, calibration.Position.z);

        gameObjectsInTheScene.Add(lineRenderer.gameObject);
	}
	
	void WriteLinkAnnotation(AnnotationLink annotationLink){
		
		GameObject textObject = new GameObject ();
		TextMeshPro textMeshPro = textObject.AddComponent<TextMeshPro> ();
		textMeshPro.gameObject.AddComponent <CameraFacingBillboard>();
		
		textMeshPro.fontSize = 2;
		textMeshPro.text = annotationLink.Link;
		textMeshPro.color = Color.yellow;
		textMeshPro.transform.position = annotationLink.PositionKin;
		
		// CALIBRATION 
		Calibration calibration = calibrations[currentCalibration];
		
		textMeshPro.transform.Rotate (calibration.RotationX,
		                              calibration.RotationY, calibration.RotationZ);
		
		textMeshPro.transform.Translate (calibration.Position.x, 
		                                 calibration.Position.y, calibration.Position.z);
		
		gameObjectsInTheScene.Add (textMeshPro.gameObject);
	}
	
	void WriteMarkAnnotation(){
		
		//Position (class)
	}
}

public class Calibration {
	
	Vector3 position;
	public Vector3 Position {
		get {return position;}
		set {position = value;}
	}
	
	float rotationX;
	public float RotationX {
		get {return rotationX;}
		set {rotationX = value;}
	}
	
	float rotationY;
	public float RotationY {
		get {return rotationY;}
		set {rotationY = value;}
	}
	
	float rotationZ;
	public float RotationZ {
		get {return rotationZ;}
		set {rotationZ = value;}
	}
	
}
