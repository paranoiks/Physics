using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraDrawer : MonoBehaviour {

    private SprintBotController SprintBot;
    private UserController Player;
    private EnforcerBotController EnforcerBot;
    
    [SerializeField]
    private bool ShowSprintBotPath;

    [SerializeField]
    private bool ShowEnforcerBotPath;

    [SerializeField]
    private bool ShowPlayerVisibleNodes;

    [SerializeField]
    private bool ShowNodesSightConnections;

	// Use this for initialization
	void Start () {
        SprintBot = (SprintBotController)GameObject.FindObjectOfType(typeof(SprintBotController));
        Player = (UserController)GameObject.FindObjectOfType(typeof(UserController));
        EnforcerBot = (EnforcerBotController)GameObject.FindObjectOfType(typeof(EnforcerBotController));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnPostRender()
    {
        if (ShowSprintBotPath)
        {
            SprintBot.DrawPath();
        }
        if(ShowEnforcerBotPath)
        {
            EnforcerBot.DrawPath();
        }
        if (ShowPlayerVisibleNodes)
        {
            Player.DrawNodesInSight();
        }
        if(ShowNodesSightConnections)
        {
            DrawNodesSightConnections();
        }
    }

    private void DrawNodesSightConnections()
    {
        Node[] nodes = (Node[])GameObject.FindObjectsOfType(typeof(Node));
        Dictionary<Node, List<Node>> nodesSightConnections = (GameObject.FindObjectOfType(typeof(NodeController)) as NodeController).NodeSightConnections;
        GL.Begin(GL.LINES);
        foreach (var node in nodes)
        {
            if (nodesSightConnections.ContainsKey(node))
            {
                foreach (var visibleNode in nodesSightConnections[node])
                {
                    GL.Vertex3(node.transform.position.x, node.transform.position.y, 0);
                    GL.Vertex3(visibleNode.transform.position.x, visibleNode.transform.position.y, 0);
                }
            }
        }
        GL.End();
    }
}
