//#define TEST
//#define TEST2
//#define TEST3

/*
 Coded Modified from CinemachineCollider.
 Date: 3/12/2021
 By: Justin Butler
 Reason: Modified how  the camera reacts to a Over-the-Shoulder Camera. Camera no longer clips through the wall. There are still some minor bugs, but I have resolved most of the issue.
 */

using UnityEngine;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine.Serialization;
using System;
using UnityEngine.SceneManagement;

namespace Cinemachine
{
    /// <summary>
    /// An add-on module for Cinemachine Virtual Camera that post-processes
    /// the final position of the virtual camera. Based on the supplied settings,
    /// the Collider will attempt to preserve the line of sight
    /// with the LookAt target of the virtual camera by moving
    /// away from objects that will obstruct the view.
    ///
    /// Additionally, the Collider can be used to assess the shot quality and
    /// report this as a field in the camera State.
    /// </summary>
    [DocumentationSorting(DocumentationSortingAttribute.Level.UserRef)]
    [AddComponentMenu("")] // Hide in menu
    [SaveDuringPlay]
#if UNITY_2018_3_OR_NEWER
    [ExecuteAlways]
#else
    [ExecuteInEditMode]
#endif
    [DisallowMultipleComponent]
    public class JButler_CameraCollider : CinemachineExtension
    {
        /// <summary>Objects on these layers will be detected.</summary>
        [Header("Obstacle Detection")]
        [Tooltip("Objects on these layers will be detected")]
        public LayerMask m_CollideAgainst = 1;

        /// <summary>Obstacles with this tag will be ignored.  It is a good idea to set this field to the target's tag</summary>
        [TagField]
        [Tooltip("Obstacles with this tag will be ignored.  It is a good idea to set this field to the target's tag")]
        public string m_IgnoreTag = string.Empty;

        /// <summary>Objects on these layers will never obstruct view of the target.</summary>
        [Tooltip("Objects on these layers will never obstruct view of the target")]
        public LayerMask m_TransparentLayers = 0;

        /// <summary>Obstacles closer to the target than this will be ignored</summary>
        [Tooltip("Obstacles closer to the target than this will be ignored")]
        public float m_MinimumDistanceFromTarget = 0.1f;

        /// <summary>
        /// When enabled, will attempt to resolve situations where the line of sight to the
        /// target is blocked by an obstacle
        /// </summary>
        [Space]
        [Tooltip("When enabled, will attempt to resolve situations where the line of sight to the target is blocked by an obstacle")]
        [FormerlySerializedAs("m_PreserveLineOfSight")]
        public bool m_AvoidObstacles = true;

        /// <summary>
        /// The raycast distance to test for when checking if the line of sight to this camera's target is clear.
        /// </summary>
        [Tooltip("The maximum raycast distance when checking if the line of sight to this camera's target is clear.  If the setting is 0 or less, the current actual distance to target will be used.")]
        [FormerlySerializedAs("m_LineOfSightFeelerDistance")]
        public float m_DistanceLimit = 0f;

        /// <summary>
        /// Don't take action unless occlusion has lasted at least this long.
        /// </summary>
        [Tooltip("Don't take action unless occlusion has lasted at least this long.")]
        public float m_MinimumOcclusionTime = 0f;

        /// <summary>
        /// Camera will try to maintain this distance from any obstacle.
        /// Increase this value if you are seeing inside obstacles due to a large
        /// FOV on the camera.
        /// </summary>
        [Tooltip("Camera will try to maintain this distance from any obstacle.  Try to keep this value small.  Increase it if you are seeing inside obstacles due to a large FOV on the camera.")]
        public float m_CameraRadius = 0.1f;

        /// <summary>The way in which the Collider will attempt to preserve sight of the target.</summary>
        public enum ResolutionStrategy
        {
            /// <summary>Camera will be pulled forward along its Z axis until it is in front of
            /// the nearest obstacle</summary>
            PullCameraForward,
            /// <summary>In addition to pulling the camera forward, an effort will be made to
            /// return the camera to its original height</summary>
            PreserveCameraHeight,
            /// <summary>In addition to pulling the camera forward, an effort will be made to
            /// return the camera to its original distance from the target</summary>
            PreserveCameraDistance
        };
        /// <summary>The way in which the Collider will attempt to preserve sight of the target.</summary>
        [Tooltip("The way in which the Collider will attempt to preserve sight of the target.")]
        public ResolutionStrategy m_Strategy = ResolutionStrategy.PreserveCameraHeight;

        /// <summary>
        /// Upper limit on how many obstacle hits to process.  Higher numbers may impact performance.
        /// In most environments, 4 is enough.
        /// </summary>
        [Range(1, 10)]
        [Tooltip("Upper limit on how many obstacle hits to process.  Higher numbers may impact performance.  In most environments, 4 is enough.")]
        public int m_MaximumEffort = 4;

        /// <summary>
        /// Smoothing to apply to obstruction resolution.  Nearest camera point is held for at least this long.
        /// </summary>
        [Range(0, 2)]
        [Tooltip("Smoothing to apply to obstruction resolution.  Nearest camera point is held for at least this long")]
        public float m_SmoothingTime = 0;

        /// <summary>
        /// How gradually the camera returns to its normal position after having been corrected.
        /// Higher numbers will move the camera more gradually back to normal.
        /// </summary>
        [Range(0, 10)]
        [Tooltip("How gradually the camera returns to its normal position after having been corrected.  Higher numbers will move the camera more gradually back to normal.")]
        [FormerlySerializedAs("m_Smoothing")]
        public float m_Damping = 0;

        /// <summary>
        /// How gradually the camera moves to resolve an occlusion.
        /// Higher numbers will move the camera more gradually.
        /// </summary>
        [Range(0, 10)]
        [Tooltip("How gradually the camera moves to resolve an occlusion.  Higher numbers will move the camera more gradually.")]
        public float m_DampingWhenOccluded = 0;

        /// <summary>If greater than zero, a higher score will be given to shots when the target is closer to
        /// this distance.  Set this to zero to disable this feature</summary>
        [Header("Shot Evaluation")]
        [Tooltip("If greater than zero, a higher score will be given to shots when the target is closer to this distance.  Set this to zero to disable this feature.")]
        public float m_OptimalTargetDistance = 0;

        [Header("Over the Shoulder")]
        [Tooltip("Our offset reference.")]
        [SerializeField] CinemachineCameraOffset cco = null;
        [Tooltip("This is the offset we are using with Offset Camera Extension.")]
        [SerializeField] Vector3 offset = Vector3.zero;
        [Tooltip("Are we using the GameObject LookAtOrigin or doing it in code.")]
        [SerializeField] GameObject lookAtOrg = null;
        [Tooltip("This is the 2nd Camera Radius.")]
        [SerializeField] float radius = 0.0f;

        [Header("Debug Color")]
        [SerializeField] float duration = 0.1f;
        [ColorUsage(true, false)]
        [SerializeField] Color orange;
        [ColorUsage(true, false)]
        [SerializeField] Color teal;
        [ColorUsage(true, false)]
        [SerializeField] Color purple;
        [ColorUsage(true, false)]
        [SerializeField] Color lightblue;
        [ColorUsage(true, false)]
        [SerializeField] Color silver;
        [ColorUsage(true, false)]
        [SerializeField] Color maroon;
        [ColorUsage(true, false)]
        [SerializeField] Color olive;

