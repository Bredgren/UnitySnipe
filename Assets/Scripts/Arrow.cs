using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {
	public float initalForce = 300.0f;
	public float life = 30.0f;
	public bool stationary = false;

	public AudioClip hitSound;
	public AudioClip arrowHitSound;

	public ParticleSystem trailParticles;
	public ParticleSystem hitParticles;

	private Rigidbody rb;
	private AudioSource audioSource;

	private bool stuck = false;
	private GameObject attachPoint;
	private float minVel = 0.1f;

	private Quaternion lastRotation;

	void Start() {
		rb = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();

		if (!stationary) {
			rb.AddForce(transform.up * initalForce);
			Invoke("Kill", life);
		}
	}

	void Update() {
		if (!stuck && !stationary) {
			if (rb.velocity.magnitude >= minVel) {
				transform.rotation = Quaternion.FromToRotation(Vector3.up, rb.velocity);	
			}
		}
		lastRotation = transform.rotation;
	}

	void OnCollisionEnter(Collision c) {
		if (stuck) {
			return; // prevents sticking to something else
		}

		GetComponent<Collider>().enabled = false;
	
		Arrow a = c.collider.GetComponentInParent<Arrow>();
		if (a != null && !a.IsStuck()) {
			MakeInert();
			hitParticles.transform.position = c.contacts[0].point;
			hitParticles.transform.rotation = Quaternion.LookRotation(c.contacts[0].normal);
			hitParticles.Play();
			return;
		}
	
		audioSource.PlayOneShot(hitSound);
	
		stuck = true;
		rb.isKinematic = true;

		// Using the last rotation helps minimize some weird rotation upon collision.
		// My guess is the physics of the collision starts taking effect slightly before OnCollisionEnter is called.
		transform.rotation = lastRotation;

		// Creating an intermediate object prevents weird scaling when reparenting
		attachPoint = new GameObject();
		attachPoint.transform.parent = c.transform;
		transform.parent = attachPoint.transform;
	
		trailParticles.Stop();
	
		hitParticles.transform.position = c.contacts[0].point;
		hitParticles.Play();
	}

	void Kill() {
		Destroy(gameObject);
		Destroy(attachPoint);
	}

	public void MakeInert() {
		stationary = true;
		audioSource.PlayOneShot(arrowHitSound);
		trailParticles.Stop();
	}

	public bool IsStuck() {
		return stuck;
	}
}
