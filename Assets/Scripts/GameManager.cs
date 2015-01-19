using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	// Game type settings
	public Vector2 aspectRatio = new Vector2 (3, 5);
	public int blocksAcross = 7;
	public int[] blockOptions = new int[3]{ 3, 5, 7};
	public Vector2 [] directions = new Vector2[4] {new Vector2 (0, 1), new Vector2 (1, 0), new Vector2 (0, -1), new Vector2 (-1, 0)};

	// Managers / Game objects
	private GameObject player;
	private GameObject camera;
	public EnvManager envManager;

	public float screenRatio;

	void Awake () {
		// blocksAcross = blockOptions [Random.Range (0, blockOptions.Length)]; // randomise level widths
		player = GameObject.FindGameObjectWithTag ("Player");
		camera = GameObject.FindGameObjectWithTag ("MainCamera");
		envManager = GameObject.Find ("Env_Manager").GetComponent<EnvManager> ();

		player.transform.position = new Vector3(aspectRatio.x * (blocksAcross / 2), 0 , 0);
		camera.transform.position = new Vector3 (aspectRatio.x * (blocksAcross / 2), 0, camera.transform.position.z);
		camera.GetComponent<Camera> ().orthographicSize = (aspectRatio.x * blocksAcross);

		screenRatio = (float)Screen.height / (float)Screen.width;
		Debug.Log("height : " + Screen.height + ", width : " + Screen.width + " ,ratio : " + screenRatio);
	}
	
	void Update () {

	}

	public GameObject CheckForNeighbor(Vector2 origin, int dir, int mul) {
		origin = new Vector2((origin.x / aspectRatio.x) , Mathf.Abs( origin.y) / aspectRatio.y)  + (new Vector2( directions[dir].x, -(directions[dir].y)) * mul);
		string neighborName = Mathf.Round( origin.y) + "_" + Mathf.Round( origin.x);
		GameObject neighbor = GameObject.Find (neighborName);
		if( neighbor != null) {
			return neighbor;
		}
		return null;
	}

	public bool OutOfBounds(Vector2 origin, int dir, int mul) {
		// 
		origin = new Vector2(origin.x  + (directions[dir].x * aspectRatio.x * mul), origin.y + (directions[dir].y * aspectRatio.y * mul));
		float height = Camera.main.orthographicSize;
		Debug.Log (" bound to check" + origin.y + " , " + height);
		if(origin.x < 0 || origin.x >= (blocksAcross * aspectRatio.x ) || 
		   origin.y < (camera.transform.position.y - height) || 
		   origin.y > (camera.transform.position.y + height)){
			Debug.Log("out of boundes");
			return true;
		}
		return false;
	}
}