        #region Stuff I don't need to worry about.
        /// <summary>See wheter an object is blocking the camera's view of the target</summary>
        /// <param name="vcam">The virtual camera in question.  This might be different from the
        /// virtual camera that owns the collider, in the event that the camera has children</param>
        /// <returns>True if something is blocking the view</returns>
        public bool IsTargetObscured(ICinemachineCamera vcam)
        {
            return GetExtraState<VcamExtraState>(vcam).targetObscured;
        }

        /// <summary>See whether the virtual camera has been moved nby the collider</summary>
        /// <param name="vcam">The virtual camera in question.  This might be different from the
        /// virtual camera that owns the collider, in the event that the camera has children</param>
        /// <returns>True if the virtual camera has been displaced due to collision or
        /// target obstruction</returns>
        public bool CameraWasDisplaced(ICinemachineCamera vcam)
        {
            return GetCameraDisplacementDistance(vcam) > 0;
        }

        /// <summary>See how far the virtual camera wa moved nby the collider</summary>
        /// <param name="vcam">The virtual camera in question.  This might be different from the
        /// virtual camera that owns the collider, in the event that the camera has children</param>
        /// <returns>True if the virtual camera has been displaced due to collision or
        /// target obstruction</returns>
        public float GetCameraDisplacementDistance(ICinemachineCamera vcam)
        {
            return GetExtraState<VcamExtraState>(vcam).colliderDisplacement;
        }

        private void OnValidate()
        {
            m_DistanceLimit = Mathf.Max(0, m_DistanceLimit);
            m_MinimumOcclusionTime = Mathf.Max(0, m_MinimumOcclusionTime);
            m_CameraRadius = Mathf.Max(0, m_CameraRadius);
            m_MinimumDistanceFromTarget = Mathf.Max(0.01f, m_MinimumDistanceFromTarget);
            m_OptimalTargetDistance = Mathf.Max(0, m_OptimalTargetDistance);
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        protected override void OnDestroy()
        {
            DestroyCollider();
            base.OnDestroy();
        }

        /// This must be small but greater than 0 - reduces false results due to precision
        const float PrecisionSlush = 0.001f;

        /// <summary>
        /// Per-vcam extra state info
        /// </summary>
        class VcamExtraState
        {
            public Vector3 m_previousDisplacement;
            public Vector3 m_previousDisplacementCorrection;
            public float colliderDisplacement;
            public bool targetObscured;
            public float occlusionStartTime;
            public List<Vector3> debugResolutionPath = null;

            public void AddPointToDebugPath(Vector3 p)
            {
#if UNITY_EDITOR
                if (debugResolutionPath == null)
                    debugResolutionPath = new List<Vector3>();
                debugResolutionPath.Add(p);
#endif
            }

            // Thanks to Sebastien LeTouze from Exiin Studio for the smoothing idea
            private float m_SmoothedDistance;
            private float m_SmoothedTime;
            public float ApplyDistanceSmoothing(float distance, float smoothingTime)
            {
                if (m_SmoothedTime != 0 && smoothingTime > Epsilon)
                {
                    float now = CinemachineCore.CurrentTime;
                    if (now - m_SmoothedTime < smoothingTime)
                        return Mathf.Min(distance, m_SmoothedDistance);
                }
                return distance;
            }
            public void UpdateDistanceSmoothing(float distance, float smoothingTime)
            {
                float now = CinemachineCore.CurrentTime;
                if (m_SmoothedDistance == 0 || distance <= m_SmoothedDistance)
                {
                    m_SmoothedDistance = distance;
                    m_SmoothedTime = now;
                }
            }
            public void ResetDistanceSmoothing(float smoothingTime)
            {
                float now = CinemachineCore.CurrentTime;
                if (now - m_SmoothedTime >= smoothingTime)
                    m_SmoothedDistance = m_SmoothedTime = 0;
            }
        };

        /// <summary>Inspector API for debugging collision resolution path</summary>
        public List<List<Vector3>> DebugPaths
        {
            get
            {
                List<List<Vector3>> list = new List<List<Vector3>>();
                List<VcamExtraState> extraStates = GetAllExtraStates<VcamExtraState>();
                foreach (var v in extraStates)
                    if (v.debugResolutionPath != null && v.debugResolutionPath.Count > 0)
                        list.Add(v.debugResolutionPath);
                return list;
            }
        }

        /// <summary>
        /// Report maximum damping time needed for this component.
        /// </summary>
        /// <returns>Highest damping setting in this component</returns>
        public override float GetMaxDampTime()
        {
            return Mathf.Max(m_Damping, Mathf.Max(m_DampingWhenOccluded, m_SmoothingTime));
        }
        #endregion

        // Seems to happen at the end(?) of the stage.
        /// <summary>
        /// Callback to do the collision resolution and shot evaluation
        /// </summary>
        /// <param name="vcam">The virtual camera being processed</param>
        /// <param name="stage">The current pipeline stage</param>
        /// <param name="state">The current virtual camera state</param>
        /// <param name="deltaTime">The current applicable deltaTime</param>
        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            // Start fresh.
            VcamExtraState extra = null;

            // Reset all my information.
            if (stage == CinemachineCore.Stage.Noise)
            {
                extra = GetExtraState<VcamExtraState>(vcam);
                extra.targetObscured = false;
                extra.colliderDisplacement = 0;
                if (extra.debugResolutionPath != null)
                    extra.debugResolutionPath.RemoveRange(0, extra.debugResolutionPath.Count);
            }

            // Calculating after noise now so that the camera moves to the proper position.
            // Move the body before the Aim is calculated
            if (stage == CinemachineCore.Stage.Noise)
            {

                // Gets called fasle in: CinemachineMenu (116), 
                // We set this true in the inspector.
                if (m_AvoidObstacles)
                {
                    // Displacement = (0,0,0)
                    Vector3 displacement = Vector3.zero;

                    // Displacement = the new position or (0,0,0).
                    displacement = PreserveLignOfSight(ref state, ref extra);

                    #region Hiding
                    // We can skip this as ours is 0.
                    if (m_MinimumOcclusionTime > Epsilon)
                    {
                        float now = CinemachineCore.CurrentTime;
                        if (displacement.sqrMagnitude < Epsilon)
                            extra.occlusionStartTime = 0;
                        else
                        {
                            if (extra.occlusionStartTime <= 0)
                                extra.occlusionStartTime = now;
                            if (now - extra.occlusionStartTime < m_MinimumOcclusionTime)
                                displacement = extra.m_previousDisplacement;
                        }
                    }

                    // Apply distance smoothing
                    // Ohhh, this is why it snaps... ours is set to 0. I am not positive if I am going to implament this or not.
                    if (m_SmoothingTime > Epsilon)
                    {
                        Vector3 pos = state.CorrectedPosition + displacement;
                        Vector3 dir = pos - state.ReferenceLookAt;
                        float distance = dir.magnitude;
                        if (distance > Epsilon)
                        {
                            dir /= distance;
                            if (!displacement.AlmostZero())
                                extra.UpdateDistanceSmoothing(distance, m_SmoothingTime);
                            distance = extra.ApplyDistanceSmoothing(distance, m_SmoothingTime);
                            displacement += (state.ReferenceLookAt + dir * distance) - pos;
                        }
                    }

                    // Ours is also set to 0...
                    float damping = m_Damping;

                    // So if the displacement is ever (0,0,0)...
                    if (displacement.AlmostZero())
                        //Put the camera back? I think that is want is happening.
                        extra.ResetDistanceSmoothing(m_SmoothingTime);
                    else
                        // Again, ours is set to 0.
                        damping = m_DampingWhenOccluded;

                    // So damping affects differently when you use the other settings.
                    // Damping = 0... so false.
                    if (damping > 0 && deltaTime >= 0 && VirtualCamera.PreviousStateIsValid)
                    {
                        Vector3 delta = displacement - extra.m_previousDisplacement;
                        delta = Damper.Damp(delta, damping, deltaTime);
                        displacement = extra.m_previousDisplacement + delta;
                    }
                    #endregion

                    // Wait, are we finally moving the camera?
                    extra.m_previousDisplacement = displacement;

                    // Yes, I believe we are. This corrects for the camera's radius. The correctedPosition has not changed, I am not certain where it is.
                    #region DEBUGGING
#if TEST
                    Debug.Log("<b><color=purple>state.CorrectedPosition= " + state.CorrectedPosition + "</color></b>");
                    Debug.DrawLine(state.CorrectedPosition - Vector3.forward * 0.01f, state.CorrectedPosition + Vector3.forward * 0.01f, purple, duration);
                    Debug.DrawLine(state.CorrectedPosition - Vector3.right * 0.01f, state.CorrectedPosition + Vector3.right * 0.01f, purple, duration);
                    Debug.DrawLine(state.CorrectedPosition - Vector3.up * 0.01f, state.CorrectedPosition + Vector3.up * 0.01f, purple, duration);
#endif
                    #endregion
                    // This is going to take the correctedPosition and add the displacement. Also a reference to its state.
                    Vector3 correction = RespectCameraRadius(state.CorrectedPosition + displacement, ref state);

                    #region Hiding
                    // Dampoing is 0, this is false.
                    if (damping > 0 && deltaTime >= 0 && VirtualCamera.PreviousStateIsValid)
                    {
                        Vector3 delta = correction - extra.m_previousDisplacementCorrection;
                        delta = Damper.Damp(delta, damping, deltaTime);
                        correction = extra.m_previousDisplacementCorrection + delta;
                    }
                    #endregion

                    // Make the final adjustment to the camera.
                    displacement += correction;

                    // Store all our knowledge.
                    extra.m_previousDisplacementCorrection = correction;
                    state.PositionCorrection += displacement;
                    extra.colliderDisplacement += displacement.magnitude;
                    #region DEBUGGING
#if TEST2
                    Debug.Log("<b><color=cyan>extra.m_previousDisplacementCorrection = correction = " + extra.m_previousDisplacementCorrection + "</color></b>");
                    Debug.DrawLine(extra.m_previousDisplacementCorrection - Vector3.forward * 0.1f, extra.m_previousDisplacementCorrection + Vector3.forward * 0.1f, Color.cyan, duration);
                    Debug.DrawLine(extra.m_previousDisplacementCorrection - Vector3.right * 0.1f, extra.m_previousDisplacementCorrection + Vector3.right * 0.1f, Color.cyan, duration);
                    Debug.DrawLine(extra.m_previousDisplacementCorrection - Vector3.up * 0.1f, extra.m_previousDisplacementCorrection + Vector3.up * 0.1f, Color.cyan, duration);
#endif
                    #endregion
                }
            }
            // Rate the shot after the aim was set
            if (stage == CinemachineCore.Stage.Finalize)
            {
                extra = GetExtraState<VcamExtraState>(vcam);
                extra.targetObscured = IsTargetOffscreen(state) || CheckForTargetObstructions(state);

                // GML these values are an initial arbitrary attempt at rating quality
                if (extra.targetObscured)
                    state.ShotQuality *= 0.2f;
                if (extra.colliderDisplacement > 0)
                    state.ShotQuality *= 0.8f;

                float nearnessBoost = 0;
                const float kMaxNearBoost = 0.2f;
                if (m_OptimalTargetDistance > 0 && state.HasLookAt)
                {
                    float distance = Vector3.Magnitude(state.ReferenceLookAt - state.FinalPosition);
                    if (distance <= m_OptimalTargetDistance)
                    {
                        float threshold = m_OptimalTargetDistance / 2;
                        if (distance >= threshold)
                            nearnessBoost = kMaxNearBoost * (distance - threshold)
                                / (m_OptimalTargetDistance - threshold);
                    }
                    else
                    {
                        distance -= m_OptimalTargetDistance;
                        float threshold = m_OptimalTargetDistance * 3;
                        if (distance < threshold)
                            nearnessBoost = kMaxNearBoost * (1f - (distance / threshold));
                    }
                    state.ShotQuality *= (1f + nearnessBoost);
                }
            }
        }

