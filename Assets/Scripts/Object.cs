using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour {
	public static List<Object> attractors = new List<Object>();

	public Rigidbody2D rb;

    void Update() {
        Vector3 force = Vector3.zero;
    	foreach(Object attractor in attractors) {
    		if(attractor != this) {
	    		Vector3 dir = (attractor.rb.position + attractor.rb.centerOfMass) - (this.rb.position + this.rb.centerOfMass);
	    		if(dir.magnitude > Settings.minGravDist) {
		    		float mag = Settings.g * Time.deltaTime * this.rb.mass * attractor.rb.mass / (dir.magnitude * dir.magnitude);
		    		dir.Normalize();
		    		force += dir * mag;
		    	}
	    	}
    	}
		rb.AddForce(force);
    }
}
