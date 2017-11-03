using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using GoogleARCore;
using GoogleARCore.HelloAR;

namespace ARcorder
{
    public class ArRecordingController : MonoBehaviour
    {
        public Camera m_firstPersonCamera;
        public GameObject m_PlanePrefab;
        public GameObject m_andyAndroidPrefab;
        public GameObject m_searchingForPlaneUI;

        private List<TrackedPlane> m_newPlanes = new List<TrackedPlane>();

        public List<TrackedPlane> trackedPlanes = new List<TrackedPlane>();
        public List<Anchor> anchors = new List<Anchor>();

        const int LOST_TRACKING_SLEEP_TIMEOUT = 15;

        private Color[] m_planeColors = new Color[] {
            new Color(1.0f, 1.0f, 1.0f),
            new Color(0.956f, 0.262f, 0.211f),
            new Color(0.913f, 0.117f, 0.388f),
            new Color(0.611f, 0.152f, 0.654f),
            new Color(0.403f, 0.227f, 0.717f),
            new Color(0.247f, 0.317f, 0.709f),
            new Color(0.129f, 0.588f, 0.952f),
            new Color(0.011f, 0.662f, 0.956f),
            new Color(0f, 0.737f, 0.831f),
            new Color(0f, 0.588f, 0.533f),
            new Color(0.298f, 0.686f, 0.313f),
            new Color(0.545f, 0.764f, 0.290f),
            new Color(0.803f, 0.862f, 0.223f),
            new Color(1.0f, 0.921f, 0.231f),
            new Color(1.0f, 0.756f, 0.027f)
        };
      
        public void Update ()
        {
            // The tracking state must be FrameTrackingState.Tracking in order to access the Frame.
            if (Frame.TrackingState != FrameTrackingState.Tracking)
            {
                Screen.sleepTimeout = LOST_TRACKING_SLEEP_TIMEOUT;
                return;
            }

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Frame.GetNewPlanes(ref m_newPlanes);

            // Iterate over planes found in this frame and instantiate corresponding GameObjects to visualize them.
            for (int i = 0; i < m_newPlanes.Count; i++)
            {
                var plane = Instantiate(m_PlanePrefab, Vector3.zero, Quaternion.identity, transform);
                plane.GetComponent<TrackedPlaneVisualizer>().SetTrackedPlane(m_newPlanes[i]);

                // Apply a random color and grid rotation.
                var randomColor = m_planeColors[Random.Range(0, m_planeColors.Length - 1)];
                var pRenderer = plane.GetComponent<Renderer>();
                pRenderer.material.SetColor("_GridColor", randomColor);
                pRenderer.material.SetFloat("_UvRotation", Random.Range(0.0f, 360.0f));
            }

            // Disable the snackbar UI when no planes are valid.
            bool showSearchingUI = true;
            Frame.GetAllPlanes(ref trackedPlanes);
            for (int i = 0; i < trackedPlanes.Count; i++)
            {
                if (trackedPlanes[i].IsValid)
                {
                    showSearchingUI = false;
                    break;
                }
            }

            m_searchingForPlaneUI.SetActive(showSearchingUI);

            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
                return;

            TrackableHit hit;
            TrackableHitFlag raycastFilter = TrackableHitFlag.PlaneWithinBounds | TrackableHitFlag.PlaneWithinPolygon;

            if (Session.Raycast(m_firstPersonCamera.ScreenPointToRay(touch.position), raycastFilter, out hit))
            {
                var anchor = Session.CreateAnchor(hit.Point, Quaternion.identity);
                anchors.Add(anchor);

                var andyObject = Instantiate(m_andyAndroidPrefab, hit.Point, Quaternion.identity,anchor.transform);

                andyObject.transform.LookAt(m_firstPersonCamera.transform);
            }
        }
      
    }
}