        private Vector3 PreserveLignOfSight(ref CameraState state, ref VcamExtraState extra)
        {
            // Displacement = (0,0,0)
            Vector3 displacement = Vector3.zero;

            // HasLookAt  is the LookAt target on the FreeLook camera.
            // If CollideAgainst is anything but 0(Nothing)...
            // And if that isn't a transparent layer.
            if (state.HasLookAt && m_CollideAgainst != 0
                && m_CollideAgainst != m_TransparentLayers)
            {

                // Grab the corrected position (I am assuming that this is after the camera initially moves)
                Vector3 cameraPos = state.CorrectedPosition;

                // Grabs the LookAt's position (I think this is were the ray is going to shoot from.)
                Vector3 lookAtPos = state.ReferenceLookAt;

                RaycastHit hitInfo = new RaycastHit();

                // A method the pulls the camera forward. Takes the corrected camera's position, the LookAt's position, the Layermask for what to collide (excluding the transparentLayers), and our hitInfo and returns a Vector3 which the camera's new position.
                displacement = PullCameraInFrontOfNearestObstacle(
                    cameraPos, lookAtPos, m_CollideAgainst & ~m_TransparentLayers, ref hitInfo);

                // Yep, like I thought. They are adding the two positions together to get the new position.
                Vector3 pos = cameraPos + displacement;
                #region DEBUGGING
#if TEST
                Debug.Log("<b><color=grey>pos = cameraPos + displacement = " + pos + "</color></b>");
                Debug.DrawLine(pos - Vector3.forward * 0.1f, pos + Vector3.forward * 0.1f, Color.grey, duration);
                Debug.DrawLine(pos - Vector3.right * 0.1f, pos + Vector3.right * 0.1f, Color.grey, duration);
                Debug.DrawLine(pos - Vector3.up * 0.1f, pos + Vector3.up * 0.1f, Color.grey, duration);
#endif
                #endregion

                // Checks to see if we are hitting anything at this new point.
                if (hitInfo.collider != null)
                {
                    // Adds the latest pos to its DebugPath. I am not sure where this information is called, but I have a guess that this is dealing with the Editor thing.
                    extra.AddPointToDebugPath(pos);
                    // Checks to see if our Strategy is not PullCameraForward. It is, so we skip this.
                    if (m_Strategy != ResolutionStrategy.PullCameraForward)
                    {
                        Vector3 targetToCamera = cameraPos - lookAtPos;
                        pos = PushCameraBack(
                            pos, targetToCamera, hitInfo, lookAtPos,
                            new Plane(state.ReferenceUp, cameraPos),
                            targetToCamera.magnitude, m_MaximumEffort, ref extra);
                    }
                }

                // Since the other code is skipped, this is next.
                // This displacement = (what every was decided before).
                // This undoes what was done in pos... right? I guess for PullCameraForward, it isn't neccessary.
                displacement = pos - cameraPos;
                #region DEBUGGING
#if TEST2
                Debug.Log("<b><color=teal>displacement = pos - cameraPos = " + displacement + "</color></b>");
                Debug.DrawLine(displacement - Vector3.forward * 0.1f, displacement + Vector3.forward * 0.1f, teal, duration);
                Debug.DrawLine(displacement - Vector3.right * 0.1f, displacement + Vector3.right * 0.1f, teal, duration);
                Debug.DrawLine(displacement - Vector3.up * 0.1f, displacement + Vector3.up * 0.1f, teal, duration);
#endif
                #endregion
            }
            // Returns the thing above or (0,0,0). It returns (0,0,0) if it isn't hitting anything, putting the point on the camera.
            return displacement;
        }

