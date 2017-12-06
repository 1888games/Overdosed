using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Medication {

	public string name					{ get; protected set; }

	public int dosesUntilCured			{ get; set; }
	public int dosesAdministered		{ get; set; }

	public float marginError			{ get; protected set; }
	public float dose					{ get; set; }
	public float effectLength 			{ get; set; }
	public float currentLevel			{ get; set; }


	public Medication (string name, float dose, float effectLength, float error, int cured) {

		this.name = name;
		this.dose = dose;
		this.effectLength = effectLength;
		this.currentLevel = 0f;
		this.marginError = error;

		this.dosesUntilCured = cured;
		dosesAdministered = 0;


	}
}
