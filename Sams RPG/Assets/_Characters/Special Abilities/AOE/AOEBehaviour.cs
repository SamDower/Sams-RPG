using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	public class AOEBehaviour : MonoBehaviour, ISpecialAbility {

		AOEConfig config;

		public void SetConfig(AOEConfig configToSet) {
			this.config = configToSet;
		}

		public void Use(AbilityUseParams useParams) {
			DealRadialDamage (useParams);
			PlayParticleEffect ();
		}

		private void DealRadialDamage(AbilityUseParams useParams) {
			// Static SphereCast For Targets
			RaycastHit[] hits = Physics.SphereCastAll (
				transform.position, 
				config.GetRadius (), 
				Vector3.up, config.GetRadius ()
			);

			foreach (RaycastHit hit in hits) {
				var damageable = hit.collider.gameObject.GetComponent<IDamageable> ();
				if (damageable != null) {
					float damageToDeal = useParams.baseDamage + config.GetDamageToEachTarget ();
					damageable.TakeDamage (damageToDeal);
				}
			}
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