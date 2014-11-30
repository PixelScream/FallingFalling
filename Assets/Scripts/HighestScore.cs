using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighestScore : MonoBehaviour {


	Text text;

	// Use this for initialization
	void Start () {
		text = GetComponent <Text> ();
		//		highScore.GetComponent<HighestScore>().UpdateScore(PlayerPrefs.GetInt ("Highest Score"));
		//text.text = "highest " + ;
	}
	

	public void ChangeScore (int score) {
		text.text = "highest " + score.ToString();
	}
}
