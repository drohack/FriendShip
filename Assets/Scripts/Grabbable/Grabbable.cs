using System;
using UnityEngine;
using System.Collections;

public struct GrabbableGrabMsg
{

    //==============================================================================
    // Fields
    //==============================================================================

    public const string MsgNameGrabBegin = "OnGrabBegin";
    public const string MsgNameGrabEnd = "OnGrabEnd";

    public Grabbable Sender;

}

public struct GrabbableOverlapMsg
{

    //==============================================================================
    // Fields
    //==============================================================================

    public const string MsgNameOverlapBegin = "OnOverlapBegin";
    public const string MsgNameOverlapEnd = "OnOverlapEnd";

    public Grabbable Sender;
    public object Hand;

}

public class Grabbable : MonoBehaviour
{

    //==============================================================================
    // Nested Types
    //==============================================================================

    public enum GrabMode
    {
        Grab,
        Drag,
        Rotate,
        None
    }

    public bool isGrabbed = false;

    //Drag Spring variables
    const float d_Spring = 200.0f;
    const float d_Damper = 1.0f;
    const float d_Drag = 10.0f;
    const float d_AngularDrag = 5.0f;
    const float d_BreakForce = 25f;
    //Spring variables
    const float k_Spring = 0.1f;
    const float k_Damper = 0f;
    const float k_Drag = 0f;
    const float k_AngularDrag = 0f;
    const float k_Distance = 0.001f;
    const float k_BreakForce = 0.01f;
    const float k_Tolerance = 0.01f;
    const bool k_AttachToCenterOfMass = false;
    private SpringJoint m_SpringJoint;
    private GameObject rigidbodyDragger;
    //Velocity variables
    private float maxVelocity = 10000000f;
    private float maxAngularVelocity = 28f; //default is 7

    //Grabbable variables
    [SerializeField]
    public GrabMode m_grabMode = GrabMode.Grab;
    [SerializeField]
    private bool m_allowOffhandGrab = true;
    [SerializeField]
    private Grab_Point[] m_grabPoints = null;
    private bool m_grabbedKinematic = false;
    private Grab_Point m_grabbedGrabPoint = null;
    private Grabbed_Hand_Script m_grabbedHand = null;

    //==============================================================================
    // Properties
    //==============================================================================

    public bool AllowOffhandGrab
    {
        get { return m_allowOffhandGrab; }
    }

    public Hand_Pose HandPose
    {
        get { return m_grabbedGrabPoint.HandPose; }
    }

    public Grabbed_Hand_Script GrabbedHand
    {
        get { return m_grabbedHand; }
    }

    public Transform GrabTransform
    {
        get { return m_grabbedGrabPoint.GrabTransform; }
    }

    public Grab_Point[] GrabPoints
    {
        get { return m_grabPoints; }
    }

    //==============================================================================
    private void Awake()
    {
        if (m_grabPoints.Length == 0)
        {
            // Get the collider from the grabbable
            Collider collider = this.GetComponent<Collider>();
            if (collider == null)
            {
                throw new ArgumentException("Grabbable: Grabbables cannot have zero grab points and no collider -- please add a grab point or collider.");
            }

            // Create a default grab point
            m_grabPoints = new Grab_Point[1] { new Grab_Point(collider) };
        }

        foreach (Grab_Point grabPoint in m_grabPoints)
        {
            // Initialize the grab point
            grabPoint.Initialize();

            // Add the grab trigger and set the grabbable
            GameObject grabObject = grabPoint.GrabCollider.gameObject;
            Grab_Trigger grabTrigger = grabObject.GetComponent<Grab_Trigger>();
            if (grabTrigger == null)
            {
                grabTrigger = grabObject.AddComponent<Grab_Trigger>();
            }
            grabTrigger.SetGrabbable(this);
        }

        // Only allow offhand grab in if GrabMode is "Grab"
        if (!m_grabMode.Equals(GrabMode.Grab))
        {
            m_allowOffhandGrab = false;
        }

        isGrabbed = false;
    }

