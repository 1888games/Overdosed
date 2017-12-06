using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BedController : MonoBehaviourSingleton<BedController> {


	#region Collections

	public List<GameObject> bedGameObjects;
	public List<Berth> berths;
	public List<Bed> beds;

	public Dictionary <Bed, GameObject> bedToGameObjectMap;
	public Dictionary <GameObject, Bed> gameObjectToBedMap;


	#endregion

	#region Start


	void Start () {

		bedToGameObjectMap = new Dictionary<Bed, GameObject> ();
		gameObjectToBedMap = new Dictionary<GameObject, Bed> ();
		berths = new List<Berth> ();
		beds = new List<Bed> ();
	
		SetupBeds ();

	}


	#endregion


	#region Update
	

	void Update () {

		foreach (Bed b in beds) {

			UpdateBed (b);

		}
	}


	void UpdateBed(Bed b) {

		if (b.treatmentDelay >= 0f) {

			b.treatmentDelay = b.treatmentDelay - Time.deltaTime;
		}

	}


	#endregion


		


	#region Move Beds


	public void ResetBedToStartPosition(Bed b) {

		GameObject go = bedToGameObjectMap [b];

		BedHud hud = go.GetComponent<BedHud> ();

		hud.boy.transform.localPosition = new Vector3 (0f, 0.747f, 0f);
		hud.girl.transform.localPosition = new Vector3 (0f, 0.747f, 0f);


		go.transform.position = new Vector3 (-20f, 0.73f, 0f);
		go.transform.eulerAngles = new Vector3 (0f, 90f, 0f);



	}



	public void WheelBedIn(Bed b) {

		InAudio.Play (this.gameObject, SoundController.Instance.wheelSound);

		GameObject go = bedToGameObjectMap [b];

		float time = 0.5f + (b.berth.column * 0.35f);

		go.transform.DOMove (new Vector3 (b.berth.berthPosition.x - 2.5f, go.transform.position.y, 0f), time)
			.SetEase(Ease.Linear)
			.OnComplete (() => WheelToBerth (b, go));


	}

	public void WheelBedOutOfBerth(Bed b) {

		InAudio.Play (this.gameObject, SoundController.Instance.wheelSound);
		GameObject go = bedToGameObjectMap [b];

		float time = 0.5f;

		go.transform.DOMove (new Vector3 (b.berth.berthPosition.x + 2.5f, go.transform.position.y, 0f), time)
			.SetEase(Ease.Linear)
			.OnComplete (() => WheelBedOut(b, go));


		float targetRotation = -90f;

		if (b.berth.berthPosition.y < 0f) {
			targetRotation = 270f;
		}

		go.transform.DORotate (new Vector3 (0f, targetRotation, 0f),time);
	


	}


	public void WheelBedOut(Bed b, GameObject go) {

		float time = 0.5f + ((float)(5 - b.berth.column) * 0.35f);

		go.transform.DOMove (new Vector3 (18f, go.transform.position.y, 0f), time)
			.SetEase(Ease.Linear)
			.OnComplete (() => RecycleBed (b, go));


	}


	void WheelToBerth(Bed b, GameObject go) {

		float time = 0.5f;

	//	Debug.Log (" On comp");
		go.transform.DOMove (new Vector3 (b.berth.berthPosition.x, go.transform.position.y, b.berth.berthPosition.y),time).
		SetEase(Ease.Linear);

		float targetRotation = 0f;

		if (b.berth.berthPosition.y < 0) {
			targetRotation = 180f;
		}

		go.transform.DORotate(new Vector3(0f, targetRotation, 0f),time)
			.OnComplete (() => StartTreatment(b.patient));


		//PatientController.Instan

	}


	#endregion



	#region  Private Methods



	void StartTreatment (Patient p) {

		p.inTreatment = true;
		PatientController.Instance.FirePulse (p);

	}


	void RecycleBed(Bed b,  GameObject go) {

		PatientController.Instance.patients.Remove (b.patient);

		Debug.Log ("LEFT HOSPITAL");

		b.berth.ClearBed ();
		b.berth = null;
		b.patient = null;

		b.treatmentDelay = 0f;
		ResetBedToStartPosition (b);

	}


	private void ShuffleList<T>(IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = Random.Range(0, n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}



	#endregion








	#region Getters

	public bool CheckIsAvailableBed() {

		bool available = false;

		for (int i = 0; i < 10; i++) {

			if (beds [i].patient == null) {
				available = true;
				break;
			}
		}

		return available;
	}

	public int BedsFree() {
		
		int bedsFree = 0;

		for (int i = 0; i < 10; i++) {

			if (beds [i].patient == null) {
				bedsFree++;

			}
		}

		return bedsFree;
	}



	public Bed GetAvailableBed() {

		ShuffleList (beds);

		Bed b = null;

		for (int i = 0; i < 10; i++) {

			if (beds [i].patient == null) {

				b = beds [i];
				break;

			}
		}

		return b;

	}

	#endregion

	#region Setters

	public void AssignBedToBerth(Bed b) {

		berths [b.id].AssignBed (b);
		b.AssignToBerth (berths [b.id]);

	}


	void CreateBerth(Vector2 pos, float rotation, int col) {

		Berth b = new Berth (pos, rotation, col);
		berths.Add (b);

	}

	void SetupBeds() {

		CreateBerth (new Vector2 (-7f, 3f), 0f,1);
		CreateBerth (new Vector2 (-3.5f, 3f), 0f,2);
		CreateBerth (new Vector2 (0f, 3f), 0f,3);
		CreateBerth (new Vector2 (3.5f, 3f), 0f,4);
		CreateBerth (new Vector2 (7f, 3f), 0f, 5);

		CreateBerth (new Vector2 (-7f, -3f), 180f, 1);
		CreateBerth (new Vector2 (-3.5f, -3f), 180f, 2);
		CreateBerth (new Vector2 (0f, -3f), 180f, 3);
		CreateBerth (new Vector2 (3.5f, -3f), 180f, 4);
		CreateBerth (new Vector2 (7f, -3f), 180f, 5);

		for (int i = 0; i < 10; i++) {

			Bed b = new Bed (i);
			bedToGameObjectMap.Add (b, bedGameObjects [i]);
			gameObjectToBedMap.Add (bedGameObjects [i], b);
			beds.Add (b);

		}

	}


	#endregion

}


