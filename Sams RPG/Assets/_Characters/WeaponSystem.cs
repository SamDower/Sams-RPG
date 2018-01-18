using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters {
	public class WeaponSystem : MonoBehaviour {

		[SerializeField] float baseDamage = 10f;
		[SerializeField] WeaponConfig currentWeaponConfig = null;

		const string ATTACK_TRIGGER = "Attack";
		const string DEFAULT_ATTACK = "DEFAULT";

		float lastHitTime;
		GameObject target = null;
		GameObject weaponObject;

		Animator animator;
		Character character;

		void Start () {
			animator = GetComponent<Animator> ();
			character = GetComponent<Character> ();

			PutWeaponInHand (currentWeaponConfig);
			SetAttackAnimation ();
		}

		void Update () {
			bool targetIsDead;
			bool targetIsOutOfRange;
			if (target == null) {
				targetIsDead = false;
				targetIsOutOfRange = false;
			} else {
				var targetHealth = target.GetComponent<HealthSystem> ().healthAsPercentage;
				targetIsDead = targetHealth <= Mathf.Epsilon;
				var distanceToTarget = Vector3.Distance (transform.position, target.transform.position);
				targetIsOutOfRange = distanceToTarget > currentWeaponConfig.GetMaxAttackRange ();
			}

			float characterHealth = GetComponent<HealthSystem> ().healthAsPercentage;
			bool characterIsDead = (characterHealth <= Mathf.Epsilon);

			if (characterIsDead || targetIsOutOfRange || targetIsDead) {
				StopAllCoroutines ();
			}
		}

		public void PutWeaponInHand(WeaponConfig weaponToUse) {
			currentWeaponConfig = weaponToUse;
			var weaponPrefab = weaponToUse.GetWeaponPrefab ();
			GameObject weaponSocket = RequestDominantHand ();
			Destroy (weaponObject);
			weaponObject = Instantiate (weaponPrefab, weaponSocket.transform);
			weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
			weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
		}

		public void AttackTarget (GameObject targetToAttack) {
			target = targetToAttack;
			StartCoroutine (AttackTargetRepeatedly ());
		}

		public void StopAttacking() {
			StopAllCoroutines ();
		}

		IEnumerator AttackTargetRepeatedly() {
			bool attackerStillAlive = GetComponent<HealthSystem> ().healthAsPercentage > Mathf.Epsilon;
			bool targetStillAlive = target.GetComponent<HealthSystem> ().healthAsPercentage > Mathf.Epsilon;
			while (attackerStillAlive && targetStillAlive) {
				float weaponHitPeriod = currentWeaponConfig.GetMinTimeBetweenHits ();
				float timeToWait = weaponHitPeriod * character.GetanimSpeedMultiplier();
				bool isTimeToHitAgain = Time.time - lastHitTime > timeToWait;
				if (isTimeToHitAgain) {
					AttackTargetOnce ();
					lastHitTime = Time.time;
				}
				yield return new WaitForSeconds (timeToWait);
			}
		}

		void AttackTargetOnce() {
			transform.LookAt (target.transform);
			animator.SetTrigger (ATTACK_TRIGGER);
			float damageDelay = currentWeaponConfig.GetDamageDelay ();
			SetAttackAnimation();
			StartCoroutine (DamageAfterDelay (damageDelay));
		}

		IEnumerator DamageAfterDelay(float delay) {
			yield return new WaitForSecondsRealtime (delay);
			target.GetComponent<HealthSystem> ().TakeDamage (CalculateDamage());
		}

		public WeaponConfig GetCurrentWeapon() {
			return currentWeaponConfig;
		}

		private void SetAttackAnimation() {
			var animatorOverrideController = character.GetOverrideController ();
			animator.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController [DEFAULT_ATTACK] = currentWeaponConfig.GetAnimClip ();
		}

		private GameObject RequestDominantHand() {
			var dominantHands = GetComponentsInChildren<DominantHand> ();
			int numberOfDominantHands = dominantHands.Length;
			Assert.IsFalse (numberOfDominantHands <= 0, "No Dominant Hand Found On Player, ADD ONE!");
			Assert.IsFalse (numberOfDominantHands > 1, "Multiple No Dominant Hands Found On Player, REMOVE SOME!");
			return dominantHands [0].gameObject;
		}

		float CalculateDamage() {
			return baseDamage + currentWeaponConfig.GetAdditionalDamage (); // TODO reimpliment critical hit?
		}
	}
}