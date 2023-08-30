using System.Collections;
using UnityEngine;

namespace Unity.VRTemplate {
    /// <summary>
    ///     Makes the object this is attached to follow a target with a slight delay
    /// </summary>
    public class LazyFollow : MonoBehaviour {
#pragma warning disable 649
        [SerializeField] [Tooltip("The object being followed.")]
        private Transform m_Target;
#pragma warning restore 649

        [SerializeField] [Tooltip("Whether to always follow or only when in-view.")]
        private bool m_FOV;

        [SerializeField] [Tooltip("Whether rotation is locked to the z-axis for can move in any direction.")]
        private bool m_ZRot = true;

        [SerializeField] [Tooltip("Adjusts the follow point from the target by this amount.")]
        private Vector3 m_TargetOffset = Vector3.forward;

        [SerializeField] [Tooltip("Snap to target position when this component is enabled.")]
        private bool m_SnapOnEnable = true;

        public bool followActive = true;
        public float smoothTime = 0.3F;
        private Camera m_Camera;
        private bool m_InFOV;

        private Vector3 m_TargetLastPos;
        private Vector3 velocity = Vector3.zero;

        private Vector3 targetPosition => m_Target.position + m_Target.TransformVector(m_TargetOffset);

        private Quaternion targetRotation {
            get {
                if (!m_ZRot) {
                    Vector3 eulerAngles = m_Target.eulerAngles;
                    eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, 0f);
                    return Quaternion.Euler(eulerAngles);
                }

                return m_Target.rotation;
            }
        }

        private void Awake() {
            if (m_Camera == null)
                m_Camera = Camera.main;

            // Default to main camera
            if (m_Target == null)
                if (m_Camera != null)
                    m_Target = m_Camera.transform;
        }

        private void Start() {
            Vector3 targetPos = targetPosition;
            m_TargetLastPos = targetPos;
        }

        private void Update() {
            if (m_FOV) {
                Vector3 screenPoint = m_Camera.WorldToViewportPoint(gameObject.transform.position);
                bool inFov = screenPoint.z > 0f && screenPoint.x > 0f && screenPoint.x < 1f && screenPoint.y > 0f &&
                             screenPoint.y < 1f;
                if (inFov)
                    return;
            }

            Vector3 targetPos = targetPosition;
            if (m_TargetLastPos == targetPos)
                return;

            if (followActive) {
                transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
                m_TargetLastPos = targetPos;
            }
        }

        private void OnEnable() {
            if (m_SnapOnEnable) {
                transform.position = targetPosition;
                velocity = Vector3.zero;
            }
        }

        public void Summon() {
            m_InFOV = false;
            if (!followActive)
                StartCoroutine(OneTimeSummonPosition());
        }

        private IEnumerator OneTimeSummonFOV() {
            while (!m_InFOV) {
                Vector3 screenPoint = m_Camera.WorldToViewportPoint(gameObject.transform.position);
                bool inFov = screenPoint.z > 0f && screenPoint.x > 0.3f && screenPoint.x < 0.7f &&
                             screenPoint.y > 0.3f && screenPoint.y < 0.7f;
                if (inFov) {
                    m_InFOV = true;
                }
                else {
                    m_InFOV = false;

                    Vector3 targetPos = targetPosition;
                    if (m_TargetLastPos != targetPos) {
                        transform.position =
                            Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
                        m_TargetLastPos = targetPos;
                    }
                }

                yield return null;
            }
        }

        private IEnumerator OneTimeSummonPosition() {
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f) {
                Vector3 targetPos = targetPosition;
                if (m_TargetLastPos != targetPos) {
                    transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
                    m_TargetLastPos = targetPos;
                }

                yield return null;
            }
        }
    }
}