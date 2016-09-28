/********************************************************************************/
\file      ReadMe.txt
\brief     ReadMe.txt
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

Simple Usage:
1. Create empty Unity project.
2. Import OVR Unity utils.
3. Import this package.
4. Set: Edit->Project Settings->Time->Fixed Timestep = 0.01111111
5. Open and run OvrTouch/Content/OvrTouchDemo scene.

Optional: you can add the following code to OVRCameraRig.cs, to reduce hand latency.

private void FixedUpdate()
{
	m_didFixedUpdate = true;
	EnsureGameObjectIntegrity();
		
	if (!Application.isPlaying)
		return;

	UpdateAnchors();
}


//==============================================================================
// Controls:
//==============================================================================

* Analog Stick Button: Change visualization (Hand, Controller, Hand and Controller)
* Grip Trigger: Fist/Grab/Release objects
* Cap Touch Index: Point
* Cap Touch Thumb: Thumbs Up

//==============================================================================
// General Notes:
//==============================================================================

Hand and TouchController demonstrate two alternate methods of tracking.
* Hand updates in FixedUpdate, and supports physics interaction. (Hand rigid body
vs. environmental objects.) It used MovePosition to update its position each frame.
* TouchController is parented to the hand anchors in the Avatar hierarchy. It's a
much simpler integration, but does not support physics. Use for visual-only hands.
(It may also allow later-in-frame updates in future revisions, allowing for
decreased latency in hand position and orientation.)

Known issue: Hands can sometimes appear juddery. This occurs when FixedUpdate is
not called during a frame. To be fixed pending upcoming integration improvements
in OVRPlugin. See comments on Hand::FixedUpdate.


