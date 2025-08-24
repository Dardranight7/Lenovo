using System.Collections.Generic;
using UnityEngine;
using Mediapipe.Unity;
using Mediapipe.Unity.Sample.PoseLandmarkDetection;
using Mediapipe.Tasks.Vision.PoseLandmarker;
public class AvatarIKController : MonoBehaviour
{
    [Header("MediaPipe Pose")]
    //public PoseTrackingGraph poseTrackingGraph;

    [Header("IK Targets")]
    public Transform targetHead;
    public Transform targetLeftHand;
    public Transform hintLeftElbow;
    public Transform targetRightHand;
    public Transform hintRightElbow;

    void Update()
    {
        //var landmarks = poseTrackingGraph.GetCurrentPoseLandmarks();
        //if (landmarks == null) return;

        //// Cabeza (nariz)
        //SetTargetFromLandmark(targetHead, landmarks, 0);

        //// Brazo Izquierdo (muñeca y codo)
        //SetTargetFromLandmark(targetLeftHand, landmarks, 15);
        //SetTargetFromLandmark(hintLeftElbow, landmarks, 13);

        //// Brazo Derecho
        //SetTargetFromLandmark(targetRightHand, landmarks, 16);
        //SetTargetFromLandmark(hintRightElbow, landmarks, 14);
    }

    //void SetTargetFromLandmark(Transform target, IList<NormalizedLandmark> landmarks, int index)
    //{
    //    var lm = landmarks[index];
    //    if (target == null) return;

    //    // Coordenadas adaptadas a Unity (ajusta escala según tu escena)
    //    target.localPosition = new Vector3(lm.X - 0.5f, -lm.Y + 0.5f, -lm.Z) * 2f;
    //}
}

