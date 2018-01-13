using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	public class PowerAttackBehaviour : AbilityBehaviour {

		PowerAttackConfig config;

		public void SetConfig(PowerAttackConfig configToSet) {
			this.config = configToSet;
		}

		public override void Use(AbilityUseParams useParams) {
			float damageToDeal = useParams.baseDamage + config.GetExtraDamage ();
			useParams.target.TakeDamage (damageToDeal);
		}
	}
}