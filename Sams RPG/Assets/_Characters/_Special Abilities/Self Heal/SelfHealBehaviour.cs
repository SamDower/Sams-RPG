using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	public class SelfHealBehaviour : AbilityBehaviour {

		Player player = null;

		void Start() {
			player = GetComponent<Player> ();
		}

		public override void Use(AbilityUseParams useParams) {
			var playerHealth = GetComponent<HealthSystem> ();
			playerHealth.Heal ((config as SelfHealConfig).GetHealthGain ());
			PlayAbilitySound ();
			PlayParticleEffect ();
		}
	}
}