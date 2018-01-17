using System.Collections;
using UnityEngine;
using RPG.CameraUI; 

namespace RPG.Characters {
	public class PlayerControl : MonoBehaviour {

		Character character;
		SpecialAbilities abilities;
		WeaponSystem weaponSystem;

		void Start() {
			character = GetComponent<Character> ();
			abilities = GetComponent<SpecialAbilities> ();
			weaponSystem = GetComponent<WeaponSystem> ();

			RegisterForMouseEvents ();
	    }

		void RegisterForMouseEvents () {
			CameraRaycaster cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
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

		void OnMouseOverEnemy (EnemyAI enemy) {
			if (Input.GetMouseButton (0) && IsTargetInRange (enemy.gameObject)) {
				weaponSystem.AttackTarget (enemy.gameObject);
			} else if (Input.GetMouseButton (0) && !IsTargetInRange (enemy.gameObject)) { 
				StartCoroutine (MoveAndAttack (enemy));
			} else if (Input.GetMouseButtonDown (1) && IsTargetInRange (enemy.gameObject)) {
				abilities.AttemtSpecialAbility(0, enemy.gameObject);
			} else if (Input.GetMouseButtonDown (1) && !IsTargetInRange (enemy.gameObject)) { 
				StartCoroutine (MoveAndPowerAttack (enemy));
			} 
		}

		IEnumerator MoveToTarget(GameObject target) {
			character.SetDestination (target.transform.position);
			while (!IsTargetInRange (target)) {
				yield return new WaitForEndOfFrame ();
			}
			yield return new WaitForEndOfFrame ();
		}

		IEnumerator MoveAndAttack(EnemyAI enemy) {
			yield return StartCoroutine (MoveToTarget (enemy.gameObject));
			weaponSystem.AttackTarget (enemy.gameObject);
		}

		IEnumerator MoveAndPowerAttack(EnemyAI enemy) {
			yield return StartCoroutine (MoveToTarget (enemy.gameObject));
			abilities.AttemtSpecialAbility (0, enemy.gameObject);
		}

		bool IsTargetInRange(GameObject target) {
			float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
			return distanceToTarget <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
		}
	}
}