using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class ButtonManager : MonoBehaviour {


	GameObject buttonNumber;
	int maxFastFoward = 6;
	Annotations3D annotations3D;
	bool playing = false;
	Scrollbar scrollBar;
	float scrollbarStep = 0.0f;
	PointCloud[] clouds = null;
	float numberOfFrames = 0.5f;
	int numberOfAnnotations = 0;
	GameObject buttonType;
	GameObject buttonDuration;
	GameObject buttonPause;
	bool isPlaying = false;

	void Start() {
	

		buttonNumber = GameObject.Find("/Canvas/PanelAnnotations/PanelNumber/ButtonNumber");
		buttonType = GameObject.Find("/Canvas/PanelAnnotations/PanelType/ButtonType");
		buttonDuration = GameObject.Find("/Canvas/PanelAnnotations/PanelDuration/ButtonDuration");
		buttonPause = GameObject.Find("/Canvas/PanelTimelineControl/ButtonPanelFrames/ButtonPause");

		scrollBar = GameObject.FindObjectOfType<Scrollbar> ();
		annotations3D = (Annotations3D)Camera.main.GetComponent<Annotations3D> ();

		clouds = GameObject.FindObjectsOfType<PointCloud> ();

		numberOfAnnotations = annotations3D.Annotations.Count;
		scrollbarStep = ((float) 1) / ((float) numberOfAnnotations);
		Debug.Log ("numberOfAnnotations = " + numberOfAnnotations);
	}

	public void OnClickNextAnnotation(){
		Debug.Log ("TEST CLICK NEXT");
		Annotation annotation = annotations3D.WriteNextAnnotation ();

		if (annotation == null)
			return;

		int currentNumber = Int32.Parse(buttonNumber.GetComponentInChildren<Text> ().text);
		if(currentNumber < numberOfAnnotations)
			buttonNumber.GetComponentInChildren<Text> ().text = (++currentNumber).ToString();

		buttonType.GetComponentInChildren<Text> ().text = annotation.Type;
		buttonType.GetComponentInChildren<Text> ().color = Color.white;

		int duration = annotation.End - annotation.Begin;
		buttonDuration.GetComponentInChildren<Text> ().text = duration.ToString ();
		buttonDuration.GetComponentInChildren<Text> ().color = Color.white;

		//move camera to position
		//Camera.main.transform.position = annotation.PositionKin;
	}

	public void OnClickPreviousAnnotation(){
		Debug.Log ("TEST CLICK PREVIOUS");
		annotations3D.WritePreviousAnnotation ();

		int currentNumber = Int32.Parse(buttonNumber.GetComponentInChildren<Text> ().text);
		if(currentNumber > 0)
			buttonNumber.GetComponentInChildren<Text> ().text = (--currentNumber).ToString();

	}


	public void OnClickNextFrame(){
		foreach(PointCloud pc in clouds){
			pc.currentCloud++;
		}

		scrollBar.value -= scrollbarStep;

	}
	
	public void OnClickPreviousFrame(){
		foreach(PointCloud pc in clouds){
			pc.currentCloud--;
		}

		scrollBar.value += scrollbarStep;
	}

	public void OnClickNextJumpFrames(){
		foreach(PointCloud pc in clouds){
			pc.currentCloud+=10;
		}

		scrollBar.value -= scrollbarStep * numberOfFrames;
	}

	public void OnClickPreviousJumpFrames(){
		foreach(PointCloud pc in clouds){
			pc.currentCloud-=10;
		}

		scrollBar.value += scrollbarStep * numberOfFrames;
	
	}

	public void OnClickPause(){
		foreach(PointCloud pc in clouds){
		
			if(pc.playing) {
				buttonPause.GetComponentInChildren<Text> ().text = "||";
				pc.playing = false;
			} 
			else {
				buttonPause.GetComponentInChildren<Text> ().text = "\u25B6";
				pc.playing = true;
			}
		}
	}

}
