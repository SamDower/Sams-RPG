using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	[CreateAssetMenu(menuName = ("RPG/Special Ability/Self Heal"))]
	public class SelfHealConfig : AbilityConfig {

		[Header("AOE Specific")]
		[SerializeField] float healthGain = 75f;

		public override void AttackComponentTo (GameObject gameObjectToAttachTo) {
			var behaviourComponent = gameObjectToAttachTo.AddComponent<SelfHealBehaviour> ();
			behaviourComponent.SetConfig (this);
			behaviour =  behaviourComponent;
		}

		public float GetHealthGain() {
			return healthGain;
		}
	}
}