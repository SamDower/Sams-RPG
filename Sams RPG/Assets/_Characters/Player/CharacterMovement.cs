using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.AI;
using RPG.CameraUI; 

namespace RPG.Characters {
	[RequireComponent(typeof(NavMeshAgent))]
	[RequireComponent(typeof (ThirdPersonCharacter))]
	public class CharacterMovement : MonoBehaviour {
		
		[SerializeField] float stoppingDistance = 1f;

		ThirdPersonCharacter character;
	    Vector3 clickPoint;
	    GameObject walkTarget;
		NavMeshAgent agent;

	    void Start() {
	        character = GetComponent<ThirdPersonCharacter>();
	        walkTarget = new GameObject("walkTarget");

			agent = GetComponent<NavMeshAgent> ();
			agent.updateRotation = false;
			agent.updatePosition = true;
			agent.stoppingDistance = stoppingDistance;

			CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
			cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
			cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
	    }

		void Update() {
			if (agent.remainingDistance > agent.stoppingDistance) {
				character.Move (agent.desiredVelocity, false, false);
			} else {
				character.Move (Vector3.zero, false, false);
			}
		}

		void OnMouseOverPotentiallyWalkable(Vector3 destination) {
			if (Input.GetMouseButton (0)) {
				agent.SetDestination (destination);
			}
		}

		void OnMouseOverEnemy (Enemy enemy) {
			if (Input.GetMouseButton (0) || Input.GetMouseButtonDown(1)) {
				agent.SetDestination (enemy.transform.position);
			}
		}
	}
}