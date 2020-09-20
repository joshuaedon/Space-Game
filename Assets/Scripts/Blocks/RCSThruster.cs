using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCSThruster : Thruster {
	public ParticleSystem leftThrusterParticles;
	public ParticleSystem rightThrusterParticles;
	private bool leftThrusterState;
	private bool rightThrusterState;

    new void Start() {
    	base.Start();
		leftThrusterParticles.Stop();
		rightThrusterParticles.Stop();
	}
}
