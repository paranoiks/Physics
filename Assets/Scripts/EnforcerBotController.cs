using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnforcerBotController : MonoBehaviour {

    [SerializeField]
    private Node StartNode;

    [SerializeField]
    private Node GoalNode;

    [SerializeField]
    private Material NextNodeMaterial;
    
    private List<Node> Path;

    private float DistanceTreshold = 0.4f;

    float PathResetTimer = 3;

    private NodeController NodeController;
    private SprintBotController SprintBot;
    private PhysicsHelpers PhysicsHelpersScript;
    private PortalController PortalControllerScript;

    private float PortalGunCooldown = 0;
    private float PortalGunCooldownMax = 3;

    // Use this for initialization
    void Start()
    {
        NodeController = (NodeController)GameObject.FindObjectOfType(typeof(NodeController));
        SprintBot = (SprintBotController)GameObject.FindObjectOfType(typeof(SprintBotController));
        PhysicsHelpersScript = (PhysicsHelpers)GameObject.FindObjectOfType(typeof(PhysicsHelpers));
        PortalControllerScript = (PortalController)GameController.FindObjectOfType(typeof(PortalController));
        GetNewPath(false);
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
        if(Path == null || Path.Count == 0)
        {
            GetNewPath(true);
            return;
        }
        Node nextNode = Path[Path.Count - 1];
        nextNode.gameObject.GetComponent<Renderer>().material = NextNodeMaterial;
        List<Action> actions = CalculateAction(nextNode);
        if (actions.Count > 0)
        {
            foreach (var a in actions)
            {
                switch (a)
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
        if (DistanceToNode(nextNode) < DistanceTreshold)
        {
            Path.RemoveAt(Path.Count - 1);
            EvaluateShootingPosition();
        }
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

        if (nodePosition.y > position.y + 2)
        {
            actions.Add(Action.Jump);
        }

        if (nodePosition.x > position.x + DistanceTreshold)
        {
            actions.Add(Action.MoveRight);
        }
        else if (nodePosition.x < position.x - DistanceTreshold)
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
        Overseer overseer = GameObject.Find("Overseer").GetComponent<Overseer>();
        Node nextNode = null;
        if (!newPath)
        {
            nextNode = (Path == null || Path.Count == 0) ? overseer.GetClosestNode(gameObject).GetComponent<Node>() : Path[Path.Count - 1];
        }
        else
        {
            nextNode = overseer.GetClosestNode(gameObject).GetComponent<Node>();
        }

        GameObject player = GameObject.Find("Player");
        Node targetNode = GetTargetNodeShootingPoint();// overseer.GetClosestNode(player).GetComponent<Node>();

        var currPath = GameObject.Find("Pathfinder").GetComponent<Pathfinder>().FindPath(nextNode, targetNode, overseer.BotHandicap);

        Path = currPath;
    }

    /// <summary>
    /// Get the closest node that "sees" at least 2 nodes on the path of the sprint bot
    /// </summary>
    /// <returns></returns>
    private Node GetTargetNodeShootingPoint()
    {
        Node closestNode = null;
        float closestDistance = float.MaxValue;
        Node[] nodes = (Node[])GameObject.FindObjectsOfType(typeof(Node));
        foreach (var node in nodes)
        {            
            if(!NodeController.NodeSightConnections.ContainsKey(node))
            {
                continue;
            }
            List<Node> visibleNodes = NodeController.NodeSightConnections[node];
            List<Node> sprintBotPath = SprintBot.PPath;
            int count = 0;
            foreach (var visibleNode in visibleNodes)
            {
                if (sprintBotPath.Contains(visibleNode))
                {
                    count++;
                }
            }
            if (count >= 2)
            {
                float distanceToNode = Vector3.Distance(gameObject.transform.position, node.transform.position);
                if (distanceToNode < closestDistance)
                {
                    closestDistance = distanceToNode;
                    closestNode = node;
                }
            }           
        }
        if(closestNode == null)
        {
            return nodes[UnityEngine.Random.Range(0, nodes.Length - 1)];
        }
        return closestNode;
    }

    private void HandlePathResetTimer()
    {
        PathResetTimer -= Time.deltaTime;
        if (PathResetTimer <= 0)
        {
            PathResetTimer = 3;
            GetNewPath(true);
        }
    }

    /// <summary>
    /// Check if the bot's current position allows him to shoot portals, that will help the other bot on his way to the goal
    /// Basically, count how many of the other bot's path's nodes can this one see, and if the result is more than one => shoot portals!
    /// </summary>
    private void EvaluateShootingPosition()
    {
        List<Node> sprintBotPath = SprintBot.PPath;
        int counter = 0;
        int lowestIndex = int.MaxValue;
        Node firstNode = null;
        Node secondNode = null;
        for (int i = 1; i < sprintBotPath.Count; i++)
        {
            Node node = sprintBotPath[i];
            if(node == null)
            {
                continue;
            }
            if (PhysicsHelpersScript.CheckVisibility(transform.position, node.gameObject.transform.position))
            {
                counter++;
                if (i < lowestIndex)
                {
                    firstNode = node;
                    lowestIndex = -1;
                }
                secondNode = node;
            }
        }
        if (counter >= 2)
        {
            //SHOOT THE PORTALS
            if (PortalGunCooldown <= 0)
            {
                PortalControllerScript.SpawnPortalsPair(firstNode.transform.position, secondNode.transform.position);
                PortalGunCooldown = PortalGunCooldownMax;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandlePathResetTimer();
        HandlePortalGunCooldown();
        EvaluatePath();
    }

    private void HandlePortalGunCooldown()
    {
        if(PortalGunCooldown >= 0)
        {
            PortalGunCooldown -= Time.deltaTime;
        }
    }

    public void DrawPath()
    {
        if(Path == null)
        {
            return;
        }
        GL.Begin(GL.LINES);
        for (int i = 0; i < Path.Count - 1; i++)
        {
            Vector3 currNodePos = Path[i].transform.position;
            GL.Vertex3(currNodePos.x, currNodePos.y, 0);
            Vector3 nextNodePos = Path[i + 1].transform.position;
            GL.Vertex3(nextNodePos.x, nextNodePos.y, 0);
        }
        GL.End();
    }
}
