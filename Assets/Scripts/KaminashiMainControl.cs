using UnityEngine;
using System.Collections;

public class KaminashiMainControl : MonoBehaviour {

	NewPlayerController newPlayerCtrl;


	void Awake()
	{
		newPlayerCtrl = FindObjectOfType<NewPlayerController> ();
	}

	void Start () 
	{

	}
	
	void Update () 
	{
		// 操作可能か？
		if (!newPlayerCtrl.activeSts) {
			return;
		} 
		// 移動
		float joyMv = Input.GetAxis ("Horizontal");
		newPlayerCtrl.ActionMove (joyMv);

		// 横入力判定
		if (joyMv > 0.0f || joyMv < 0.0f) {
			newPlayerCtrl.InputStickHorizontal = true;
		} else if (joyMv == 0.0f) {
			newPlayerCtrl.InputStickHorizontal = false;
		}

		// しゃがみ
		if (Input.GetAxisRaw ("Vertical") == -1.0f) {
			newPlayerCtrl.ActionSquat ();
			if (Input.GetButtonDown("Fire2") && !Input.GetButtonDown("Fire1") && newPlayerCtrl.kaminashiState == NewPlayerController.KaminashiState.SQUAT) {
				newPlayerCtrl.ActionAttack_ATK_G ();
			}
			return;
		}

		// ジャンプ
		if (Input.GetButtonDown ("Jump")) {
			newPlayerCtrl.ActionJump ();
			return;
		}

		if (Input.GetAxisRaw ("Vertical") >= 0.8f && Input.GetButtonDown ("Fire1") && !Input.GetButtonDown ("Fire2")) {
			newPlayerCtrl.ActionAttack_ATK_C ();
		}
			
		// Acombo攻撃
		if (Input.GetAxisRaw ("Vertical") < 0.8f && Input.GetButtonDown("Fire1") && !Input.GetButtonDown("Fire2")) {
			if (newPlayerCtrl.kaminashiState == NewPlayerController.KaminashiState.IDLE || newPlayerCtrl.kaminashiState == NewPlayerController.KaminashiState.ATTACK) {
				newPlayerCtrl.ActionAttack_CMB_A ();
			} else if (newPlayerCtrl.kaminashiState == NewPlayerController.KaminashiState.JUMP) {
				//Debug.Log (string.Format ("Vertical {0} {1}",Input.GetAxisRaw ("Vertical"),vp.vertical));
				newPlayerCtrl.ActionAttack_CMBJump_A ();
			}
			return;
		}

		// Bcombo攻撃
		if (Input.GetButtonDown("Fire2") && !Input.GetButtonDown("Fire1")) {
			if (newPlayerCtrl.kaminashiState == NewPlayerController.KaminashiState.IDLE || newPlayerCtrl.kaminashiState == NewPlayerController.KaminashiState.ATTACK) {
				newPlayerCtrl.ActionAttack_CMB_B();
			} else if (newPlayerCtrl.kaminashiState == NewPlayerController.KaminashiState.JUMP) {
				//Debug.Log (string.Format ("Vertical {0} {1}",Input.GetAxisRaw ("Vertical"),vp.vertical));
				newPlayerCtrl.ActionAttack_CMBJump_B();
			}
			return;
		}
	}
		
}
