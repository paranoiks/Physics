using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Action
{
    MoveLeft,
    MoveRight,
    Jump
}

public class SprintBotController : MonoBehaviour {

    [SerializeField]
    private Node StartNode;

    [SerializeField]
    private Node GoalNode;

    [SerializeField]
    private Material NextNodeMaterial;

    private GameObject Collectible;

    private List<Node> Path;

    public List<Node> PPath
    {
        get
        {
            return Path;
        }
    }

    private float DistanceTreshold = 0.4f;

    float PathResetTimer = 3;

    private Overseer OverseerScript;
    private Pathfinder PathfinderScript;

    // Use this for initialization
    void Start () {
        GetNewPath(false);
        OverseerScript = (Overseer)GameObject.FindObjectOfType(typeof(Overseer));
        PathfinderScript = (Pathfinder)GameObject.FindObjectOfType(typeof(Pathfinder));
	}

    private void Move(bool left)
    {
        float k = left ? -1 : 1;

        PhysicsHandler physicsComponent = gameObject.GetComponent<PhysicsHandler>();

        if (physicsComponent != null)
        {
            physicsComponent.AddForce(Vector2.right, physicsComponent.MovePower * k);
        }
    }

    private void Jump()
    {
        PhysicsHandler physicsComponent = gameObject.GetComponent<PhysicsHandler>();
        //add an impulse to go up           
        if (physicsComponent != null)
        {
            if (physicsComponent.OnGround)
            {
                physicsComponent.OnGround = false;
                physicsComponent.AddImpulse(Vector2.up, physicsComponent.JumpPower);
            }
        }
    }

    private void EvaluatePath()
    {
        if(Path.Count == 0)
        {
            GetNewPath(true);
            return;
        }
        Node nextNode = Path[Path.Count - 1];
        if (nextNode == null)
        {
            GetNewPath(true);
            return;
        }
        List<Action> actions = CalculateAction(nextNode);
        if(actions.Count == 1 && actions[0] == Action.Jump)
        {
            GetNewPath(true);
            return;
        }
        if(actions.Count > 0)
        {
            foreach(var a in actions)
            {
                switch(a)
                {
                    case Action.Jump:
                        Jump();
                        break;
                    case Action.MoveLeft:
                        Move(true);
                        break;
                    case Action.MoveRight:
                        Move(false);
                        break;
                }
            }
        }
        if(DistanceToNode(nextNode) < DistanceTreshold)
        {
            Path.RemoveAt(Path.Count - 1);
        }
    }

    public void NewCollectibleSpawned(GameObject collectible)
    {
        Collectible = collectible;
        GetNewPath(true);
    }

    /// <summary>
    /// Calculate the distance to a given node
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private float DistanceToNode(Node node)
    {
        return Vector3.Distance(transform.position, node.gameObject.transform.position);
    }

    /// <summary>
    /// Return the actions that need to be taken to get to the given node
    /// from the bot's current position
    /// </summary>
    /// <param name="nextNode"></param>
    /// <returns></returns>
    private List<Action> CalculateAction(Node nextNode)
    {
        List<Action> actions = new List<Action>();

        Vector3 position = transform.position;
        Vector3 nodePosition = nextNode.gameObject.transform.position;       

        if(nodePosition.y > position.y + 2)
        {
            actions.Add(Action.Jump);
        }

        if(nodePosition.x > position.x + DistanceTreshold)
        {
            actions.Add(Action.MoveRight);
        }
        else if(nodePosition.x < position.x - DistanceTreshold)
        {
            actions.Add(Action.MoveLeft);
        }

        return actions;
    }
	
    /// <summary>
    /// Get a new path to follow
    /// </summary>
    private void GetNewPath(bool newPath)
    {
        if(Collectible == null)
        {
            return;
        }
        Node nextNode = null;
        if (!newPath)
        {
            nextNode = (Path == null || Path.Count == 0) ? OverseerScript.GetClosestNode(gameObject).GetComponent<Node>() : Path[Path.Count - 1];
        }
        else
        {
            nextNode = OverseerScript.GetClosestNode(gameObject).GetComponent<Node>();
        }
        Node targetNode = OverseerScript.GetClosestNode(Collectible).GetComponent<Node>();

        var currPath =  PathfinderScript.FindPath(nextNode, targetNode, OverseerScript.BotHandicap);
        
        Path = currPath;
    }

    public void PortalsChanged()
    {
        GetNewPath(true);
    }

    private void HandlePathResetTimer()
    {
        PathResetTimer -= Time.deltaTime;
        if(PathResetTimer <= 0)
        {
            PathResetTimer = 3;
            GetNewPath(true);
        }
    }

    public void DrawPath()
    {
        GL.Begin(GL.LINES);
        for (int i = 0; i < Path.Count-1; i++)
        {
            if(Path[i] == null || Path[i+1] == null)
            {
                continue;
            }
            Vector3 currNodePos = Path[i].transform.position;
            GL.Vertex3(currNodePos.x, currNodePos.y, 0);
            Vector3 nextNodePos = Path[i + 1].transform.position;
            GL.Vertex3(nextNodePos.x, nextNodePos.y, 0);
        }
        GL.End();
    }

	// Update is called once per frame
	void Update () {
        HandlePathResetTimer();
        EvaluatePath();
	}
}
