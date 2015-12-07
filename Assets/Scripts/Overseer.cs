using UnityEngine;
using System.Collections;

public class Overseer : MonoBehaviour {

    [SerializeField]
    private GameObject SprintBot;

    private GameController GameControllerScript;

    public float BotHandicap
    {
        get
        {
            return 0;
        }
    }

	// Use this for initialization
	void Start () {
        GameControllerScript = (GameController)GameObject.FindObjectOfType(typeof(GameController));
	}

    /// <summary>
    /// Tell the bots that a new collectible has been spawned
    /// </summary>
    public void TellTheBotsNewCollectible(GameObject collectible)
    {
        SprintBot.GetComponent<SprintBotController>().NewCollectibleSpawned(collectible);
    }

    /// <summary>
    /// Return the node that is currently closest to the game object
    /// </summary>
    /// <returns></returns>
    public GameObject GetClosestNode(GameObject g)
    {
        float minDistance = float.MaxValue;
        GameObject closestNode = null;

        foreach(var go in GameObject.FindGameObjectsWithTag("Node"))
        {
            if (go.GetComponent<Node>() != null)
            {
                float currDist = Vector3.Distance(g.transform.position, go.transform.position);
                if (currDist < minDistance)
                {
                    minDistance = currDist;
                    closestNode = go;
                }
            }
        }

        return closestNode;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
