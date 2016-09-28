using UnityEngine;
using System.Collections;

namespace OVRTouchSample
{
    // SIMPLE HAPTICS DEMO
    // See documentation in Update function.
    public class TapeMeasure : MonoBehaviour
    {
        public OVRInput.Controller controller = OVRInput.Controller.LTouch;
        public GameObject tapeAnchor;
        public GameObject tape;
        public GameObject tapeBack;
        public OVRCameraRig rig;

        private OVRHapticsClip m_proceduralClip;

        private TrackedController m_trackedController = null;
        private OVRHaptics.OVRHapticsChannel m_hapticsChannel = null;

        public AudioClip m_clipA;
        public AudioClip m_clipB;
        private OVRHapticsClip m_hapticsClipA;
        private OVRHapticsClip m_hapticsClipB;
        private bool m_prevButton1 = false;
        private bool m_prevButton2 = false;

        private TapeState state = TapeState.Retracted;
        private float prevHapticsMag = 0;
        private Vector3 lockedPos = Vector3.zero;
        private System.Random rand = new System.Random();


        private enum TapeState { Retracted, Retracting, Locked };

        private void Awake()
        {
            m_proceduralClip = new OVRHapticsClip();
        }

        private void Start()
        {
            m_trackedController = TrackedController.GetController(controller);
            m_hapticsChannel = controller == OVRInput.Controller.LTouch ? OVRHaptics.LeftChannel : OVRHaptics.RightChannel;
            if (m_clipA != null) m_hapticsClipA = new OVRHapticsClip(m_clipA);
            if (m_clipB != null) m_hapticsClipB = new OVRHapticsClip(m_clipB);
        }

        private void Update()
        {
            // SIMPLE HAPTICS CLIP DEMO
            // This is a primitive haptics demo, intended simply to show the API in action.
            // NOTE 1: looping clips is currently unsupported. To be added in a future update of the utilities. 
            // NOTE 2: terminating individual clips is currently unsupported; entire channel must be cleared. To be added in a future update of the utilities.
            // See more documentation re: procedural haptics below.
            if (OVRInput.Get(OVRInput.Button.One, controller) != m_prevButton1)
            {
                m_prevButton1 = !m_prevButton1;
                if (m_prevButton1)
                {
                    m_hapticsChannel.Queue(m_hapticsClipA);
                }
                else
                {
                    m_hapticsChannel.Clear();
                }
            }
            if (OVRInput.Get(OVRInput.Button.Two, controller) != m_prevButton2)
            {
                m_prevButton2 = !m_prevButton2;
                if (m_prevButton2)
                {
                    m_hapticsChannel.Queue(m_hapticsClipB);
                }
                else
                {
                    m_hapticsChannel.Clear();
                }
            }

            // PROCEDURAL HAPTICS UPDATE
            // This demonstrates how an app might generate haptics procedurally in response to user action.
            // NOTE 1: the use of "Preempt" here interferes with haptics executed above. To be fixed in a future update of the utilities.
            Vector3 pos = tapeAnchor.transform.position;
            Vector3 vel = OVRInput.GetLocalControllerVelocity(controller);

            if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller) < 0.5f)
            {
                if (state == TapeState.Locked)
                {
                    state = TapeState.Retracting;
                }
            }
            else if (state != TapeState.Locked)
            {
                lockedPos = pos;
                state = TapeState.Locked;
            }

            if (state == TapeState.Retracting)
            {
                lockedPos = Vector3.Lerp(lockedPos, pos, 0.15f);
                Vector3 retractDelta = lockedPos - pos;
                float retractThreshold = 0.05f;
                if (retractDelta.magnitude < retractThreshold)
                {
                    state = TapeState.Retracted;
                }
            }

            if (state == TapeState.Retracted)
            {
                if (tape)
                {
                    tape.transform.localScale = new Vector3(0, 0, 0);
                }
            }
            else
            {
                Vector3 lockedDelta = lockedPos - pos;
                float lockedDeltaMag = lockedDelta.magnitude;
                float hapticsDeltaMag = lockedDeltaMag - prevHapticsMag;

                float intervalMag = 0.0004f;

                if (Mathf.Abs(hapticsDeltaMag) > intervalMag)
                {
                    prevHapticsMag = lockedDeltaMag;

                    m_proceduralClip.Reset();

                    float minSens = 0.000f;
                    float maxSens = 0.006f;

                    float sens = Mathf.Clamp(Mathf.Abs(hapticsDeltaMag), minSens, maxSens);
                    float scale = sens / maxSens;
                    if (hapticsDeltaMag < 0)
                        scale *= 0.95f;
                    if (state == TapeState.Retracting)
                        scale *= 1.4f;

                    int numSamples = rand.Next(3, 9);
                    if (hapticsDeltaMag < 0)
                        numSamples = rand.Next(6, 8);
                    if (state == TapeState.Retracting)
                        numSamples = rand.Next(7, 10);
                    for (int i = 0; i < numSamples; i++)
                    {
                        float finalScale = 0.0f;

                        if (state == TapeState.Retracting)
                        {
                            float r = (float)rand.NextDouble();
                            if (r < 0.25f)
                                r += 0.15f;
                            finalScale = Mathf.Clamp01(scale * r);
                        }
                        else if (hapticsDeltaMag < 0)
                        {
                            float r = (float)rand.NextDouble();
                            if (r > 0.55f)
                                r -= 0.55f;
                            finalScale = Mathf.Clamp01(scale * r);
                        }
                        else
                        {
                            finalScale = Mathf.Clamp01(scale * (float)rand.NextDouble());
                        }

                        m_proceduralClip.WriteSample((byte)(finalScale * byte.MaxValue));
                    }

                    int numPadding = (int)Mathf.Clamp(10 - (vel.magnitude * 5), 0, 10);
                    for (int i = numSamples; i <= numPadding; i++)
                    {
                        m_proceduralClip.WriteSample((byte)(0));
                    }
                }

                if (tape)
                {
                    Transform anchor = (controller == OVRInput.Controller.LTouch) ? rig.leftHandAnchor : rig.rightHandAnchor;
                    Vector3 up = Vector3.Cross(lockedPos - anchor.position, anchor.right);
                    tape.transform.LookAt(lockedPos, up);
                    tape.transform.localScale = new Vector3(0.002f, 1, lockedDeltaMag / 2.0f * 0.2f);
                    tape.transform.position = lockedPos - (lockedDelta / 2.0f);
                    tape.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1, lockedDeltaMag / 0.03f);
                }
                if (tapeBack)
                {
                    tapeBack.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1, lockedDeltaMag / 0.03f);
                }

                if (controller == OVRInput.Controller.LTouch)
                    OVRHaptics.LeftChannel.Preempt(m_proceduralClip);
                if (controller == OVRInput.Controller.RTouch)
                    OVRHaptics.RightChannel.Preempt(m_proceduralClip);
            }
        }
    }

}