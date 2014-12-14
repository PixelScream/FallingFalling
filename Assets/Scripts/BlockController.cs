using UnityEngine;
using System.Collections;

public class BlockController : MonoBehaviour {
	private Vector2 dimensions;
	void Start() {
		dimensions = transform.GetComponent<BoxCollider2D> ().size;
	}
	
	public void BreakBlock() {
		//transform.collider2D.enabled = false;
		Destroy (gameObject);
	}

	public void CheckSides() {

	}
}
