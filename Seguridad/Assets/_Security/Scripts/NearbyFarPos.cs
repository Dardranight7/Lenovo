using UnityEngine;

public class NearbyFarPos : MonoBehaviour
{
    public static NearbyFarPos Instance { get; private set; }
    public Transform pointNear;   // Punto más cercano (escala grande)
    public Transform pointFar;    // Punto más lejano (escala pequeña)
    public Transform pointGoal, pointGoalTwo;

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 RandomPointBetweenPointGoalAndPointGoalTwo()
    {
        return Vector3.Lerp(pointGoal.position, pointGoalTwo.position, Random.Range(0f, 1f));
    }
}
