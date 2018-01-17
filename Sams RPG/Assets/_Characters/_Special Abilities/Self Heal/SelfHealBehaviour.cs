using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	public class SelfHealBehaviour : AbilityBehaviour {

		PlayerControl player = null;

		void Start() {
			player = GetComponent<PlayerControl> ();
		}

		public override void Use(GameObject target) {
			var playerHealth = GetComponent<HealthSystem> ();
			playerHealth.Heal ((config as SelfHealConfig).GetHealthGain ());
			PlayAbilitySound ();
			PlayParticleEffect ();
			PlayAbilityAnimation ();
		}
	}
}