        private Vector3 PullCameraInFrontOfNearestObstacle(
            Vector3 cameraPos, Vector3 lookAtPos, int layerMask, ref RaycastHit hitInfo)
        {

            // A fresh vector.
            Vector3 displacement = Vector3.zero;

            // Dir equals the camera's position - the LookAt's position.
            Vector3 dir = cameraPos - lookAtPos;
            #region DEBUGGING
#if TEST2
            Debug.Log("<b><color=yellow>Dir (cameraPos - lookAtPos) = " + dir + "</color></b>");
            Debug.DrawLine(dir - Vector3.forward * 0.01f, dir + Vector3.forward * 0.01f, Color.yellow, duration);
            Debug.DrawLine(dir - Vector3.right * 0.01f, dir + Vector3.right * 0.01f, Color.yellow, duration);
            Debug.DrawLine(dir - Vector3.up * 0.01f, dir + Vector3.up * 0.01f, Color.yellow, duration);
#endif
            #endregion

            // The length of dir.
            float targetDistance = dir.magnitude;
            #region DEBUGGING
#if TEST2
            Debug.Log("<b><color=cyan>targetDistance (cameraPos - lookAtPos).magnitude = " + targetDistance + "</color></b>");
#endif
            #endregion

            // Is the length of dir greater then 0.0001? I feel like this is always going to be true.
            if (targetDistance > Epsilon)
            {

                // dir = dir / targetDistance
                // Divides each componnent of dir (x,y,z) by targetDistance.
                dir /= targetDistance;
                #region DEBUGGING
#if TEST2
                Debug.Log("<b><color=orange>Dir /= targetDistance = " + dir + "</color></b>");
                Debug.DrawLine(dir - Vector3.forward * 0.01f, dir + Vector3.forward * 0.01f, orange, duration);
                Debug.DrawLine(dir - Vector3.right * 0.01f, dir + Vector3.right * 0.01f, orange, duration);
                Debug.DrawLine(dir - Vector3.up * 0.01f, dir + Vector3.up * 0.01f, orange, duration);
#endif
                #endregion

                // Grabs the minDistance allowed from the target (in our case 0.1). The Epsilon makes sure that we have a constant value.
                float minDistanceFromTarget = Mathf.Max(m_MinimumDistanceFromTarget, Epsilon);
                #region DEBUGGING
#if TEST
                Debug.Log("<b><color=red>minDistanceFromTarget = " + minDistanceFromTarget + "</color></b>");
                Debug.DrawLine(lookAtPos, lookAtPos - Vector3.forward * minDistanceFromTarget, Color.red, duration);
#endif
                #endregion

                // If distance from the camera to the lookatPos is less than the minDistance + 0.0001 (0.1001 in our case).
                // I feel like this is the catch code, while the else is the intended code.
                if (targetDistance < minDistanceFromTarget + Epsilon)
                {

                    // Displacement (0,0,0) = (cameraPos - lookAtPos) * (0.1 - length of dir)
                    displacement = dir * (minDistanceFromTarget - targetDistance);
                    #region DEBUGGING
#if TEST2
                    Debug.Log("<b><color=magenta>displacement = dir * (minDistanceFromTarget - targetDistance) = " + displacement + "</color></b>");
                    Debug.DrawLine(displacement - Vector3.forward * 0.01f, displacement + Vector3.forward * 0.01f, Color.magenta, duration);
                    Debug.DrawLine(displacement - Vector3.right * 0.01f, displacement + Vector3.right * 0.01f, Color.magenta, duration);
                    Debug.DrawLine(displacement - Vector3.up * 0.01f, displacement + Vector3.up * 0.01f, Color.magenta, duration);
#endif
                    #endregion
                }
                else
                {
                    // A float for the ray's length, takes the target's distance and subtracts teh minDistance. I am not sure how to visualize this yet.
                    float rayLength = targetDistance - minDistanceFromTarget;

                    // Checks with the Distance Limit in the inspector to see if we need to adjust the ray's length. Ours is set to 0.
                    if (m_DistanceLimit > Epsilon)
                        // if true, we want the smaller value of these two.
                        rayLength = Mathf.Min(m_DistanceLimit, rayLength);

                    // Make a ray that looks towards the camera, to get the obstacle closest to target
                    Ray ray = new Ray(cameraPos - rayLength * dir, dir);
                    #region DEBUGGING
#if TEST
                    //Debug.Log("<b><color=lime> = rayLength = targetDistance - minDistanceFromTarget = " + rayLength + "</color></b>\nIf these don't match, our DistanceLimit is set to !0.");
                    Debug.DrawRay(cameraPos - rayLength * dir, dir, Color.green, duration);
#endif
                    #endregion

                    // Adjusts the rayLength slightly. I think that this is fine, just add 0.001.
                    rayLength += PrecisionSlush;

                    // Because of the PrecisionSlush, this should always be true.
                    if (rayLength > Epsilon)
                    {

                        // Uses a raycast bool to see the clostest obstacle.
                        // Ray comes from here, hitInfo comes from PerserveLineofSight(410), rayLength comes from here, the layerMask comes from calculating collideAgainst and excluding our transparentLayers, and ignoring our tag (Player).
                        if (RuntimeUtility.RaycastIgnoreTag(
                            ray, out hitInfo, rayLength, layerMask, m_IgnoreTag))
                        {

                            // First obstacle it hits, pulls the camera forward.
                            // Pull camera forward in front of obstacle
                            // This will adjust where on the ray the point will go. If the point is too close, it will be set to 0. Notice here as well, that the PrecisionSlush is removed from the equations. This is so that we have the correct distance values.
                            float adjustment = Mathf.Max(0, hitInfo.distance - PrecisionSlush);
                            #region DEBUGGING
#if TEST2
                            Debug.Log("<b><color=olive>adjustment = " + adjustment + "</color></b>\nIf it is 0, we were too close to the wall.");
#endif
                            #endregion

                            // Displacement (0,0,0) = GetsPoint - cameraPos.
                            // This is practically hit.point. Displacement is probably how much the camera needs to move from its current position to get past the obstacle.
                            displacement = ray.GetPoint(adjustment) - cameraPos;
                            #region DEBUGGING
#if TEST
                            Debug.Log("<b><color=lightblue>displacement = ray.GetPoint(adjustment) - cameraPos = " + displacement + "</color></b>");
                            Debug.DrawLine(displacement - Vector3.forward * 0.01f, displacement + Vector3.forward * 0.01f, Color.blue, duration);
                            Debug.DrawLine(displacement - Vector3.right * 0.01f, displacement + Vector3.right * 0.01f, Color.blue, duration);
                            Debug.DrawLine(displacement - Vector3.up * 0.01f, displacement + Vector3.up * 0.01f, Color.blue, duration);
#endif
                            #endregion
                        }
                    }
                }
            }
            // If the length of dir is less then 0.0001, return (0,0,0).
            // If the distance is too small, push it back out? I am not sure.
            // If everything works properly, it should move it in front of the obstacle.
            return displacement;
        }

