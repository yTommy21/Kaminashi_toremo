using UnityEngine;
using System.Collections;


public class SandbagController : NewBaseEnemyController {

	// === 外部パラメータ（インスペクタ表示） =====================
	public float 		initHpMax 			= 5.0f;
	public float 		initSpeed 			= 6.0f;
	public bool			jumpActionEnabled 	= true;
	public Vector2 		jumpPower		 	= new Vector2(0.0f,1500.0f); 
	public int			addScore 		 	= 500;

	// === 外部パラメータ ======================================
	[System.NonSerialized] public bool 		cameraRendered			= false;
	[System.NonSerialized] public bool 		attackEnabled			= false;
	[System.NonSerialized] public int		attackDamage 			= 1;
	[System.NonSerialized] public Vector2	attackNockBackVector	= Vector3.zero;
	
	// アニメーションのハッシュ名
	public readonly static int ANISTS_Idle 	 	= Animator.StringToHash("Base Layer.Enemy_Idle");
	public readonly static int ANISTS_Run 	 	= Animator.StringToHash("Base Layer.Enemy_Run");
	public readonly static int ANISTS_Jump 	 	= Animator.StringToHash("Base Layer.Enemy_Jump");
	public readonly static int ANITAG_ATTACK 	= Animator.StringToHash("Attack");
	public readonly static int ANISTS_DMG_A		= Animator.StringToHash("Base Layer.Enemy_DMG_A");
	public readonly static int ANISTS_DMG_B 	= Animator.StringToHash("Base Layer.Enemy_DMG_B");
	public readonly static int ANISTS_Dead 	 	= Animator.StringToHash("Base Layer.Enemy_Dead");

	// ふっとび倍率操作用
	[Header("ふっとび倍率操作用 AddForce")]
	[Header("大ふっとび")]
	public float BigBlowawayVx;
	public float BigBlowawayVy;
	[Header("打ち上げ")]
	public float LaunchVx;
	public float LaunchVy;
	[Header("斜め打ち上げ")]
	public float DisgonallyawayVx;
	public float DisgonallyawayVy;
	[Header("メテオ")]
	public float MeteorVx;
	public float MeteorVy;
	[Header("小ふっとび")]
	public float BlowawayVx;
	public float BlowawayVy;
	[Header("空中小ふっとび")]
	public float JumpBlowawayVx;
	public float JumpBlowawayVy;

	// === キャッシュ ==========================================
	NewPlayerController 	newPlayerCtrl;
	Animator 			playerAnim;
	
	// === コード（Monobehaviour基本機能の実装） ================
	protected override void Awake () {
		base.Awake ();
#if xxx
		Debug.Log (">>> ANISTS_Idle : {0}" + ANISTS_Idle);
		Debug.Log (">>> ANISTS_Run : " + ANISTS_Run);
		Debug.Log (">>> ANISTS_Jump : " + ANISTS_Jump);
		Debug.Log (">>> ANITAG_ATTACK : " + ANITAG_ATTACK);
		Debug.Log (">>> ANISTS_DMG_A : " + ANISTS_DMG_A);
		Debug.Log (">>> ANISTS_DMG_B : " + ANISTS_DMG_B);
		Debug.Log (">>> ANISTS_Dead : " + ANISTS_Dead);
		Debug.Log(string.Format("0 -> {0}",animator.GetLayerName (0)));
		Debug.Log(string.Format("1 -> {0}",animator.GetLayerName (1)));
#endif
		newPlayerCtrl 	= FindObjectOfType<NewPlayerController>();
		playerAnim 	= newPlayerCtrl.GetComponent<Animator>();

		hpMax 	= initHpMax;
		hp 		= hpMax;
		speed 	= initSpeed;
	}

	protected override void Start() {
		base.Start ();

		seAnimationList = new AudioSource[5];
		seAnimationList [0] = AppSound.instance.SE_ATK_A1;
		seAnimationList [1] = AppSound.instance.SE_ATK_A2;
		seAnimationList [2] = AppSound.instance.SE_ATK_A3;
		seAnimationList [3] = AppSound.instance.SE_ATK_ARIAL;
		seAnimationList [4] = AppSound.instance.SE_MOV_JUMP;
	}
	
