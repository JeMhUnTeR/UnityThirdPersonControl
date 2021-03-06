/// <summary>
/// UnityTutorials - A Unity Game Design Prototyping Sandbox
/// <copyright>(c) John McElmurray and Julian Adams 2013</copyright>
/// 
/// UnityTutorials homepage: https://github.com/jm991/UnityTutorials
/// 
/// This software is provided 'as-is', without any express or implied
/// warranty.  In no event will the authors be held liable for any damages
/// arising from the use of this software.
///
/// Permission is granted to anyone to use this software for any purpose,
/// and to alter it and redistribute it freely, subject to the following restrictions:
///
/// 1. The origin of this software must not be misrepresented; you must not
/// claim that you wrote the original software. If you use this software
/// in a product, an acknowledgment in the product documentation would be
/// appreciated but is not required.
/// 2. Altered source versions must be plainly marked as such, and must not be
/// misrepresented as being the original software.
/// 3. This notice may not be removed or altered from any source distribution.
/// </summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class CharacterControllerLogic : MonoBehaviour 
{
	[SerializeField] private Animator animator;
	[SerializeField] private float directionDampTime = 0.25f;
	[SerializeField] private float directionSpeed = 3.0f;
	[SerializeField] private ThirdPersonCamera gamecam;

	// camera orbit (when holding left/right)
	[SerializeField] private float rotationDegreesPerSecond = 120f;
	
	private float speed = 0.0f;
	private float direction = 0.0f;
	private float horizontal = 0.0f;
	private float vertical = 0.0f;
	private AnimatorStateInfo stateInfo;
	
	// Hash ID (I don't even know what the fuck is this)
	private int m_LocomotionId = 0;

	void Start() {
		animator = GetComponent<Animator>();
		if (animator.layerCount >= 2) {
			animator.SetLayerWeight(1, 1);
		}

		// Hash all animation names. wtf is this
		m_LocomotionId = Animator.StringToHash("Base Layer.Locomotion");
	}

	void Update() {
		if (animator) {
			stateInfo = animator.GetCurrentAnimatorStateInfo(0);

			horizontal = Input.GetAxis("Horizontal");
			vertical = Input.GetAxis("Vertical");

			speed = new Vector2(horizontal, vertical).sqrMagnitude;
			
			StickToWorldSpace(this.transform, gamecam.transform, ref direction, ref speed);

			animator.SetFloat("Speed", speed);
			animator.SetFloat("Direction", direction, directionDampTime, Time.deltaTime);

		}
	}
	
	void FixedUpdate () {
		if (IsInLocomotion() && ((direction >= 0 && horizontal >= 0) || (direction < 0 && horizontal < 0))) {
			Vector3 rotationAmount = Vector3.Lerp(Vector3.zero,
				new Vector3(0f, rotationDegreesPerSecond + (horizontal < 0f ? -1f : 1f), 0f), Mathf.Abs(horizontal));
			Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
			this.transform.rotation *= deltaRotation;
		}
	}
	
	public void StickToWorldSpace (Transform root, Transform camera, ref float directionOut, ref float speedOut) {
		Vector3 rootDirection = root.forward;
		Vector3 stickDirection = new Vector3(horizontal, 0, vertical);
		speedOut = stickDirection.sqrMagnitude;
		
		// get camera rotation
		Vector3 cameraDirection = camera.forward;
		cameraDirection.y = 0.0f;
		Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, cameraDirection);
		
		// convert joystick input in WorldSpace coordinates
		Vector3 moveDirection = referentialShift * stickDirection;
		Vector3 axisSign = Vector3.Cross(moveDirection, rootDirection);
		
		float angleRootToMove = Vector3.Angle(rootDirection, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);
		
		angleRootToMove /= 180f;
		
		directionOut = angleRootToMove * directionSpeed;
	}
	
	public bool IsInLocomotion () {
		return stateInfo.nameHash == m_LocomotionId;
	}
}
