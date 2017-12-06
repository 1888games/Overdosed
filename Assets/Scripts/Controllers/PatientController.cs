using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PatientController : MonoBehaviourSingleton<PatientController> {


	#region Inspector Variables

	public float pulseRateIncreaseFromPain = 0.7f;
	public float pulseRateIncreaseFromInfection = 0.9f;
	public float pulseRateIncreaseFromBloodLoss = 1.1f;
	public float pulseRateReturnToNormal = 0.5f;

	public float pulseRateMaxBeforeCardiacArrest = 130f;
	public float timeInCardiacArrestUntilDead = 25f;

	public float maximumPulseRateIncreaseFromOverdose = 60f;
	public float maximumPulseRateIncreaseFromUnderdose = 30f;

	public int	scoreForCuringPatient = 5000;


	#endregion


	#region Collections

	public List<Patient> patients;
	public Dictionary<string, float> maximumWrongMedEffects;
	public Dictionary<string, float> minimumWrongMedEffects;

	string[] medicationNames = { "Pain", "Infection", "Blood", "Defib" };

	#endregion



	#region Initialise


	// Use this for initialization
	void Start () {

		patients = new List<Patient> ();
		maximumWrongMedEffects = new Dictionary<string, float> ();
		minimumWrongMedEffects = new Dictionary<string, float> ();

		maximumWrongMedEffects.Add ("Pain", 25f);
		maximumWrongMedEffects.Add ("Infection", 35f);
		maximumWrongMedEffects.Add ("Blood", 45f);
		maximumWrongMedEffects.Add ("Defib", 55f);

		minimumWrongMedEffects.Add ("Pain", 10f);
		minimumWrongMedEffects.Add ("Infection", 20f);
		minimumWrongMedEffects.Add ("Blood", 30f);
		minimumWrongMedEffects.Add ("Defib", 40f);


	}


	public void ClearPatients() { 

		patients.Clear ();

	}

	#endregion



	#region Patient Loop


	// Update is called once per frame
	void Update () {

		if (GameController.Instance.gameOver == false) {

			foreach (Patient p in patients) {

				if (p.inTreatment) {

					//UpdatePatient (p);
					UpdatePatientStatus (p);

				}

			}

		}
	}

	bool RecoverAndCheckWhetherMedicationComplete(Medication m, bool requiresTreatment) {

		if (m.dosesAdministered == 99) {

			m.currentLevel = m.currentLevel - (m.dose / m.effectLength * Time.deltaTime * 3f);

			if (m.currentLevel <= 0f) {

				m.currentLevel = 0f;
				m.dose = 0f;

			} else {

				requiresTreatment = true;
			}


		}

		return requiresTreatment;

	}

	Medication UpdateCurrentLevel(Medication m, Patient p) {

		if (m.dose > 0f && m.dosesAdministered < 99 && p.pulseRate > 0f) {

			if (m.currentLevel > 0f) {
				m.currentLevel = m.currentLevel + (m.dose / m.effectLength * Time.deltaTime);

			} else {

				m.currentLevel = m.currentLevel + (m.dose / m.effectLength * Time.deltaTime) / 2f;
			}

		}

		//Debug.Log (m.name + " " + m.currentLevel);

		return m;

	}

	Patient UpdatePulseRateUpwards(Medication m, Patient p) {

		if (m.dose > 0f &&  p.pulseRate > 0f && m.currentLevel >= m.dose && m.dosesAdministered < 99) {


			if (m.name == "Pain") {

				p.pulseRate = p.pulseRate + pulseRateIncreaseFromPain * Time.deltaTime;

			}

			if (m.name == "Infection") {

				p.pulseRate = p.pulseRate + pulseRateIncreaseFromInfection * Time.deltaTime; 

			}

			if (m.name == "Blood") {

				p.pulseRate = p.pulseRate + pulseRateIncreaseFromBloodLoss * Time.deltaTime; 

			}


		}

		return p;

	}


	Patient ReturnPulseToRestingRate(Patient p, float previousRate) {


		if (previousRate == p.pulseRate) {

			if (p.pulseRate > p.restingRate) {
				p.pulseRate = Mathf.Max (p.pulseRate - (pulseRateReturnToNormal * Time.deltaTime), p.restingRate);
			}

			if (p.pulseRate < p.restingRate) {

				p.pulseRate = Mathf.Min (p.pulseRate + (pulseRateReturnToNormal * Time.deltaTime), p.restingRate);
			}

			//Debug.Log ("Return pulse to normal....");

		} else {

			//Debug.Log ("Pulse changed, leave alone...");

		}

		return p;

	}





	Medication CheckWhetherToStartRecovery (Medication m, Patient p) {

		if (m.dose > 0f && m.dosesAdministered < 99) {

			//Debug.Log (m.name + " has a dose...");

			if (m.dosesAdministered == m.dosesUntilCured && m.currentLevel > m.dose * 0.5f) {

				Debug.Log (m.name + " START RECOVERY!!!");

				if (m.name == "Blood") {

					GameObject bedGo = BedController.Instance.bedToGameObjectMap [p.bed];
					BedHud hud = bedGo.GetComponent<BedHud> ();

					hud.bloodText.transform.parent.transform.gameObject.SetActive (false);

				}

				m.dosesAdministered = 99;
			}

		} else {

			//Debug.Log (m.name + " no dose.");

		}

		return m;

	}


	void CheckPulseRateDanger(Patient p) {


		p.pulseRate = Mathf.Clamp (p.pulseRate, 0f, pulseRateMaxBeforeCardiacArrest);

		if (p.pulseRate >= pulseRateMaxBeforeCardiacArrest) {

			TriggerCardiacArrest (p);

		}


	}


	Patient CheckWhetherFullyRecovered (Patient p, bool stillRequiringTreatment) {

		if (stillRequiringTreatment == false && p.pulseRate > 0f) {

			Debug.Log ("Patient all better!");

			p.inTreatment = false;
			BedController.Instance.WheelBedOutOfBerth (p.bed);
			GameController.Instance.UpdateScore (scoreForCuringPatient);
			GameController.Instance.patientsCured++;
			GameController.Instance.curedText.text = "Cured: " + GameController.Instance.patientsCured.ToString();
			InAudio.Play (this.gameObject, SoundController.Instance.bye);

		}


		return p;


	}


	void UpdatePatientStatus(Patient p) {

		bool intoCardiacArrest = false;
		bool pulseRateRising = false;
		bool stillRequiresTreatment = false;

		float currentPulseRate = p.pulseRate;
	

		for (int i = 0; i < p.currentPrescriptions.Count; i++) {

			Medication m = p.currentPrescriptions [medicationNames [i]];

			string medicationName = m.name;

		
			m = CheckWhetherToStartRecovery (m, p);

			stillRequiresTreatment = RecoverAndCheckWhetherMedicationComplete (m, stillRequiresTreatment);

			float startLevel = m.currentLevel;

			m = UpdateCurrentLevel (m, p);

			if (m.currentLevel != startLevel) {
				stillRequiresTreatment = true;
			}

			p = UpdatePulseRateUpwards (m, p);


		}

		if (p.pulseRate > 0) {

			p = ReturnPulseToRestingRate (p, currentPulseRate);
			CheckPulseRateDanger (p);
			p = CheckWhetherFullyRecovered (p, stillRequiresTreatment);

		} else {

			p = CheckWhetherDead (p);

		}

		UpdateBedVisuals (p);



	}



	Patient PatientIsDead(Patient p ) {

		p.inTreatment = false;

		BedController.Instance.WheelBedOutOfBerth (p.bed);
		GameController.Instance.lives [GameController.Instance.patientsDied].SetActive (false);
		GameController.Instance.patientsDied++;

		GameObject bedGo = BedController.Instance.bedToGameObjectMap [p.bed];
		BedHud hud = bedGo.GetComponent<BedHud> ();

		hud.nameText.text = hud.nameText.text + " (RIP)";

		if (p.isMale) {

			hud.boy.transform.DOLocalMove (new Vector3 (hud.boy.transform.position.x, 10f, hud.boy.transform.position.z), 1f);

		} else {

			hud.girl.transform.DOLocalMove (new Vector3 (hud.girl.transform.position.x, 10f, hud.girl.transform.position.z), 1f);
		}


		InAudio.Play (this.gameObject, SoundController.Instance.dead);

		if (GameController.Instance.patientsDied == 3) {

			GameController.Instance.GameOver ();

		}



		return p;


	}

	Patient CheckWhetherDead(Patient p) {

		p.deathTimer = p.deathTimer + Time.deltaTime;

		if (p.deathTimer >= timeInCardiacArrestUntilDead) {

			p = PatientIsDead (p);


		}


		return p;



	}


	#endregion


	#region Old Code



	#endregion



	#region Setters

	public void SetupPatientVisuals(Patient p) {

		GameObject bedGo = BedController.Instance.bedToGameObjectMap[p.bed];
		BedHud hud = bedGo.GetComponent<BedHud> ();

		hud.successText.gameObject.SetActive (false);

		if (p.isMale) {
			hud.boy.SetActive (true);
			hud.girl.SetActive (false);
		} else {
			hud.boy.SetActive (false);
			hud.girl.SetActive (true);

		}

		hud.nameText.text = p.name;
		hud.pulseRateText.text = p.pulseRate.ToString ();
		hud.traumaBar.sizeDelta = new Vector2 (0f, hud.traumaBar.sizeDelta.y);


		hud.bloodText.transform.parent.transform.gameObject.SetActive (false);
		hud.defibText.transform.parent.transform.gameObject.SetActive (false);



		foreach (Medication m in p.currentPrescriptions.Values) {

			if (m.name == "Pain") {

				if (m.dose > 0f) {
					hud.painText.text = m.dose.ToString();
					hud.painObject.SetActive (true);

				} else {

					hud.painBar.sizeDelta = new Vector2 (0f, hud.painBar.sizeDelta.y);
					hud.painObject.SetActive (false);

				}

			}

			if (m.name == "Infection") {

				if (m.dose > 0f) {
					hud.infectionText.text = m.dose.ToString();
					hud.infectionObject.SetActive (true);
				} else {

					hud.infectionBar.sizeDelta = new Vector2 (0f, hud.infectionBar.sizeDelta.y);
					hud.infectionObject.SetActive (false);

				}

			}

			if (m.name == "Blood") {

				if (m.dose > 0f) {
					hud.successText.gameObject.SetActive (true);
					hud.successText.text = "Administer " + m.dose.ToString() + " units!";
				
					hud.bloodText.transform.parent.transform.gameObject.SetActive (true);
					hud.bloodText.text = m.dose.ToString ();


				} else {

					hud.bloodText.transform.parent.transform.gameObject.SetActive (false);
					hud.successText.gameObject.SetActive (false);
					//hud.traumaBar.sizeDelta = new Vector2 (0f, hud.traumaBar.sizeDelta.y);


				}

			}

		}

		UpdateBedVisuals (p);


	}




	#region Treatment

	bool RequiredThisMedication(Medication m) {

		if (m.dose > 0f) {

			return true;

		}

		return false;

	}

	Patient CheckWhetherCausedBloodLoss(Patient p, Medication m, float injectionQuality) {

		if (injectionQuality < 0.5f && m.name != "Blood" && m.name != "Defib") {

			InAudio.Play (this.gameObject, SoundController.Instance.ouch);

			float chanceOfLoss = Mathf.Pow (10f - (injectionQuality * 10f), 4f);
			float choose = Random.Range (0f, 25000f);

			if (choose < chanceOfLoss) {

				p = TriggerBloodLoss (p);

			}

		}

		return p;


	}

	bool CheckWhetherMedicationFailed (Patient p, Medication m, float injectionQuality) {

		bool failed = false;

		if (injectionQuality < 0.5f && (m.name == "Blood" || m.name == "Defib")) {

			InAudio.Play (this.gameObject, SoundController.Instance.ouch);

			float chanceOfLoss = Mathf.Pow (10f - (injectionQuality * 10f), 4f);
			float choose = Random.Range (0f, 25000f);

			if (choose < chanceOfLoss) {

				failed = true;

			}

		}

		return failed;


	}



	Patient TriggerBloodLoss(Patient p) {

		Debug.Log ("Blood loss!!");

		GameObject bedGo = BedController.Instance.bedToGameObjectMap[p.bed];
		BedHud hud = bedGo.GetComponent<BedHud> ();

		Medication m = p.currentPrescriptions ["Blood"];

		m.dose = (float)UnityEngine.Random.Range (20, 50);
		m.currentLevel =  UnityEngine.Random.Range (m.dose * 0.4f, m.dose);
		m.dosesUntilCured = Random.Range (1, 3);
		m.dosesAdministered = 0;
		m.effectLength = UnityEngine.Random.Range (10f, 20f);

		hud.successText.gameObject.SetActive (true);
		hud.successText.text = "Administer " + m.dose.ToString() + " units!";
		InAudio.Play (this.gameObject, SoundController.Instance.alarm);

		hud.bloodText.transform.parent.transform.gameObject.SetActive (true);
		hud.bloodText.text = m.dose.ToString ();


		return p;



	}

	Medication DoTreatment(Medication m, float amountInjected) {


		m.currentLevel = Mathf.Min (m.dose, m.currentLevel) - amountInjected;

		m.dosesAdministered = m.dosesAdministered + 1;


		return m;


	}

	Medication AdjustTreatmentBasedOnDosage(Patient p, Medication m, BedHud hud) {


		if (m.currentLevel < 0f) {

			//Debug.Log ("Overdose by " + -m.currentLevel);

			m.effectLength = m.effectLength * (1f * (1f - (-m.currentLevel / m.dose)));

			int success =  Mathf.RoundToInt ((-m.currentLevel / m.dose) * 100f);

			GameController.Instance.UpdateScore (success);
			hud.successText.text = success.ToString () + "% overdose!!";
		}

		if (m.currentLevel > 0f) {

			//Debug.Log ("Underdose by " + m.currentLevel);
			m.effectLength = m.effectLength * (1f * (1.5f - (m.currentLevel / m.dose)));

			int success =  Mathf.RoundToInt ((m.currentLevel / m.dose) * 100f);

			GameController.Instance.UpdateScore (success);
			hud.successText.text = success.ToString () + "% underdose";

		}


		return m;


	}

	Patient AdjustPulseBasedOnDosage(Patient p, Medication m) {

		if (p.pulseRate > 0f) {

			if (m.currentLevel < 0f) {

				p.pulseRate = p.pulseRate + maximumPulseRateIncreaseFromOverdose / m.dose * -m.currentLevel;

			}

			if (m.currentLevel > 0f) {

				p.pulseRate = p.pulseRate + maximumPulseRateIncreaseFromUnderdose / m.dose * m.currentLevel;


			}

		} else {

			if (m.name == "Defib") {

				if (m.currentLevel < 0f) {

					p.pulseRate = 70f + maximumPulseRateIncreaseFromOverdose / m.dose * -m.currentLevel;

				}

				if (m.currentLevel > 0f) {

					p.pulseRate = 60f + maximumPulseRateIncreaseFromOverdose / m.dose * m.currentLevel;


				}



			}

		}


		return p;



	}


	void CalculatePainCaused(Patient p, Medication m, BedHud hud, float injectionQuality) {

		hud.successText.gameObject.SetActive (true);

		int painCaused = Mathf.RoundToInt (100f - (injectionQuality * 100f));

		hud.successText.transform.DOPunchScale (new Vector2 (0.1f, 0.1f), 2f, 5)
			.OnComplete (() => DoAccuracyText (hud, painCaused.ToString () + "% pain caused"));


	}

	public void AdministerMedication(Patient p, float amountInjected, float maxAllowed, float injectionQuality, string medName) {


		GameObject bedGo = BedController.Instance.bedToGameObjectMap[p.bed];
		BedHud hud = bedGo.GetComponent<BedHud> ();

		Medication m = p.currentPrescriptions [medName];

		float startingRate = p.pulseRate;

		p = CheckWhetherCausedBloodLoss(p, m, injectionQuality);
		bool failed = CheckWhetherMedicationFailed (p, m, injectionQuality);

		if (RequiredThisMedication(m)) {	

			if (failed == false) {

				m = DoTreatment (m, amountInjected);

				m = AdjustTreatmentBasedOnDosage (p, m, hud);
				p = AdjustPulseBasedOnDosage (p, m);

				CalculatePainCaused (p, m, hud, injectionQuality);

				if (p.pulseRate > 0f && startingRate == 0f) {

					EndCardiacArrest (p);

				}


				p.deathTimer = 0f;

			} else {

				hud.successText.gameObject.SetActive (true);
				hud.successText.text = "Treatment Failed!!!";
				hud.successText.transform.DOPunchScale (new Vector2 (0.1f, 0.1f), 2f, 5)
					.OnComplete (() => TurnOffText (hud));

			}



		} else {

			p.pulseRate = p.pulseRate + Random.Range (minimumWrongMedEffects [medName], maximumWrongMedEffects [medName]);
			hud.successText.gameObject.SetActive (true);
			hud.successText.transform.DOPunchScale (new Vector2 (0.1f, 0.1f), 2f, 5)
				.OnComplete (() => TurnOffText (hud));
			hud.successText.text = "Wrong medication!!!";


		}


	}






	#endregion




	void DoAccuracyText (BedHud hud, string text) {

		hud.successText.text = text;

		hud.successText.transform.DOPunchScale (new Vector2 (0.1f, 0.1f), 2f, 5)
			.OnComplete (() => TurnOffText (hud));

	}



	void TurnOffText(BedHud hud) {

		hud.successText.gameObject.SetActive (false);


	}


	void TriggerCardiacArrest(Patient p) {


		Debug.Log ("Cardiac arrest!!");

		GameObject bedGo = BedController.Instance.bedToGameObjectMap[p.bed];
		BedHud hud = bedGo.GetComponent<BedHud> ();

		Medication m = p.currentPrescriptions ["Defib"];

		m.dose = (float)UnityEngine.Random.Range (50, 200);
		m.currentLevel = m.dose;
		m.dosesUntilCured = 1;
		m.dosesAdministered = 0;
		m.effectLength = 50f;
		p.pulseRate = 0f;

		hud.successText.gameObject.SetActive (true);
		hud.successText.text = "Shock with " + m.dose.ToString() + " joules!";

		hud.defibText.text = m.dose.ToString ();
		hud.defibText.transform.parent.transform.gameObject.SetActive (true);

		InAudio.Play (this.gameObject, SoundController.Instance.beep);


	}

	void EndCardiacArrest(Patient p) {


		Debug.Log ("Ended Cardiac arrest!!");

		GameObject bedGo = BedController.Instance.bedToGameObjectMap[p.bed];
		BedHud hud = bedGo.GetComponent<BedHud> ();

		Medication m = p.currentPrescriptions ["Defib"];

		m.dose = 0f;
		m.currentLevel = m.dose;
		m.dosesUntilCured = 0;
		m.dosesAdministered = 0;
		m.effectLength = 0f;
		p.deathTimer = 0f;

		hud.defibText.transform.parent.transform.gameObject.SetActive (false);

		//InAudio.Play (this.gameObject, SoundController.Instance.beep);


	}

	void CardiacArrest(Patient p) {

		Debug.Log ("Cardiac arrest!!!");

		GameObject bedGo = BedController.Instance.bedToGameObjectMap[p.bed];
		BedHud hud = bedGo.GetComponent<BedHud> ();

		p.pulseRate = 0f;

		Medication defib = new Medication ("Defib", (float)Random.Range (50, 200), 1f, 1f, 1);
		defib.currentLevel = defib.dose;
		p.prescriptions.Add (defib);

		hud.successText.gameObject.SetActive (true);
		hud.successText.text = "Shock with " + defib.dose.ToString() + " joules!";

		InAudio.Play (this.gameObject, SoundController.Instance.beep);


	}



	public void FirePulse(Patient p) {


		if (p.inTreatment && p.pulseRate > 0f) {

			GameObject bedGo = BedController.Instance.bedToGameObjectMap [p.bed];
			BedHud hud = bedGo.GetComponent<BedHud> ();

			float heartRate = 1f / p.pulseRate * 60f;

			hud.heart.transform.DOPunchScale (new Vector3 (0.1f, 0.1f, 0.1f), heartRate, 4)
				.OnComplete (() => FirePulse (p));

		}

	}

	public void UpdateBedVisuals (Patient p) {


		GameObject bedGo = BedController.Instance.bedToGameObjectMap [p.bed];
		BedHud hud = bedGo.GetComponent<BedHud> ();

		hud.pulseRateText.text = Mathf.RoundToInt (p.pulseRate).ToString ();

		foreach (Medication m in p.currentPrescriptions.Values) {

			if (m.name == "Pain") {

				if (m.dose > 0f) {


					p.painLevel = (m.currentLevel / m.dose);
					hud.painBar.sizeDelta = new Vector2 (200f * p.painLevel, hud.painBar.sizeDelta.y);

					//Debug.Log ("Pain: " + m.currentLevel + " / " + m.dose);

				}

			}

			if (m.name == "Infection") {

				if (m.dose > 0f) {

					p.infection = (m.currentLevel / m.dose);
					hud.infectionBar.sizeDelta = new Vector2 (200f * p.infection, hud.infectionBar.sizeDelta.y);

					//Debug.Log ("Infection: " + m.currentLevel + " / " + m.dose);
				} 

			}

			if (m.name == "Blood") {

				if (m.dose > 0f) {

					p.trauma = (m.currentLevel / m.dose);
					hud.traumaBar.sizeDelta = new Vector2 (200f * p.trauma, hud.traumaBar.sizeDelta.y);

					//Debug.Log ("Infection: " + m.currentLevel + " / " + m.dose);
				} 

			}

		}


		if (p.pulseRateTimer <= 0f) {




		}

	}




	#endregion













}
