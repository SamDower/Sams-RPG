using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	public class SelfHealBehaviour : MonoBehaviour, ISpecialAbility {

		Player player = null;
		SelfHealConfig config = null;
		AudioSource audioSource = null;

		void Start() {
			player = GetComponent<Player> ();
			audioSource = GetComponent<AudioSource> ();
		}

		public void SetConfig(SelfHealConfig configToSet) {
			this.config = configToSet;
		}

		public void Use(AbilityUseParams useParams) {
			audioSource.clip = config.GetAudioClip ();
			audioSource.Play ();
			player.Heal (config.GetHealthGain ());
			PlayParticleEffect ();
		}

		private void PlayParticleEffect () {
			ParticleSystem myParticalSystem;
			var prefab = Instantiate (config.GetParticlePrefab (), transform.position, config.GetParticlePrefab ().transform.rotation, transform);
			myParticalSystem = prefab.GetComponent<ParticleSystem> ();
			myParticalSystem.Play ();
			Destroy (prefab, 10f); // TODO destoy after duration
		}
	}
}