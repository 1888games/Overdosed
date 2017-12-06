using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.AI;

public class GameController : MonoBehaviourSingleton<GameController> {

	#region Inspector Variables

	public float fillSpeed = 1f;
	public bool isRunningOnMobile = false;


	#endregion


	#region Member Variables


	public int patientsCured = 0;
	public int patientsDied = 0;
	public int totalPatients = 0;
	int nextMaleID = 0;
	int nextFemaleID = 0;
	public int score = 0;
	public int high = 0;
	public int clicks = 0;

	public float percentageAdministered = 0f;
	public float injectionQuality = 0f;
	public float NurseStressFactor = 1f;

	public bool isOverPoint = false;
	public bool gameOver = false;
	public bool isAdministeringTreatment = false;

	public string medHeld = "";

	float secondTimer = 0f;

	public Vector3 nurseTarget = Vector3.one;

	public GameObject nurse;
	public Control nurseControl;

	public bool firstClick = false;

	#endregion



	#region Collections



	List<string> maleNames;
	List<string> femaleNames;

	public Dictionary<string, string> medTypes;
	public Dictionary<string, string> medDesc;
	public Dictionary<string, float> medMax;
	Dictionary <int, string> restrictLookup;



	#endregion



	#region Instances


	public InAudioNode music;

	public GameObject pgaMeter;
	public GameObject syringeStuff;
	public GameObject syringePlunger;
	public GameObject[] lives;
	public GameObject bedNurseNextTo;
	public GameObject menu;

	public TextMeshProUGUI maxLevel;
	public TextMeshProUGUI medType;
	public TextMeshProUGUI administer;
	public TextMeshProUGUI scoreText;
	public TextMeshProUGUI highText;
	public TextMeshProUGUI curedText;

	public RectTransform pgaBarLeft;
	public RectTransform pgaBarRight;
	public RectTransform pgaClickMark;

	public GameObject mobileButton;


	#endregion




	#region Initialise


	public void Start() {

		medTypes = new Dictionary<string, string> ();
		medDesc = new Dictionary<string, string> ();
		medMax = new Dictionary<string, float> ();
		restrictLookup = new Dictionary<int, string> ();

		restrictLookup.Add (0, "Pain");
		restrictLookup.Add (1, "Infection");

		InAudio.Play (this.gameObject, music);

		medTypes.Add ("Pain", "ml");
		medTypes.Add ("Infection", "ml");
		medTypes.Add ("Blood", "units");
		medTypes.Add ("Defib", "joules");

		medDesc.Add ("Pain", "Pain Meds");
		medDesc.Add ("Infection", "Antibiotics");
		medDesc.Add ("Blood", "Transfusion");
		medDesc.Add ("Defib", "Shock");

		medMax.Add ("Pain", 10f);
		medMax.Add ("Infection", 15f);
		medMax.Add ("Blood", 50f);
		medMax.Add ("Defib", 200f);

		menu.SetActive (true);

		if (!PlayerPrefs.HasKey("High")) {
			PlayerPrefs.SetInt("High",0);
		}

		high = PlayerPrefs.GetInt ("High");
		highText.text = "High: " + high;
		//Invoke ("GameOver", 5f);


	}


	#endregion



	#region Update

	public void StartGame() {

		if (isRunningOnMobile) {
			mobileButton.SetActive (true);
		}
		ResetGame ();

		Invoke ("NewPatient", 1f);
		Invoke ("SpawnPatient", 9f);



	}


	public void OnClickButton() {

		Debug.Log ("Clicked...");

		StartGame ();
		menu.SetActive (false);


	}

	public void Update() {

		if (gameOver == false) {

		
			if (Input.GetMouseButtonDown (0) || Input.GetKeyDown ("space")) {

				LeftClick ();

			} else {

				//CheckMovement ();
			}

			CheckPgaBar ();
			secondTimer += Time.deltaTime;

			if (secondTimer >= 1f) {
				secondTimer = 0f;
				UpdateScore (1);
			}

		} else {



		}



	}

