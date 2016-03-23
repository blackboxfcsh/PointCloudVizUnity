using UnityEngine;
using System.Collections;

public class GameObjectFollowTransform : MonoBehaviour {

	Transform parentTransform;

	// Use this for initialization
	void Start () {

		parentTransform = transform.parent.transform;
	}
	
	// Update is called once per frame
	void Update () {
	
		transform.position = parentTransform.position;
	}
}