        #region Hiding
        private Vector3 PushCameraBack(
            Vector3 currentPos, Vector3 pushDir, RaycastHit obstacle,
            Vector3 lookAtPos, Plane startPlane, float targetDistance, int iterations,
            ref VcamExtraState extra)
        {
            // Take a step along the wall.
            Vector3 pos = currentPos;
            Vector3 dir = Vector3.zero;
            if (!GetWalkingDirection(pos, pushDir, obstacle, ref dir))
                return pos;

            Ray ray = new Ray(pos, dir);
            float distance = GetPushBackDistance(ray, startPlane, targetDistance, lookAtPos);
            if (distance <= Epsilon)
                return pos;

            // Check only as far as the obstacle bounds
            float clampedDistance = ClampRayToBounds(ray, distance, obstacle.collider.bounds);
            distance = Mathf.Min(distance, clampedDistance + PrecisionSlush);

            RaycastHit hitInfo;
            if (RuntimeUtility.RaycastIgnoreTag(ray, out hitInfo, distance,
                    m_CollideAgainst & ~m_TransparentLayers, m_IgnoreTag))
            {
                // We hit something.  Stop there and take a step along that wall.
                float adjustment = hitInfo.distance - PrecisionSlush;
                pos = ray.GetPoint(adjustment);
                extra.AddPointToDebugPath(pos);
                if (iterations > 1)
                    pos = PushCameraBack(
                        pos, dir, hitInfo,
                        lookAtPos, startPlane,
                        targetDistance, iterations - 1, ref extra);

                return pos;
            }

            // Didn't hit anything.  Can we push back all the way now?
            pos = ray.GetPoint(distance);

            // First check if we can still see the target.  If not, abort
            dir = pos - lookAtPos;
            float d = dir.magnitude;
            RaycastHit hitInfo2;
            if (d < Epsilon || RuntimeUtility.RaycastIgnoreTag(
                    new Ray(lookAtPos, dir), out hitInfo2, d - PrecisionSlush,
                    m_CollideAgainst & ~m_TransparentLayers, m_IgnoreTag))
                return currentPos;

            // All clear
            ray = new Ray(pos, dir);
            extra.AddPointToDebugPath(pos);
            distance = GetPushBackDistance(ray, startPlane, targetDistance, lookAtPos);
            if (distance > Epsilon)
            {
                if (!RuntimeUtility.RaycastIgnoreTag(ray, out hitInfo, distance,
                        m_CollideAgainst & ~m_TransparentLayers, m_IgnoreTag))
                {
                    pos = ray.GetPoint(distance); // no obstacles - all good
                    extra.AddPointToDebugPath(pos);
                }
                else
                {
                    // We hit something.  Stop there and maybe take a step along that wall
                    float adjustment = hitInfo.distance - PrecisionSlush;
                    pos = ray.GetPoint(adjustment);
                    extra.AddPointToDebugPath(pos);
                    if (iterations > 1)
                        pos = PushCameraBack(
                            pos, dir, hitInfo, lookAtPos, startPlane,
                            targetDistance, iterations - 1, ref extra);
                }
            }
            return pos;
        }

        private RaycastHit[] m_CornerBuffer = new RaycastHit[4];
        private bool GetWalkingDirection(
            Vector3 pos, Vector3 pushDir, RaycastHit obstacle, ref Vector3 outDir)
        {
            Vector3 normal2 = obstacle.normal;

            // Check for nearby obstacles.  Are we in a corner?
            float nearbyDistance = PrecisionSlush * 5;
            int numFound = Physics.SphereCastNonAlloc(
                pos, nearbyDistance, pushDir.normalized, m_CornerBuffer, 0,
                m_CollideAgainst & ~m_TransparentLayers, QueryTriggerInteraction.Ignore);
            if (numFound > 1)
            {
                // Calculate the second normal
                for (int i = 0; i < numFound; ++i)
                {
                    if (m_CornerBuffer[i].collider == null)
                        continue;
                    if (m_IgnoreTag.Length > 0 && m_CornerBuffer[i].collider.CompareTag(m_IgnoreTag))
                        continue;
                    Type type = m_CornerBuffer[i].collider.GetType();
                    if (type == typeof(BoxCollider)
                        || type == typeof(SphereCollider)
                        || type == typeof(CapsuleCollider))
                    {
                        Vector3 p = m_CornerBuffer[i].collider.ClosestPoint(pos);
                        Vector3 d = p - pos;
                        if (d.magnitude > Vector3.kEpsilon)
                        {
                            if (m_CornerBuffer[i].collider.Raycast(
                                new Ray(pos, d), out m_CornerBuffer[i], nearbyDistance))
                            {
                                if (!(m_CornerBuffer[i].normal - obstacle.normal).AlmostZero())
                                    normal2 = m_CornerBuffer[i].normal;
                                break;
                            }
                        }
                    }
                }
            }

            // Walk along the wall.  If we're in a corner, walk their intersecting line
            Vector3 dir = Vector3.Cross(obstacle.normal, normal2);
            if (dir.AlmostZero())
                dir = Vector3.ProjectOnPlane(pushDir, obstacle.normal);
            else
            {
                float dot = Vector3.Dot(dir, pushDir);
                if (Mathf.Abs(dot) < Epsilon)
                    return false;
                if (dot < 0)
                    dir = -dir;
            }
            if (dir.AlmostZero())
                return false;

            outDir = dir.normalized;
            return true;
        }

        const float AngleThreshold = 0.1f;
        float GetPushBackDistance(Ray ray, Plane startPlane, float targetDistance, Vector3 lookAtPos)
        {
            float maxDistance = targetDistance - (ray.origin - lookAtPos).magnitude;
            if (maxDistance < Epsilon)
                return 0;
            if (m_Strategy == ResolutionStrategy.PreserveCameraDistance)
                return maxDistance;

            float distance;
            if (!startPlane.Raycast(ray, out distance))
                distance = 0;
            distance = Mathf.Min(maxDistance, distance);
            if (distance < Epsilon)
                return 0;

            // If we are close to parallel to the plane, we have to take special action
            float angle = Mathf.Abs(UnityVectorExtensions.Angle(startPlane.normal, ray.direction) - 90);
            if (angle < AngleThreshold)
                distance = Mathf.Lerp(0, distance, angle / AngleThreshold);
            return distance;
        }

        float ClampRayToBounds(Ray ray, float distance, Bounds bounds)
        {
            float d;
            if (Vector3.Dot(ray.direction, Vector3.up) > 0)
            {
                if (new Plane(Vector3.down, bounds.max).Raycast(ray, out d) && d > Epsilon)
                    distance = Mathf.Min(distance, d);
            }
            else if (Vector3.Dot(ray.direction, Vector3.down) > 0)
            {
                if (new Plane(Vector3.up, bounds.min).Raycast(ray, out d) && d > Epsilon)
                    distance = Mathf.Min(distance, d);
            }

            if (Vector3.Dot(ray.direction, Vector3.right) > 0)
            {
                if (new Plane(Vector3.left, bounds.max).Raycast(ray, out d) && d > Epsilon)
                    distance = Mathf.Min(distance, d);
            }
            else if (Vector3.Dot(ray.direction, Vector3.left) > 0)
            {
                if (new Plane(Vector3.right, bounds.min).Raycast(ray, out d) && d > Epsilon)
                    distance = Mathf.Min(distance, d);
            }

            if (Vector3.Dot(ray.direction, Vector3.forward) > 0)
            {
                if (new Plane(Vector3.back, bounds.max).Raycast(ray, out d) && d > Epsilon)
                    distance = Mathf.Min(distance, d);
            }
            else if (Vector3.Dot(ray.direction, Vector3.back) > 0)
            {
                if (new Plane(Vector3.forward, bounds.min).Raycast(ray, out d) && d > Epsilon)
                    distance = Mathf.Min(distance, d);
            }
            return distance;
        }

