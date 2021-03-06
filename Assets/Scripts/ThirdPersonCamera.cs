﻿using UnityEngine;
using System.Collections;

// struct for camera alignment
struct CameraPosition {
	// position to align camera to, probably somewhere behind the character
	// or position to point camera at, probably somewhere along character's axis
	private Vector3 position;
	// transform used for any rotation
	private Transform xForm;
	
	public Vector3 Position {
		get { return position; }
		set { position = value; }
	}
	
	public Transform XForm {
		get { return xForm; }
		set { xForm = value; }
	}

	public void Init(string camName, Vector3 pos, Transform transform, Transform parent) {
		position = pos;
		xForm = transform;
		xForm.name = camName;
		xForm.parent = parent;
		xForm.localPosition = Vector3.zero;
		xForm.localPosition = position;
	}
}


[RequireComponent (typeof(BarsEffect))]

public class ThirdPersonCamera : MonoBehaviour {
	
	[SerializeField] private float distanceAway = 30f;
	[SerializeField] private float distanceUp = 30f;
	[SerializeField] private float smooth = 1f;
	[SerializeField] private Transform followXForm;
	[SerializeField] private CharacterControllerLogic follow;

	// camera targeting
	[SerializeField] private float widescreen = 0.2f;
	[SerializeField] private float targetingTime = 0.5f;
	[SerializeField] private float firstPersonThreshold = 0.5f;

	private Vector3 lookDir;
	private Vector3 targetPosition;
	private BarsEffect barEffect;
	private CamStates camState = CamStates.Behind;
	private CameraPosition firstPersonCamPos;
	private float xAxisRot = 0.0f;
	private float lookWeight;
	private float targetingThreshold = 0.01f;


	// camera states
	public enum CamStates {
		Behind,
		FirstPerson,
		Target,
		Free
	}

	// smoothing and damping
	private Vector3 velocityCamSmooth = Vector3.zero;
	[SerializeField] private float camSmoothDampTime = 0.1f;

	// Use this for initialization
	void Start () {

		follow = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterControllerLogic>();
		followXForm = GameObject.FindGameObjectWithTag("Player").transform;
		lookDir = followXForm.forward;

		barEffect = GetComponent<BarsEffect>();

		// position and parent a GameObject where first person view should be
		firstPersonCamPos = new CameraPosition();
		firstPersonCamPos.Init("First Person Camera", new Vector3(0f, 0.1f, 0.2f), new GameObject().transform, followXForm);
	}

	void LateUpdate () {
		float lookX = Input.GetAxis("LookX");
		float lookY = Input.GetAxis("LookY");
		float moveX = Input.GetAxis("Horizontal");
		float moveY = Input.GetAxis("Vertical");

		Vector3 characterOffset = followXForm.position + new Vector3(0, distanceUp, 0);

		if (Input.GetAxis("Target") > targetingThreshold) {
			barEffect.coverage = Mathf.SmoothStep (barEffect.coverage, widescreen, targetingTime);
			camState = CamStates.Target;
		} else {
			barEffect.coverage = Mathf.SmoothStep (barEffect.coverage, 0f, targetingTime);

			// first person
			if (lookY > firstPersonThreshold && !follow.IsInLocomotion()) {
				// reset look before entering firs person mode
				xAxisRot = 0;
				lookWeight = 0;
				camState = CamStates.FirstPerson;
			}

			if ((camState == CamStates.FirstPerson && Input.GetButton("ExitFPV")) ||
			    (camState == CamStates.Target && (Input.GetAxis("Target") <= targetingThreshold)))
			{
				camState = CamStates.Behind;
			}
		}

		follow.Animator.SetLookWeight(lookWeight);

		// calculate look direction based on camera state
		switch (camState) {
		case CamStates.Behind:
			// calculate direction from camera to player, kill Y, normalize
			lookDir = characterOffset - transform.position;
			lookDir.y = 0;
			lookDir.Normalize();
			break;
		case CamStates.Target:
			lookDir = followXForm.forward;
			break;
		case CamStates.FirstPerson:
			Debug.Log("fps");
			break;
		}
		targetPosition = characterOffset + followXForm.up * distanceUp - lookDir * distanceAway;
		CompensateForWalls(characterOffset, ref targetPosition);
		SmoothPosition(transform.position, targetPosition);
		transform.LookAt (characterOffset);
	}

	public void SmoothPosition (Vector3 source, Vector3 target) {
		transform.position = Vector3.SmoothDamp(source, target, ref velocityCamSmooth, camSmoothDampTime);
	}

	public void CompensateForWalls (Vector3 source, ref Vector3 target) {
		RaycastHit wallHit = new RaycastHit ();
		if (Physics.Linecast(source, target, out wallHit)) {
			target = new Vector3(wallHit.point.x, target.y, wallHit.point.z);
		}
	}
}
