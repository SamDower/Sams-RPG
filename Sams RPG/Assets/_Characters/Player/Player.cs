using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using RPG.CameraUI; // TODO Consider re-wiring
using RPG.Core;
using RPG.Weapons;

namespace RPG.Characters {
	public class Player : MonoBehaviour, IDamageable {

	    [SerializeField] float maxHealthPoints = 100f;
	    [SerializeField] float baseDamage = 10f;
		[SerializeField] Weapon weaponInUse = null;
		[SerializeField] AnimatorOverrideController animatorOverrideController = null;

		// Temporarily serialized for dubbing
		[SerializeField] SpecialAbility[] abilities;

		Animator animator;
	    float currentHealthPoints;
	    CameraRaycaster cameraRaycaster;
	    float lastHitTime = 0f;

	    public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; }}

	    void Start()
	    {
			RegisterForMouseClick ();
			SetCurrentMaxHealth ();
			PutWeaponInHand ();
			SetupRuntimeAnimator ();
			abilities[0].AttackComponentTo (gameObject);
	    }

		private float SetCurrentMaxHealth () {
			return currentHealthPoints = maxHealthPoints;
		}

		private void SetupRuntimeAnimator() {
			animator = GetComponent<Animator> ();
			animator.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController ["DEFAULT"] = weaponInUse.GetAnimClip (); // TODO Remove const
		}

		private void PutWeaponInHand() {
			var weaponPrefab = weaponInUse.GetWeaponPrefab ();
			GameObject weaponSocket = RequestDominantHand ();
			var weapon = Instantiate (weaponPrefab, weaponSocket.transform);
			weapon.transform.localPosition = weaponInUse.gripTransform.localPosition;
			weapon.transform.localRotation = weaponInUse.gripTransform.localRotation;
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

		void OnMouseOverEnemy (Enemy enemy) {
			if (Input.GetMouseButton (0) && IsTargetInRange(enemy.gameObject)) {
				AttackTarget (enemy);
			} else if (Input.GetMouseButtonDown(1)) {
				AttemtSpecialAbility(0, enemy);
			}
		}

		private void AttemtSpecialAbility (int abilityIndex, Enemy enemy) {
			var energyComponent = GetComponent<Energy> ();
			var energyCost = abilities [abilityIndex].GetEnergyCost ();

			if (energyComponent.IsEnergyAvailable (energyCost)) {
				energyComponent.ConsumeEnergy (energyCost);
				var abilityParams = new AbilityUseParams (enemy, baseDamage);
				abilities [abilityIndex].Use (abilityParams);
			}
		}

		void AttackTarget (Enemy enemy) {
			if (Time.time - lastHitTime > weaponInUse.GetMinTimeBetweenHits()) {
				animator.SetTrigger ("Attack"); // TODO Make const
				enemy.TakeDamage (baseDamage);
				lastHitTime = Time.time;
			}
		} 

		private bool IsTargetInRange(GameObject target) {
			float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
			return distanceToTarget <= weaponInUse.GetMaxAttackRange();
		}

	    public void TakeDamage(float damage)
	    {
	        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
	    }
	}
}