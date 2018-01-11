using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	public class PowerAttackBehaviour : MonoBehaviour, ISpecialAbility {

		PowerAttackConfig config;

		public void SetConfig(PowerAttackConfig configToSet) {
			this.config = configToSet;
		}

		public void Use() {

		}
	}
}