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
	    [SerializeField] float damagePerHit = 10f;
		[SerializeField] Weapon weaponInUse = null;
		[SerializeField] AnimatorOverrideController animatorOverrideController = null;

		// Temporarily serialized for dubbing
		[SerializeField] SpecialAbilityConfig ability1;

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
			ability1.AddComponent (gameObject); // Custom AddComponent
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
				AttemtSpecialAbility1(enemy);
			}
		}

		private void AttemtSpecialAbility1 (Enemy enemy) {
			var energyComponent = GetComponent<Energy> ();
			if (energyComponent.IsEnergyAvailable (10f)) { // TODO Read from scriptable object
				energyComponent.ConsumeEnergy (10f);
				// TODO Use the ability
			}
		}

		void AttackTarget (Enemy enemy) {
			if (Time.time - lastHitTime > weaponInUse.GetMinTimeBetweenHits()) {
				animator.SetTrigger ("Attack"); // TODO Make const
				enemy.TakeDamage (damagePerHit);
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