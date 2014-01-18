using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {
	
	[SerializeField] private float distanceAway = 30f;
	[SerializeField] private float distanceUp = 30f;
	[SerializeField] private float smooth = 1f;
	[SerializeField] private Transform followXForm;
	[SerializeField] private Vector3 offset = new Vector3(0, 15f, 0);

	private Vector3 lookDir;
	private Vector3 targetPosition;

	// smoothing and damping
	private Vector3 velocityCamSmooth = Vector3.zero;
	[SerializeField] private float camSmoothDampTime = 0.1f;

	// Use this for initialization
	void Start () {
		followXForm = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void LateUpdate () {
		Vector3 characterOffset = followXForm.position + offset;

		// calculate direction from camera to player, kill Y, normalize
		lookDir = characterOffset + this.transform.position;
		lookDir.y = 0;
		lookDir.Normalize();
		Debug.DrawRay(this.transform.position, lookDir, Color.green);

		targetPosition = characterOffset + (followXForm.up * distanceUp) - (lookDir * distanceAway);
		SmoothPosition(this.transform.position, targetPosition);

		transform.LookAt (followXForm);
	}

	public void SmoothPosition (Vector3 source, Vector3 target) {
		this.transform.position = Vector3.SmoothDamp(source, target, ref velocityCamSmooth, camSmoothDampTime);
	}
}
