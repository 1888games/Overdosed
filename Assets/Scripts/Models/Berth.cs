using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berth {

	public Vector2	berthPosition		{ get; protected set; }
	public float	berthRotation		{ get; protected set; }
	public Bed		bed					{ get; set; }
	public int	 	column				{ get; protected set; }

	public Berth (Vector2 pos, float rotation, int col) {

		this.berthPosition = pos;
		this.berthRotation = rotation;
		this.column = col;


	}

	public void AssignBed(Bed b) {

		this.bed = b;



	}

	public void ClearBed() {

		this.bed = null;
	}
}
