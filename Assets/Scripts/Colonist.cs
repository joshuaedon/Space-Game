using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colonist : Object {
	public ColonistPanel colonistPanel;

	public ParticleSystem[] thrusterParticles;
	public AudioSource thrusterAudio;
	// private bool[] thrusterActive;
	// private bool[] RCSActive;

	new void Start() {
        base.Start();

		foreach(ParticleSystem ps in thrusterParticles)
			ps.Stop();
	}

    new void FixedUpdate() {
    	// Movement
        if(this.thrusterActive[0] || this.RCSActive[0])
        	this.rb.AddForce(Settings.s.colonistMovement * transform.right);
        if(this.thrusterActive[1] || this.RCSActive[1])
        	this.rb.AddForce(-Settings.s.colonistMovement * transform.right);
		if(this.thrusterActive[2] || this.RCSActive[2])
        	this.rb.AddForce(Settings.s.colonistMovement * transform.up);
        if(this.thrusterActive[3] || this.RCSActive[3])
        	this.rb.AddForce(-Settings.s.colonistMovement * transform.up);
    	// Rotation
		if(this.thrusterActive[4] || this.RCSActive[4])
        	this.rb.AddTorque(Settings.s.colonistRotation);
        if(this.thrusterActive[5] || this.RCSActive[5])
        	this.rb.AddTorque(-Settings.s.colonistRotation);
    }

    public void control() {
        KeyCode[] inputs = new KeyCode[] {KeyCode.D, KeyCode.A, KeyCode.W, KeyCode.S, KeyCode.Q, KeyCode.E};

        for(int i = 0; i < inputs.Length; i++) {
            if(Input.GetKeyDown(inputs[i]))
                setThruster(i, true, false);
            if(Input.GetKeyUp(inputs[i]))
                setThruster(i, false, false);
        }
    }

    

    private void setThruster(int i, bool active, bool RCS) {
    	// Debug.Log(i + ", Active: " + active + ", RCS: " + RCS);
    	if(RCS) {
    		if(!this.RCS && active)
    			return;
			this.RCSActive[i] = active;
    	} else
			this.thrusterActive[i] = active;

		active = this.RCSActive[i] || this.thrusterActive[i]; // Only turn off if both not thrust or RCS

    	if(i < 4) {
    		if(active)
    			thrusterParticles[i].Play();
    		else
        		thrusterParticles[i].Stop();
    	} else {
    		if(active) {
    			thrusterParticles[(i - 4) * 2 + 4].Play();
    			thrusterParticles[(i - 4) * 2 + 5].Play();
    		} else {
        		thrusterParticles[(i - 4) * 2 + 4].Stop();
    			thrusterParticles[(i - 4) * 2 + 5].Stop();
    		}
    	}

    	if(active && !thrusterAudio.isPlaying)
    		thrusterAudio.Play();
    	else if(!active && thrusterAudio.isPlaying) {
    		for(int j = 0; j < thrusterActive.Length; j++) {
    			if(thrusterActive[j] || RCSActive[j])
    				return;
    		}
    		thrusterAudio.Stop();
    	}
    }

    public void turnOffThrust() {
    	for(int i = 0; i < this.thrusterActive.Length; i++) {
    		if(this.thrusterActive[i])
    			setThruster(i, false, false);
    	}
    }
}
