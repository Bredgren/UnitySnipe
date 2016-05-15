using UnityEngine;
using System.Collections;

public class SniperWeapon : MonoBehaviour {
	public Transform barrel;
	public float reloadTime = 1.0f;
	public float effectsDisplayTime = 0.2f;

	public LineRenderer bulletLine;

	public bool drawCrosshair = true;
	public Color crosshairColor = Color.white;

	private float lastFireTime;

	// ch = crosshair
	private Texture2D chTex;
	private float chNewHeight;
	private GUIStyle chLineStyle;
	private float chSize = 10.0f;
	private float chMaxBloom = 20.0f;
	private float chWidth = 2.0f;

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
			//	gunLight.enabled = true;
			bulletLine.enabled = true;
			bulletLine.SetPosition(0, barrel.position);

			Ray ray = GetComponentInParent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				bulletLine.SetPosition(1, hit.point);
			} else {
				bulletLine.SetPosition(1, barrel.position + barrel.forward * 10000);
			}
			lastFireTime = Time.time;
		}
		if (Input.GetButtonDown("Fire2")) {
			Debug.Log("zoom in");
		}
		if (Input.GetButtonUp("Fire2")) {
			Debug.Log("zoom out");
		}

		if (Time.time > lastFireTime + effectsDisplayTime) {
			bulletLine.enabled = false;
			//	gunLight.enabled = false;
		}
	}
		
	//void Shoot() {
	//	timer = 0f;
	//
	//	gunAudio.Play();
	//
	//
	//	gunParticles.Stop();
	//	gunParticles.Play();
	//
	//	gunLine.enabled = true;
	//	gunLine.SetPosition(0, transform.position);
	//
	//	shootRay.origin = transform.position;
	//	shootRay.direction = transform.forward;
	//
	//	if (Physics.Raycast(shootRay, out shootHit, range, shootableMask)) {
	//		EnemyHealth enemyHealth = shootHit.collider.GetComponent <EnemyHealth>();
	//		if (enemyHealth != null) {
	//			enemyHealth.TakeDamage(damagePerShot, shootHit.point);
	//		}
	//		gunLine.SetPosition(1, shootHit.point);
	//	} else {
	//		gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
	//	}
	//}

	void OnGUI() {
		Vector2 centerPoint = new Vector2(Screen.width / 2, Screen.height / 2);

		if (drawCrosshair) {
			float percent = (lastFireTime + reloadTime - Time.time) / reloadTime;
			float offset = Mathf.Lerp(0, chMaxBloom, percent);
			float size = Mathf.Lerp(chSize, chSize * 2, percent);

			GUI.Box(new Rect(centerPoint.x - (chWidth / 2), centerPoint.y - size - offset, chWidth, size), GUIContent.none, chLineStyle);
			GUI.Box(new Rect(centerPoint.x - (chWidth / 2), centerPoint.y + offset, chWidth, size), GUIContent.none, chLineStyle);

			GUI.Box(new Rect(centerPoint.x - size - offset, centerPoint.y - (chWidth / 2), size, chWidth), GUIContent.none, chLineStyle);
			GUI.Box(new Rect(centerPoint.x + offset, centerPoint.y - (chWidth / 2), size, chWidth), GUIContent.none, chLineStyle);
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
