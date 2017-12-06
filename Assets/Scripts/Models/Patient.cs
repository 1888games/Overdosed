using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patient  {

	public string 	name			{ get; protected set; }
	public bool 	isMale			{ get; protected set; }
	public float	painLevel		{ get; set; }
	public float	pulseRate		{ get; set; }
	public float	pulseRateTimer	{ get; set; }
	public float	infection		{ get; set; }
	public float	trauma			{ get; set; }
	public float	restingRate 	{ get; set; }
	public Bed		bed				{ get; protected set; }
	public bool		inTreatment;
	public float 	deathTimer = 0f;

	public List<Medication> prescriptions;
	public Dictionary<string, Medication> currentPrescriptions;


	public Patient(string name, bool isMale ) {

		this.name = name;
		this.isMale = isMale;

		this.painLevel = 0;
		this.pulseRate = 0;
		this.infection = 0;
		this.trauma = 0;

		this.inTreatment = false;
	}

	public void AssignToBed(Bed b) {

		this.bed = b;

	}


}
