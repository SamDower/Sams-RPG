using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {

	public  abstract class AbilityConfig : ScriptableObject {

		[Header("Special Ability General")]
		[SerializeField] float energyCost = 10f;
		[SerializeField] GameObject particlePrefab;
		[SerializeField] AudioClip[] audioClips;

		protected AbilityBehaviour behaviour;

		abstract public void AttackComponentTo (GameObject gameObjectToAttachTo);

		public void Use (GameObject target) {
			behaviour.Use (target);
		}

		public float GetEnergyCost() {
			return energyCost;
		}
	
		public GameObject GetParticlePrefab() {
			return particlePrefab;
		}

		public AudioClip GetRandomAbilitySound() {
			return audioClips[Random.Range(0, audioClips.Length-1)];
		}
	}
}