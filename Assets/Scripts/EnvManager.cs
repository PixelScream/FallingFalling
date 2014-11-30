using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnvManager : MonoBehaviour {
	public GameObject [] envBlocks;
	public GameObject enemy;
	public int spawnRate = 5;
	public float probablity = 0.2f;
	public Vector2 aspectRatio = new Vector2 (3, 5);
	public int blocksAcross = 7;
	public int rowsDown = 7;
	private float startY;

	public GameObject cam;

	void Awake () {
		startY = transform.position.y;
		if(cam == null) { cam = Camera.main.gameObject; }
	}

	void Start () {
		for(int i = 0; i < rowsDown; i++ ) {
			BuildLayer(i);
		}	
	}
	
	public void CameraCheck () {
		if((cam.transform.position.y / aspectRatio.y) < -(rowsDown - 3)) {
			BuildLayer(rowsDown);
			rowsDown ++;
		}
	}
	private void BuildLayer(int row) {
		for(int i = 0; i < blocksAcross; i++) {
			GameObject sp;
			if( Random.value < probablity ) {
				sp = enemy;
			} else {
				int b = Random.Range (0, envBlocks.Length );
				sp = envBlocks[b];
			}
			GameObject go = (GameObject) GameObject.Instantiate (sp, new Vector3((i * aspectRatio.x), startY -(row * aspectRatio.y), -1), Quaternion.identity);
			//go.name = go.name + i;
			go.transform.parent = transform;
		}
	}
}
