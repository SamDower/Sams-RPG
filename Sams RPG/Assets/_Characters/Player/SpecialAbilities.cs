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
		// TODO out of energy sound

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

		public void AttemtSpecialAbility (int abilityIndex) {
			var energyCost = abilities [abilityIndex].GetEnergyCost ();

			if (energyCost <= currentEnergy) {
				ConsumeEnergy (energyCost);
				// TODO Use ability
			} else {
				// TODO Play out of energy sound
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
			energybar.fillAmount = energyAsPercent;
		}
	}
}