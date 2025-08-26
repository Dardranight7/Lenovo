using System.Collections.Generic;
using UnityEngine;

public class ExperienceFlow : MonoBehaviour
{
    public List<GameObject> experienceSteps; // List of experience steps to be executed in order
    int currentStepIndex = 0; // Index of the current step
    public bool reloadScene = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnableIndex(0); // Enable the first step by default
    }

    public void Next()
    {
        if (currentStepIndex + 1 >= experienceSteps.Count) {
            //Reload the scene
            if (reloadScene)
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            else
                currentStepIndex = -1;
        }
        EnableIndex(currentStepIndex + 1); // Enable the next step
        currentStepIndex++; // Increment the current step index
    }

    public void EnableIndex(int targetIndex)
    {
        //disable all steps
        foreach (GameObject step in experienceSteps)
        {
            step.SetActive(false);
        }
        experienceSteps[targetIndex].SetActive(true); // Activate the step at the target index
    }

}