	protected override void FixedUpdateCharacter () {
		if (!cameraRendered) {
			return;
		}

		// 着地チェック
		if (jumped) {
			if ((grounded && !groundedPrev) || 
				(grounded && Time.fixedTime > jumpStartTime + 1.0f)) {
				jumped 	  = false;
				GetComponent<Rigidbody2D>().gravityScale = gravityScale;
			}
			if (Time.fixedTime > jumpStartTime + 1.0f) {
				GetComponent<Rigidbody2D>().gravityScale = gravityScale;
			}
		} else {
			GetComponent<Rigidbody2D>().gravityScale = gravityScale;
		}

		// キャラの方向
		transform.localScale = new Vector3 (basScaleX * dir, transform.localScale.y, transform.localScale.z);

		// Memo:空中ダメージではX移動を禁止
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (stateInfo.nameHash == SandbagController.ANISTS_DMG_A ||
		    stateInfo.nameHash == SandbagController.ANISTS_DMG_B ||
		    stateInfo.nameHash == SandbagController.ANISTS_Dead) {
			speedVx = 0.0f;
			GetComponent<Rigidbody2D>().velocity = new Vector2 (0.0f, GetComponent<Rigidbody2D>().velocity.y);
		}
	}

	// === コード（基本アクション） =============================
	public void ActionDamage() {
		AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);
		// 大吹っ飛び
		// 打ち上げ
		// 斜め吹っ飛び
		// メテオ
		// 小ふっとび
		// 空中小ふっとび
		if (newPlayerCtrl.kaminashiAttackState == NewPlayerController.KaminashiAttackState.BIGBLOWAWAY) {
			animator.SetTrigger ("DMG_A");
			AddForceAnimatorVx (BigBlowawayVx);
			AddForceAnimatorVy (BigBlowawayVy);
			// ダメージ分類表示用
			// Debug.Log(string.Format(">>> DMG_BigBlowAway {0}",stateInfo.nameHash + " " + newPlayerCtrl.kaminashiAttackState));
		} else if (newPlayerCtrl.kaminashiAttackState == NewPlayerController.KaminashiAttackState.LAUNCH) {
			animator.SetTrigger ("DMG_A");
			AddForceAnimatorVx (LaunchVx);
			AddForceAnimatorVy (LaunchVy);
			// ダメージ分類表示用
			// Debug.Log(string.Format(">>> DMG_Launch {0}",stateInfo.nameHash + " " + newPlayerCtrl.kaminashiAttackState));
		} else if (newPlayerCtrl.kaminashiAttackState == NewPlayerController.KaminashiAttackState.DISGONALLYAWAY){
			animator.SetTrigger ("DMG_A");
			AddForceAnimatorVx (DisgonallyawayVx);
			AddForceAnimatorVy (DisgonallyawayVy);
			// ダメージ分類表示用
			// Debug.Log(string.Format(">>> DMG_DisgonallyAway {0}",stateInfo.nameHash + " " + newPlayerCtrl.kaminashiAttackState));
		}else if (newPlayerCtrl.kaminashiAttackState == NewPlayerController.KaminashiAttackState.METEOR) {
			animator.SetTrigger ("DMG_A");
			AddForceAnimatorVx (MeteorVx);
			AddForceAnimatorVy (MeteorVy);
			// ダメージ分類表示用
			// Debug.Log(string.Format(">>> DMG_Meteor {0}",stateInfo.nameHash + " " + newPlayerCtrl.kaminashiAttackState));
		}else if (newPlayerCtrl.kaminashiAttackState == NewPlayerController.KaminashiAttackState.BLOWAWAY) {
			animator.SetTrigger ("DMG_A");
			AddForceAnimatorVx (BlowawayVx);
			AddForceAnimatorVy (BlowawayVy);
			// ダメージ分類表示用
			// Debug.Log(string.Format(">>> DMG_BlowAway {0}",stateInfo.nameHash + " " + newPlayerCtrl.kaminashiAttackState));
		}else if (newPlayerCtrl.kaminashiAttackState == NewPlayerController.KaminashiAttackState.JUMPBLOWAWAY) {
			animator.SetTrigger ("DMG_A");
			AddForceAnimatorVx (JumpBlowawayVx);
			AddForceAnimatorVy (JumpBlowawayVy);
			// ダメージ分類表示用
			// Debug.Log(string.Format(">>> DMG_JumpBlowAway {0}",stateInfo.nameHash + " " + newPlayerCtrl.kaminashiAttackState));
		}

		if (SetHP(hp,hpMax)) {
			Dead(false);

			int addScoreV = ((int)((float)addScore * (newPlayerCtrl.hp / newPlayerCtrl.hpMax))) * newPlayerCtrl.comboCount;
			addScoreV = (int)((float)addScore * (grounded ? 1.0 : 1.5f));
			NewPlayerController.score += addScoreV;
		}

		newPlayerCtrl.AddCombo();

	}

	#region BOOLEAN_METHOD
	// === 条件判定用関数 ==========================================	

	#endregion

	// === コード（その他） ====================================
	public override void Dead(bool gameOver) {
		base.Dead (gameOver);
	}
}

