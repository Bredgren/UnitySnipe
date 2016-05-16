using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class SniperWeapon : MonoBehaviour {
	public Transform barrel;
	public float reloadTime = 2.0f;
	public float effectsDisplayTime = 0.2f;
	public float zoomFOV = 10.0f;
	public float zoomMouseSensitivity = 0.5f;

	public LineRenderer bulletLine;
	public ParticleSystem bullitHitParticles;
	public AudioClip shootSound;

	public bool drawCrosshair = true;
	public Color crosshairColor = Color.white;

	private float lastFireTime;

	private AudioSource audioSource;

	private Camera playerCamera;
	private float baseFOV;
	private MouseLook mouseLook;
	private Vector2 baseMouseSensitivity;

	private bool active;

	// ch = crosshair
	private Texture2D chTex;
	private float chNewHeight;
	private GUIStyle chLineStyle;
	private float chSize = 10.0f;
	private float chMaxBloom = 20.0f;
	private float chWidth = 1.0f;

	void Awake() {
		chTex = new Texture2D(1, 1);
		chLineStyle = new GUIStyle();
		chLineStyle.normal.background = chTex;
		SetCrosshairColor();
	}

	void Start() {
		lastFireTime = Time.time - reloadTime;
		audioSource = GetComponent<AudioSource>();
		playerCamera = GetComponentInParent<Camera>();
		baseFOV = playerCamera.fieldOfView; 
		mouseLook = GetComponentInParent<MouseLook>();
		baseMouseSensitivity.x = mouseLook.XSensitivity;
		baseMouseSensitivity.y = mouseLook.YSensitivity;
	}

	void Update() {
		if (!active) {
			return;
		}
		if (Time.time > lastFireTime + reloadTime && Input.GetButtonDown("Fire1")) {
			audioSource.PlayOneShot(shootSound);
			//	gunLight.enabled = true;
			bulletLine.enabled = true;
			bulletLine.SetPosition(0, barrel.position);

			Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				bulletLine.SetPosition(1, hit.point);
				bullitHitParticles.transform.position = hit.point;
				bullitHitParticles.transform.rotation = Quaternion.LookRotation(hit.normal);
				bullitHitParticles.Play();
			} else {
				bulletLine.SetPosition(1, barrel.position + barrel.forward * 10000);
			}
			lastFireTime = Time.time;
		}
		if (Input.GetButtonDown("Fire2")) {
			playerCamera.fieldOfView = zoomFOV;
			mouseLook.XSensitivity = zoomMouseSensitivity;
			mouseLook.YSensitivity = zoomMouseSensitivity;
		}
		if (Input.GetButtonUp("Fire2")) {
			playerCamera.fieldOfView = baseFOV;
			mouseLook.XSensitivity = baseMouseSensitivity.x;
			mouseLook.YSensitivity = baseMouseSensitivity.y;
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
		if (!active) {
			return;
		}
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

	public void SwitchFrom() {
		playerCamera.fieldOfView = baseFOV;
		mouseLook.XSensitivity = baseMouseSensitivity.x;
		mouseLook.YSensitivity = baseMouseSensitivity.y;
		foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
			r.enabled = false;
		}
		active = false;
	}

	public void SwitchTo() {
		foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
			r.enabled = true;
		}
		active = true;
	}
}