	void CheckMovement() {

		if (isRunningOnMobile && clicks == 0 && firstClick) {


			if (nurseTarget != Vector3.one && nurseTarget != nurse.transform.position) {

				Debug.Log ("STEP...");
				float step = nurseControl.moveSpeed * Time.deltaTime;
				nurse.transform.position = Vector3.MoveTowards(nurse.transform.position, nurseTarget, step);
				nurse.transform.LookAt (nurseTarget);
			
				if (nurseTarget == nurse.transform.position) {
					firstClick = false;
					nurseTarget = Vector3.one;
				}
			}
		}



	}


	void ClickPgaBar() {


		if (clicks == 0) {
			FirstClick ();
			return;
		}
		;

		if (clicks == 1) {

			SecondClick ();
			return;

		}

		if (clicks == 2 || clicks == 99) {

			ThirdClick ();



		}


	}

	void LeftClick() {

		if (isRunningOnMobile && gameOver == false) {

			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, 100)) {
				
				if (hit.collider.CompareTag ("HitPoint")) {

					firstClick = true;

					nurseTarget = new Vector3 (hit.point.x, nurseControl.normalHeight, hit.point.z);

				}

			}



		} else {

			if (bedNurseNextTo != null) {

				ClickPgaBar ();


			}

		}

	}


	#endregion



	#region PGA Meter


	public void OnMobileButtonClicked() {

		if (gameOver == false && bedNurseNextTo != null) {

			ClickPgaBar ();


		}

	}

	void FirstClick() {


		Bed b = BedController.Instance.gameObjectToBedMap [bedNurseNextTo];

		if (b.treatmentDelay <= 0f) {

			InAudio.Play (this.gameObject, SoundController.Instance.bop1);
			isAdministeringTreatment = true;
			fillSpeed = UnityEngine.Random.Range (1.15f * NurseStressFactor, 1.85f * NurseStressFactor);
			clicks = 1;
			//	Debug.Log ("Start Bar");

		}


	}


	void SecondClick() {


		InAudio.Play (this.gameObject, SoundController.Instance.bop2);
		percentageAdministered = pgaBarLeft.sizeDelta.x / 297f;
		Debug.Log ("First Click" + percentageAdministered + pgaBarLeft.sizeDelta.x);
		clicks = 2;

		pgaClickMark.gameObject.SetActive (true);
		pgaClickMark.anchoredPosition= new Vector2 (250.9f - pgaBarLeft.sizeDelta.x, -12f);

		if (medHeld == "Defib") {
			InAudio.Play (this.gameObject, SoundController.Instance.clear);
		}



	}


	void ThirdClick() {

		Debug.Log (pgaBarLeft.sizeDelta.x);

		if (pgaBarLeft.sizeDelta.x < 50f) {

			if (pgaBarLeft.sizeDelta.x > 0f) {

				injectionQuality = (50f - pgaBarLeft.sizeDelta.x) / 50f;

			} else {

				injectionQuality = (50f - pgaBarRight.sizeDelta.x) / 50f;
			}

			InAudio.Play (this.gameObject, SoundController.Instance.bop3);
			//	Debug.Log ("Second Click");
			clicks = 3;
			AdministerDrugs ();
			clicks = 0;

			if (medHeld == "Defib") {
				InAudio.Play (this.gameObject, SoundController.Instance.doof);
			}

		}
			

	}

	void CheckPgaBar() {

		if (isAdministeringTreatment) {

			if (clicks == 1) {


				pgaBarLeft.sizeDelta = new Vector2 (
					pgaBarLeft.sizeDelta.x + 297f / fillSpeed * Time.deltaTime,
					pgaBarLeft.sizeDelta.y);

				if (pgaBarLeft.sizeDelta.x >= 325f) {

					pgaBarLeft.sizeDelta = new Vector2 (
						325f,
						pgaBarLeft.sizeDelta.y);

					percentageAdministered = 325f / 297f;
					clicks = 2;


					InAudio.Play (this.gameObject, SoundController.Instance.bop2);

					if (medHeld == "Defib") {
						InAudio.Play (this.gameObject, SoundController.Instance.clear);
					}
				}


			}

			if (clicks == 2) {

				pgaBarLeft.sizeDelta = new Vector2 (
					pgaBarLeft.sizeDelta.x - 297f / fillSpeed * Time.deltaTime,
					pgaBarLeft.sizeDelta.y);

				if (pgaBarLeft.sizeDelta.x <= 0f) {


					pgaBarLeft.sizeDelta = new Vector2 (0f, pgaBarLeft.sizeDelta.y);

					clicks = 99;

				}

			}

			if (clicks == 99) {


				pgaBarRight.sizeDelta = new Vector2 (
					pgaBarRight.sizeDelta.x + 297 / fillSpeed * Time.deltaTime,
					pgaBarRight.sizeDelta.y);

				if (pgaBarRight.sizeDelta.x >= 28f) {

					pgaBarRight.sizeDelta = new Vector2 (
						28f,
						pgaBarRight.sizeDelta.y);

					injectionQuality = 0f;
					clicks = 3;
					InAudio.Play (this.gameObject, SoundController.Instance.bop3);
					AdministerDrugs ();
					clicks = 0;

					if (medHeld == "Doof") {
						InAudio.Play (this.gameObject, SoundController.Instance.clear);
					}

				}

			}

		}





	}

	public void ResetPgaBar() {

		clicks = 0;
		pgaClickMark.gameObject.SetActive (false);

		percentageAdministered = 0f;
		injectionQuality = 0f;

		pgaBarRight.sizeDelta = new Vector2 (0f,pgaBarRight.sizeDelta.y);
		pgaBarLeft.sizeDelta = new Vector2 (0f,pgaBarLeft.sizeDelta.y);



	}

	#endregion



	void AdministerDrugs() {

		isAdministeringTreatment = false;

		Bed b = BedController.Instance.gameObjectToBedMap [bedNurseNextTo];
	
		PatientController.Instance.AdministerMedication (b.patient, 
			medMax[medHeld] * percentageAdministered, medMax[medHeld], injectionQuality, medHeld);

		b.treatmentDelay = 4f;

	}




	#region Setters




	public void GameOver() {

		mobileButton.SetActive (false);
		menu.SetActive (true);
		gameOver = true;
		CancelInvoke ();

	}

	public void UpdateScore(int addScore) {

		score = score + addScore;
		scoreText.text = "Score: " + score;

		if (score > high) {
			high = score;
			PlayerPrefs.SetInt ("High", high);

			highText.text = "High: " + high;
		}

	}




	void ResetGame () {

		foreach (GameObject go in lives) {

			go.SetActive (true);

		}

		PatientController.Instance.ClearPatients ();

		scoreText.text = "Score: 0";
		curedText.text = "Cured: 0";
		score = 0;
		patientsDied = 0;
		patientsCured = 0;
		isAdministeringTreatment = false;
		MoveBedsToStartingPositions ();
		pgaMeter.SetActive (false);
		gameOver = false;
		firstClick = false;
		SetNotOverPoint ();

	}


	public void SetOverPoint(GameObject bed, string medName) {

		Debug.Log (medName);
		maxLevel.text = medMax [medName].ToString();
		medType.text = medTypes [medName];
		administer.text = medDesc [medName];

		medHeld = medName;

		isOverPoint = true;
		this.bedNurseNextTo = bed;
		pgaMeter.SetActive (true);

		if (isRunningOnMobile) {
			mobileButton.SetActive (true);
		}


	}



	public void SetNotOverPoint() {


	//	maxLevel.text = 
			
		mobileButton.SetActive (false);
		medHeld = "";
		isOverPoint = false;
		this.bedNurseNextTo = null;
		pgaMeter.SetActive (false);
		ResetPgaBar ();


	}

	void MoveBedsToStartingPositions() {

		foreach (Bed b in BedController.Instance.beds) {

			BedController.Instance.ResetBedToStartPosition (b);

		}
			
	}


	#endregion




	#region Spawning

	void SpawnPatient () {

		if (gameOver == false) {

			NewPatient ();
			float delay = 7f + (3.5f * (10f - (float)BedController.Instance.BedsFree ()));
			Invoke ("SpawnPatient", delay);

		}

	}



	string GetPatientName(bool isMale) {

		string name = "";

		if (isMale) {

			if (nextMaleID >= Names.Instance.maleNames.Count) {
				Names.Instance.ShuffleList (Names.Instance.maleNames);
				nextMaleID = 0;
			}

			name = Names.Instance.maleNames [nextMaleID];
			nextMaleID++;

		} else {

			if (nextFemaleID >= Names.Instance.femaleNames.Count) {
				Names.Instance.ShuffleList (Names.Instance.femaleNames);
				nextFemaleID = 0;
			}

			name = Names.Instance.femaleNames [nextFemaleID];
			nextFemaleID++;

		}

		return name;





	}

	bool GetPatientSex() {

		bool isMale = true;
		if (UnityEngine.Random.Range (0, 2) == 0) {
			isMale = false;
		}


		return isMale;

	}


	Patient CreatePatientAndAssignToBed(Bed b, string name, bool isMale) {

		Patient p = new Patient (name, isMale);
		p.AssignToBed (b);
		b.patient = p;
		BedController.Instance.AssignBedToBerth (b);

		totalPatients++;

		p.pulseRate = UnityEngine.Random.Range (70, 90);
		p.restingRate = p.pulseRate;

		PatientController.Instance.patients.Add(p);

		return p;


	}

	public void NewPatient () {

		Bed b = BedController.Instance.GetAvailableBed ();

		if (b != null) {

			bool isMale = GetPatientSex ();
			string name = GetPatientName(isMale);

			Patient p = CreatePatientAndAssignToBed (b, name, isMale);
			p.prescriptions = GetPrescription (p);
			p.currentPrescriptions = GetPrescriptions (p);

			PatientController.Instance.SetupPatientVisuals (p);
			BedController.Instance.WheelBedIn (b);
		
		}

	}


	private void ShuffleList<T>(IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = UnityEngine.Random.Range(0, n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}



	Dictionary<string, Medication> GetPrescriptions (Patient p ) {

		Dictionary<string, Medication> prescription = new Dictionary<string, Medication>();


		prescription.Add ("Pain", GetPainPrescription ());
		prescription.Add ("Infection", GetInfectionPrescription ());
		prescription.Add ("Blood", DecideWhetherBloodLoss (p));
		prescription.Add ("Defib", SetupBlankDefib());

		if (totalPatients < 4 || UnityEngine.Random.Range(0,2) == 0) {

			Medication m = prescription[restrictLookup[UnityEngine.Random.Range(0,2)]];

			m.currentLevel = 0f;
			m.dose = 0f;
			m.dosesUntilCured = 0;

		}

		foreach (Medication m in prescription.Values) {

		//	Debug.Log (m.name + " - Dose: " + m.dose + " Courses: " + m.dosesUntilCured + " Current: " + m.currentLevel);


	
		}

	
		return prescription;


	}


	Medication SetupBlankDefib() {


		Medication defib = new Medication ("Defib", 0f, 0f, 0f, 0);

		return defib;
	
	}

	Medication DecideWhetherBloodLoss(Patient p) {


		float bloodAmt = (float)UnityEngine.Random.Range (20, 50);
		Medication blood = new Medication ("Blood", bloodAmt, UnityEngine.Random.Range (6f, 18f), 5f, UnityEngine.Random.Range (1, 3));
		blood.currentLevel = UnityEngine.Random.Range (blood.dose * 0.4f, blood.dose);

		if (totalPatients > 2 && UnityEngine.Random.Range (0, 8) == 2) {

			GameObject bedGo = BedController.Instance.bedToGameObjectMap [p.bed];
			BedHud hud = bedGo.GetComponent<BedHud> ();

			hud.successText.gameObject.SetActive (true);
			hud.successText.text = "Administer " + bloodAmt.ToString () + " units!";

			hud.bloodText.transform.parent.transform.gameObject.SetActive (true);
			hud.bloodText.text = bloodAmt.ToString();


			InAudio.Play (this.gameObject, SoundController.Instance.alarm);


		} else {


			blood.dose = 0f;
			blood.currentLevel = 0f;
			blood.dosesUntilCured = 0;
		

		}


		return blood;



	}

	Medication GetPainPrescription () {

		float painAmt = (float)UnityEngine.Random.Range (20, 101) / 10f;

		Medication pain = new Medication ("Pain", painAmt, UnityEngine.Random.Range (8f, 24f), 5f, UnityEngine.Random.Range (1, 5));
	
		pain.currentLevel = UnityEngine.Random.Range (pain.dose * 0.4f, pain.dose);

		if (totalPatients == 1) {

			//pain.currentLevel = pain.dose;
			pain.dosesUntilCured = 1;

		}

		return pain;

	}

	Medication GetInfectionPrescription () {

		float infAmt = (float)UnityEngine.Random.Range (25, 151) / 10f;

		Medication infection = new Medication ("Infection", infAmt, UnityEngine.Random.Range (7f, 21f), 5f, UnityEngine.Random.Range (1, 5));

		infection.currentLevel = UnityEngine.Random.Range (infection.dose * 0.55f, infection.dose);

		if (totalPatients == 1) {

		//	infection.currentLevel = infection.dose;
			infection.dosesUntilCured = 1;

		}

		return infection;


	}

	List<Medication> GetPrescription (Patient p) {

		List<Medication> prescription = new List<Medication> ();

		float painAmt = (float)UnityEngine.Random.Range (20, 101) / 10f;
		float infAmt = (float)UnityEngine.Random.Range (25, 151) / 10f;

		Medication pain = new Medication ("Pain", painAmt, UnityEngine.Random.Range (10f, 30f), 5f, UnityEngine.Random.Range (1, 5));
		Medication infection = new Medication ("Infection", infAmt, UnityEngine.Random.Range (10f, 30f), 5f, UnityEngine.Random.Range (1, 5));

		prescription.Add (pain);
		prescription.Add (infection);
	

		pain.currentLevel = UnityEngine.Random.Range (pain.dose * 0.5f, pain.dose);
		infection.currentLevel = UnityEngine.Random.Range (infection.dose * 0.5f, infection.dose);

	

		if (totalPatients == 1) {
			pain.currentLevel = pain.dose;
			infection.currentLevel = infection.dose;
			pain.dosesUntilCured = 1;
			infection.dosesUntilCured = 1;
		}

		ShuffleList (prescription);

		//float bloodAmt2 = (float)UnityEngine.Random.Range (20, 50);
		//Medication blood2 = new Medication ("Blood", bloodAmt2, UnityEngine.Random.Range (10f, 30f), 5f, UnityEngine.Random.Range (1, 2));
		//blood2.currentLevel = UnityEngine.Random.Range (blood2.dose * 0.5f, blood2.dose);
	//	rescription.Add (blood2);


		if (totalPatients < 4) {

			prescription [0].dose = 0f;

		} else {

			if (UnityEngine.Random.Range (0, 2) == 0) {
				prescription [UnityEngine.Random.Range (0, 2)].dose = 0f;
			}

			if (UnityEngine.Random.Range (0, 8) == 2) {

				GameObject bedGo = BedController.Instance.bedToGameObjectMap[p.bed];
				BedHud hud = bedGo.GetComponent<BedHud> ();

				float bloodAmt = (float)UnityEngine.Random.Range (20, 50);
				Medication blood = new Medication ("Blood", bloodAmt, UnityEngine.Random.Range (10f, 30f), 5f, UnityEngine.Random.Range (1, 2));
				blood.currentLevel = UnityEngine.Random.Range (blood.dose * 0.5f, blood.dose);
				prescription.Add (blood);

				hud.successText.gameObject.SetActive (true);
				hud.successText.text = "Administer " + bloodAmt.ToString() + " units!";

				InAudio.Play (this.gameObject, SoundController.Instance.alarm);

			} 

		}

		return prescription;
	}

	#endregion

}


