using UnityEngine;
using System.Collections;

public class BlockController : MonoBehaviour {
	
	public void BreakBlock() {
		//transform.collider2D.enabled = false;
		Destroy (gameObject);
	}
}
