using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters {
	public class Energy : MonoBehaviour {

		[SerializeField] RawImage energyBar = null;
		[SerializeField] float maxEnergy = 100f;

		float currentEnergy;

		void Start () {
			currentEnergy = maxEnergy;
		}

		public bool IsEnergyAvailable (float amount) {
			return amount <= currentEnergy;
		}

		public void ConsumeEnergy (float amount) {
			float newEnergy = currentEnergy - amount;
			currentEnergy = Mathf.Clamp (newEnergy, 0, maxEnergy);
			UpdateEnergyBar ();
		}

		void UpdateEnergyBar () {
			// TODO Remove magical numbers
			float xValue = -(EnergyAsPercent() / 2f) - 0.5f;
			energyBar.uvRect = new Rect (xValue, 0f, 0.5f, 1f);
		}

		float EnergyAsPercent() {
			return currentEnergy / maxEnergy;
		}
	}
}