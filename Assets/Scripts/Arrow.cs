﻿using UnityEngine;
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
	private Transform frozenTransform;
	private float minVel = 0.1f;

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
//		if (stuck) {
//			transform.position = frozenTransform.position;
//			transform.rotation = frozenTransform.rotation;
//		}
	}

	void OnTriggerEnter(Collider col) {
		if (stuck) {
			return; // or we might stick to something else
		}
	
		Arrow a = col.GetComponentInParent<Arrow>();
		if (a != null && !a.IsStuck()) {
			stationary = true;
			GetComponent<Collider>().isTrigger = false;
			audioSource.PlayOneShot(arrowHitSound);
			trailParticles.Stop();
			hitParticles.Play();
			return;
		}
	
		audioSource.PlayOneShot(hitSound);
	
		stuck = true;
		rb.isKinematic = true;
	
		// Creating an intermediate object prevents weird scalling when reparenting
		attachPoint = new GameObject();
		attachPoint.transform.parent = col.transform;
		transform.parent = attachPoint.transform;
	
		trailParticles.Stop();
		hitParticles.Play();
	}

	void OnCollisionEnter(Collision c) {
		if (stuck) {
			return; // or we might stick to something else
		}
	
		Arrow a = c.collider.GetComponentInParent<Arrow>();
		if (a != null && !a.IsStuck()) {
			stationary = true;
			GetComponent<Collider>().isTrigger = false;
			audioSource.PlayOneShot(arrowHitSound);
			trailParticles.Stop();
			hitParticles.transform.position = c.contacts[0].point;
			hitParticles.Play();
			return;
		}
	
		audioSource.PlayOneShot(hitSound);
	
		stuck = true;
		rb.isKinematic = true;
		//rb.detectCollisions = false;

		// Creating an intermediate object prevents weird scaling when reparenting
		attachPoint = new GameObject();
		attachPoint.transform.parent = c.transform;
		transform.parent = attachPoint.transform;
//		frozenTransform = transform;
//		rb.constraints = RigidbodyConstraints.FreezeAll;
	
		trailParticles.Stop();
	
		hitParticles.transform.position = c.contacts[0].point;
		hitParticles.Play();
	}

	void Kill() {
		Destroy(gameObject);
		Destroy(attachPoint);
	}

	bool IsStuck() {
		return stuck;
	}
}
