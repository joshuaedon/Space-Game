using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public GameObject followObject = null;
	private float orthSize = 10f;
	private float rotation = 0f;
	private Vector3 pos = new Vector3(0f, 0f, -10f);

    void Update() {
    	// Follow object
    	if(this.followObject != null) {
    		this.rotation = this.followObject.eulerAngles.z;
    		this.pos = this.followObject.transform.position;// + this.followObject.transform    direction and velocity
    	}

        // Zoom
        if(Input.GetAxis("Mouse ScrollWheel") != 0)
    		this.orthSize = Mathf.Clamp(this.orthSize - Input.GetAxis("Mouse ScrollWheel") * Settings.zoomSpeed * 100f * Time.deltaTime, Settings.minZoom, Settings.maxZoom);
    	// Android support
		if(Input.touchCount == 2) {
			Vector2 touchZeroPrevPos = Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition;
			Vector2 touchOnePrevPos  = Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition;

			float prevMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float curMag = (Input.GetTouch(0).position - Input.GetTouch(1).position).magnitude;

			GetComponent<Camera>().orthographicSize -= (curMag - prevMag) * Settings.zoomSpeed * Time.deltaTime;
		}

		// Rotation
		if(Controller.selectedBlock == null) {
			if(Input.GetKey(KeyCode.Q))
            	this.rotation = (this.rotation + Settings.rotationSpeed * Time.deltaTime + 180f) % 360f - 180f;
            if(Input.GetKey(KeyCode.E))
            	this.rotation = (this.rotation - Settings.rotationSpeed * Time.deltaTime + 180f) % 360f - 180f;
        }

        // Pan
        if(Input.GetKey(KeyCode.D))
        	this.pos += new Vector3(Mathf.Cos(this.rotation * Mathf.Deg2Rad), Mathf.Sin(this.rotation * Mathf.Deg2Rad), 0f) * Settings.panSpeed * Time.deltaTime;
        if(Input.GetKey(KeyCode.A))
        	this.pos -= new Vector3(Mathf.Cos(this.rotation * Mathf.Deg2Rad), Mathf.Sin(this.rotation * Mathf.Deg2Rad), 0f) * Settings.panSpeed * Time.deltaTime;
		if(Input.GetKey(KeyCode.W))
        	this.pos += new Vector3(-Mathf.Sin(this.rotation * Mathf.Deg2Rad), Mathf.Cos(this.rotation * Mathf.Deg2Rad), 0f) * Settings.panSpeed * Time.deltaTime;
        if(Input.GetKey(KeyCode.S))
        	this.pos -= new Vector3(-Mathf.Sin(this.rotation * Mathf.Deg2Rad), Mathf.Cos(this.rotation * Mathf.Deg2Rad), 0f) * Settings.panSpeed * Time.deltaTime;

        // Camera Follow
    	GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, this.orthSize, Settings.zoomFollow * Time.deltaTime);
    	transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, this.rotation), Settings.rotationFollow * Time.deltaTime);
    	transform.position = Vector3.Lerp(transform.position, this.pos, Settings.panFollow * Time.deltaTime);
    }
}
