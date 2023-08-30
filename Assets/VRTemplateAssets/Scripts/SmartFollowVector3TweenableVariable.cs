using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;

namespace Unity.VRTemplate {
    /// <summary>
    ///     This class expands on the vector3 tweenable variable to introduce two concepts:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 A dynamic threshold distance that grows over time in a range,
    ///                 that prevents updating the target so long as the value being assigned to the target is within that
    ///                 threshold.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 A variable speed tween (<see cref="HandleSmartTween" />) that inputs a lower and upper range speed for
    ///                 tweening.
    ///                 The closer the value is to the target, the faster the tween.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </summary>
#if BURST_PRESENT
    [BurstCompile]
#endif
    public class SmartFollowVector3TweenableVariable : Vector3TweenableVariable {
        /// <summary>
        ///     Minimum distance from target before which tween starts.
        /// </summary>
        /// <seealso cref="maxDistanceAllowed" />
        public float minDistanceAllowed { get; set; }

        private float m_MaxDistanceAllowed;

        /// <summary>
        ///     Maximum distance from target before tween targets, when time threshold is reached.
        /// </summary>
        /// <seealso cref="minDistanceAllowed" />
        public float maxDistanceAllowed {
            get => m_MaxDistanceAllowed;
            set {
                m_MaxDistanceAllowed = value;
                m_SqrMaxDistanceAllowed = m_MaxDistanceAllowed * m_MaxDistanceAllowed;
            }
        }

        /// <summary>
        ///     Time required to elapse before the max distance allowed goes from the min distance to the max.
        /// </summary>
        public float minToMaxDelaySeconds { get; set; }

        private float m_SqrMaxDistanceAllowed;

        private float m_LastUpdateTime;

        /// <summary>
        ///     Initializes and returns an instance of <see cref="SmartFollowVector3TweenableVariable" />.
        /// </summary>
        /// <param name="minDistanceAllowed">Minimum distance from target before which tween starts.</param>
        /// <param name="maxDistanceAllowed">Maximum distance from target before tween targets, when time threshold is reached.</param>
        /// <param name="minToMaxDelaySeconds">
        ///     Time required to elapse (in seconds) before the max distance allowed goes from the
        ///     min distance to the max.
        /// </param>
        public SmartFollowVector3TweenableVariable(
            float minDistanceAllowed = 0.01f,
            float maxDistanceAllowed = 0.3f,
            float minToMaxDelaySeconds = 3f) {
            this.minDistanceAllowed = minDistanceAllowed;
            this.maxDistanceAllowed = maxDistanceAllowed;
            this.minToMaxDelaySeconds = minToMaxDelaySeconds;
        }

        /// <summary>
        ///     Checks if the squared distance between the current target and a new target is within a dynamically determined
        ///     threshold,
        ///     based on the time since the last update.
        /// </summary>
        /// <param name="newTarget">The new target position as a float3 vector.</param>
        /// <returns>
        ///     Returns true if the squared distance between the current and new targets is within the allowed threshold,
        ///     false otherwise.
        /// </returns>
        public bool IsNewTargetWithinThreshold(float3 newTarget) {
            float3 currentValue = Value;
            return IsNewTargetWithinThreshold(currentValue, newTarget, minDistanceAllowed, m_MaxDistanceAllowed,
                Time.unscaledTime - m_LastUpdateTime, minToMaxDelaySeconds);
        }

        /// <summary>
        ///     Updates the target position to a new value if it is within a dynamically determined threshold,
        ///     based on the time since the last update.
        /// </summary>
        /// <param name="newTarget">The new target position as a float3 vector.</param>
        /// <returns>Returns true if the target position is updated, false otherwise.</returns>
        public bool SetTargetWithinThreshold(float3 newTarget) {
            bool isWithinThreshold = IsNewTargetWithinThreshold(newTarget);
            if (isWithinThreshold) {
                target = newTarget;
                m_LastUpdateTime = Time.unscaledTime;
            }

            return isWithinThreshold;
        }

        /// <summary>
        ///     Tween to new target with variable speed according to distance from target.
        ///     The closer the target is to the current value, the faster the tween.
        /// </summary>
        /// <param name="deltaTime">Time in seconds since the last frame (such as <see cref="Time.deltaTime" />).</param>
        /// <param name="lowerSpeed">Lower range speed for tweening.</param>
        /// <param name="upperSpeed">Upper range speed for tweening.</param>
        /// <seealso cref="TweenableVariableBase{T}.HandleTween" />
        public void HandleSmartTween(float deltaTime, float lowerSpeed, float upperSpeed) {
            float3 currentValue = Value;
            float3 currentTarget = target;
            ComputeNewTweenTarget(currentValue, currentTarget, m_SqrMaxDistanceAllowed, deltaTime, lowerSpeed,
                upperSpeed, out float newTweenTarget);
            HandleTween(newTweenTarget);
        }

#if BURST_PRESENT
        [BurstCompile]
#endif
        private static void ComputeNewTweenTarget(in float3 currentValue, in float3 targetValue,
            in float sqrMaxDistanceAllowed, in float deltaTime, in float lowerSpeed, in float upperSpeed,
            out float newTweenTarget) {
            float sqrDistFromTarget = math.distancesq(currentValue, targetValue);
            float speedMultiplier = 1f - math.clamp(sqrDistFromTarget / sqrMaxDistanceAllowed, 0f, 1f);
            float speed = math.clamp(speedMultiplier * upperSpeed, lowerSpeed, upperSpeed);
            newTweenTarget = deltaTime * speed;
        }

#if BURST_PRESENT
        [BurstCompile]
#endif
        private static bool IsNewTargetWithinThreshold(in float3 currentValue, in float3 targetValue,
            in float minDistanceAllowed, in float maxDistanceAllowed, in float timeSinceLastUpdate,
            in float minToMaxDelaySeconds) {
            float newSqrTargetOffset = math.distancesq(currentValue, targetValue);

            // Widen tolerance zone over time
            float allowedTargetDistanceOffset = math.lerp(minDistanceAllowed, maxDistanceAllowed,
                math.clamp(timeSinceLastUpdate / minToMaxDelaySeconds, 0f, 1f));
            return newSqrTargetOffset > allowedTargetDistanceOffset * allowedTargetDistanceOffset;
        }
    }
}