        private Collider[] mColliderBuffer = new Collider[5];
        Collider[] newColliderBuffer = new Collider[5];
        private static SphereCollider mCameraCollider;
        private static GameObject mCameraColliderGameObject;

        static void DestroyCollider()
        {
            if (mCameraColliderGameObject != null)
            {
                mCameraColliderGameObject.SetActive(false);
                RuntimeUtility.DestroyObject(mCameraColliderGameObject.GetComponent<Rigidbody>());
            }
            RuntimeUtility.DestroyObject(mCameraCollider);
            RuntimeUtility.DestroyObject(mCameraColliderGameObject);
            mCameraColliderGameObject = null;
            mCameraCollider = null;
        }
        #endregion

        private Vector3 RespectCameraRadius(Vector3 cameraPos, ref CameraState state)
        {
            // Fresh vector.
            Vector3 result = Vector3.zero;

            // If the camera's radius is less than 0.0001 OR we are not colliding against anything...
            if (m_CameraRadius < Epsilon || m_CollideAgainst == 0)
                // Return (0,0,0). If it does this, I don't think the camera moves.
                return result;

            // if (state.HasLookAt) {cameraPos - state.ReferenceLookAt} else {Vector3.zero}
            Vector3 dir = state.HasLookAt ? (cameraPos - state.ReferenceLookAt) : Vector3.zero;
            #region DEBUGGING
#if TEST2
            Debug.Log("<b><color=lightblue>dir = state.HasLookAt ? (cameraPos - state.ReferenceLookAt) : Vecotr3.zero = " + dir + "</color></b>");
            Debug.DrawLine(dir - Vector3.forward * 0.1f, dir + Vector3.forward * 0.1f, lightblue, duration);
            Debug.DrawLine(dir - Vector3.right * 0.1f, dir + Vector3.right * 0.1f, lightblue, duration);
            Debug.DrawLine(dir - Vector3.up * 0.1f, dir + Vector3.up * 0.1f, lightblue, duration);
#endif
            #endregion

            // A ray that we can use later.
            Ray ray = new Ray();

            // Another length of dir, but for another part.
            float distance = dir.magnitude;
            #region DEBUGGING
#if TEST2
            Debug.Log("<b><color=brown>distance = dir.magnitude = " + distance + "</color></b>");
#endif
            #endregion

            // If distance is greater than 0.0001
            if (distance > Epsilon)
            {

                // Still unsure how this works.
                dir /= distance;
                #region DEBUGGING
#if TEST2
                Debug.Log("<b><color=white>Dir /= targetDistance #2 = " + dir + "</color></b>");
                Debug.DrawLine(dir - Vector3.forward * 0.01f, dir + Vector3.forward * 0.01f, Color.white, duration);
                Debug.DrawLine(dir - Vector3.right * 0.01f, dir + Vector3.right * 0.01f, Color.white, duration);
                Debug.DrawLine(dir - Vector3.up * 0.01f, dir + Vector3.up * 0.01f, Color.white, duration);
#endif
                #endregion

                // This actually fills in the ray from earlier. If we didn't, the ray would be nothing.
                ray = new Ray(state.ReferenceLookAt, dir);
                #region DEBUGGING
#if TEST
                Debug.Log("<b><color=silver>state.ReferenceLookAt = " + state.ReferenceLookAt + "</color></b>");
                Debug.DrawRay(state.ReferenceLookAt, dir, silver, duration);
#endif
                #endregion
            }

            // Pull it out of any intersecting obstacles
            // Setup to use later.
            RaycastHit hitInfo;

            // Different amounts of Obstacles mean different ways in resolving it. So we find that number out.
            int numObstacles = Physics.OverlapSphereNonAlloc(
                cameraPos, m_CameraRadius, mColliderBuffer,
                m_CollideAgainst, QueryTriggerInteraction.Ignore);

            #region Hiding
            // If we are not hitting anything, our transparentLayers isn't 0 (Nothing), and the distance from the cameraPos and LookAt object is greater than the MinimumDistance + 0.0001...
            // I don't run this code currently...
            if (numObstacles == 0 && m_TransparentLayers != 0
                && distance > m_MinimumDistanceFromTarget + Epsilon)
            {

                // Make sure the camera position isn't completely inside an obstacle.
                // OverlapSphereNonAlloc won't catch those.
                float d = distance - m_MinimumDistanceFromTarget;
                Vector3 targetPos = state.ReferenceLookAt + dir * m_MinimumDistanceFromTarget;
                if (RuntimeUtility.RaycastIgnoreTag(new Ray(targetPos, dir),
                    out hitInfo, d, m_CollideAgainst, m_IgnoreTag))
                {
                    // Only count it if there's an incoming collision but not an outgoing one
                    Collider c = hitInfo.collider;
                    if (!c.Raycast(new Ray(cameraPos, -dir), out hitInfo, d))
                        mColliderBuffer[numObstacles++] = c;
                }
            }
            #endregion

            // I run this code when I hit an object. What it is doing is giving me an GameObject with a collider that will eventually go on the camera. I am assuming that this is how it stays away from the wall.
            if (numObstacles > 0 && distance == 0 || distance > m_MinimumDistanceFromTarget)
            {

                if (mCameraColliderGameObject == null)
                {
                    mCameraColliderGameObject = new GameObject("CinemachineCollider Collider");
                    mCameraColliderGameObject.hideFlags = HideFlags.HideAndDontSave;
                    mCameraColliderGameObject.transform.position = Vector3.zero;
                    mCameraColliderGameObject.SetActive(true);
                    mCameraCollider = mCameraColliderGameObject.AddComponent<SphereCollider>();
                    mCameraCollider.isTrigger = true;
                    var rb = mCameraColliderGameObject.AddComponent<Rigidbody>();
                    rb.detectCollisions = false;
                    rb.isKinematic = true;
                }

                // Making the sphere collider on the hidden GameObject match with what we set it as (0.1).
                mCameraCollider.radius = m_CameraRadius;

                // A reference Vector.
                Vector3 offsetDir;

                // A reference float.
                float offsetDistance;

                // A reference to the cameraPos without changing the Camera's Position while we doing our math.
                Vector3 newCamPos = cameraPos;

                // Start calculating with colliders.
                for (int i = 0; i < numObstacles; ++i)
                {

                    // Start checking the colliders one by one.
                    Collider c = mColliderBuffer[i];

                    // Ignore the Player.
                    if (m_IgnoreTag.Length > 0 && c.CompareTag(m_IgnoreTag))
                        continue;

                    // If we have a lookAt target, move the camera to the nearest edge of obstacle
                    // This checks the distance from earlier (cameraPos - state.ReferenceLookAt).magnitude.
                    if (distance > m_MinimumDistanceFromTarget)
                    {
                        // Change the dir from earlier to what it originally was.
                        dir = newCamPos - state.ReferenceLookAt;

                        // Basically what distance was, but now it is a different variable. I am not positive why they are stating this again.
                        float d = dir.magnitude;

                        if (d > Epsilon)
                        {

                            dir /= d;
                            ray = new Ray(state.ReferenceLookAt, dir);
                            #region DEBUGGING
#if TEST
                            Debug.Log("<b><color=maroon>new dir = " + dir + "</color></b>");
                            Debug.DrawRay(state.ReferenceLookAt, dir, maroon, duration);
#endif
                            #endregion

                            // Checks to see if the collider is outside the obstacle.
                            if (c.Raycast(ray, out hitInfo, d + m_CameraRadius))
                            {

                                newCamPos = ray.GetPoint(hitInfo.distance) - (dir * PrecisionSlush);
                                #region DEBUGGING
#if TEST
                                Debug.Log("<b><color=green>newCamPos = ray.GetPoint(hit.distance) - (dir * 0.001) = " + newCamPos + "</color></b>");
                                Debug.DrawLine(newCamPos - Vector3.forward * 0.1f, newCamPos + Vector3.forward * 0.1f, Color.green, duration);
                                Debug.DrawLine(newCamPos - Vector3.right * 0.1f, newCamPos + Vector3.right * 0.1f, Color.green, duration);
                                Debug.DrawLine(newCamPos - Vector3.up * 0.1f, newCamPos + Vector3.up * 0.1f, Color.green, duration);
#endif
                                #endregion
                            }
                        }
                    }
                    // Calculates the amount we need to offset our camera.
                    if (Physics.ComputePenetration(
                        mCameraCollider, newCamPos, Quaternion.identity,
                        c, c.transform.position, c.transform.rotation,
                        out offsetDir, out offsetDistance))
                    {

                        // Use what we found and apply these to the camera's position.
                        newCamPos += offsetDir * offsetDistance;
                        #region DEBUGGING
#if TEST
                        Debug.Log("<b><color=olive>newCamPos += offsetDir * offsetDistance = " + newCamPos + "</color></b>");
                        Debug.DrawLine(newCamPos - Vector3.forward * 0.01f, newCamPos + Vector3.forward * 0.01f, olive, duration);
                        Debug.DrawLine(newCamPos - Vector3.right * 0.01f, newCamPos + Vector3.right * 0.01f, olive, duration);
                        Debug.DrawLine(newCamPos - Vector3.up * 0.01f, newCamPos + Vector3.up * 0.01f, olive, duration);
#endif
                        #endregion
                    }
                }
                // Collect what the results of the offsets needs to be.
                result = newCamPos - cameraPos;
                #region DEBUGGING
#if TEST2
                Debug.Log("<b><color=red>result = newCamPos - cameraPos = " + result + "</color></b>");
                Debug.DrawLine(result - Vector3.forward * 0.1f, result + Vector3.forward * 0.1f, Color.red, duration);
                Debug.DrawLine(result - Vector3.right * 0.1f, result + Vector3.right * 0.1f, Color.red, duration);
                Debug.DrawLine(result - Vector3.up * 0.1f, result + Vector3.up * 0.1f, Color.red, duration);
#endif
                #endregion
            }

            // Respect the minimum distance from target - push camera back if we have to
            if (distance > Epsilon)
            {

                // Max between minimum distance and camera radius. Then add 0.001.
                float minDistance = Mathf.Max(m_MinimumDistanceFromTarget, m_CameraRadius) + PrecisionSlush;

                // Calculate a new Offset with all the previously calculated information.
                Vector3 newOffset = cameraPos + result - state.ReferenceLookAt;
                #region DEBUGGING
#if TEST2
                Debug.Log("<b><color=white>newOffset.magnitude = " + newOffset.magnitude + "</color></b>");
#endif
                #endregion

                Vector3 lookAtPos = state.ReferenceLookAt;
                RaycastHit hit = new RaycastHit();
                RaycastHit hitInfo2 = new RaycastHit();

                if (TooClose(lookAtPos, ref hit))
                {
                    result = ThirdPersonCheck(ref state, cameraPos, lookAtPos, ref hit, ref hitInfo2);
                }

                // if we are too small now...
                if (newOffset.magnitude < minDistance)
                {
                    result = state.ReferenceLookAt - cameraPos + dir * minDistance;
                    #region DEBUGGING
#if TEST
                    Debug.Log("<b><color=maroon>result = state.ReferenceLookAt - cameraPos + dir * minDistance = " + result + "</color></b>");
                    Debug.DrawLine(result - Vector3.forward * 0.1f, result + Vector3.forward * 0.1f, maroon, duration);
                    Debug.DrawLine(result - Vector3.right * 0.1f, result + Vector3.right * 0.1f, maroon, duration);
                    Debug.DrawLine(result - Vector3.up * 0.1f, result + Vector3.up * 0.1f, maroon, duration);
#endif
                    #endregion
                }

            }
            // the final positions for the camera.
            return result;
        }

