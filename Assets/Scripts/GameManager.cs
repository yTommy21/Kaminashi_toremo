using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {


	public AudioSource[] SoundSourceList;

	void Awake () {
		SoundSourceList [0].Play ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.Escape)) {
			Application.Quit ();
		}
	}
}
