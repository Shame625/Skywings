using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	
	public GameObject ScoreObj;
    public GameObject CoinsObj;
    public GameObject deadPanel;
    public GameObject autoPilot;
    Color32 deadPanelColor;

    void Awake()
    {
        deadPanelColor = deadPanel.GetComponent<Image>().color;
    }

	public void SetScore(int score)
	{
		ScoreObj.GetComponent<Text> ().text = "Score: " + score.ToString ();
	}

    public void SetCoins(int coins)
    {
        CoinsObj.GetComponent<Text>().text = "Coins: " + coins.ToString();
    }

    public void CloseDeadPanel()
    {
        deadPanel.GetComponent<Image>().color = deadPanelColor;
        deadPanel.transform.GetChild(0).gameObject.SetActive(false);
        deadPanel.transform.GetChild(1).gameObject.SetActive(false);
        deadPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        deadPanel.SetActive(false);
    }

    public void DeadPanel(bool active, int currentScore, int highScore,int coinsGained, int totalCoins)
    {
        deadPanel.SetActive(active);
        if (active == true)
        {
            StartCoroutine(FadeTo(0.75f, 0.25f));
        }
        string newText = "Score: " + currentScore.ToString() + "\nHighest Score: " + highScore.ToString() + "\n\nCoins: " + coinsGained.ToString() +
            "\nTotal Coins: " + totalCoins.ToString();

        if (currentScore >= highScore)
        {
            deadPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        }

        deadPanel.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = newText;

    }

    public void SetAutoPilotOn()
    {
        autoPilot.SetActive(true);
    }

    IEnumerator FadeTo(float aValue, float aTime)
    {
        float alpha = deadPanel.GetComponent<Image>().color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            deadPanel.GetComponent<Image>().color = newColor;
            yield return null;
        }
        deadPanel.transform.GetChild(0).gameObject.SetActive(true);
        deadPanel.transform.GetChild(1).gameObject.SetActive(true);
    }
}
