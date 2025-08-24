using Mediapipe.Tasks.Components; // Para PoseLandmarkerResult
using Mediapipe.Tasks.Vision.PoseLandmarker;
using Mediapipe.Unity.Sample.PoseLandmarkDetection;
using UnityEngine;
using System.Threading;

public class AvatarPoseController : MonoBehaviour
{
    [Header("Rig Targets (Animation Rigging)")]
    public Transform headTarget;
    public Transform leftHandTarget;
    public Transform rightHandTarget;
    public Transform leftFootTarget;
    public Transform rightFootTarget;

    [Header("Avatar Root Reference")]
    public Transform avatarRoot; // Hips/pelvis del avatar en el mundo

    private PoseLandmarkerRunner poseRunner;

    // 🔒 buffer seguro entre hilos
    private readonly object _resultLock = new object();
    private PoseLandmarkerResult _latestResult;
    private bool _hasNewResult = false;

    void Start()
    {
        poseRunner = Object.FindFirstObjectByType<PoseLandmarkerRunner>();
        if (poseRunner != null)
        {
            poseRunner.OnPoseLandmarksOutput += OnPoseLandmarks;
        }
    }

    private void OnDestroy()
    {
        if (poseRunner != null)
            poseRunner.OnPoseLandmarksOutput -= OnPoseLandmarks;
    }

    // ⚠️ Este callback puede venir en otro hilo → NO tocar Unity aquí
    private void OnPoseLandmarks(PoseLandmarkerResult result, Mediapipe.Image image, long timestamp)
    {
        if (result.poseLandmarks == null || result.poseLandmarks.Count == 0)
            return;

        lock (_resultLock)
        {
            result.CloneTo(ref _latestResult);
            _hasNewResult = true;
        }
    }

    void Update()
    {
        PoseLandmarkerResult result = new PoseLandmarkerResult();

        lock (_resultLock)
        {
            if (_hasNewResult)
            {
                result = _latestResult;
                _hasNewResult = false;
            }
        }

        if (result.poseLandmarks == null || result.poseLandmarks.Count == 0)
            return;

        var landmarks = result.poseLandmarks[0].landmarks;
        if (landmarks == null || landmarks.Count == 0)
            return;

        // Ahora sí, aplicar en el hilo principal de Unity
        SetTargetPosition(headTarget, landmarks[0]);      // Nose (head approx)
        SetTargetPosition(leftHandTarget, landmarks[15]); // Wrist left
        SetTargetPosition(rightHandTarget, landmarks[16]); // Wrist right
        SetTargetPosition(leftFootTarget, landmarks[27]); // Ankle left
        SetTargetPosition(rightFootTarget, landmarks[28]); // Ankle right
    }

    private void SetTargetPosition(Transform target, Mediapipe.Tasks.Components.Containers.NormalizedLandmark lm)
    {
        if (target == null || avatarRoot == null) return;

        Vector3 pos = LandmarkToAvatarSpace(lm);
        target.position = avatarRoot.position + pos;
    }

    private Vector3 LandmarkToAvatarSpace(Mediapipe.Tasks.Components.Containers.NormalizedLandmark lm)
    {
        float x = (lm.x - 0.5f) * 2f;   // [-1,1]
        float y = (lm.y - 0.5f) * -2f;  // invertimos Y
        float z = -lm.z;                // Z ya viene en metros aprox.

        return new Vector3(x, y, z) * 0.5f; // escalar para encajar
    }
}