using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Score : MonoBehaviour {
	public GameObject player;

	Text text;

	// Use this for initialization
	void Start () {
		if (player == null) { GameObject.FindGameObjectWithTag("Player"); }

		text = GetComponent <Text> ();

	}
	
	// Update is called once per frame
	void Update () {
		text.text = (Mathf.Round(Mathf.Abs( player.transform.position.y))).ToString();
	}
}
