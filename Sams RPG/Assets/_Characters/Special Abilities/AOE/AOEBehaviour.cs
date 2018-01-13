using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	public class AOEBehaviour : AbilityBehaviour {

		AOEConfig config;

		public void SetConfig(AOEConfig configToSet) {
			this.config = configToSet;
		}

		public override void Use(AbilityUseParams useParams) {
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
				bool hitPlayer = hit.collider.gameObject.GetComponent<Player> ();
				if (damageable != null && !hitPlayer) {
					float damageToDeal = useParams.baseDamage + config.GetDamageToEachTarget ();
					damageable.TakeDamage (damageToDeal);
				}
			}
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