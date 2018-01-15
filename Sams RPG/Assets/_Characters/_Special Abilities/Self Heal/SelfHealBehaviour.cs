using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	public class SelfHealBehaviour : AbilityBehaviour {

		PlayerMovement player = null;

		void Start() {
			player = GetComponent<PlayerMovement> ();
		}

		public override void Use(GameObject target) {
			var playerHealth = GetComponent<HealthSystem> ();
			playerHealth.Heal ((config as SelfHealConfig).GetHealthGain ());
			PlayAbilitySound ();
			PlayParticleEffect ();
		}
	}
}