using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour {
	public float speed = 5;
	public Vector2 destination = new Vector2 (0,0);
	public Vector2 dragStart;
	public Vector2 dragEnd;
	public bool dragging;
	public float slideSpeed = 5;
	public Transform groundCheck;
	public List<Vector2> directions = new List<Vector2>();
	public LayerMask whatIsGround;

	private float cameraSize;
	private float deathPoint = -20;
	public Animator anim;
	public GameObject sprite;
	private BlockController blockBreaker;

	public float swipeLength = 1;

	public EnvManager envManager;
	//public Collider2D box;

	public Vector2 dimensions;
	public GameObject sceneManager;
	public MenuInteractions menuInteractions;
	public float lineMagnitued = 2;
	private Vector2 aspectRatio = new Vector2 (3, 5);

	private bool ded = false;
	public bool paused = false;

	void Start () {
		destination = transform.position;
		// add the 4 directions
		directions.Add (new Vector2 (0, 1));
		directions.Add (new Vector2 (1, 0));
		directions.Add (new Vector2 (0, -1));
		directions.Add (new Vector2 (-1, 0));

		swipeLength = Screen.width * swipeLength;
		dimensions = new Vector2 (collider2D.bounds.extents.x / 2, collider2D.bounds.extents.y / 2);
		cameraSize = Camera.main.orthographicSize; 
		if (sprite == null) { sprite = transform.FindChild("Character").gameObject; }
		if (anim == null) {	anim = sprite.GetComponent<Animator> (); }
		if (sceneManager == null) { sceneManager = GameObject.FindGameObjectWithTag("Scene_Manager"); }
		menuInteractions = sceneManager.GetComponent<MenuInteractions>();
	}
	
	void Update () {
		if(ded) { return; }
		// Limit player movement to the width of the grid
		if (!paused) {

			if (transform.position.x > 18 ) {
				rigidbody2D.velocity = new Vector2( 0, rigidbody2D.velocity.y);
				transform.position = new Vector3(18, transform.position.y, 0);
			}
			if(transform.position.x < 0) {
				rigidbody2D.velocity = new Vector2( 0, rigidbody2D.velocity.y);
				transform.position = new Vector3(0, transform.position.y, 0);
			}

			if(Input.GetKeyDown(KeyCode.E)) {
				Application.LoadLevel(Application.loadedLevel);
			}

			// Mouse/ drag controlls
			if (Input.GetMouseButtonDown(0)) {
				dragStart = Input.mousePosition;
				//Debug.Log("Drag start : " + dragStart);
				dragging = true;
			}
			if (Input.GetMouseButtonUp(0)) {
				envManager.CameraCheck();
				dragEnd = Input.mousePosition;
				//Debug.Log("Drag end : " + dragEnd);
				dragging = false;
				int dir = GetDirectionDrag(dragStart, dragEnd);
				Debug.Log(dir);

				if(dir != -1) {
					GameObject neighbor = CheckForNeighbor(directions[dir], 1);

					if (neighbor != null) {
						int i = 1;
						Debug.Log(neighbor);

						// set off some chain reaction of checks in the blocks
						bool knockKnock = true;

						GameObject tempNeighbor = CheckForNeighbor(directions[dir], i);

						while(tempNeighbor != null && (tempNeighbor.name.Split('_'))[0] == (neighbor.name.Split('_'))[0] ) {
							neighbor = tempNeighbor;
							tempNeighbor = CheckForNeighbor(directions[dir], i);
							i++;
						}
						destination = neighbor.transform.position;
					}

					//MoveChar(dir);
					//BlockBreak(dir);
				}
			}
			transform.position = Vector2.Lerp(transform.position, destination, speed * Time.deltaTime );

		}
	}

	// function which compares the dragging start location and the end pos to get the direction of the drag
	private int GetDirectionDrag(Vector2 start, Vector2 end) {
		// checks the magnitude of the drag, mainly so an accidental drag doesn't register
		if( (start - end).magnitude > swipeLength) {
			Vector2 dir = (start - end).normalized;

			// dot product to see if the swipe is vertical or horizontal
			float yDot = Vector2.Dot (dir, new Vector2 (0, 1));

			// returns 0 - 3, directions of movement are clockwise winding, think north east south west = up right down left,
			// this was a late night decesion... 
			if ( Mathf.Abs(yDot) > 0.5 ) {
				if( dir.y > 0 ) { 
					return 2;
				}
				else { 
					// checks if grounded so infinite jumping is impossible
					if ( IsGrounded() == true) {
						return 0;
					} else {
						Debug.Log("not grounded");
					}
				}
			} else {
				if( dir.x > 0 ) { 
					return 3;
				}
				else { 
					return 1;
				}
			}
		}
		return -1;
	}

	private void MoveChar(int dir) {
		switch(dir)
		{
		case 0:
			rigidbody2D.velocity += new Vector2 (0, slideSpeed);
			anim.SetTrigger("BoingUp");
			break;
		case 1:
			anim.SetTrigger("Boing");
			rigidbody2D.velocity += new Vector2 (slideSpeed, 0);
			break;
		case 2:
			anim.SetTrigger("Boing");
			rigidbody2D.velocity += new Vector2 (0, -(slideSpeed));
			break;
		case 3:
			anim.SetTrigger("Boing");
			rigidbody2D.velocity += new Vector2 (-(slideSpeed), 0);
			break;
		}
		envManager.CameraCheck ();
	}

	public GameObject CheckForNeighbor(Vector2 dir, int mul) {
		Vector3 d = destination + new Vector2(dir.x * aspectRatio.x * mul, dir.y * aspectRatio.y * mul);
		Collider2D  col = Physics2D.OverlapCircle (d, 1.3f, whatIsGround);
		if(col != null) {
			return col.gameObject;
		}
		return null;
	}

	//checks below character to see if he's standing on anything
	public bool IsGrounded() {
		return  Physics2D.OverlapCircle (groundCheck.position, 1.3f, whatIsGround);
	}


	// handels dieing
	void OnTriggerEnter2D(Collider2D other)
	{
		//Debug.Log ("ay " + other.tag);
		if(other.tag == "Enemy") {
			Debug.Log("he ded");
			menuInteractions.DedMenu ();
			ded = true;
		}
	}
}

