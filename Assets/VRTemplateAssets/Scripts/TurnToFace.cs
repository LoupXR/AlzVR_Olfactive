using UnityEngine;

namespace Unity.VRTemplate {
    /// <summary>
    ///     Makes this object face a target smoothly and along specific axes
    /// </summary>
    public class TurnToFace : MonoBehaviour {
        private void Awake() {
            // Default to main camera
            if (m_FaceTarget == null)
                if (Camera.main != null)
                    m_FaceTarget = Camera.main.transform;
        }

        private void Update() {
            if (m_FaceTarget != null) {
                Vector3 facePosition = m_FaceTarget.position;
                Vector3 forward = facePosition - transform.position;
                Quaternion targetRotation = forward.sqrMagnitude > float.Epsilon
                    ? Quaternion.LookRotation(forward, Vector3.up)
                    : Quaternion.identity;
                targetRotation *= Quaternion.Euler(m_RotationOffset);
                if (m_IgnoreX || m_IgnoreY || m_IgnoreZ) {
                    Vector3 targetEuler = targetRotation.eulerAngles;
                    Vector3 currentEuler = transform.rotation.eulerAngles;
                    targetRotation = Quaternion.Euler
                    (
                        m_IgnoreX ? currentEuler.x : targetEuler.x,
                        m_IgnoreY ? currentEuler.y : targetEuler.y,
                        m_IgnoreZ ? currentEuler.z : targetEuler.z
                    );
                }

                float ease = 1f - Mathf.Exp(-m_TurnToFaceSpeed * Time.unscaledDeltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, ease);
            }
        }
#pragma warning disable 649
        [SerializeField] [Tooltip("Target to face towards. If not set, this will default to the main camera")]
        private Transform m_FaceTarget;

        [SerializeField] [Tooltip("Speed to turn")]
        private float m_TurnToFaceSpeed = 5f;

        [SerializeField] [Tooltip("Local rotation offset")]
        private Vector3 m_RotationOffset = Vector3.zero;

        [SerializeField] [Tooltip("If enabled, ignore the x axis when rotating")]
        private bool m_IgnoreX;

        [SerializeField] [Tooltip("If enabled, ignore the y axis when rotating")]
        private bool m_IgnoreY;

        [SerializeField] [Tooltip("If enabled, ignore the z axis when rotating")]
        private bool m_IgnoreZ;
#pragma warning restore 649
    }
}