using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour {
	public static List<Object> attractors = new List<Object>();

	public Rigidbody2D rb;

	public bool RCS = true;

	protected bool[] thrusterActive;
	protected bool[] RCSActive;

	private List<Thruster> Thrusters;
	private List<Thruster> RCSThrusters;
	// private List<Thruster> activeThrusters;
	// private List<Thruster> inactiveThrusters;

	protected void Start() {
		this.thrusterActive = new bool[6];
		this.RCSActive = new bool[6];
	}

    void Update() {
    	Vector2 relativeVelocity = transform.InverseTransformDirection(this.rb.velocity);
    	bool refresh = false;
		//// RCS
		if(this.RCS) {
			// Movement
			if(!this.RCSActive[0] && !this.thrusterActive[1] && relativeVelocity.x < -Settings.s.minRCSVelocity) {
				this.RCSActive[0] = true; refresh = true;
			} else if(this.RCSActive[0] && (this.thrusterActive[1] || relativeVelocity.x > -Settings.s.minRCSVelocity)) {
				this.RCSActive[0] = false; refresh = true;
			}
			if(!this.RCSActive[1] && !this.thrusterActive[0] && relativeVelocity.x > Settings.s.minRCSVelocity) {
				this.RCSActive[1] = true; refresh = true;
			} else if(this.RCSActive[1] && (this.thrusterActive[0] || relativeVelocity.x < Settings.s.minRCSVelocity)) {
				this.RCSActive[1] = false; refresh = true;
			}
			if(!this.RCSActive[2] && !this.thrusterActive[3] && relativeVelocity.y < -Settings.s.minRCSVelocity) {
				this.RCSActive[2] = true; refresh = true;
			} else if(this.RCSActive[2] && (this.thrusterActive[3] || relativeVelocity.y > -Settings.s.minRCSVelocity)) {
				this.RCSActive[2] = false; refresh = true;
			}
			if(!this.RCSActive[3] && !this.thrusterActive[2] && relativeVelocity.y > Settings.s.minRCSVelocity) {
				this.RCSActive[3] = true; refresh = true;
			} else if(this.RCSActive[3] && (this.thrusterActive[2] || relativeVelocity.y < Settings.s.minRCSVelocity)) {
				this.RCSActive[3] = false; refresh = true;
			}
			// Rotation
			if(!this.RCSActive[4] && !this.thrusterActive[5] && this.rb.angularVelocity < -Settings.s.minRCSVelocity * 100f) {
				this.RCSActive[4] = true; refresh = true;
			} else if(this.RCSActive[4] && (this.thrusterActive[5] || this.rb.angularVelocity > -Settings.s.minRCSVelocity * 100f)) {
				this.RCSActive[4] = false; refresh = true;
			}
			if(!this.RCSActive[5] && !this.thrusterActive[4] && this.rb.angularVelocity > Settings.s.minRCSVelocity * 100f) {
				this.RCSActive[5] = true; refresh = true;
			} else if(this.RCSActive[5] && (this.thrusterActive[4] || this.rb.angularVelocity < Settings.s.minRCSVelocity * 100f)) {
				this.RCSActive[5] = false; refresh = true;
			}

			if(refresh);
		}

    	this.rb.position = new Vector2((this.rb.position.x + 600) % 400 - 200, (this.rb.position.y + 600) % 400 - 200);
    }

    protected void FixedUpdate() {
    	Vector3 force = Vector3.zero;
    	foreach(Object attractor in attractors) {
    		if(attractor != this) {
	    		Vector3 dir = (attractor.rb.position + attractor.rb.centerOfMass) - (this.rb.position + this.rb.centerOfMass);
	    		if(dir.magnitude > Settings.s.minGravDist) {
		    		float mag = Settings.s.g * Time.deltaTime * this.rb.mass * attractor.rb.mass / (dir.magnitude * dir.magnitude);
		    		dir.Normalize();
		    		force += dir * mag;
		    	}
	    	}
    	}
		rb.AddForce(force);
    }

    public void setRCS(bool b) {
    	this.RCS = b;

    	if(!this.RCS) {
    		this.RCSActive = new bool[6];

    		// Refresh
    	}
    }
}
