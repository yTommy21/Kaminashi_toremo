using UnityEngine;
using System.Collections;

public class SandbagMain : MonoBehaviour {

	public enum SandbagState
	{
		LOOKRIGHT,
		LOOKLEFT,
		NULL,
	}

	public SandbagState sandbagState = SandbagState.NULL;

	public GameObject Player;

	float playerX = 0.0f;

	public GameObject Sandbag;

	float sandbagX = 0.0f;

	private KaminashiMainControl kaminashiCtrl;
	private SandbagController sandbagCtrl;

	// Use this for initialization
	void Start () {
		sandbagCtrl = FindObjectOfType<SandbagController> ();
		kaminashiCtrl = FindObjectOfType<KaminashiMainControl> ();
	
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			Sandbag.transform.position = new Vector3(2.49f, 3.13f, 0.0f);
		}

		Vector3 sandbagScale = Sandbag.transform.localScale;


		if (Player.transform.position.x < Sandbag.transform.position.x) {
			Sandbag.transform.localScale = new Vector3 (1.0f, sandbagScale.y, sandbagScale.z);;

		}else if (Player.transform.position.x > Sandbag.transform.position.x) {
			Sandbag.transform.localScale = new Vector3 (-1.0f, sandbagScale.y, sandbagScale.z);;
		}

		//Sandbag.transform.localScale.x = (Player.transform.position.x < Sandbag.transform.position.x) ? +1.0f : -1.0f; 

	}
}
