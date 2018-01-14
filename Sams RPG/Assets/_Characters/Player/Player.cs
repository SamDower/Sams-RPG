using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

// TODO Consider re-wiring
using RPG.CameraUI; 
using RPG.Core;

namespace RPG.Characters {
	public class Player : MonoBehaviour, IDamageable {

	    [SerializeField] float baseDamage = 10f;
		[SerializeField] Weapon currentWeaponConfig = null;
		[SerializeField] AnimatorOverrideController animatorOverrideController = null;

		[Range(0.1f,1.0f)] [SerializeField] float critChance = 0.1f;
		[SerializeField] float critMultiplier = 1.25f;

		// Temporarily serialized for dubbing
		[SerializeField] AbilityConfig[] abilities;

		const string ATTACK_TRIGGER = "Attack";
		const string DEFAULT_ATTACK = "DEFAULT";

		Enemy enemy = null;
		Animator animator = null;
		CameraRaycaster cameraRaycaster = null;
	    float lastHitTime = 0f;
		GameObject weaponObject;

		void Start() {
			audioSource = GetComponent<AudioSource> ();

			RegisterForMouseClick ();
			SetCurrentMaxHealth ();
			PutWeaponInHand (currentWeaponConfig);
			SetAttackAnimation ();
			AttachInitialAbilities ();
	    }

		private void AttachInitialAbilities() {
			for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++) {
				abilities [abilityIndex].AttackComponentTo (gameObject);
			}
		}

		public void PutWeaponInHand(Weapon weaponToUse) {
			currentWeaponConfig = weaponToUse;
			var weaponPrefab = weaponToUse.GetWeaponPrefab ();
			GameObject weaponSocket = RequestDominantHand ();
			Destroy (weaponObject);
			weaponObject = Instantiate (weaponPrefab, weaponSocket.transform);
			weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
			weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
		}

		void Update() {
			if (healthAsPercentage > Mathf.Epsilon) {
				ScanForAbilityKeyDown ();
			}
		}

		private void ScanForAbilityKeyDown() {
			for (int keyIndex = 1; keyIndex < abilities.Length; keyIndex++) {
				if (Input.GetKeyDown (keyIndex.ToString ())) {
					AttemtSpecialAbility (keyIndex);
				}
			}
		}

		private float SetCurrentMaxHealth () {
			return currentHealthPoints = maxHealthPoints;
			// TODO Play Sound
		}

		private void SetAttackAnimation() {
			animator = GetComponent<Animator> ();
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

		void RegisterForMouseClick () {
			cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
			cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
		}

		void OnMouseOverEnemy (Enemy enemyToSet) {
			this.enemy = enemyToSet;
			if (Input.GetMouseButton (0) && IsTargetInRange(enemy.gameObject)) {
				AttackTarget ();
			} else if (Input.GetMouseButtonDown(1)) {
				AttemtSpecialAbility(0);
			}
		}

		private void AttemtSpecialAbility (int abilityIndex) {
			var energyComponent = GetComponent<Energy> ();
			var energyCost = abilities [abilityIndex].GetEnergyCost ();

			if (energyComponent.IsEnergyAvailable (energyCost)) {
				energyComponent.ConsumeEnergy (energyCost);
				var abilityParams = new AbilityUseParams (enemy, baseDamage);
				abilities [abilityIndex].Use (abilityParams);
			}
		}

		void AttackTarget () {
			if (Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBetweenHits()) {
				SetAttackAnimation ();
				animator.SetTrigger (ATTACK_TRIGGER);
				enemy.TakeDamage (CalculateDamage());
				lastHitTime = Time.time;
			}
		}

		private float CalculateDamage() {
			bool isCriticalHit = Random.Range (0f, 1f) <= critChance;
			float damageBeforeCrit = baseDamage + currentWeaponConfig.GetAdditionalDamage ();
			if (isCriticalHit) {
				return damageBeforeCrit * critMultiplier;
				// TODO Partical? Floating text coming off it?
			}
			return damageBeforeCrit;
		}

		private bool IsTargetInRange(GameObject target) {
			float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
			return distanceToTarget <= currentWeaponConfig.GetMaxAttackRange();
		}
	}
}