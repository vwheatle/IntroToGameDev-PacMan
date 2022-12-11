using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energizer : MonoBehaviour {
	EnemyManager em;
	void Awake() {
		// 🙂👍
		em = transform.parent.parent.GetComponentInChildren<EnemyManager>();
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log("asdlajskd");
		if (!other.CompareTag("Player")) return;
		em.SendMessage("StartScare", SendMessageOptions.DontRequireReceiver);
		this.gameObject.SetActive(false);
	}
}
