using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	public class EnemyAI : MonoBehaviour {

	    [SerializeField] float chaseRadius = 6f;
		[SerializeField] WaypointContainer patrolPath;
		[SerializeField] float waypointTolerance;
		[SerializeField] float waypointDwellTime = 2f;

		enum State { idle, patrolling, attacking, chasing }
		State state = State.idle;

		PlayerControl player;
		Character character;

		float currentWeaponRange;
		float distanceToPlayer;
		int nextWaypointIndex = 0;

	    void Start() {
			player = FindObjectOfType<PlayerControl> ();
			character = GetComponent<Character> ();
		}

	    void Update() {
	        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
			WeaponSystem weaponSystem = GetComponent<WeaponSystem> ();
			currentWeaponRange = weaponSystem.GetCurrentWeapon ().GetMaxAttackRange ();

			bool inWeaponCircle = distanceToPlayer <= currentWeaponRange;
			bool inChaseCircle = distanceToPlayer > currentWeaponRange && distanceToPlayer <= chaseRadius;
			bool outsideChaseCircle = distanceToPlayer > chaseRadius;


			if (outsideChaseCircle) {
				StopAllCoroutines ();
				weaponSystem.StopAttacking ();
				StartCoroutine (Patrol ());
			}
			if (inChaseCircle) {
				StopAllCoroutines ();
				weaponSystem.StopAttacking ();
				StartCoroutine (ChasePlayer ());
			}
			if (inWeaponCircle) {
				StopAllCoroutines ();
				state = State.attacking;
				weaponSystem.AttackTarget(player.gameObject);
			}
	    }

		IEnumerator Patrol() {
			state = State.patrolling;
			while (patrolPath != null) {
				Vector3 nextWaypointPos = patrolPath.transform.GetChild (nextWaypointIndex).position;
				character.SetDestination (nextWaypointPos);
				CycleWaypointWhenClose (nextWaypointPos);
				yield return new WaitForSeconds (waypointDwellTime);
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