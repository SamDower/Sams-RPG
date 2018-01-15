using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.AI;
using RPG.CameraUI; 

namespace RPG.Characters {
	[SelectionBase]
	public class Character : MonoBehaviour {

		[Header("Animator")]
		[SerializeField] RuntimeAnimatorController animatorController;
		[SerializeField] AnimatorOverrideController animatorOverrideController;
		[SerializeField] Avatar characterAvatar;

		[Header("Audio")]
		[SerializeField] [Range(0f, 1f)] float audioSourceSpatialBlend = 0.5f;

		[Header("Collider")]
		[SerializeField] Vector3 colliderCenter = new Vector3(0f, 0.9f, 0f);
		[SerializeField] float colliderRadius = 0.2f;
		[SerializeField] float colliderHeight = 1.8f;

		[Header("Movement")]
		[SerializeField] float movingTurnSpeed = 360;
		[SerializeField] float stationaryTurnSpeed = 180;
		[SerializeField] float moveThreshold = 1f;
		[SerializeField] float moveSpeedMultiplier = 1f;
		[SerializeField] float animSpeedMultiplier = 1f;

		[Header("Nav Mesh Agent")]
		[SerializeField] float navMeshAgentSteeringSpeed = 1.0f;
		[SerializeField] float navMeshAgentStoppingDistance = 1.3f;

		NavMeshAgent navMeshAgent;
		Animator animator;
		Rigidbody rb;

		float turnAmount;
		float forwardAmount;
		bool isAlive = true;

		void Awake() {
			AddRequiredComponents ();
		}

		void AddRequiredComponents() {
			animator = gameObject.AddComponent<Animator> ();
			animator.runtimeAnimatorController = animatorController;
			animator.avatar = characterAvatar;

			rb = gameObject.AddComponent<Rigidbody> ();
			rb.constraints = RigidbodyConstraints.FreezeRotation;

			var audioSource = gameObject.AddComponent<AudioSource> ();
			audioSource.spatialBlend = audioSourceSpatialBlend;

			var colliderComponent = gameObject.AddComponent<CapsuleCollider> ();
			colliderComponent.center = colliderCenter;
			colliderComponent.radius = colliderRadius;
			colliderComponent.height = colliderHeight;

			navMeshAgent = gameObject.AddComponent<NavMeshAgent> ();
			navMeshAgent.speed = navMeshAgentSteeringSpeed;
			navMeshAgent.stoppingDistance = navMeshAgentStoppingDistance;
			navMeshAgent.autoBraking = false;
			navMeshAgent.updateRotation = false;
			navMeshAgent.updatePosition = true;
		}

		void Update() {
			if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance && isAlive) {
				Move (navMeshAgent.desiredVelocity);
			} else {
				Move (Vector3.zero);
			}
		}

		public void SetDestination(Vector3 worldPos) {
			navMeshAgent.destination = worldPos;
		}

		void Move(Vector3 movement) {
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
			isAlive = false;
		}
	}
}