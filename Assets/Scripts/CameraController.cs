using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public GameObject player;
	public float deadZone = 5;
	private float initialX;
	public float speed = 5;

	void Start () {
		initialX = transform.position.x;
		if (player == null) { player = GameObject.FindGameObjectWithTag("Player"); }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(player.transform.position.y < (transform.position.y - deadZone)) {
			transform.position = Vector3.Lerp(transform.position, new Vector3(initialX, player.transform.position.y + deadZone, -10), speed * Time.deltaTime);
		}
	}
}
