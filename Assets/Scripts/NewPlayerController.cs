using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewPlayerController : NewBaseCharacterController {

	// === 外部パラメータ（インスペクタ表示） =====================
	public float 	initHpMax = 20.0f;
	[Range(0.1f,100.0f)] public float 	initSpeed = 12.0f;

	// === 外部パラメータ ======================================
	// セーブデータパラメータ
	public static	float 		nowHpMax 				= 0;
	public static	float 		nowHp 	  				= 0;
	public static 	int			score 					= 0;

	public static 	bool 		checkPointEnabled 		= false;
	public static 	string		checkPointSceneName 	= "";
	public static 	string		checkPointLabelName 	= "";
	public static	float 		checkPointHp 			= 0;

	public static 	bool 		itemKeyA				= false;
	public static 	bool 		itemKeyB 				= false;
	public static 	bool 		itemKeyC 				= false;

	// 外部からの処理操作用パラメータ
	public static 	bool		initParam 	  			= true;
	public static 	float		startFadeTime 			= 2.0f;

	// 基本パラメータ
	[System.NonSerialized] public float		groundY 	= 0.0f;
	[System.NonSerialized] public bool		superMode	= false;

	[System.NonSerialized] public int 		comboCount	= 0;

	[System.NonSerialized] public Vector3 	enemyActiveZonePointA;
	[System.NonSerialized] public Vector3 	enemyActiveZonePointB;

	public enum KaminashiState
	{
		IDLE,
		RUN,
		WALK,
		ATTACK,
		SQUAT,
		DODGE,
		JUMP,
		JUMPATTACK,
		NULL,
	}

	public KaminashiState kaminashiState;

	public enum KaminashiAttackState
	{
		BLOWAWAY,
		BIGBLOWAWAY,
		LAUNCH,
		DISGONALLYAWAY,
		METEOR,
		JUMPBLOWAWAY,
		NULL,

	}

	public KaminashiAttackState kaminashiAttackState = KaminashiAttackState.NULL;

	// アニメーションのハッシュ名
	#region AnimationHash
	public readonly static int ANISTS_Idle 	 		= Animator.StringToHash("Base Layer.Player_Idle");
	public readonly static int ANISTS_Walk 	 		= Animator.StringToHash("Base Layer.Player_Walk");
	public readonly static int ANISTS_Run 	 	 	= Animator.StringToHash("Base Layer.Player_Run");
	public readonly static int ANISTS_Jump 	 		= Animator.StringToHash("Base Layer.Player_Jump");
	public readonly static int ANISTS_SQUAT         = Animator.StringToHash("Base Layer.Player_Squat");
	public readonly static int ANISTS_DODGE         = Animator.StringToHash("Base Layer.Player_Doge");
	public readonly static int ANISTS_DEAD          = Animator.StringToHash("Base Layer.Player_Dead");
	public readonly static int ANISTS_ATK_A         = Animator.StringToHash("Base Layer.Player_ATK_A");
	public readonly static int ANISTS_ATK_B         = Animator.StringToHash("Base Layer.Player_ATK_B");
	public readonly static int ANISTS_ATK_C         = Animator.StringToHash("Base Layer.Player_ATK_C");
	public readonly static int ANISTS_ATK_D         = Animator.StringToHash("Base Layer.Player_ATK_D");
	public readonly static int ANISTS_ATK_E         = Animator.StringToHash("Base Layer.Player_ATK_E");
	public readonly static int ANISTS_ATK_F         = Animator.StringToHash("Base Layer.Player_ATK_F");
	public readonly static int ANISTS_ATK_G         = Animator.StringToHash("Base Layer.Player_ATK_G");
	public readonly static int ANISTS_CMB_A1        = Animator.StringToHash("Base Layer.Player_CMB_A1");
	public readonly static int ANISTS_CMB_A2 		= Animator.StringToHash("Base Layer.Player_CMB_A2");
	public readonly static int ANISTS_CMB_A3	 	= Animator.StringToHash("Base Layer.Player_CMB_A3");
	public readonly static int ANISTS_CMB_A4        = Animator.StringToHash("Base Layer.Player_CMB_A4");
	public readonly static int ANISTS_CMB_B1        = Animator.StringToHash("Base Layer.Player_CMB_B1");
	public readonly static int ANISTS_CMB_B2 		= Animator.StringToHash("Base Layer.Player_CMB_B2");
	public readonly static int ANISTS_CMB_B3	 	= Animator.StringToHash("Base Layer.Player_CMB_B3");
	public readonly static int ANISTS_CMB_B4        = Animator.StringToHash("Base Layer.Player_CMB_B4");
	public readonly static int ANISTS_CMBJUMP_A1  = Animator.StringToHash("Base Layer.Player_CMBJump_A1");
	public readonly static int ANISTS_CMBJUMP_A2  = Animator.StringToHash("Base Layer.Player_CMBJump_A2");
	public readonly static int ANISTS_CMBJUMP_A3  = Animator.StringToHash("Base Layer.Player_CMBJump_A3");
	public readonly static int ANISTS_CMBJUMP_B1  = Animator.StringToHash("Base Layer.Player_CMBJump_B1");
	public readonly static int ANISTS_CMBJUMP_B2  = Animator.StringToHash("Base Layer.Player_CMBJump_B2");
	public readonly static int ANISTS_CMBJUMP_B3  = Animator.StringToHash("Base Layer.Player_CMBJump_B3");
	#endregion

	//public readonly static string ANISTS = animator.GetCurrentAnimatorStateInfo (0).IsName;

	// === キャッシュ ==========================================
	LineRenderer	hudHpBar;
	TextMesh		hudScore;
	TextMesh 		hudCombo;

	// === 内部パラメータ ======================================
	int 			jumpCount			= 0;

	volatile bool 	atkInputEnabled		= false;
	volatile bool	atkInputNow			= false;
	bool hasAttackEnd = false;
	public bool InputStickHorizontal = false;

	bool			breakEnabledMovement= true;
	bool            breakEnabledSquat   = false;
	float 			groundFriction		= 0.0f;

	float 			comboTimer 			= 0.0f;


	// === コード（サポート関数） ===============================
	public static GameObject GetGameObject() {
		return GameObject.FindGameObjectWithTag ("Player");
	}
	public static Transform GetTranform() {
		return GameObject.FindGameObjectWithTag ("Player").transform;
	}
	public static NewPlayerController GetController() {
		return GameObject.FindGameObjectWithTag ("Player").GetComponent<NewPlayerController>();
	}
	public static Animator GetAnimator() {
		return GameObject.FindGameObjectWithTag ("Player").GetComponent<Animator>();
	}

	// === コード（Monobehaviour基本機能の実装） ================
	protected override void Awake () {
		base.Awake ();

		#if xxx
		Debug.Log (">>> ANISTS_Idle : " + ANISTS_Idle);
		Debug.Log (">>> ANISTS_Walk : " + ANISTS_Walk);
		Debug.Log (">>> ANISTS_Run : " + ANISTS_Run);
		Debug.Log (">>> ANISTS_Jump : " + ANISTS_Jump);
		Debug.Log (">>> ANITAG_ATTACK_A : " + ANISTS_ATTACK_A);
		Debug.Log (">>> ANITAG_ATTACK_B : " + ANISTS_ATTACK_B);
		Debug.Log (">>> ANITAG_ATTACK_C : " + ANISTS_ATTACK_C);
		Debug.Log(string.Format("0 -> {0}",playerAnim.GetLayerName (0)));
		Debug.Log(string.Format("1 -> {0}",playerAnim.GetLayerName (1)));
		#endif

		// !!! ガベコレ強制実行 !!!
		System.GC.Collect ();
		// !!!!!!!!!!!!!!!!!!!!!

		// キャッシュ
		//hudHpBar 		= GameObject.Find ("HUD_HPBar").GetComponent<LineRenderer> ();
		//hudScore 		= GameObject.Find ("HUD_Score").GetComponent<TextMesh> ();
		//hudCombo 		= GameObject.Find ("HUD_Combo").GetComponent<TextMesh> ();

		// パラメータ初期化
		speed 			= initSpeed;
		groundY 		= groundCheck_C.transform.position.y + 2.0f;

		// アクティブゾーンをBoxCollider2Dから取得
		//BoxCollider2D boxCol2D = transform.Find("Collider_EnemyActiveZone").GetComponent<BoxCollider2D>();
		//enemyActiveZonePointA = new Vector3 (boxCol2D.offset.x - boxCol2D.size.x / 2.0f, boxCol2D.offset.y - boxCol2D.size.y / 2.0f);
		//enemyActiveZonePointB = new Vector3 (boxCol2D.offset.x + boxCol2D.size.x / 2.0f, boxCol2D.offset.y + boxCol2D.size.y / 2.0f);
		//boxCol2D.transform.gameObject.SetActive(false);
	}


	protected override void Start() {
		base.Start ();

		//zFoxFadeFilter.instance.FadeIn (Color.black, startFadeTime);
		startFadeTime = 2.0f;

		seAnimationList = new AudioSource[5];
		seAnimationList [0] = AppSound.instance.SE_ATK_A1;
		seAnimationList [1] = AppSound.instance.SE_ATK_A2;
		seAnimationList [2] = AppSound.instance.SE_ATK_A3;
		seAnimationList [3] = AppSound.instance.SE_ATK_ARIAL;
		seAnimationList [4] = AppSound.instance.SE_MOV_JUMP;
	}

	protected override void Update () {
		base.Update ();

		/*
		// ステータス表示
		hudHpBar.SetPosition (1, new Vector3 (5.0f * (hp / hpMax), 0.0f, 0.0f));
		hudScore.text = string.Format("Score {0}",score);

		if (comboTimer <= 0.0f) {
			hudCombo.gameObject.SetActive(false);
			comboCount = 0;
			comboTimer = 0.0f;
		} else {
			comboTimer -= Time.deltaTime;
			if (comboTimer > 5.0f) {
				comboTimer = 5.0f;
			}
			float s = 0.3f + 0.5f * comboTimer;
			hudCombo.gameObject.SetActive(true);
			hudCombo.transform.localScale = new Vector3(s,s,1.0f);
		}
		*/

		#if xxx
		// Debug
		BoxCollider2D boxCol2D = GameObject.Find("Collider_EnemyActiveZone").GetComponent<BoxCollider2D>();
		Vector3 vecA = transform.position + new Vector3 (boxCol2D.center.x - boxCol2D.size.x / 2.0f, boxCol2D.center.y - boxCol2D.size.y / 2.0f);
		Vector3 vecB = transform.position + new Vector3 (boxCol2D.center.x + boxCol2D.size.x / 2.0f, boxCol2D.center.y + boxCol2D.size.y / 2.0f);
		Collider2D[] col2DList = Physics2D.OverlapAreaAll (vecA,vecB);
		foreach(Collider2D col2D in col2DList) {
		if (col2D.tag == "EnemyBody") {
		col2D.GetComponentInParent<EnemyMain>().cameraEnabled = true;
		}
		}
		#endif		
	}

	protected override void FixedUpdateCharacter () {
		// 現在のステートを取得
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

		// 着地チェック
		if (jumped) {
			if ((grounded && !groundedPrev) || 
				(grounded && Time.fixedTime > jumpStartTime + 1.0f)) {
				animator.SetBool ("hasJump", false);
				jumped 	  = false;
				jumpCount = 0;
				GetComponent<Rigidbody2D>().gravityScale = gravityScale;
			}
			if (Time.fixedTime > jumpStartTime + 1.0f) {
				if (stateInfo.nameHash == ANISTS_Idle || stateInfo.nameHash == ANISTS_Walk || 
					stateInfo.nameHash == ANISTS_Run  || stateInfo.nameHash == ANISTS_Jump) {
					GetComponent<Rigidbody2D>().gravityScale = gravityScale;
				}
			}
		} else {
			jumpCount = 0;
			GetComponent<Rigidbody2D>().gravityScale = gravityScale;
		}

		// 攻撃中か？
		if (kaminashiState == KaminashiState.ATTACK) {
			// 移動停止
			speedVx = 0;
			ActionMove (0.0f);
		}

		// キャラの方向（攻撃中やジャンプ中に振り向き禁止にする
		if (kaminashiState == KaminashiState.ATTACK) {
			transform.localScale = new Vector3 (basScaleX * dir, transform.localScale.y, transform.localScale.z);
		} else {
			// キャラの方向
			transform.localScale = new Vector3 (basScaleX * dir, transform.localScale.y, transform.localScale.z);
		}
		/*
		if (kaminashiState == KaminashiState.ATTACK) {
			float localX = transform.localScale.x;
			if (localX <= 1.0f) {
				transform.localScale = new Vector3 (1.0f, transform.localScale.y, transform.localScale.z);
			}else if (localX >= -1.0f) {
				transform.localScale = new Vector3 (-1.0f, transform.localScale.y, transform.localScale.z);
			}
		} else {
			// キャラの方向
			transform.localScale = new Vector3 (basScaleX * dir, transform.localScale.y, transform.localScale.z);
		}
		*/

		// ジャンプ中の横移動減速
		if (jumped && !grounded && groundCheck_OnMoveObject == null) {
			if (breakEnabledMovement) {
				breakEnabledMovement = false;
				speedVx *= 0.9f;
			}
		}

		// 移動停止（減速）処理
		if (breakEnabledMovement) {
			speedVx *= groundFriction;
		}

		// 座ってる時の立ち判定
		if (breakEnabledSquat) {
			if (Input.GetAxisRaw ("Vertical") != -1.0f) {
				animator.SetBool ("hasSquat", false);
				kaminashiState = KaminashiState.IDLE;
				breakEnabledSquat = false;
			}
		}

		Camera.main.transform.position = transform.position + Vector3.back;
	}

	// === コード（アニメーションイベント用コード） ===============
	public void EnebleAttackInput() {
		atkInputEnabled = true;
	}

	public void SetNextAttack(string name) {
		if (atkInputNow == true) {
			atkInputNow = false;
			animator.Play (name);
		} else if (atkInputNow == false) {
			animator.Play ("Player_Idle");
		}
	}

	public void SetNextCommandAttack(string name) {
		if (atkInputNow == true || InputStickHorizontal == true) {
			atkInputNow = false;
			animator.Play (name);
		} else if (atkInputNow == false) {
			animator.Play ("Player_Idle");
		}
	}

	public void SetATK_F() {
		if (atkInputNow == true) {
			atkInputNow = false;
			ActionAttack_ATK_F ();
		}
	}

	public void SetStateToJump() {
		kaminashiState = KaminashiState.JUMP;
	}

	public void SetStateToAttack() {
		kaminashiState = KaminashiState.ATTACK;
	}

	public void WaitForComplete()
	{
		ActionMove (0.0f);
		StartCoroutine (WaitForSec ());
	}

	IEnumerator WaitForSec() {
		yield return new WaitForSeconds (0.5f);
	}
	// 仮のStateをIDLEに移行するScript
	public void SetStateLanding() {
		kaminashiState = KaminashiState.IDLE;
	}

	#region SetAttackStateMethod
	public void SetAttackStateToBlowAway() {
		kaminashiAttackState = KaminashiAttackState.BLOWAWAY;
	}
	public void SetAttackStateToBigBlowAway() {
		kaminashiAttackState = KaminashiAttackState.BIGBLOWAWAY;
	}
	public void SetAttackStateToLaunch() {
		kaminashiAttackState = KaminashiAttackState.LAUNCH;
	}
	public void SetAttackStateToSisgonallyAway() {
		kaminashiAttackState = KaminashiAttackState.DISGONALLYAWAY;
	}
	public void SetAttackStateToMeteor() {
		kaminashiAttackState = KaminashiAttackState.METEOR;
	}
	public void SetAttackStateToJumpBlowaway() {
		kaminashiAttackState = KaminashiAttackState.JUMPBLOWAWAY;
	}
	public void SetAttackStateToDefault() {
		kaminashiAttackState = KaminashiAttackState.NULL;
	}

	#endregion
	// === コード（基本アクション） =============================
	public override void ActionMove(float n) {
		if (!activeSts) {
			return;
		}

		// 初期化
		float dirOld = dir;
		breakEnabledMovement = false;

		// アニメーション指定
		float moveSpeed = Mathf.Clamp(Mathf.Abs (n),-1.0f,+1.0f);
		animator.SetFloat("MoveSpeed",moveSpeed);
		//animator.speed = 1.0f + moveSpeed;

		// 移動チェック
		if (n != 0.0f) {
			// 移動
			dir 	  = Mathf.Sign(n);
			moveSpeed = (moveSpeed < 0.5f) ? (moveSpeed * (1.0f / 0.5f)) : 1.0f;
			speedVx   = initSpeed * moveSpeed * dir;
		} else {
			// 移動停止
			breakEnabledMovement = true;
		}

		// その場振り向きチェック
		if (dirOld != dir) {
			breakEnabledMovement = true;
		}
	}

	public void ActionSquat() {
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		speedVx = 0;
		if (stateInfo.nameHash == ANISTS_Idle || stateInfo.nameHash == ANISTS_Walk || stateInfo.nameHash == ANISTS_Run) {
			kaminashiState = KaminashiState.SQUAT;
			animator.SetBool ("hasSquat", true);
			breakEnabledSquat = true;
		}
	}

	public void ActionJump() {
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (stateInfo.nameHash == ANISTS_Idle || stateInfo.nameHash == ANISTS_Walk || stateInfo.nameHash == ANISTS_Run || 
			(stateInfo.nameHash == ANISTS_Jump && GetComponent<Rigidbody2D>().gravityScale >= gravityScale)) {
			kaminashiState = KaminashiState.JUMP;
			switch(jumpCount) {
			case 0 :
				if (grounded) {
					//animator.SetTrigger ("Jump");
					animator.SetBool ("hasJump", true);
					//rigidbody2D.AddForce (new Vector2 (0.0f, 1500.0f));	// Bug
					GetComponent<Rigidbody2D>().velocity = Vector2.up * 32.5f;
					jumpStartTime = Time.fixedTime;
					jumped = true;
					jumpCount ++;
				}
				break;
			case 1 :
				if (!grounded) {
					animator.Play("Player_Jump",0,0.0f);
					GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x,20.0f);
					jumped = true;
					jumpCount ++;
				}
				break;
			}
			//Debug.Log(string.Format("Jump 1 {0} {1} {2} {3}",jumped,transform.position,grounded,groundedPrev));
			//Debug.Log(groundCheckCollider[1].name);
			//AppSound.instance.SE_MOV_JUMP.Play ();
		}
	}

	// 地上Aコンボ
	public void ActionAttack_CMB_A() {
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (stateInfo.nameHash == ANISTS_Idle) {

			animator.SetTrigger ("Combo_A1");
			kaminashiState = KaminashiState.ATTACK;

			if (stateInfo.nameHash == ANISTS_Jump) {
				GetComponent<Rigidbody2D>().velocity     = new Vector2(0.0f,0.0f);
				GetComponent<Rigidbody2D>().gravityScale = 0.1f;
			}
		} else {
			if (atkInputEnabled) {
				atkInputEnabled = false;
				atkInputNow 	= true;
			}
		}
	}

	// 地上Bコンボ
	public void ActionAttack_CMB_B() {
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (stateInfo.nameHash == ANISTS_Idle) {

			animator.SetTrigger ("Combo_B1");
			kaminashiState = KaminashiState.ATTACK;

			/*
			if (stateInfo.nameHash == ANISTS_Jump || stateInfo.nameHash == ANISTS_ATTACK_C) {
				GetComponent<Rigidbody2D>().velocity     = new Vector2(0.0f,0.0f);
				GetComponent<Rigidbody2D>().gravityScale = 0.1f;
			}*/
		} else {
			if (atkInputEnabled) {
				atkInputEnabled = false;
				atkInputNow 	= true;
			}
		}
	}

	// 空中Aコンボ
	public void ActionAttack_CMBJump_A() {
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (stateInfo.nameHash == ANISTS_Jump && kaminashiState == KaminashiState.JUMP) {
			animator.SetTrigger ("JumpCombo_A1");
			jumpCount = 2;
		} else {
			if (atkInputEnabled) {
				atkInputEnabled = false;
				atkInputNow 	= true;
			}
		}
	}

	// 空中Bコンボ
	public void ActionAttack_CMBJump_B() {
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (stateInfo.nameHash == ANISTS_Jump && kaminashiState == KaminashiState.JUMP) {
			animator.SetTrigger ("JumpCombo_B1");
			jumpCount = 2;
		} else {
			if (atkInputEnabled) {
				atkInputEnabled = false;
				atkInputNow 	= true;
			}
		}
	}

	// 地上切り上げ攻撃
	public void ActionAttack_ATK_C() {
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo (0);
		if (stateInfo.nameHash == ANISTS_Idle) {
			animator.Play ("Player_ATK_C");
			animator.SetBool ("hasJump", true);
			jumpStartTime = Time.fixedTime;
			jumped = true;
			jumpCount = 2;
		}
	}

	// 空中横A攻撃
	public void ActionAttack_ATK_D() {
	}

	// 空中横B攻撃
	public void ActionAttack_ATK_E() {
	}


	// しゃがみ切り上がり攻撃
	public void ActionAttack_ATK_F() {
		animator.Play ("Player_ATK_F");
		animator.SetBool ("hasJump", true);
		jumpStartTime = Time.fixedTime;
		jumped = true;
		jumpCount = 2;
	}

	// しゃがみ打ち上げ攻撃
	public void ActionAttack_ATK_G() {
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo (0);
		if (stateInfo.nameHash == ANISTS_SQUAT && kaminashiState == KaminashiState.SQUAT) {
			animator.SetTrigger ("Attack_G");
		} else {
			if (atkInputEnabled) {
				atkInputEnabled = false;
				atkInputNow 	= true;
			}
		}
	}

	public void ActionDamage(float damage) {
		/*
		// Debug:無敵モード
		if (SaveData.debug_Invicible) {
			return;
		}
		*/
		// ダメージ処理をしてもいいか？
		if (!activeSts) {
			return;
		}

		#if xxx
		// ランダムにヒット音を再生
		switch(Random.Range(0,3)) {
		case 0 : AppSound.instance.SE_HIT_A1.Play (); break;
		case 1 : AppSound.instance.SE_HIT_A2.Play (); break;
		case 2 : AppSound.instance.SE_HIT_A3.Play (); break;
		}
		#else
		// ヒット音を再生
		//AppSound.instance.SE_HIT_A1.Play ();
		#endif

		#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
		Handheld.Vibrate();
		#endif

		animator.SetTrigger ("DMG_A");
		speedVx = 0;
		GetComponent<Rigidbody2D>().gravityScale = gravityScale;

		// Combo Reset
		comboCount = 0;
		comboTimer = 0.0f;

		if (jumped) {
			damage *= 1.5f;
		}

		if (SetHP(hp - damage,hpMax)) {
			Dead(true); // 死亡
		}
	}

	// === コード（その他） ====================================
	public override void Dead(bool gameOver) {
		// 死亡処理をしてもいいか？
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (!activeSts || stateInfo.nameHash == ANISTS_DEAD) {
			return;
		}

		base.Dead (gameOver);

		//zFoxFadeFilter.instance.FadeOut (Color.black, 2.0f);

		if (gameOver) {
			SetHP(0,hpMax);
			Invoke ("GameOver", 3.0f);
		} else {
			SetHP(hp / 2,hpMax);
			Invoke ("GameReset", 3.0f);
		}

		GameObject.Find ("HUD_Dead").GetComponent<MeshRenderer> ().enabled = true;
		GameObject.Find ("HUD_DeadShadow").GetComponent<MeshRenderer> ().enabled = true;
		if (GameObject.Find ("VRPad") != null) {
			GameObject.Find ("VRPad").SetActive(false);
		}
	}

	public void GameOver() {
		NewPlayerController.score = 0;
		NewPlayerController.nowHp = NewPlayerController.checkPointHp;

		Application.LoadLevel("Menu_HiScore");
	}

	void GameReset() {
		//SaveData.SaveGamePlay ();
		Application.LoadLevel(Application.loadedLevelName);
	}

	public override bool SetHP(float _hp,float _hpMax) {
		if (_hp > _hpMax) {
			_hp = _hpMax;
		}
		nowHp 		= _hp;
		nowHpMax 	= _hpMax;
		return base.SetHP (_hp, _hpMax);
	}

	public void AddCombo() {
		comboCount ++;
		comboTimer += 1.0f;
		//hudCombo.text = string.Format("Combo {0}",comboCount);
	}

	#region BOOLEAN_METHOD
	// === 条件判定用関数 ==========================================	
	public bool KaminashiJumpSquatCheck() {
		if (kaminashiState != NewPlayerController.KaminashiState.JUMP) { return false; }
		if (kaminashiState != NewPlayerController.KaminashiState.SQUAT) { return false; }
		return true;
	}

	bool KaminashiAttackCheck() {
		return true;
	}
	#endregion
}
