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
	[SerializeField] private float DirectionDampTime = 0.25f;

	private float speed = 0.0f;
	private float h = 0.0f;
	private float v = 0.0f;

	void Start() {
		animator = GetComponent<Animator>();
		if (animator.layerCount >= 2) {
			animator.SetLayerWeight(1, 1);
		}
	}

	void Update() {
		h = Input.GetAxis("Horizontal");
		v = Input.GetAxis("Vertical");

		speed = new Vector2(h, v).sqrMagnitude;
		animator.SetFloat("Speed", speed);
		animator.SetFloat("Direction", h, DirectionDampTime, Time.deltaTime);
	}
}
