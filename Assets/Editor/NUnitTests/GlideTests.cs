using UnityEngine;
using UnityEditor;
//using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class GlideTests
{
    /*
	private Vector3 forwardVectorRollNone = new Vector3 (0.0f, 0.0f, 0.0f);
	private Vector3 forwardVectorRollRight = new Vector3 (0.0f, -0.5f, 0.0f);
	private Vector3 forwardVectorRollLeft = new Vector3 (0.0f, 0.5f, 0.0f);
	private Vector3 rightVectorPitchNone = new Vector3 (0.0f, 0.0f, 0.0f);
	private Vector3 rightVectorPitchDown = new Vector3 (0.0f, -0.5f, 0.0f);
	private Vector3 rightVectorPitchUp = new Vector3 (0.0f, 0.5f, 0.0f);

	[Test]
	public void GlideRollTest()
	{
		float rollNone = Glide.rollFromForwardVector (forwardVectorRollNone);
		Assert.AreEqual (0.0f, rollNone);

		float rollRight = Glide.rollFromForwardVector (forwardVectorRollRight);
		Assert.Less (rollRight, 0.0f);

		float rollLeft = Glide.rollFromForwardVector (forwardVectorRollLeft);
		Assert.Greater (rollLeft, 0.0f);
	}

	[Test]
	public void GlidePitchTest()
	{
		float pitchNone = Glide.pitchFromRightVector (rightVectorPitchNone);
		Assert.AreEqual (0.0f, pitchNone);

		float pitchDown = Glide.pitchFromRightVector (rightVectorPitchDown);
		Assert.Less (pitchDown, 0.0f);

		float pitchUp = Glide.rollFromForwardVector (rightVectorPitchUp);
		Assert.Greater (pitchUp, 0.0f);
	}

	[Test]
	public void GlideRotateTest()
	{
		float rollNone = Glide.rollFromForwardVector (forwardVectorRollNone);
		float rollRight = Glide.rollFromForwardVector (forwardVectorRollRight);
		float rollLeft = Glide.rollFromForwardVector (forwardVectorRollLeft);
		float pitchNone = Glide.pitchFromRightVector (rightVectorPitchNone);
		float pitchDown = Glide.pitchFromRightVector (rightVectorPitchDown);
		float pitchUp = Glide.rollFromForwardVector (rightVectorPitchUp);

		float rotateNoneNoPitch = Glide.rotationFromPitchAndRoll (pitchNone, rollLeft);
		Assert.AreEqual (0.0f, rotateNoneNoPitch);
		float rotateNoneNoRoll = Glide.rotationFromPitchAndRoll (pitchUp, rollNone);
		Assert.AreEqual (0.0f, rotateNoneNoRoll);

		float rotateLeftForward = Glide.rotationFromPitchAndRoll (pitchDown, rollLeft);
		Assert.Less (rotateLeftForward, 0.0f);
		float rotateRightForward = Glide.rotationFromPitchAndRoll (pitchDown, rollRight);
		Assert.Greater (rotateRightForward, 0.0f);

		float rotateLeftBackward = Glide.rotationFromPitchAndRoll (pitchUp, rollRight);
		Assert.Less (rotateLeftBackward, 0.0f);
		float rotateRightBackward = Glide.rotationFromPitchAndRoll (pitchUp, rollLeft);
		Assert.Greater (rotateRightBackward, 0.0f);
	}
    */
}