    public void GrabBegin(Grabbed_Hand_Script grabbedHand, Grab_Point grabPoint)
    {
        isGrabbed = true;

        // Store the grabbed data
        m_grabbedHand = grabbedHand;
        m_grabbedGrabPoint = grabPoint;

        // Add Spring
        if (!rigidbodyDragger)
        {
            rigidbodyDragger = new GameObject("Rigidbody dragger");
            rigidbodyDragger.transform.parent = transform;
            Rigidbody body = rigidbodyDragger.AddComponent<Rigidbody>();
            body.useGravity = false;
            body.isKinematic = true;
        }
        if (!m_SpringJoint)
        {
            m_SpringJoint = rigidbodyDragger.AddComponent<SpringJoint>();
        }

        if (m_grabMode.Equals(GrabMode.Grab))
            m_SpringJoint.transform.position = this.transform.position;
        else 
            m_SpringJoint.transform.position = m_grabbedHand.transform.position;

        m_SpringJoint.anchor = Vector3.zero;
        m_SpringJoint.maxDistance = k_Distance;
        m_SpringJoint.connectedBody = transform.GetComponent<Rigidbody>();

        if (m_grabMode.Equals(GrabMode.Drag))
        {
            m_SpringJoint.spring = d_Spring;
            m_SpringJoint.damper = d_Damper;
            m_SpringJoint.breakForce = d_BreakForce;
            m_SpringJoint.connectedBody.drag = d_Drag;
            m_SpringJoint.connectedBody.angularDrag = d_AngularDrag;
        }
        else if (!m_grabMode.Equals(GrabMode.None))
        {
            m_SpringJoint.spring = k_Spring;
            m_SpringJoint.damper = k_Damper;
            m_SpringJoint.breakForce = k_BreakForce;
            m_SpringJoint.tolerance = k_Tolerance;
            m_SpringJoint.connectedBody.drag = k_Drag;
            m_SpringJoint.connectedBody.angularDrag = k_AngularDrag;
        }

        // Send grab begin message
        GrabbableGrabMsg grabMsg = new GrabbableGrabMsg()
        {
            Sender = this,
        };
        SendMsg(GrabbableGrabMsg.MsgNameGrabBegin, grabMsg);
    }
    public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        // Get rid of the spring joint, but make sure it's not being grabbed by the off hand
        if (m_SpringJoint != null && !isGrabbed)
            Destroy(m_SpringJoint);

        // Keep the object's velocity and angular velocity
        if (m_grabbedGrabPoint.Rigidbody != null)
        {
            m_grabbedGrabPoint.Rigidbody.isKinematic = m_grabbedKinematic;
            m_grabbedGrabPoint.Rigidbody.velocity = linearVelocity;
            m_grabbedGrabPoint.Rigidbody.angularVelocity = angularVelocity;
        }

        // Send grab end message
        GrabbableGrabMsg grabMsg = new GrabbableGrabMsg()
        {
            Sender = this,
        };
        SendMsg(GrabbableGrabMsg.MsgNameGrabEnd, grabMsg);

        // Clear the grabbed data
        m_grabbedHand = null;
        m_grabbedGrabPoint = null;
        isGrabbed = false;
    }

    private void LateUpdate()
    {
        if (isGrabbed && m_SpringJoint == null)
        {
            // If the spring breaks while grabbing the object end the grab
            m_grabbedHand.GrabEnd();
        }
        else if (isGrabbed && m_SpringJoint != null)
        {
            // If grabbed update the springjoint's transform
            m_SpringJoint.transform.position = m_grabbedHand.transform.position;
        }

        // If the grabbable is GrabMode "Grab" update the objects velocity/angularVelocity to match the hand so it keeps it's physics
        if (isGrabbed && m_grabMode.Equals(GrabMode.Grab))
        {
            Quaternion rotationDelta = m_grabbedHand.transform.rotation * Quaternion.Inverse(this.transform.rotation);
            Vector3 positionDelta = (m_grabbedHand.transform.position - this.transform.position);

            float angle;
            Vector3 axis;
            rotationDelta.ToAngleAxis(out angle, out axis);

            // If the angle delta is grater than 180 reverse the direction of rotation to take the shortest path
            if (angle > 180)
                angle -= 360;

            // If the angle has changed update the angularVelocity
            if (angle != 0)
            {
                Vector3 angularTarget = angle * axis;
                this.GetComponent<Rigidbody>().maxAngularVelocity = maxAngularVelocity;
                this.GetComponent<Rigidbody>().angularVelocity = Vector3.MoveTowards(this.GetComponent<Rigidbody>().angularVelocity, angularTarget, maxVelocity * Time.fixedDeltaTime);
            }

            Vector3 VelocityTarget = positionDelta / Time.fixedDeltaTime;
            this.GetComponent<Rigidbody>().velocity = Vector3.MoveTowards(this.GetComponent<Rigidbody>().velocity, VelocityTarget, maxVelocity * Time.fixedDeltaTime);
        }
    }

    private void OnDestroy()
    {
        // If this object is destroyed while being grabbed end the grab animation of the hand
        if (isGrabbed)
        {
            m_grabbedHand.GrabEnd();
        }
    }

    //==============================================================================
    public void OverlapBegin(object hand)
    {
        if (this != null)
        {
            GrabbableOverlapMsg overlapMsg = new GrabbableOverlapMsg()
            {
                Sender = this,
                Hand = hand,
            };
            SendMsg(GrabbableOverlapMsg.MsgNameOverlapBegin, overlapMsg);
        }
    }

    //==============================================================================
    public void OverlapEnd(object hand)
    {
        GrabbableOverlapMsg overlapMsg = new GrabbableOverlapMsg()
        {
            Sender = this != null ? this : null,
            Hand = hand,
        };
        SendMsg(GrabbableOverlapMsg.MsgNameOverlapEnd, overlapMsg);
    }

    //==============================================================================
    private void SendMsg(string msgName, object msg)
    {
        this.transform.SendMessage(msgName, msg, SendMessageOptions.DontRequireReceiver);
    }
}