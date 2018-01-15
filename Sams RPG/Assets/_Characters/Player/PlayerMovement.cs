using UnityEngine;
using UnityEngine.Assertions;
using RPG.CameraUI; 


// TODO Extract weapon system
namespace RPG.Characters {
	public class PlayerMovement : MonoBehaviour {

	    [SerializeField] float baseDamage = 10f;
		[SerializeField] Weapon currentWeaponConfig = null;
		[SerializeField] AnimatorOverrideController animatorOverrideController = null;

		[Range(0.1f,1.0f)] [SerializeField] float critChance = 0.1f;
		[SerializeField] float critMultiplier = 1.25f;

		const string ATTACK_TRIGGER = "Attack";
		const string DEFAULT_ATTACK = "DEFAULT";

		Enemy enemy = null;
		Animator animator = null;
		SpecialAbilities abilities;
		CameraRaycaster cameraRaycaster = null;
	    float lastHitTime = 0f;
		GameObject weaponObject;
		Character character;

		void Start() {
			character = GetComponent<Character> ();
			abilities = GetComponent<SpecialAbilities> ();

			RegisterForMouseEvents ();
			PutWeaponInHand (currentWeaponConfig); // TODO Move to WeaponSystem
			SetAttackAnimation (); // TODO Move to WeaponSystem
	    }

		void RegisterForMouseEvents () {
			cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
			cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
			cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
		}

		// TODO Extract
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
			ScanForAbilityKeyDown ();
		}

		private void ScanForAbilityKeyDown() {
			for (int keyIndex = 1; keyIndex < abilities.GetNumberOfAbilities(); keyIndex++) {
				if (Input.GetKeyDown (keyIndex.ToString ())) {
					abilities.AttemtSpecialAbility (keyIndex);
				}
			}
		}

		void OnMouseOverPotentiallyWalkable(Vector3 destination) {
			if (Input.GetMouseButton (0)) {
				character.SetDestination (destination);
			}
		}

		void OnMouseOverEnemy (Enemy enemyToSet) {
			this.enemy = enemyToSet;
			if (Input.GetMouseButton (0) && IsTargetInRange(enemy.gameObject)) {
				AttackTarget ();
			} else if (Input.GetMouseButtonDown(1)) {
				abilities.AttemtSpecialAbility(0);
			}
		}

		// TODO Extract
		private void SetAttackAnimation() {
			animator = GetComponent<Animator> ();
			animator.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController [DEFAULT_ATTACK] = currentWeaponConfig.GetAnimClip ();
		}

		// TODO Extract
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

		// TODO Extract
		float CalculateDamage() {
			bool isCriticalHit = Random.Range (0f, 1f) <= critChance;
			float damageBeforeCrit = baseDamage + currentWeaponConfig.GetAdditionalDamage ();
			if (isCriticalHit) {
				return damageBeforeCrit * critMultiplier;
				// TODO Partical? Floating text coming off it?
			}
			return damageBeforeCrit;
		}

		// TODO Extract?
		bool IsTargetInRange(GameObject target) {
			float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
			return distanceToTarget <= currentWeaponConfig.GetMaxAttackRange();
		}
	}
}