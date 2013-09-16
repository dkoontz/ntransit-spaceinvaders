using UnityEngine;
using System.Collections;

public class SpaceInvader : MonoBehaviour {
	public enum State {
		MovingHorizontal,
		MovingDown,
	}

	public State CurrentState { get; set; }
}