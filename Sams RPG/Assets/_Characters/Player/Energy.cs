using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters {
	public class Energy : MonoBehaviour {

		[SerializeField] Image energyOrb = null;
		[SerializeField] float maxEnergy = 100f;
		[SerializeField] float regenPerSecond = 1f;

		float currentEnergy;

		void Start () {
			currentEnergy = maxEnergy;
		}

		void Update() {
			if (currentEnergy < maxEnergy) {
				AddEnergyPoints (regenPerSecond * Time.deltaTime);
				UpdateEnergyBar ();
			}
		}

		public bool IsEnergyAvailable (float amount) {
			return amount <= currentEnergy;
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
			energyOrb.fillAmount = EnergyAsPercent ();
		}

		float EnergyAsPercent() {
			return currentEnergy / maxEnergy;
		}
	}
}