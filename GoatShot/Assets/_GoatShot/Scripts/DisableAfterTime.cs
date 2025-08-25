using UnityEngine;

public class DisableAfterTime : MonoBehaviour
{
    float timer = 0f;
    private void OnEnable()
    {
        timer = Time.time + 2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer - Time.time < 0)
        {
            gameObject.SetActive(false);
        }
    }
}
