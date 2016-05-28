/********************************************************************************//**
\file      ReadMe.txt
\brief     ReadMe.txt
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

//==============================================================================
// Controls:
//==============================================================================

* Analog Stick Button: Change visualization (Hand, Controller, Hand and Controller)
* Grip Trigger: Fist/Grab/Release objects
* Cap Touch Index: Point
* Cap Touch Thumb: Thumbs Up

//==============================================================================
// Components
//==============================================================================

//------------------------------------------------------------------------------
// Hand
//------------------------------------------------------------------------------

This is a basic hand implementation that handles grabbing, throwing, poking and punching objects using tracked controllers.

* Collision is only active on the hand when making a fist or holding an object.
* The hands are updated in FixedUpdate since they are animating collision and toggling physics state.

//------------------------------------------------------------------------------
// Hand Pose
//------------------------------------------------------------------------------

The hand pose specifies how the hand will animate when grabbing an object and if that object will have its position or rotation snapped to the hand.

* Hand Poses:
  - HandPoseDefaultPf: No hand pose, no snap -- allows pointing and thumbs up
  - HandPoseGenericPf: Generic hand pose, no snap -- allows pointing
  - HandPosePingPongBallPf: Pinch pose for ping pong balls, snap -- disallows pointing and thumbs up

//-----------------------------------------------------------------------------
// Grabbable
/------------------------------------------------------------------------------

The grabbable component is what allows the hand to grab an object in the world.

Sends the following messages to the owning game object:
  - OnOverlapBegin\End: Occurs when an object overlaps the hand and can be grabbed
  - OnGrabBegin\End: Occurs when an object is grabbed by the hand

Grabbables have an array of grab points which define how and what colliders can be grabbed. It is an error to have a grabbable with zero grab points.

//-----------------------------------------------------------------------------
// GrabPoint
/------------------------------------------------------------------------------

Contains a hand pose, collider and an override transform.

* Grab points must have a collider in order to have overlap events.
* Allows for a grab override transform which redirects the hand to grab that transform instead. This is useful for multi-collider rigid bodies where the rigid body transform should be grabbed.
* It is an error to have a grab point without a collider.

//-----------------------------------------------------------------------------
// GrabTrigger
/------------------------------------------------------------------------------

The grab trigger is automatically added to the game object of a grab point collider. When overlapped, this provides a link between the grab collider and the grabbable component that owns the collider.

//==============================================================================
// General Notes:
//==============================================================================

* Demo Scene: Content\OvrTouchDemo.unity
* Hand Prefabs: Content\Hands\LeftHandPf and RightHandPf
* Controller Prefabs: Content\Controllers\LeftControllerPf and RightControllerPf
* Toy Prefabs: Content\Toys\ToyBallPf, ToyCubePf and ToyT-BlockPf (multiple grab point example)

* Add controller prefabs and/or hand prefabs to a scene containing an OVRCameraRig and they will automaticlly work.
* For better physics interactions between the hands, grabbed objects and other physics objects, try a FixedJoint instead of a kinematic rigid body.

* For more stable and accurate physics behaviour, increase the fixed timestep to at least the v-sync rate of the device.
    - Edit -> Project Settings -> Time: Fixed Timestep
