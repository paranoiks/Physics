using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [SerializeField]
    private Text PlayerScoreLabel;

    [SerializeField]
    private Text BotScoreLabel;

	// Use this for initialization
	void Start () {
	
	}

    public void UpdateScore(int playerScore, int botScore)
    {
        PlayerScoreLabel.text = playerScore.ToString();
        BotScoreLabel.text = botScore.ToString();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
