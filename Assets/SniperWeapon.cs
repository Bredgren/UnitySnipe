using UnityEngine;
using System.Collections;

public class SniperWeapon : MonoBehaviour {
	public Transform barrel;
	public float reloadTime = 1.0f;

	public bool drawCrosshair = true;
	public Color crosshairColor = Color.white;

	private float lastFireTime;

	// ch = crosshair
	private Texture2D chTex;
	private float chNewHeight;
	private GUIStyle chLineStyle;

	void Awake() {
		chTex = new Texture2D(1, 1);
		chLineStyle = new GUIStyle();
		chLineStyle.normal.background = chTex;
		SetCrosshairColor();
	}

	void Start() {
		lastFireTime = Time.time - reloadTime;
	}

	void Update() {
		if (Time.time > lastFireTime + reloadTime && Input.GetButtonDown("Fire1")) {
			Debug.Log("fire");
			lastFireTime = Time.time;
		}
		if (Input.GetButtonDown("Fire2")) {
			Debug.Log("zoom in");
		}
		if (Input.GetButtonUp("Fire2")) {
			Debug.Log("zoom out");
		}
	}

	void SetCrosshairColor() {
		for (int y = 0; y < chTex.height; y++) {
			for (int x = 0; x < chTex.width; x++)
				chTex.SetPixel(x, y, crosshairColor);
			chTex.Apply();
		}
	}
}
