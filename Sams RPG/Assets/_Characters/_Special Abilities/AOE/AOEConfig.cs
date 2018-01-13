using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters {
	[CreateAssetMenu(menuName = ("RPG/Special Ability/AOE"))]
	public class AOEConfig : AbilityConfig {

		[Header("AOE Specific")]
		[SerializeField] float radius = 5f;
		[SerializeField] float damageToEachTarget = 40f;

		public override void AttackComponentTo (GameObject gameObjectToAttachTo) {
			var behaviourComponent = gameObjectToAttachTo.AddComponent<AOEBehaviour> ();
			behaviourComponent.SetConfig (this);
			behaviour =  behaviourComponent;
		}

		public float GetDamageToEachTarget() {
			return damageToEachTarget;
		}

		public float GetRadius() {
			return radius;
		}
	}
}