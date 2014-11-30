using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	public Vector2 dragStart;
	public Vector2 dragEnd;
	public bool dragging;
	public float slideSpeed = 5;
	public Transform groundCheck;
	public Transform[] checks;
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

	void Start () {
		Debug.Log (Screen.width * swipeLength);
		swipeLength = Screen.width * swipeLength;
		for (int i = 0; i < checks.Length; i++) {
			Debug.Log(checks[i]);
		}
		dimensions = new Vector2 (collider2D.bounds.extents.x / 2, collider2D.bounds.extents.y / 2);
		cameraSize = Camera.main.orthographicSize; 
		if (sprite == null) { sprite = transform.FindChild("Character").gameObject; }
		if (anim == null) {	anim = sprite.GetComponent<Animator> (); }
		Debug.Log("env mnager says :" + envManager.rowsDown);
		if (sceneManager == null) { sceneManager = GameObject.FindGameObjectWithTag("Scene_Manager"); }
		menuInteractions = sceneManager.GetComponent<MenuInteractions>();
	}
	
	void FixedUpdate () {
		if (transform.position.x > 18 ) {
			rigidbody2D.velocity = new Vector2( 0, rigidbody2D.velocity.y);
			transform.position = new Vector3(18, transform.position.y, 0);
		}
		if(transform.position.x < 0) {
			rigidbody2D.velocity = new Vector2( 0, rigidbody2D.velocity.y);
			transform.position = new Vector3(0, transform.position.y, 0);
		}
		if(Input.GetKeyDown(KeyCode.R)) {
			ResetChar();
		}
		if(Input.GetKeyDown(KeyCode.E)) {
			Application.LoadLevel(Application.loadedLevel);
		}
		if (Input.GetMouseButtonDown(0)) {
			dragStart = Input.mousePosition;
			//Debug.Log("Drag start : " + dragStart);
			dragging = true;
		}
		if (Input.GetMouseButtonUp(0)) {
			dragEnd = Input.mousePosition;
			//Debug.Log("Drag end : " + dragEnd);
			dragging = false;
			int dir = GetDirectionDrag(dragStart, dragEnd);
			MoveChar(dir);
			BlockBreak(dir);
		}
		if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
			if ( IsGrounded() == true) {
				MoveChar(0);
				BlockBreak(0);
			} else {
				Debug.Log("not grounded");
			}
		}
		if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
			MoveChar(1);
			BlockBreak(1);
		}
		if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
			MoveChar(2);
			BlockBreak(2);
		}
		if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
			MoveChar(3);
			BlockBreak(3);
		}
		if (dragging == true) {
			/*
			Vector3 lineStart = new Vector3((dragStart.x / 10) - (cameraSize / 2), (dragStart.y) - (cameraSize / 2), 0);
			Vector3 lineEnd = new Vector3((Input.mousePosition.x / 10) - (cameraSize / 2), (Input.mousePosition.y / 10) - (cameraSize / 2), 0);
			Debug.DrawLine(lineStart, lineEnd, Color.green);
			*/
		}
	}
	//private void NowMove(
	private int GetDirectionDrag(Vector2 start, Vector2 end) {
		Debug.Log ("Magnitude " + (start - end).magnitude);
		if( (start - end).magnitude > swipeLength) {
			Vector2 dir = (start - end).normalized;
			float yDot = Vector2.Dot (dir, new Vector2 (0, 1));
			//bool isGrounded = Physics2D.OverlapCircle (groundCheck.position, 1.3f, whatIsGround);
			if ( Mathf.Abs(yDot) > 0.5 ) {
				if( dir.y > 0 ) { 
					return 2;
				}
				else { 
					//Debug.Log("going up"); 
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
	private void ResetChar() {
		transform.position = new Vector3(0,0,0);
		rigidbody2D.velocity = new Vector2 (0, 0);
	}
	private void BlockBreak(int dir) {
		if ( dir != -1) {
			//Debug.Log ("check " + dir + "out of " + checks.Length + ", " + checks[dir].name);
			Collider2D  col = Physics2D.OverlapCircle ( checks[dir].position, 1.3f, whatIsGround);
			if(col != null) {
				//Debug.Log("hit - " + col);
				col.GetComponent<BlockController>().BreakBlock();;
				//Debug.Log(col.GetComponents<BlockController>());
			} else {
				//Debug.Log("No hit");
			}
		} 
	}
	public bool IsGrounded() {
		return  Physics2D.OverlapCircle (groundCheck.position, 1.3f, whatIsGround);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		//Debug.Log ("ay " + other.tag);
		if(other.tag == "Enemy") {
			Debug.Log("he ded");
			menuInteractions.DedMenu ();
		}
	}
}

