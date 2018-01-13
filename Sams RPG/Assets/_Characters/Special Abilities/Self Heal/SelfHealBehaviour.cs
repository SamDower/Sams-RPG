using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	public class SelfHealBehaviour : MonoBehaviour, ISpecialAbility {

		SelfHealConfig config;

		public void SetConfig(SelfHealConfig configToSet) {
			this.config = configToSet;
		}

		public void Use(AbilityUseParams useParams) {
			Heal ();
			PlayParticleEffect ();
		}

		private void Heal() {
			Player player = GetComponent<Player> ();
			player.RestoreHealth (config.GetHealthGain ());
		}

		private void PlayParticleEffect () {
			ParticleSystem myParticalSystem;
			var prefab = Instantiate (config.GetParticlePrefab (), transform.position, Quaternion.identity, transform);
			myParticalSystem = prefab.GetComponent<ParticleSystem> ();
			myParticalSystem.Play ();
			Destroy (prefab, 10f); // TODO destoy after duration
		}
	}
}