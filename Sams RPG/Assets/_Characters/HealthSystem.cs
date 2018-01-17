using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RPG.Characters {
	public class HealthSystem : MonoBehaviour {

		[SerializeField] float maxHealthPoints = 100f;
		[SerializeField] Image healthBar;
		[SerializeField] AudioClip[] damageSounds;
		[SerializeField] AudioClip[] deathSounds;
		[SerializeField] float deathVanishSeconds = 2f;

		const string DEATH_TRIGGER = "Death";

		float currentHealthPoints;
		Animator animator;
		AudioSource audioSource;
		Character characterMovement;

		public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; }}

		void Start () {
			animator = GetComponent<Animator> ();
			audioSource = GetComponent<AudioSource> ();
			characterMovement = GetComponent<Character> ();

			currentHealthPoints = maxHealthPoints;
		}

		void Update () {
			UpdateHealthBar ();
		}

		void UpdateHealthBar() {
			if (healthBar) {
				healthBar.fillAmount = healthAsPercentage;
			}
		}

		public void TakeDamage(float damage) {
			bool characterDies = (currentHealthPoints - damage <= 0);
			currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
			var clip = damageSounds[Random.Range(0, damageSounds.Length)];
			audioSource.PlayOneShot (clip);

			if (characterDies) {
				StartCoroutine (KillCharacter());
			}
		}

		public void Heal(float amount) {
			currentHealthPoints = Mathf.Clamp(currentHealthPoints + amount, 0f, maxHealthPoints);
		}

		IEnumerator KillCharacter() {
			StopAllCoroutines ();
			characterMovement.Kill ();
			animator.SetTrigger (DEATH_TRIGGER);

			var playerComponent = GetComponent<PlayerControl> ();
			if (playerComponent && playerComponent.isActiveAndEnabled) {
				audioSource.clip = deathSounds [Random.Range (0, deathSounds.Length)];
				audioSource.Play ();
				yield return new WaitForSecondsRealtime (audioSource.clip.length);
				SceneManager.LoadScene (0);
			} else {
				Destroy (gameObject, deathVanishSeconds);
			}
		}
	}
}