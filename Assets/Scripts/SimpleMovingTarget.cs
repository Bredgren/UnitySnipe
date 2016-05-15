using UnityEngine;
using System.Collections;

public class SimpleMovingTarget : MonoBehaviour {
	public float speed = 10.0f;
	public Vector3 direction = Vector3.right;
	public float distance = 10.0f;

	private Vector3 currentDirection;
	private Vector3 startPos;
	private Vector3 endPos;

	// Use this for initialization
	void Start() {
		startPos = transform.position;
		endPos = transform.position + direction.normalized * distance;
		currentDirection = direction;
	}
	
	// Update is called once per frame
	void Update() {
		transform.position += currentDirection * speed * Time.deltaTime;
		Vector3 a = transform.position - startPos;
		Vector3 b = transform.position - endPos;
		if (Vector3.Dot(a, b) > 0) {
			if (a.magnitude < b.magnitude) {
				currentDirection = direction.normalized;
			} else {
				currentDirection = -direction.normalized;
			}
		}
//		if ((transform.position - startPos).magnitude > distance / 2) {
//			currentDirection = -currentDirection;
//		}
	}
}
