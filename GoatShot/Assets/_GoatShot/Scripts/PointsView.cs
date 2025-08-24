using TMPro;
using UnityEngine;

public class PointsView : MonoBehaviour
{
    public CounterMinigame counterMinigame;
    public TextMeshProUGUI goodText,badText, excellentText;

    private void Start()
    {
        counterMinigame.OnGameEnd += GetMinigameInfo;
    }

    private void OnDestroy()
    {
        counterMinigame.OnGameEnd -= GetMinigameInfo;
    }

    public void GetMinigameInfo(int excelent,int good,int bad)
    {
        goodText.text = good.ToString();
        badText.text = bad.ToString();  
        excellentText.text = excelent.ToString();
    }
}
