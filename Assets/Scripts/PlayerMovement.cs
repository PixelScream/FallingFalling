using UnityEngine;
using System.Collections;
using UnityEngine.UI;
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
	//private Vector2 aspectRatio = new Vector2 (3, 5);

	public bool ded = false;
	public bool paused = false;
	public bool canMove = true;
	public float closeEnough = 2;
	public float distanceBetweenDestination;
	public GameObject highScore;
	public Text deadScore;
	public Text text;

	public GameObject cross;
	public float crossDistance = 2;

	public int pickedup;
	public Text pickupText;

	public float maxTime = 30;
	public float time;
	public Slider timeSlide;
	public float pickupTime = 5;

	private GameManager gameManager;

	void Start () {
		gameManager = GameObject.Find ("Game_Manager").GetComponent<GameManager> ();
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
		text = highScore.GetComponent<Text>();
		text.text = "highest " + PlayerPrefs.GetInt ("High Score").ToString ();
		time = maxTime + Time.time;
		timeSlide.maxValue = maxTime;
	}
	
	void Update () {
		if(ded || paused) { return; }
		// Limit player movement to the width of the grid
		if (!paused && canMove) {

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

			if(Input.GetKeyDown(KeyCode.T)) {
				PlayerPrefs.DeleteKey("hue");
				Debug.Log("hue deleted , " + PlayerPrefs.GetFloat ("hue"));
			}

			// Mouse/ drag controlls
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
				Debug.Log(dir);

				if(dir != -1) {
					MoveChar(dir);
				}
				cross.transform.position = transform.position;
			}

			if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
				MoveChar(0);
			}
			if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
				MoveChar(1);
			}
			if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
				MoveChar(2);
			}
			if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
				MoveChar(3);
			}
		}
		//transform.position = Vector2.Lerp(transform.position, destination, (speed - (distanceBetweenDestination / 6 )) * Time.deltaTime );
		transform.position = Vector2.Lerp(transform.position, destination, speed * Time.deltaTime );
		if(!canMove) {
			if((Vector2.Distance(transform.position, destination) < closeEnough)) {
				transform.position = destination;
				canMove = true;
				if(!IsGrounded()) {
					Debug.Log("about to fall");
					Fall();
				}
			}
		}

		if(dragging) {
			int dir = GetDirectionDrag(dragStart, Input.mousePosition);
			Vector3 crossPos;
			switch(dir){ 
			case 0:
				crossPos = transform.position +(new Vector3 (0, 1, 0) * crossDistance * gameManager.aspectRatio.y);
				break;
			case 1:
				crossPos = transform.position + (new Vector3 (1, 0, 0) * crossDistance * gameManager.aspectRatio.x);
				break;
			case 2:
				crossPos = transform.position + (new Vector3 (0, -1, 0) * crossDistance * gameManager.aspectRatio.y);
				break;
			case 3:
				crossPos = transform.position + (new Vector3 (-1, 0, 0) * crossDistance * gameManager.aspectRatio.x);
				break;
			default:
				crossPos = transform.position;
				break;
			}
			cross.transform.position = Vector3.Lerp(cross.transform.position, crossPos, 0.6f);
		}

		// time shtuff

		if (Time.time > time)
			PlayerDead ();

		timeSlide.value = time - Time.time;
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

		//origin = new Vector2((origin.x / aspectRatio.x) , Mathf.Abs( origin.y) / aspectRatio.y)  + (new Vector2( directions[dir].x, -(directions[dir].y)) * mul);
		if( gameManager.OutOfBounds(destination ,dir, 1)){
			return;
		}

		GameObject neighbor = gameManager.CheckForNeighbor(destination ,dir, 1);
		int i = 1;
		bool knockKnock = true;
		if (neighbor != null) {
			string tag = neighbor.tag;
			while(knockKnock) {
				if( gameManager.OutOfBounds(destination ,dir, i + 1 )) {
					knockKnock = false;
				} else {
					neighbor = gameManager.CheckForNeighbor(destination ,dir, i + 1);
					if (neighbor != null) {
						if(neighbor.tag == tag) {
							i++;
						} 
						else if(neighbor.tag == "Enemy" || neighbor.tag == "Pickup") {
							i++;
							knockKnock = false;
						}
						else {
							knockKnock = false;
						}
					} else {
						knockKnock = false;
					}
				}
			}
		} else {
			while(knockKnock) {
				if( gameManager.OutOfBounds(destination ,dir, i + 1 )) {
					knockKnock = false;
				} else {
					neighbor = gameManager.CheckForNeighbor(destination ,dir, i + 1);
					if (neighbor == null) {
						i++;
					} else {
						knockKnock = false;
					}
				}
			}
		}

		destination = new Vector2 (destination.x + (gameManager.directions [dir].x * gameManager.aspectRatio.x * (i)),
		                           destination.y + (gameManager.directions [dir].y * gameManager.aspectRatio.y * (i)));
		distanceBetweenDestination = (Vector2.Distance(transform.position, destination));

		if (dir == 0) {
			anim.SetTrigger("BoingUp");
		} else {
			anim.SetTrigger("Boing");
		}

		envManager.CameraCheck();
		canMove = false;
	}


	public void Fall() {
		bool groundFound = false;
		int i = 0;

		while (groundFound == false) {
			GameObject neighbor = gameManager.CheckForNeighbor(destination ,2, i + 1);
			if(neighbor != null && neighbor.tag != "Pickup") {
				groundFound = true;
			} else {
				i++;
			}
		}
		canMove = false;
		destination += new Vector2 (0, -(gameManager.aspectRatio.y * i));
	}



	//checks below character to see if he's standing on anything
	public bool IsGrounded() {
		GameObject neighbor = gameManager.CheckForNeighbor(new Vector2( transform.position.x, transform.position.y) , 2, 1);
		if(neighbor != null)
			return true;
		return false;
		//return  Physics2D.OverlapCircle (groundCheck.position, 0.3f, whatIsGround);
	}


	// handels dieing
	void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log ("ay " + other.tag);
		if(other.tag == "Enemy") {
			PlayerDead();
		}
		else if(other.tag == "Pickup") {
			pickedup += 1;
			pickupText.text = pickedup + "x";
			Destroy(other.gameObject);
			time += pickupTime;
			if (time > Time.time + maxTime)
				time = Time.time + maxTime;
		}
		else if(other.tag.Split('_')[1] == "Block") {
			other.GetComponent<BlockController>().BreakBlock();
			Debug.Log("Breaking block");
		}
	}

	public void PlayerDead() {
		int curScore = (int) Mathf.Round(Mathf.Abs( transform.position.y)) * pickedup; 
		if (PlayerPrefs.GetInt("High Score") == null || PlayerPrefs.GetInt("High Score") < curScore) {
			PlayerPrefs.SetInt("High Score", curScore);
			Debug.Log("new highscore is " + PlayerPrefs.GetInt("High Score"));
			//highScore.GetComponent<HighestScore>().ChangeScore(curScore);
			text.text = "highest " + curScore.ToString ();
		}
		Debug.Log("he ded , " + curScore);
		deadScore.text = curScore.ToString();
		menuInteractions.DedMenu ();
		ded = true;
	}
}

