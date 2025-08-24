using UnityEngine;

public class VirusCharacter : MonoBehaviour
{
    public BalloonCharacter balloon; // Reference to the balloon transform
    private void Update()
    {
        //move from current position to the balloon position
        transform.position = Vector3.MoveTowards(transform.position, balloon.transform.position, Time.deltaTime * 100f);

        // compare sqr magnitude to trigger balloon function when close enough and destrty the virus
        if (Vector3.SqrMagnitude(transform.position - balloon.transform.position) < 0.5f * 0.5f) // Adjust the distance threshold as needed
        {
            balloon.OnVirusReached(); // Call the method in BalloonCharacter to handle the virus reaching the balloon
            Destroy(gameObject); // Destroy this virus instance
        }
    }
}
