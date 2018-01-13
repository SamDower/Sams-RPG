using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Weapons {
	[CreateAssetMenu(menuName = ("RPG/Weapon"))]
	public class Weapon : ScriptableObject {

		public Transform gripTransform;

		[SerializeField] GameObject weaponPrefab;
		[SerializeField] AnimationClip attackAnimation;
		[SerializeField] float minTimeBetweenHits = .5f;
		[SerializeField] float maxAttackRange = 2f;
		[SerializeField] float additionalDamage = 10f;

		public GameObject GetWeaponPrefab () {
			return weaponPrefab;
		}

		public AnimationClip GetAnimClip () {
			attackAnimation.events = new AnimationEvent[0];
			return attackAnimation;
		}

		public float GetMinTimeBetweenHits() {
			return minTimeBetweenHits;
		}

		public float GetMaxAttackRange() {
			return maxAttackRange;
		}

		public float GetAdditionalDamage() {
			return additionalDamage;
		}
	}
}