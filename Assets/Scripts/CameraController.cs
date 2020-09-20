using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	private Object followObject = null;
    // private static bool caughtObject = false;

	public float orthSize = 10f;
	public float rotation = 0f;
	public Vector3 pos = new Vector3(0f, 0f, -10f);

    public void load(CameraData cameraData) {
        // Set position and rotation of transform but not size so that the camera zooms in/out when the save is loaded
        this.pos = new Vector3(cameraData.pos[0], cameraData.pos[1], -10);
        transform.position = this.pos;
        this.rotation = cameraData.rotation;
        transform.eulerAngles = new Vector3(0, 0, this.rotation);
        this.orthSize = cameraData.orthSize;
    }

    void Update() {
    	// Follow object
    	if(this.followObject != null) {
    		this.rotation = this.followObject.transform.eulerAngles.z;
            // Look in the direction of the selected object's velocity if the object is not rotating fast
    		this.pos = this.followObject.transform.position + Mathf.Clamp(Settings.s.camLookAhead - Mathf.Abs(this.followObject.rb.angularVelocity / Settings.s.rotationSpeed), 0f, this.followObject.rb.velocity.magnitude) * ((Vector3)this.followObject.rb.velocity.normalized);
            this.pos.z = -10;
    	}

        // Zoom
        if(Input.GetAxis("Mouse ScrollWheel") != 0)
    		this.orthSize = Mathf.Clamp(this.orthSize - Input.GetAxis("Mouse ScrollWheel") * Settings.s.zoomSpeed * Time.deltaTime, Settings.s.minZoom, Settings.s.maxZoom);
    	// Android support
		if(Input.touchCount == 2) {
			Vector2 touchZeroPrevPos = Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition;
			Vector2 touchOnePrevPos  = Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition;

			float prevMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float curMag = (Input.GetTouch(0).position - Input.GetTouch(1).position).magnitude;

			GetComponent<Camera>().orthographicSize -= (curMag - prevMag) * Settings.s.zoomSpeed * Time.deltaTime;
		}

		// Rotation
		if(ConstructionManager.selectedBlock == null) {
			if(Input.GetKey(KeyCode.Q))
            	this.rotation = (this.rotation + Settings.s.rotationSpeed * Time.deltaTime + 180f) % 360f - 180f;
            if(Input.GetKey(KeyCode.E))
            	this.rotation = (this.rotation - Settings.s.rotationSpeed * Time.deltaTime + 180f) % 360f - 180f;
        }

        // Pan
        if(Input.GetKey(KeyCode.D))
        	this.pos += new Vector3(Mathf.Cos(this.rotation * Mathf.Deg2Rad), Mathf.Sin(this.rotation * Mathf.Deg2Rad), 0f) * Settings.s.panSpeed * Time.deltaTime;
        if(Input.GetKey(KeyCode.A))
        	this.pos -= new Vector3(Mathf.Cos(this.rotation * Mathf.Deg2Rad), Mathf.Sin(this.rotation * Mathf.Deg2Rad), 0f) * Settings.s.panSpeed * Time.deltaTime;
		if(Input.GetKey(KeyCode.W))
        	this.pos += new Vector3(-Mathf.Sin(this.rotation * Mathf.Deg2Rad), Mathf.Cos(this.rotation * Mathf.Deg2Rad), 0f) * Settings.s.panSpeed * Time.deltaTime;
        if(Input.GetKey(KeyCode.S))
        	this.pos -= new Vector3(-Mathf.Sin(this.rotation * Mathf.Deg2Rad), Mathf.Cos(this.rotation * Mathf.Deg2Rad), 0f) * Settings.s.panSpeed * Time.deltaTime;
    }

    void FixedUpdate() {
        if(this.followObject != null) {
            transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z + this.followObject.rb.angularVelocity / 50f);
            transform.position += (Vector3)this.followObject.rb.velocity / 50f;
        }

        // Camera Follow
        GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, this.orthSize, Settings.s.zoomFollow);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, this.rotation), Settings.s.rotationFollow);

        transform.position = Vector3.Lerp(transform.position, this.pos, Settings.s.panFollow);
    }

    public void follow(Object obj) {
        this.followObject = obj;
        this.orthSize = 10f;
    }

    public void unfollow() {
        this.followObject = null;
    }
}

[System.Serializable]
public class CameraData {
    public float[] pos;
    public float rotation;
    public float orthSize;

    public CameraData(CameraController camera) {
        this.pos = new float[2];
        this.pos[0] = camera.pos.y;
        this.pos[1] = camera.pos.y;

        this.rotation = camera.rotation;

        this.orthSize = camera.orthSize;
    }
}