using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    [SerializeField]
    private GameObject CollectiblePrefab;

    private GameObject Collectible;

    public int PlayerScore { get; set; }

    public int BotScore { get; set; }

	// Use this for initialization
	void Start () {
        PlayerScore = BotScore = 0;
        SpawnCollectible();
        SetScore();
	}

    /// <summary>
    /// Function to spanw a new collectible
    /// </summary>
    public void SpawnCollectible()
    {
        var spawningPoints = GameObject.FindGameObjectsWithTag("SpawningPoint");
        int index = Random.Range(0, spawningPoints.Length - 1);
        Vector3 spawningPoint = spawningPoints[index].transform.position;
        Collectible = Instantiate(CollectiblePrefab, spawningPoint, Quaternion.identity) as GameObject;
        GameObject.Find("Overseer").GetComponent<Overseer>().TellTheBotsNewCollectible(Collectible);
    }

    /// <summary>
    /// Set the initial score (0:0)
    /// </summary>
    private void SetScore()
    {
        UIController uiController = GameObject.Find("UIController").GetComponent<UIController>();
        uiController.UpdateScore(0, 0);
    }

    /// <summary>
    /// Add a score either to the player or the bots
    /// </summary>
    /// <param name="player"></param>
    public void AddScore(bool player)
    {
        if(player)
        {
            PlayerScore++;
        }
        else
        {
            BotScore++;
        }
        UIController uiController = GameObject.Find("UIController").GetComponent<UIController>();
        uiController.UpdateScore(PlayerScore, BotScore);
    }

    public void ResetCollectible()
    {
        Destroy(Collectible);
        SpawnCollectible();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
