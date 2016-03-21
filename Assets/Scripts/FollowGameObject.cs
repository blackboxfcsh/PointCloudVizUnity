using UnityEngine;
using System.Collections;

public class FollowGameObject : MonoBehaviour {

	// Use this for initialization
    BoxCollider parentBoxCollider;

	void Start () {
        parentBoxCollider = transform.parent.GetComponent<BoxCollider>();
	
	}
	
	// Update is called once per frame
	void Update () {

        transform.position = new Vector3(parentBoxCollider.center.x, parentBoxCollider.size.y, 
            parentBoxCollider.center.z);
	}
}
