using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters {
	public class SelfHealBehaviour : AbilityBehaviour {

		Player player = null;
		AudioSource audioSource = null;

		void Start() {
			player = GetComponent<Player> ();
			audioSource = GetComponent<AudioSource> ();
		}

		public override void Use(AbilityUseParams useParams) {
			audioSource.clip = config.GetAudioClip ();
			audioSource.Play ();
			player.Heal ((config as SelfHealConfig).GetHealthGain ());
			PlayParticleEffect ();
		}
	}
}