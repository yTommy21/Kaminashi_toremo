using UnityEngine;
using System.Collections;

public class SandbagBodyCollider : MonoBehaviour {

	SandbagController 	sandbagCtrl;
	NewPlayerController newPlayerCtrl;
	public GameObject Player;
	public GameObject Sandbag;
	Animator 			playerAnim;
	int 				attackHash = 0;

	void Awake () {
		sandbagCtrl 	= FindObjectOfType<SandbagController>();
		playerAnim 	= NewPlayerController.GetAnimator();
	}

	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log ("Enemy OnTriggerEnter2D Hit!: " + other.name);
		//if (sandbagCtrl.cameraRendered) {
			if (other.tag == "PlayerArmBullet") {
			sandbagCtrl.dir = (Player.transform.position.x < Sandbag.transform.position.x) ? +1 : -1;


			//Sandbag.transform.localScale = new Vector3 (sandbagScale.x, sandbagScale.y, sandbagScale.z);;

			sandbagCtrl.ActionDamage ();

			//Camera.main.GetComponent<CameraFollow>().AddCameraSize(-0.01f,-0.3f);
			}
		//}
	}

	void Update () {
		AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);
		if (attackHash != 0 && stateInfo.nameHash == NewPlayerController.ANISTS_Idle) {
			attackHash = 0;
		}
	}
}

