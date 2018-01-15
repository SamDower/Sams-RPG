using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.AI;
using RPG.CameraUI; 

namespace RPG.Characters {
	[SelectionBase]
	[RequireComponent(typeof(NavMeshAgent))]
	public class CharacterMovement : MonoBehaviour {

		[SerializeField] float movingTurnSpeed = 360;
		[SerializeField] float stationaryTurnSpeed = 180;
		[SerializeField] float moveThreshold = 1f;
		[SerializeField] float stoppingDistance = 1f;
		[SerializeField] float moveSpeedMultiplier = 1f;
		[SerializeField] float animSpeedMultiplier = 1f;

	    Vector3 clickPoint;
		NavMeshAgent agent;
		Animator animator;
		Rigidbody rb;

		float turnAmount;
		float forwardAmount;

	    void Start() {
			animator = GetComponent<Animator>();

			rb = GetComponent<Rigidbody> ();
			rb.constraints = RigidbodyConstraints.FreezeRotation;

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
				Move (agent.desiredVelocity);
			} else {
				Move (Vector3.zero);
			}
		}



		public void Move(Vector3 movement) {
			SetForwardAndTurn (movement);
			ApplyExtraTurnRotation ();
			UpdateAnimator();
		}

		void SetForwardAndTurn (Vector3 movement) {
			if (movement.magnitude > moveThreshold) {
				movement.Normalize ();
			}
			var localMove = transform.InverseTransformDirection (movement);
			turnAmount = Mathf.Atan2 (localMove.x, localMove.z);
			forwardAmount = localMove.z;
		}

		void UpdateAnimator() {
			animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
			animator.SetFloat ("Turn", turnAmount, 0.1f, Time.deltaTime);
			animator.speed = animSpeedMultiplier;
		}

		void ApplyExtraTurnRotation() {
			float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
			transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
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

		void OnAnimatorMove() {
			// we implement this function to override the default root motion.
			// this allows us to modify the positional speed before it's applied.
			if (Time.deltaTime > 0) {
				Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

				// we preserve the existing y part of the current velocity.
				velocity.y = rb.velocity.y;
				rb.velocity = velocity;
			}
		}



		public void Kill() {
			// TODO Allow death signalling
		}
	}
}