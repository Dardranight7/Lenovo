using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DeffenseMinigame : MonoBehaviour
{
    public TouchableArea spawnZone; // The spawn zone for the enemies
    [SerializeField] Transform parent;
    [SerializeField] GameObject shieldPrefab, enemyPrefab, virusPrefab;
    List<GameObject> shields = new List<GameObject>();
    [SerializeField] float minTimeBetweenSpawns, maxTimeBetweenSpawns; // Time between enemy spawns, if needed
    [SerializeField] List<Transform> polygonPointsTransform, enemySpawnPoints; // Points defining the spawn area polygon
    public static DeffenseMinigame Instance { get; private set; }

    public List<BalloonCharacter> balloonEnemies = new List<BalloonCharacter>(); // List to keep track of balloon enemies

    public int maxEnemies = 10; // Maximum number of enemies to spawn
    public int maxShields = 4;
    int currentShields, currentEnemy;

    public GameObject AudioParent;

    public TextMeshProUGUI EnteredText, RejectedText, RejectedTwoText; // UI Text elements to display counters
    int entered, rejectedShield, rejectedShield2;
    public int Entered { 
        get => entered;
        set 
        {
            entered = value;
            EnteredText.text = entered.ToString("00"); // Update the UI text for rejected enemies
        }
    } // Counters for rejected and entered enemies
    public int RejectedShield
    {
        get => rejectedShield;
        set
        {
            rejectedShield = value;
            RejectedText.text = rejectedShield.ToString("00"); // Update the UI text for entered enemies
        } // These will be updated in the BalloonCharacter script
    }

    public int RejectedShield2
    {
        get => rejectedShield2;
        set
        {
            rejectedShield2 = value;
            RejectedTwoText.text = rejectedShield2.ToString("00"); // Update the UI text for entered enemies
        }
    }
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        spawnZone.OnAreaClick.AddListener(PlaceShield);
        StartGame();

        Screen.SetResolution(1080, 1920, true);
    }

    private void OnDestroy()
    {
        spawnZone.OnAreaClick.RemoveListener(PlaceShield);
    }

    public void PlaceShield(PointerEventData pointerEvent)
    {
        if (currentShields < maxShields)
        {
            // Check if the shield can be placed at the clicked position
            // Check for existing shields and compare sqr magnitude to avoid overlap or to close proximity
            foreach (var shield in shields)
            {
                if (Vector3.SqrMagnitude(shield.transform.position - new Vector3(pointerEvent.position.x, pointerEvent.position.y,0)) < 0.5f * 0.5f) // Adjust the distance threshold as needed
                {
                    Debug.Log("Shield too close to another shield.");
                    return;
                }
            }
            currentShields++;
            // Logic to visually place the shield in the game
            shields.Add(Instantiate(shieldPrefab, pointerEvent.position, Quaternion.identity, parent));
        }
        else
        {
            Debug.Log("Maximum shields reached.");
        }
    }

    public void StartGame()
    {
        StartCoroutine(GameLoop());
    }

    public TextMeshProUGUI timerText;
    bool doOnce = true; // Flag to ensure the game loop starts only once

    IEnumerator GameLoop()
    {
        float gameDuration = 60f; // Duration of the game in seconds
        float currentTime = Time.time + gameDuration;
        while (currentTime - Time.time > 0)
        {
            while (currentShields < maxShields)
            {
                spawnZone.gameObject.SetActive(true);
                // Wait for the player to place a shield
                yield return new WaitUntil(() => currentShields >= maxShields);
            }
            if (doOnce)
            {
                doOnce = false;
                spawnZone.gameObject.SetActive(false);
                AudioParent.gameObject.SetActive(true);
                SFXManager.Instance.PlaySFX("Pitazo");
            }
            // Spawn enemies or perform game logic here
            while (currentTime - Time.time > 0)
            {
                float remainingTime = currentTime - Time.time;
                int minutes = Mathf.FloorToInt(remainingTime / 60);
                int seconds = Mathf.FloorToInt(remainingTime % 60);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

                List<Vector2> polygonPoints = polygonPointsTransform.ConvertAll(point => new Vector2(point.position.x, point.position.y)); // Convert Transform positions to Vector2
                List<Vector2> enemyPoints = enemySpawnPoints.ConvertAll(point => new Vector2(point.position.x, point.position.y)); // Convert Transform positions to Vector2
                Vector2 randomPoint = PolygonUtils.GetRandomPointInPolygon(polygonPoints);
                Vector2 enemyRandomPoint = PolygonUtils.GetRandomPointInPolygon(enemyPoints);

                // Spawn an enemy at a random position within the spawn zone
                BalloonCharacter balloonCharacter = Instantiate(enemyPrefab, randomPoint, Quaternion.identity, parent).GetComponent<BalloonCharacter>();
                VirusCharacter virusCharacter = Instantiate(virusPrefab, enemyRandomPoint, Quaternion.identity, parent).GetComponent<VirusCharacter>();
                currentEnemy++;
                balloonCharacter.virus = virusCharacter.transform; // Assign the virus to the balloon character
                virusCharacter.balloon = balloonCharacter; // Assign the balloon to the virus character
                balloonEnemies.Add(balloonCharacter); // Add the balloon character to the list
                yield return new WaitForSeconds(Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns));
            }
            timerText.text = "00:00"; // Update the timer text
            yield return null; // Wait for the next frame
        }
        SFXManager.Instance.PlaySFX("Pitazo");
        experienceFlow.Next();
    }

    public ExperienceFlow experienceFlow;


    public void RemoveBalloonFromList(BalloonCharacter balloonCharacter)
    {
        if (balloonEnemies.Contains(balloonCharacter))
        {
            balloonEnemies.Remove(balloonCharacter);
        }
    }
}
