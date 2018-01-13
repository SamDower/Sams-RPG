using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	public abstract class AbilityBehaviour : MonoBehaviour {

		protected AbilityConfig config;

		const float PARTICLE_CLEAN_UP_DELAY = 10f;

		public abstract void Use (AbilityUseParams useParams);

		public void SetConfig(AbilityConfig configToSet) {
			config = configToSet;
		}

		protected void PlayParticleEffect () {
			var particlePrefab = config.GetParticlePrefab ();
			var particleObject = Instantiate (
				particlePrefab, 
				transform.position, 
				particlePrefab.transform.rotation, 
				transform
			);
			particleObject.GetComponent<ParticleSystem> ().Play ();
			StartCoroutine (DestroyParticleWhenFinished (particleObject));
		}

		IEnumerator DestroyParticleWhenFinished (GameObject particalPrefab) {
			while (particalPrefab.GetComponent<ParticleSystem> ().isPlaying) {
				yield return new WaitForSeconds (PARTICLE_CLEAN_UP_DELAY);
			}
			Destroy (particalPrefab);
			yield return new WaitForEndOfFrame ();
		}
	}
}