using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using RPG.CameraUI; // TODO Consider re-wiring
using RPG.Core;
using RPG.Weapons;

namespace RPG.Characters {
	public class Player : MonoBehaviour, IDamageable {

	    [SerializeField] float maxHealthPoints = 100f;
	    [SerializeField] float baseDamage = 10f;
		[SerializeField] Weapon weaponInUse = null;
		[SerializeField] AnimatorOverrideController animatorOverrideController = null;

		[SerializeField] AudioClip[] damageSounds;
		[SerializeField] AudioClip[] deathSounds;

		[Range(0.1f,1.0f)] [SerializeField] float critChance = 0.1f;
		[SerializeField] float critMultiplier = 1.25f;

		// Temporarily serialized for dubbing
		[SerializeField] AbilityConfig[] abilities;

		const string DEATH_TRIGGER = "Death";
		const string ATTACK_TRIGGER = "Attack";

		Enemy enemy = null;
		AudioSource audioSource = null;
		Animator animator = null;
	    float currentHealthPoints = 0f;
		CameraRaycaster cameraRaycaster = null;
	    float lastHitTime = 0f;

	    public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; }}

		void Start() {
			audioSource = GetComponent<AudioSource> ();

			RegisterForMouseClick ();
			SetCurrentMaxHealth ();
			PutWeaponInHand ();
			SetupRuntimeAnimator ();
			AttachInitialAbilities ();
	    }

		private void AttachInitialAbilities() {
			for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++) {
				abilities [abilityIndex].AttackComponentTo (gameObject);
			}
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

		public void TakeDamage(float damage) {
			currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
			audioSource.clip = damageSounds[Random.Range(0, damageSounds.Length)];
			audioSource.Play ();

			if (currentHealthPoints <= 0) {
				StartCoroutine (KillPlayer());
			}
		}

		public void Heal(float amount) {
			currentHealthPoints = Mathf.Clamp(currentHealthPoints + amount, 0f, maxHealthPoints);
		}

		IEnumerator KillPlayer() {
			animator.SetTrigger (DEATH_TRIGGER);

			audioSource.clip = deathSounds[Random.Range(0, deathSounds.Length)];
			audioSource.Play ();

			yield return new WaitForSecondsRealtime(audioSource.clip.length);
			SceneManager.LoadScene(0);
		}

		private float SetCurrentMaxHealth () {
			return currentHealthPoints = maxHealthPoints;
			// TODO Play Sound
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
			if (Time.time - lastHitTime > weaponInUse.GetMinTimeBetweenHits()) {
				animator.SetTrigger (ATTACK_TRIGGER);
				enemy.TakeDamage (CalculateDamage());
				lastHitTime = Time.time;
			}
		}

		private float CalculateDamage() {
			bool isCriticalHit = Random.Range (0f, 1f) <= critChance;
			float damageBeforeCrit = baseDamage + weaponInUse.GetAdditionalDamage ();
			if (isCriticalHit) {
				return damageBeforeCrit * critMultiplier;
				// TODO Partical? Floating text coming off it?
			}
			return damageBeforeCrit;
		}

		private bool IsTargetInRange(GameObject target) {
			float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
			return distanceToTarget <= weaponInUse.GetMaxAttackRange();
		}
	}
}