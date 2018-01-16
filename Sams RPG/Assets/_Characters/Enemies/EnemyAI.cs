using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	public class EnemyAI : MonoBehaviour {

	    [SerializeField] float chaseRadius = 6f;

		enum State { idle, patrolling, attacking, chasing }
		State state = State.idle;

		PlayerMovement player;
		Character character;

		float currentWeaponRange;
		float distanceToPlayer;

	    void Start() {
			player = FindObjectOfType<PlayerMovement> ();
			character = GetComponent<Character> ();
		}

	    void Update() {
	        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
			WeaponSystem weaponSystem = GetComponent<WeaponSystem> ();
			currentWeaponRange = weaponSystem.GetCurrentWeapon ().GetMaxAttackRange ();

			if (distanceToPlayer > chaseRadius && state != State.patrolling) {
				StopAllCoroutines ();
				state = State.patrolling; // TODO Replace to use coroutine
			}
			if (distanceToPlayer <= chaseRadius && state != State.chasing) {
				StopAllCoroutines ();
				StartCoroutine (ChasePlayer ());
			}
			if (distanceToPlayer <= currentWeaponRange && state != State.attacking) {
				StopAllCoroutines ();
				state = State.attacking; // TODO Replace to use coroutine
			}
	    }

		IEnumerator ChasePlayer() {
			state = State.chasing;
			while (distanceToPlayer >= currentWeaponRange) {
				character.SetDestination (player.transform.position);
				yield return new WaitForEndOfFrame ();
			}
		}

	    void OnDrawGizmos() {
	        // Draw attack sphere 
	        Gizmos.color = new Color(255f, 0, 0, .5f);
	        Gizmos.DrawWireSphere(transform.position, currentWeaponRange);

	        // Draw chase sphere 
	        Gizmos.color = new Color(0, 0, 255, .5f);
	        Gizmos.DrawWireSphere(transform.position, chaseRadius);
	    }
	}
}