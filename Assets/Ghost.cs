using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {
	EnemyManager em;
	Animator animator;
	
	void Awake() {
		em = this.transform.parent.GetComponent<EnemyManager>();
		animator = GetComponent<Animator>();
	}
	
	void Start() {
		animator.SetFloat("ScareTimeLeft", 0f);
		animator.SetBool("Dead", false);
	}
	
	void Update() {
		animator.SetFloat("ScareTimeLeft", em.scareTimeLeftPercent);
	}
	
	// MESSAGES
	
	// (not really, )
	void Die() => animator.SetBool("Dead", true);
	
	void OnTriggerEnter2D(Collider2D other) {
		if (!other.CompareTag("Player")) return;
		
		if (em.scareTimeLeftPercent > 0f) {
			Die();
		} else if (!animator.GetBool("Dead")) {
			other.gameObject.SendMessage("Die");
		}
	}
}
