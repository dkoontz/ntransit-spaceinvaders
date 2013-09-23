using UnityEngine;
using System.Collections;
using NTransit.Unity;

public class SpaceInvader : MonoBehaviour {
	public ParticleSystem DestructionEffect;

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