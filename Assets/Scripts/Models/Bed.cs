using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed  {


	public int id;
	public Patient patient;
	public Berth berth;
	public float treatmentDelay = 0f;

	public Bed (int id) {

		this.id = id;
	}

	public void AssignToBerth(Berth b) {

		berth = b;

	}

	public void ClearBerth() {

		berth = null;

	}

}
