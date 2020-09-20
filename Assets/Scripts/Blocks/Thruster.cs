using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : Block {
	public ParticleSystem thrusterParticles;
	private bool thrusterState;

	protected void Start() {
		thrusterParticles.Stop();
	}

	public Vector3 returnForce(bool[] active) {
		return Vector3.zero;
	}
}