        #region Hiding
        private bool CheckForTargetObstructions(CameraState state)
        {
            if (state.HasLookAt)
            {
                Vector3 lookAtPos = state.ReferenceLookAt;
                Vector3 pos = state.CorrectedPosition;
                Vector3 dir = lookAtPos - pos;
                float distance = dir.magnitude;
                if (distance < Mathf.Max(m_MinimumDistanceFromTarget, Epsilon))
                    return true;
                Ray ray = new Ray(pos, dir.normalized);
                RaycastHit hitInfo;
                if (RuntimeUtility.RaycastIgnoreTag(ray, out hitInfo,
                        distance - m_MinimumDistanceFromTarget,
                        m_CollideAgainst & ~m_TransparentLayers, m_IgnoreTag))
                    return true;
            }
            return false;
        }

        private bool IsTargetOffscreen(CameraState state)
        {
            if (state.HasLookAt)
            {
                Vector3 dir = state.ReferenceLookAt - state.CorrectedPosition;
                dir = Quaternion.Inverse(state.CorrectedOrientation) * dir;
                if (state.Lens.Orthographic)
                {
                    if (Mathf.Abs(dir.y) > state.Lens.OrthographicSize)
                        return true;
                    if (Mathf.Abs(dir.x) > state.Lens.OrthographicSize * state.Lens.Aspect)
                        return true;
                }
                else
                {
                    float fov = state.Lens.FieldOfView / 2;
                    float angle = UnityVectorExtensions.Angle(dir.ProjectOntoPlane(Vector3.right), Vector3.forward);
                    if (angle > fov)
                        return true;

                    fov = Mathf.Rad2Deg * Mathf.Atan(Mathf.Tan(fov * Mathf.Deg2Rad) * state.Lens.Aspect);
                    angle = UnityVectorExtensions.Angle(dir.ProjectOntoPlane(Vector3.up), Vector3.forward);
                    if (angle > fov)
                        return true;
                }
            }
            return false;
        }
        #endregion

