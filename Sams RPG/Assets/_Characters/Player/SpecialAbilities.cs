using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters {
	public class SpecialAbilities : MonoBehaviour {

		[SerializeField] AbilityConfig[] abilities;
		[SerializeField] Image energybar = null;
		[SerializeField] float maxEnergy = 100f;
		[SerializeField] float regenPerSecond = 1f;
		[SerializeField] AudioClip outOfEnergySound;

		float currentEnergy;
		AudioSource audioSource;

		float energyAsPercent { get { return currentEnergy / maxEnergy; }	}

		void Start () {
			audioSource = GetComponent<AudioSource> ();

			currentEnergy = maxEnergy;
			UpdateEnergyBar ();

			AttachInitialAbilities ();
		}

		void Update() {
			if (currentEnergy < maxEnergy) {
				AddEnergyPoints (regenPerSecond * Time.deltaTime);
				UpdateEnergyBar ();
			}
		}

		private void AttachInitialAbilities() {
			for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++) {
				abilities [abilityIndex].AttackComponentTo (gameObject);
			}
		}

		public void AttemtSpecialAbility (int abilityIndex, GameObject target = null) {
			var energyCost = abilities [abilityIndex].GetEnergyCost ();

			if (energyCost <= currentEnergy) {
				ConsumeEnergy (energyCost);
				abilities [abilityIndex].Use (target);
			} else {
				if (!audioSource.isPlaying) { // TODO remove if?
					audioSource.PlayOneShot (outOfEnergySound);
				}
			}
		}

		public int GetNumberOfAbilities() {
			return abilities.Length;
		}

		private void AddEnergyPoints (float amount) {
			currentEnergy = Mathf.Clamp (currentEnergy + amount, 0, maxEnergy);
		}

		public void ConsumeEnergy (float amount) {
			float newEnergy = currentEnergy - amount;
			currentEnergy = Mathf.Clamp (newEnergy, 0, maxEnergy);
			UpdateEnergyBar ();
		}

		void UpdateEnergyBar () {
			if (energybar) {
				energybar.fillAmount = energyAsPercent;
			}
		}
	}
}