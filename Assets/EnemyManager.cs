using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
	public float scareTimeLength = 7f;
	
	private bool scared;
	private float scareStartTime;
	private float scareTimeLeft;
	public float scareTimeLeftPercent {
		get => scared ? scareTimeLeft / scareTimeLength : 0f;
	}
	
	void Start() {
		SetScared(false);
	}
	
	void Update() {
		scareTimeLeft = scareTimeLength - (Time.time - scareStartTime);
		if (scareTimeLeft <= 0f) {
			SetScared(false);
		}
	}
	
	// OTHER STUFF
	
	void SetScared(bool next) {
		if (scared == next) {
			scareStartTime = Time.time;
			return;
		} else {
			scared = next;
			scareStartTime = Time.time;
			// foreach (Transform thing in this.transform) {
			// 	Ghost ghost = thing.GetComponent<Ghost>();
			// 	if (!ghost) continue;
			// 	// ASDJSKADALSD
			// }
		}
	}
	
	// MESSAGES
	
	void StartScare() => SetScared(true);
}
