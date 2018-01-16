using UnityEngine;
using RPG.CameraUI; 

namespace RPG.Characters {
	public class PlayerMovement : MonoBehaviour {

		[Range(0.1f,1.0f)] [SerializeField] float critChance = 0.1f;
		[SerializeField] float critMultiplier = 1.25f;

		Character character;
		EnemyAI enemy;
		SpecialAbilities abilities;
		CameraRaycaster cameraRaycaster;
		WeaponSystem weaponSystem;

		void Start() {
			character = GetComponent<Character> ();
			abilities = GetComponent<SpecialAbilities> ();
			weaponSystem = GetComponent<WeaponSystem> ();

			RegisterForMouseEvents ();
	    }

		void RegisterForMouseEvents () {
			cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
			cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
			cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
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

		void OnMouseOverEnemy (EnemyAI enemyToSet) {
			this.enemy = enemyToSet;
			if (Input.GetMouseButton (0) && IsTargetInRange(enemy.gameObject)) {
				weaponSystem.AttackTarget (enemy.gameObject);
			} else if (Input.GetMouseButtonDown(1)) {
				abilities.AttemtSpecialAbility(0);
			}
		}

		bool IsTargetInRange(GameObject target) {
			float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
			return distanceToTarget <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
		}
	}
}