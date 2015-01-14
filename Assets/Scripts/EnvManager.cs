using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnvManager : MonoBehaviour {
	public GameObject [] envBlocks;
	public GameObject enemy;
	public GameObject pickup;
	public float pickupProbability = 0.01f;
	public int spawnRate = 5;
	public float probablity = 0.2f;
	public float adjustedProbability;
	public float maxProbability = 0.6f;
	public Vector2 aspectRatio = new Vector2 (3, 5);
	public int blocksAcross = 7;
	public int rowsDown = 7;
	private float startY;
	public int scrollBuffer = 5;
	public GameObject cam;
	public int burnLayer = 0;
	public PlayerMovement playerMovement;

	void Awake () {
		playerMovement = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMovement> ();
		startY = transform.position.y;
		if(cam == null) { cam = Camera.main.gameObject; }
	}

	void Start () {
		for(int i = 0; i < rowsDown; i++ ) {
			BuildLayer(i);
		}	
	}
	
	public void CameraCheck () {
		adjustedProbability = probablity + Mathf.Abs (playerMovement.destination.y * 0.00001f);
		if(adjustedProbability > maxProbability) { adjustedProbability = maxProbability; }

		// build rows
		while((playerMovement.destination.y / aspectRatio.y) < -(rowsDown - scrollBuffer)) {
			BuildLayer(rowsDown);
			rowsDown ++;
		}
		// destroy them
		Debug.Log (((cam.transform.position.y + startY) / aspectRatio.y) + 6);
		while ( ((cam.transform.position.y + startY) / aspectRatio.y) + 6 < -burnLayer) {
			Destroy(GameObject.Find("Row_" + burnLayer));
			burnLayer++;
		}
	}
	private void BuildLayer(int row) {
		GameObject rowHolder = new GameObject("Row_" + row);
		float rowOffset = startY - (row * aspectRatio.y);
		rowHolder.transform.position = new Vector3(0, rowOffset, 0);
		rowHolder.transform.parent = transform;
		for(int i = 0; i < blocksAcross; i++) {
			GameObject sp;
			if( Random.value < adjustedProbability ) {
				sp = enemy;
			}
			else if(Random.value < pickupProbability) {
				sp = pickup;
			}
			else {
				int b = Random.Range (0, envBlocks.Length );
				sp = envBlocks[b];
			}
			GameObject go = (GameObject) GameObject.Instantiate (sp, new Vector3((i * aspectRatio.x), rowOffset, 0), Quaternion.identity);
			//go.name = go.name + i;
			go.transform.parent = rowHolder.transform;
		}
	}
}
