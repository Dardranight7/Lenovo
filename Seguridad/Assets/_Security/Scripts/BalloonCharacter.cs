using UnityEngine;

public class BalloonCharacter : MonoBehaviour
{
    public Transform virus; // Reference to the balloon transform
    public Animator animator; // Reference to the balloon animator
    Vector3 targetPos;
    public bool Shooting = false;

    private void Start()
    {
        targetPos = NearbyFarPos.Instance.RandomPointBetweenPointGoalAndPointGoalTwo();
    }

    public void OnVirusReached()
    {
        if (Shooting)
            return;
        // Logic to handle when the virus reaches the balloon
        Debug.Log("Virus reached the balloon!");
        animator.Play("Shooting");
        SFXManager.Instance.PlaySFX("Shoot"); // Play the balloon shoot sound effect
        // You can add more logic here, like playing an animation or sound
        Shooting = true;
    }

    private void Update()
    {
        if (Shooting)
        {
            //move from current pos to target pos
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * 300f);
            // compare sqr magnitude to trigger balloon function when close enough
            if (Vector3.SqrMagnitude(transform.position - targetPos) < 0.5f * 0.5f) // Adjust the distance threshold as needed
            {
                DestroyBalloon();
                DeffenseMinigame.Instance.Entered++; // Increment the Entered counter in DeffenseMinigame
                SFXManager.Instance.PlaySFX("Goal"); // Play the balloon pop sound effect
            }
        }
    }

    public void DestroyBalloon()
    {
        Debug.Log("Balloon destroyed!");
        DeffenseMinigame.Instance.RemoveBalloonFromList(this); // Remove from the list in DeffenseMinigame
        Destroy(gameObject);
    }
}
