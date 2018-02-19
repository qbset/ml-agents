﻿using UnityEngine;
using System.Collections;


public class RotationLimitElbow : RotationLimit {


	#region Main Interface
	
	/// <summary>
	/// Should the rotation be limited around the axis?
	/// </summary>
	public bool useLimits = true;
	/// <summary>
	/// The min limit around the axis.
	/// </summary>
	public float min = -45;
	/// <summary>
	/// The max limit around the axis.
	/// </summary>
	public float max = 90;
	
	#endregion Main Interface
	
	/*
		* Limits the rotation in the local space of this instance's Transform.
		* */
	protected override Quaternion LimitRotation(Quaternion rotation) {
		lastRotation = LimitHinge(rotation);
		return lastRotation;
	}

	[HideInInspector] public float zeroAxisDisplayOffset; // Angular offset of the scene view display of the Hinge rotation limit
	
	private Quaternion lastRotation = Quaternion.identity;
	private float lastAngle;
	
	/*
		* Apply the hinge rotation limit
		* */
	private Quaternion LimitHinge(Quaternion rotation) {
		// If limit is zero return rotation fixed to axis
		if (min == 0 && max == 0 && useLimits) return Quaternion.AngleAxis(0, axis);
		
		// Get 1 degree of freedom rotation along axis
		Quaternion free1DOF = Limit1DOF(rotation, axis);
		if (!useLimits) return free1DOF;

		// Get offset from last rotation in angle-axis representation
		Quaternion addR = free1DOF * Quaternion.Inverse(lastRotation);

		float addA = Quaternion.Angle(Quaternion.identity, addR);

		Vector3 secondaryAxis = new Vector3(axis.z, axis.x, axis.y);
		Vector3 cross = Vector3.Cross(secondaryAxis, axis);
		if (Vector3.Dot(addR * secondaryAxis, cross) > 0f) addA = - addA;
		
		// Clamp to limits
		lastAngle = Mathf.Clamp(lastAngle + addA, min, max);
		
		return Quaternion.AngleAxis(lastAngle, axis);
	}
}
