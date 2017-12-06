using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Control : MonoBehaviour {

	public Vector2 limitsX;
	public Vector2 limitsY;
	public float moveSpeed = 0.1f;

	public SimpleTouchController leftController;

	float xMove = 0f;
	float yMove = 0f;

	public float normalHeight = 1.41f;

	public Medication medication;

	public Renderer sphere;

	public string medicationName = "Pain";
	string name;

	public List<Material> sphereMaterials;
	public Dictionary<string, int> materialLookup;

	private Rigidbody _rigidbody;

	public bool isOverPoint = false;

	Vector2 joystickPosition = new Vector2(0,0);

	void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		leftController.TouchEvent += LeftController_TouchEvent;
		leftController.TouchStateEvent += LeftController_TouchStateEvent;
	}

	public void LeftController_TouchEvent(Vector2 value) {
		
		joystickPosition = value;

		//Debug.Log (value);



	}


	void LeftController_TouchStateEvent (bool touchPresent)
	{
		if (!touchPresent) {
			joystickPosition = Vector2.zero;
		}

	}
	

	public void OnCollisionStay (Collision collision) {

		name = collision.collider.gameObject.name;

		if (name == "Pain" || name == "Infection" || name == "Blood" || name == "Defib") {

			if (name != medicationName) {
				
				medicationName = name;
				sphere.material = sphereMaterials [materialLookup [name]];
				InAudio.Play (this.gameObject, SoundController.Instance.collect);

			}

		}

	
		//if (collision.collider.gameObject.name == 


	}

	void stopMovement() {

		GameController.Instance.firstClick = false;
	}

	public void CancelInvokes() {
		CancelInvoke ();
	}
	public void OnCollisionEnter(Collision collision) {

		name = collision.collider.gameObject.name;

		if (name != "Floor" && name != "Floor Target" && name != "Point") {
			Invoke ("stopMovement", 0.1f);
		}


		if (name == "Point") {

			GameObject bedGo = collision.collider.gameObject.transform.parent.transform.gameObject;
			Bed b = BedController.Instance.gameObjectToBedMap [bedGo];

			if (b.treatmentDelay <= 0f) {


				GameController.Instance.SetOverPoint (collision.collider.gameObject.transform.parent.transform.gameObject, medicationName);
				isOverPoint = true;

			}
		


		}


	}

	public void OnCollisionExit(Collision collision) {

		CancelInvoke ();

		name = collision.collider.gameObject.name;

		if (name == "Point") {

			GameController.Instance.SetNotOverPoint ();
			isOverPoint = false;


		}


	}

	public void Start() {

		materialLookup = new Dictionary<string, int> ();

		materialLookup.Add ("Pain", 0);
		materialLookup.Add ("Infection", 1);
		materialLookup.Add ("Defib", 2);
		materialLookup.Add ("Blood", 3);

	}

	public void Update() {

		if (GameController.Instance.gameOver == false && GameController.Instance.clicks == 0) {

			xMove = Mathf.Clamp (transform.position.x + Input.GetAxis ("Horizontal") * moveSpeed * Time.deltaTime, limitsX.x, limitsX.y);
			yMove = Mathf.Clamp (transform.position.z + Input.GetAxis ("Vertical") * moveSpeed * Time.deltaTime, limitsY.x, limitsY.y);


//		if (Input.GetAxis ("Horizontal") > 0) {
//			transform.eulerAngles = new Vector3 (0f, 90f, 0f);
//		}
//
//		if (Input.GetAxis ("Horizontal") < 0) {
//			transform.eulerAngles= new Vector3 (0f, -90f, 0f);
//		}

			if (Input.GetAxis ("Horizontal") != 0 || Input.GetAxis ("Vertical") != 0) {

				transform.eulerAngles = new Vector3 (0f, Mathf.Atan2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical")) * Mathf.Rad2Deg, 0f);
				transform.position = (new Vector3 (xMove, transform.position.y, yMove));

			}
				
			xMove = Mathf.Clamp (transform.position.x + joystickPosition.x * moveSpeed * Time.deltaTime, limitsX.x, limitsX.y);
			yMove = Mathf.Clamp (transform.position.z + joystickPosition.y * moveSpeed * Time.deltaTime, limitsY.x, limitsY.y);


			if ( joystickPosition.x != 0 ||  joystickPosition.y  != 0) {

				transform.eulerAngles = new Vector3 (0f, Mathf.Atan2 (joystickPosition.x, joystickPosition.y ) * Mathf.Rad2Deg, 0f);
				transform.position = (new Vector3 (xMove, transform.position.y, yMove));

			}


		}



	}

	public void LateUpdate() {

		transform.eulerAngles = new Vector3 (0f, transform.eulerAngles.y, 0f);
	}
}
