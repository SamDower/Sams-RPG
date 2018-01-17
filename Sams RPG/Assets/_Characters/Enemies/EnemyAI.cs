using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	public class EnemyAI : MonoBehaviour {

	    [SerializeField] float chaseRadius = 6f;
		[SerializeField] WaypointContainer patrolPath;
		[SerializeField] float waypointTolerance;

		enum State { idle, patrolling, attacking, chasing }
		State state = State.idle;

		PlayerMovement player;
		Character character;

		float currentWeaponRange;
		float distanceToPlayer;
		int nextWaypointIndex = 0;

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
				StartCoroutine (Patrol ());
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

		IEnumerator Patrol() {
			state = State.patrolling;
			while (true) {
				Vector3 nextWaypointPos = patrolPath.transform.GetChild (nextWaypointIndex).position;
				character.SetDestination (nextWaypointPos);
				CycleWaypointWhenClose (nextWaypointPos);
				yield return new WaitForSeconds (0.5f); // TODO Parameterise
			}
		}

		private void CycleWaypointWhenClose (Vector3 nextWaypointPos) {
			if (Vector3.Distance (transform.position, nextWaypointPos) <= waypointTolerance) {
				nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
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