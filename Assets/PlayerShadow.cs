using UnityEngine;
using System.Collections;

public class PlayerShadow : MonoBehaviour {

	public GameObject player;

	public float playerYjenePos;

	private NewPlayerController newPlayerCtrl;

	// Use this for initialization
	void Start () {
		newPlayerCtrl = FindObjectOfType<NewPlayerController> ();
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 playerPos = player.transform.position;
		playerYjenePos = playerPos.y;
		if (playerYjenePos < 0) {
			playerYjenePos = 1.0f;
		}
		this.transform.position = new Vector3 (this.transform.position.x, -0.83f, this.transform.position.z);
		// ジャンプすると小さくなる処理
		/*
		if (newPlayerCtrl.kaminashiState == NewPlayerController.KaminashiState.JUMP) {
			this.transform.localScale = new Vector3 (this.transform.localScale.x / playerYjenePos / 200,
				this.transform.localScale.y / playerYjenePos / 200,
				this.transform.localScale.z / playerYjenePos / 200);
		} else if (newPlayerCtrl.kaminashiState == NewPlayerController.KaminashiState.IDLE) {
			this.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			
		}
		*/



	}
}
