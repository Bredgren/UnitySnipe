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
	public Texture2D crosshairDot;
	public Texture2D crosshairCircle;

	void Awake() {
		chTex = new Texture2D(1, 1);
		chLineStyle = new GUIStyle();
		chLineStyle.normal.background = chTex;
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
			float dotSize = 5.0f;
			GUI.DrawTexture(new Rect(centerPoint.x - dotSize / 2, centerPoint.y - dotSize / 2, dotSize, dotSize), crosshairDot);
			float circleMaxSize = 41.0f;
			float circleMinSize = 9.0f;
			float circleSize = Mathf.Lerp(circleMaxSize, circleMinSize, pullPercent);
			GUI.DrawTexture(new Rect(centerPoint.x - circleSize / 2, centerPoint.y - circleSize / 2, circleSize, circleSize), crosshairCircle);
		}
	}
}
