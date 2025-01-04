using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowGemOnUI : MonoBehaviour
{
    public TextMeshProUGUI gemOnMainUI;
    public TextMeshProUGUI gemOnUIGameplay;
    public TextMeshProUGUI gemOnContinueButton;
    public TextMeshProUGUI gemOnWinCollectedText;

    // Update is called once per frame
    void Update()
    {
        gemOnMainUI.text = string.Format("{0:0}", GameManager.totalGemAmount);
        gemOnUIGameplay.text = string.Format("{0:0}", GameManager.totalGemAmount);
        gemOnWinCollectedText.text = string.Format("{0:0}", GameManager.instance.currentGemCollected);
        gemOnContinueButton.text = string.Format("{0:0}", GameManager.instance.currentGemCollected * MenuManager.instance.multiplierValue);
    }
}
