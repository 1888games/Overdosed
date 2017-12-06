using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class bounce : MonoBehaviour {

	RectTransform rect;

	public void Start() {

		rect = gameObject.GetComponent<RectTransform> ();



	}
}