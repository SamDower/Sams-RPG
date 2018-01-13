using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	public class AOEBehaviour : AbilityBehaviour {
		
		public override void Use(AbilityUseParams useParams) {
			DealRadialDamage (useParams);
			PlayParticleEffect ();
		}

		private void DealRadialDamage(AbilityUseParams useParams) {
			// Static SphereCast For Targets
			RaycastHit[] hits = Physics.SphereCastAll (
				transform.position, 
				(config as AOEConfig).GetRadius (), 
				Vector3.up, 
				(config as AOEConfig).GetRadius ()
			);

			foreach (RaycastHit hit in hits) {
				var damageable = hit.collider.gameObject.GetComponent<IDamageable> ();
				bool hitPlayer = hit.collider.gameObject.GetComponent<Player> ();
				if (damageable != null && !hitPlayer) {
					float damageToDeal = useParams.baseDamage + (config as AOEConfig).GetDamageToEachTarget ();
					damageable.TakeDamage (damageToDeal);
				}
			}
		}
	}
}