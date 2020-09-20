using UnityEngine;

[CreateAssetMenu(fileName = "New Settings Profile", menuName = "Settings Profile")]
public class SettingsProfile : ScriptableObject {
	//// Camera
	// Zoom
	public float minZoom;
	public float maxZoom;
	public float zoomSpeed;
	public float zoomFollow;
	// Rotation
	public float rotationSpeed;
	public float rotationFollow;
	// Pan
	public float panSpeed;
	public float panFollow;
	// Other
	public float camLookAhead;
	//// Colonists
	// Movement
	public float colonistRotation;
	public float colonistMovement;
	public float minRCSVelocity;
	//// Other
	// Gravity
	public float g;
	public float minGravDist;
}
