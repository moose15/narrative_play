﻿using UnityEngine;
using System.Collections;

public class DragDrop : MonoBehaviour {

	private Vector3 screenPoint;
	private Vector3 offset;
	private Vector3 original;
	private Vector3 target;
	private bool isTargetFound;
	public bool isSettled;
	private Node node;
	private GameObject net;
	private bool isClickPlay=false;

	void Start(){
		isTargetFound = false;
		isSettled = false;
		original = gameObject.transform.position;
		net = GameObject.FindGameObjectWithTag("network");
		//oriPos = new Vector3 (original.x, original.y, original.z);
	}


	void OnMouseDown()
	{ 
		if (!isSettled) {
			// get the current gameobj position in screen
			// world --> screen 
			screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
			// get the offset (difference in cooordinates) for dragging 
			offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
		}
	}


	void OnMouseDrag() 
	{  
		if (!isSettled) {
			Vector3 curScreenPoint = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
			Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenPoint) + offset;
			gameObject.transform.position = curPosition;
		}
	}

	// when release the mouse, check for collision 
	void OnMouseUp()
	{
		 
		if (!isSettled) {
			if (!isTargetFound) {
				target = original;

			} else {
				//unfold other nodes 
				node.activateMore ();
				node.isFilled = true;
				//add the current item to the network 
				//node.addItem(gameObject);
				net.GetComponent<Network> ().addItemToCell (node.nodeIdx.x, node.nodeIdx.y, gameObject);
				gameObject.transform.parent = node.transform;
				// play the audio when the object is placed 
				gameObject.GetComponent<StoryItem> ().playAudio ();
				// play weather sound 
				if(gameObject.GetComponent<Weather>()){
					gameObject.GetComponent<Weather>().changeWeather();
				}
				//search for the othello flipping 
				net.GetComponent<Network> ().searchForReverse (node);
				isSettled = true;

			}


			// move the object to the target position 
			iTween.MoveTo (gameObject, target, 1.0f);

		} else {
			// if the item is settled
			isClickPlay = !isClickPlay;
			if(isClickPlay)
			{
				gameObject.GetComponent<StoryItem>().playAudio();
			}else{
				gameObject.GetComponent<StoryItem>().stopAudio();
			}

		}

	}


	void OnTriggerEnter(Collider other)
	{
		if (other != null && other.CompareTag("node") && !other.GetComponent<Node>().isFilled) {
			float distance = Vector3.Distance(gameObject.transform.position, other.transform.position);
			if(distance <= 3.0f)
			{
				isTargetFound = true;
				target = other.transform.position;
				target -= 1.0f * Vector3.forward;
				node = other.GetComponent<Node>();
			}else{
				isTargetFound = false;
				target = original;
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		isTargetFound = false;
//		float distance = Vector3.Distance(gameObject.transform.position, other.transform.position);
//		//isTargetFound = false;
//		if(distance <= 3.0f)
//		{
//			isTargetFound = true;
//			target = other.transform.position;
//			target -= 1.0f * Vector3.forward;
//			node = other.GetComponent<Node>();
//		}else{
//			isTargetFound = false;
//			target = original;
//		}
	}
	
}
