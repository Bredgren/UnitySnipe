using UnityEngine;
using System.Collections;

public class BowWeapon : MonoBehaviour {
	public GameObject arrowPrefab;
	public GameObject drawingArrow;
	public float reloadTime = 0.5f;
	public float pullTime = 0.5f;
	public float minPullForce = 500.0f;
	public float maxPullForce = 4000.0f;

	public bool drawCrosshair = true;
	public Color crosshairColor = Color.white;

	private bool pull;
	private bool pullStarted;
	private float pullStartTime;
	private Vector3 drawingArrowInitPos;
	private Vector3 maxPullOffset;
	private float pullPercent;

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
		drawingArrowInitPos = drawingArrow.transform.localPosition;
		float halfLength = drawingArrow.GetComponentInChildren<Renderer>().bounds.size.z / 2;
		maxPullOffset = new Vector3(0, 0, -halfLength + 0.1f);
		lastFireTime = Time.time - reloadTime;
	}

	void Update() {
		if (Input.GetButtonDown("Fire1")) {
			pull = true;
		}
		if (Input.GetButtonDown("Fire2")) {
			pull = false;
			pullStarted = false;
			drawingArrow.transform.localPosition = drawingArrowInitPos;
			pullPercent = 0.0f;
		}
		if (Time.time > lastFireTime + reloadTime) {
			drawingArrow.SetActive(true);

			if (pull && !pullStarted) {
				pullStartTime = Time.time;
				pullStarted = true;
			}

			if (pullStarted) {
				pullPercent = Mathf.Min(Time.time - pullStartTime, pullTime) / pullTime;
			}

			if (pull && Input.GetButtonUp("Fire1")) {
				pull = false;
				pullStarted = false;
				drawingArrow.transform.localPosition = drawingArrowInitPos;

				GameObject go = (GameObject)Instantiate(arrowPrefab, drawingArrow.transform.position, drawingArrow.transform.rotation);
				go.GetComponent<Arrow>().initalForce = Mathf.Lerp(minPullForce, maxPullForce, pullPercent);
				foreach (Collider collider in GetComponentsInParent<Collider>()) {
					Physics.IgnoreCollision(collider, go.GetComponent<Collider>());
				}

				pullPercent = 0.0f;

				lastFireTime = Time.time;
			}

			if (pull) {
				drawingArrow.transform.localPosition = drawingArrowInitPos + maxPullOffset * pullPercent;
			}
		} else {
			if (Input.GetButtonUp("Fire1")) {
				pull = false;
			}
			drawingArrow.SetActive(false);
		}
	}

	void OnGUI() {
		Vector2 centerPoint = new Vector2(Screen.width / 2, Screen.height / 2);
	
		if (drawCrosshair) {
			float maxWidth = 21;
			float width = Mathf.Lerp(maxWidth, 3, pullPercent);
			float height = Mathf.Lerp(3, 2, pullPercent);

			GUI.Box(new Rect(centerPoint.x - (width / 2), centerPoint.y - (height / 2), width, height), GUIContent.none, chLineStyle);

			//float yOffset = Mathf.Lerp(20, 5, pullPercent);
			float yOffset = 20;
			width = maxWidth * 0.75f;
			height *= 0.75f;
			GUI.Box(new Rect(centerPoint.x - (width / 2), centerPoint.y - (height / 2) + yOffset, width, height), GUIContent.none, chLineStyle);

			//yOffset = Mathf.Lerp(40, 10, pullPercent);
			yOffset = 40;
			width *= 0.75f;
			height *= 0.75f;
			GUI.Box(new Rect(centerPoint.x - (width / 2), centerPoint.y - (height / 2) + yOffset, width, height), GUIContent.none, chLineStyle);
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
