using UnityEngine;
using System.Collections;

public class WeaponSwitcher : MonoBehaviour {

	public GameObject bow;
	public GameObject sniper;

	private int currentWeapon = -1;

	// Use this for initialization
	void Start() {
	}
	
	// Update is called once per frame
	void Update() {
		if (Input.GetAxis("Mouse ScrollWheel") != 0) {
			currentWeapon = (currentWeapon + 1) % 2;
			switch (currentWeapon) {
			case 0:
				bow.SetActive(false);
				sniper.GetComponent<SniperWeapon>().SwitchTo();
				break;
			case 1:
				sniper.GetComponent<SniperWeapon>().SwitchFrom();
				bow.SetActive(true);
				break;
			}
		}
		if (currentWeapon == -1) {
			sniper.GetComponent<SniperWeapon>().SwitchFrom();
			bow.SetActive(true);
		}
	}
}