        Vector3 ThirdPersonCheck(ref CameraState state, Vector3 cameraPos, Vector3 lookAtPos, ref RaycastHit hit, ref RaycastHit hitInfo)
        {
            Vector3 result = Vector3.zero;
            #region DEBUGGING
#if TEST
            Debug.Log("<b><color=orange>Where are new ray hit = " + hit.point + "</color></b>");
            Debug.DrawLine(hit.point - Vector3.forward * 0.01f, hit.point + Vector3.forward * 0.01f, orange, duration);
            Debug.DrawLine(hit.point - Vector3.right * 0.01f, hit.point + Vector3.right * 0.01f, orange, duration);
            Debug.DrawLine(hit.point - Vector3.up * 0.01f, hit.point + Vector3.up * 0.01f, orange, duration);
#endif
            #endregion
            Vector3 lookAtOrigin = CheckLookAtOrigin() ? (lookAtPos - offset) : lookAtOrg.transform.position;
            Vector3 dir = CheckLookAtOrigin() ? Vector3.zero : (cameraPos - lookAtOrigin);
            float distance = dir.magnitude;
            Ray ray = new Ray();

            int numObstacles = Physics.OverlapSphereNonAlloc(
                hit.point, radius, mColliderBuffer,
                m_CollideAgainst, QueryTriggerInteraction.Ignore);
            #region DEBUGGING
#if TEST3
            Debug.Log("<b><color=grey>Shpere Collider Radius = " + radius + "</color></b>");
            Debug.DrawLine(cameraPos - Vector3.forward * radius, cameraPos + Vector3.forward * radius, Color.grey, duration);
            Debug.DrawLine(cameraPos - Vector3.right * radius, cameraPos + Vector3.right * radius, Color.grey, duration);
            Debug.DrawLine(cameraPos - Vector3.up * radius, cameraPos + Vector3.up * radius, Color.grey, duration);
#endif
            #endregion


            #region DEBUGGING
#if TEST
            Debug.Log("<b><color=white>NumObstacles = " + numObstacles + "</color></b>");
#endif
            #endregion
            if (numObstacles > 0 && distance == 0 || distance > m_MinimumDistanceFromTarget)
            {
                if (mCameraColliderGameObject == null)
                {
                    mCameraColliderGameObject = new GameObject("CinemachineCollider Collider");
                    mCameraColliderGameObject.hideFlags = HideFlags.HideAndDontSave;
                    mCameraColliderGameObject.transform.position = Vector3.zero;
                    mCameraColliderGameObject.SetActive(true);
                    mCameraCollider = mCameraColliderGameObject.AddComponent<SphereCollider>();
                    mCameraCollider.isTrigger = true;
                    var rb = mCameraColliderGameObject.AddComponent<Rigidbody>();
                    rb.detectCollisions = false;
                    rb.isKinematic = true;
                }
                mCameraCollider.radius = m_CameraRadius;
                Vector3 offsetDir;
                float offsetDistance;

                Vector3 newCamPos = cameraPos;

                for (int i = 0; i < numObstacles; ++i)
                {
                    Collider c = mColliderBuffer[i];

                    if (m_IgnoreTag.Length > 0 && c.CompareTag(m_IgnoreTag))
                        continue;

                    if (distance > m_MinimumDistanceFromTarget)
                    {
                        dir = newCamPos - lookAtOrigin;
                        float d = dir.magnitude;

                        if (d > Epsilon)
                        {
                            dir /= d;
                            ray = new Ray(lookAtOrigin, dir);
                            #region DEBUGGING
#if TEST
                            Debug.Log("<b><color=maroon>new dir = " + dir + "</color></b>");
                            Debug.DrawRay(lookAtOrigin, dir, maroon, duration);
#endif
                            #endregion
                            if (Physics.Raycast(ray, out hitInfo, d + m_CameraRadius, m_CollideAgainst & ~m_TransparentLayers))
                            {
                                newCamPos = ray.GetPoint(hitInfo.distance) - (dir * PrecisionSlush);
                                #region DEBUGGING
#if TEST
                                Debug.Log("<b><color=green>newCamPos = ray.GetPoint(hit.distance) - (dir * 0.001) = " + newCamPos + "</color></b>");
                                Debug.DrawLine(newCamPos - Vector3.forward * 0.1f, newCamPos + Vector3.forward * 0.1f, Color.green, duration);
                                Debug.DrawLine(newCamPos - Vector3.right * 0.1f, newCamPos + Vector3.right * 0.1f, Color.green, duration);
                                Debug.DrawLine(newCamPos - Vector3.up * 0.1f, newCamPos + Vector3.up * 0.1f, Color.green, duration);
#endif
                                #endregion
                            }
                        }
                    }
                    int newColliders = Physics.OverlapSphereNonAlloc(
    newCamPos, radius, newColliderBuffer,
    m_CollideAgainst, QueryTriggerInteraction.Ignore);
                    for (int a = 0; a < newColliders; ++a)
                    {
                        Collider newC = newColliderBuffer[a];

                        if (m_IgnoreTag.Length > 0 && c.CompareTag(m_IgnoreTag))
                            continue;

                        // Calculates the amount we need to offset our camera.
                        if (Physics.ComputePenetration(
                        mCameraCollider, newCamPos, Quaternion.identity,
                        newC, newC.transform.position, newC.transform.rotation,
                        out offsetDir, out offsetDistance))
                        {

                            // Use what we found and apply these to the camera's position.
                            newCamPos += offsetDir * offsetDistance;
                            #region DEBUGGING
#if TEST
                            Debug.Log("<b><color=olive>newCamPos += offsetDir * offsetDistance = " + newCamPos + "</color></b>");
                            Debug.DrawLine(newCamPos - Vector3.forward * 0.01f, newCamPos + Vector3.forward * 0.01f, olive, duration);
                            Debug.DrawLine(newCamPos - Vector3.right * 0.01f, newCamPos + Vector3.right * 0.01f, olive, duration);
                            Debug.DrawLine(newCamPos - Vector3.up * 0.01f, newCamPos + Vector3.up * 0.01f, olive, duration);
#endif
                            #endregion
                        }
                    }
                }
                // Collect what the results of the offsets needs to be.
                result = newCamPos - cameraPos;
                #region DEBUGGING
#if TEST2
                Debug.Log("<b><color=red>result = newCamPos - cameraPos = " + result + "</color></b>");
                Debug.DrawLine(result - Vector3.forward * 0.1f, result + Vector3.forward * 0.1f, Color.red, duration);
                Debug.DrawLine(result - Vector3.right * 0.1f, result + Vector3.right * 0.1f, Color.red, duration);
                Debug.DrawLine(result - Vector3.up * 0.1f, result + Vector3.up * 0.1f, Color.red, duration);
#endif
                #endregion
            }

            return result;
        }

        bool CheckOtherColliders(Ray ray, out RaycastHit hitInfo, float maxDistance)
        {
            if (RuntimeUtility.RaycastIgnoreTag(ray, out hitInfo, maxDistance, m_CollideAgainst & ~m_TransparentLayers, m_IgnoreTag))
                return true;
            else
                return false;
        }

        bool TooClose(Vector3 lookAtPos, ref RaycastHit hit)
        {
            CheckOffset();
            Vector3 lookAtOrigin = CheckLookAtOrigin() ? (lookAtPos - offset) : lookAtOrg.transform.position;
            Vector3 dir = lookAtPos - lookAtOrigin;
            #region DEBUGGING
#if TEST
            Debug.DrawRay(lookAtOrigin, dir, Color.yellow, duration);
            //Debug.DrawLine(lookAtOrigin, lookAtPos, Color.yellow, duration);
#endif
            #endregion
            Vector3 lookAtOffset = lookAtPos + new Vector3(0, 0, -m_MinimumDistanceFromTarget - PrecisionSlush);
            Vector3 dirOffset = lookAtOffset - lookAtOrigin;
            #region DEBUGGING
#if TEST
            Debug.DrawRay(lookAtOrigin, dirOffset, orange, duration);
            //Debug.DrawLine(lookAtOrigin, lookAtPos, Color.yellow, duration);
#endif
            #endregion
            float distance = dirOffset.magnitude;
            Ray tooClose = new Ray(lookAtOrigin, dirOffset);
            float rayLength = distance + PrecisionSlush;

            if (RuntimeUtility.RaycastIgnoreTag(tooClose, out hit, rayLength, m_CollideAgainst & ~m_TransparentLayers, m_IgnoreTag))
                return true;
            else
                return true;
        }

        bool CheckOffset()
        {
            offset = cco.m_Offset;
            if (offset.x >= 0)
                return true;
            else
                return false;
        }

        bool CheckLookAtOrigin()
        {
            if (lookAtOrg == null)
                return true;
            else
                return false;
        }
    }
}
