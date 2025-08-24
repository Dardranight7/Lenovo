using System.Collections.Generic;
using UnityEngine;

public class ShieldCharacter : MonoBehaviour
{
    public float EffectArea = 60f; // The area of effect radius for the shield
    public bool isPlayer = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<BalloonCharacter> balloonCharacters = new List<BalloonCharacter>(DeffenseMinigame.Instance.balloonEnemies);
        //if shield is nearby to any balloon enemy, destroy the balloon enemy
        foreach (var balloon in balloonCharacters)
        {
            if (Vector3.SqrMagnitude(balloon.transform.position - transform.position) < EffectArea * EffectArea) // Adjust the distance threshold as needed
            {
                if (balloon.Shooting)
                {
                    balloon.DestroyBalloon(); // Destroy the balloon enemy}
                    if (isPlayer)
                    {
                        DeffenseMinigame.Instance.RejectedShield2++;
                    }
                    else
                    {
                        DeffenseMinigame.Instance.RejectedShield++; // Increment the Entered counter in DeffenseMinigame
                    }
                    SFXManager.Instance.PlaySFX("Protect"); // Play the balloon pop sound effect
                    Debug.Log("Shield destroyed a balloon enemy!");
                }
            }
        }
    }
}
