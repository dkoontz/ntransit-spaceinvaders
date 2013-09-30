using UnityEngine;
using System.Collections;
using NTransit.Unity;

public class SpaceInvader : MonoBehaviour {
	public float TimeSinceLastProjectileFired { get; set; }

	public float TimeBetweenShots;
	public GameObject Projectile;
	public ParticleSystem DestructionEffect;
	public Transform FireLocation;

	public GameObject FireProjectile() {
		return Instantiate(Projectile, FireLocation.position, FireLocation.rotation) as GameObject;
	}

	public void TurnAround() {
		var movement = GetComponent<TranslationMovement>();
		movement.Direction = new Vector3(-1 * movement.Direction.x, movement.Direction.y, movement.Direction.z);
	}

	public void StartMovement() {
		GetComponent<TranslationMovement>().CanMove = true;
	}

	public void Destroy() {
		DestructionEffect.transform.parent = null;
		DestructionEffect.Emit(60);
		Destroy(DestructionEffect.gameObject, DestructionEffect.duration);
		Destroy(gameObject);
	}
}