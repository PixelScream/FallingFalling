﻿using UnityEngine;
using System.Collections;

public class BlockController : MonoBehaviour {
	public Vector2 dimensions;
	public Sprite[] alone;
	public Sprite[] ver;
	public Sprite[] hor;
	public Sprite[] full;

	public LayerMask env;

	public SpriteRenderer[] quaters;
	public bool[] neighbors;
	void Start() {
		dimensions = transform.GetComponent<BoxCollider2D> ().size;
		quaters = transform.GetComponentsInChildren<SpriteRenderer> ();
		CheckSides();
	}
	
	public void BreakBlock() {
		//transform.collider2D.enabled = false;
		Destroy (gameObject);
	}

	public void CheckSides() {
		neighbors = new bool[4] {CheckForNeighbor(new Vector3(0,1 * dimensions.y,0)), 
			CheckForNeighbor(new Vector3(1 * dimensions.y,0,0)), 
			CheckForNeighbor(new Vector3(0,-1 * dimensions.y,0)), 
			CheckForNeighbor(new Vector3(-1 * dimensions.y,0,0))};
		int[] quaterState = new int[4]{0,0,0,0};
		if(neighbors[0]) {
			quaterState[0] ++;
			quaterState[1] ++;
		}
		if(neighbors[1]) {
			quaterState[1] ++;
			quaterState[2] ++;
		}
		if(neighbors[2]) {
			quaterState[2] ++;
			quaterState[3] ++;
		}
		if(neighbors[3]) {
			quaterState[3] ++;
			quaterState[0] ++;
		}
		for(int i = 0; i < quaterState.Length; i++) {
			switch(quaterState[i]) {
			case 0: 
				quaters[i].sprite = alone[i];
				break;
			case 1: 
				quaters[i].sprite = ver[i];
				break;
			case 2: 
				quaters[i].sprite = hor[i];
				break;
			case 3: 
				quaters[i].sprite = full[i];
				break;
			}
		}
	}

	public bool CheckForNeighbor(Vector3 dir) {
		Vector3 d = transform.position + dir;
		Collider2D  col = Physics2D.OverlapCircle (d, 0.3f, env);
		if(col != null) {
			Debug.Log("true");
			return true;
		}
		Debug.Log("false");
		return false;
	}
}
