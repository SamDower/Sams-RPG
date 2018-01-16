using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters {
	public class WeaponSystem : MonoBehaviour {

		[SerializeField] float baseDamage = 10f;
		[SerializeField] WeaponConfig currentWeaponConfig = null;

		const string ATTACK_TRIGGER = "Attack";
		const string DEFAULT_ATTACK = "DEFAULT";

		float lastHitTime;
		GameObject target;
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
			// TODO Use a repeat attack coroutine
		}

		public WeaponConfig GetCurrentWeapon() {
			return currentWeaponConfig;
		}

		private void SetAttackAnimation() {
			animator = GetComponent<Animator> ();
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

		// TODO Use Coroutines
		void AttackTarget () { 
			if (Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBetweenHits()) {
				SetAttackAnimation ();
				animator.SetTrigger (ATTACK_TRIGGER);
				lastHitTime = Time.time;
			} 
		}

		float CalculateDamage() {
			return baseDamage + currentWeaponConfig.GetAdditionalDamage (); // TODO reimpliment critical hit?
		}
	}
}