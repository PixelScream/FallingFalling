using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	private float startX;

	void Awake () {
		startX = transform.position.x;
	}
	
	void FixedUpdate () {
		if (transform.position.x != startX) {
			transform.position = new Vector3(startX, transform.position.y, 0);
		}
	}
}
