using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters {
	[RequireComponent(typeof(Image))]
	public class PlayerHealthBar : MonoBehaviour
	{
		Image healthOrb;
	    Player player;

	    void Start() {
	        player = FindObjectOfType<Player>();
			healthOrb = GetComponent<Image>();
	    }

	    void Update() {
			healthOrb.fillAmount = player.healthAsPercentage;
	    }
	}
}