using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeController : MonoBehaviour {

    public Dictionary<Node, List<Node>> NodeSightConnections { get; set; }

    private Platform[] Platforms;

	// Use this for initialization
	void Start () {
        Platforms = (Platform[])GameObject.FindObjectsOfType(typeof(Platform));
        NodeSightConnections = new Dictionary<Node, List<Node>>();
        StartCoroutine(NodecalculatorCoroutine());
	}

    private IEnumerator NodecalculatorCoroutine()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("StartNodesStuff");
        CreateNodeSightConnections();
        Debug.Log("FinishNodesStuff");
    }

    private void CreateNodeSightConnections()
    {
        Node[] nodes = (Node[])GameObject.FindObjectsOfType(typeof(Node));
        foreach(var node in nodes)
        {
            foreach(var node2 in nodes)
            {
                if(node == node2)
                {
                    continue;
                }
                Vector2 point1 = PhysicsHelpers.GetVector2(node.transform.position);
                Vector2 point2 = PhysicsHelpers.GetVector2(node2.transform.position);
                bool flag = false;
                foreach(var platform in Platforms)
                {
                    if(PhysicsHelpers.RayHit(point1, point2, platform.gameObject.GetComponent<Shape>()))
                    {
                        flag = true;
                    }
                }
                if(flag == false)
                {
                    if(NodeSightConnections.ContainsKey(node))
                    {
                        NodeSightConnections[node].Add(node2);
                    }
                    else
                    {
                        NodeSightConnections.Add(node, new List<Node>());
                        NodeSightConnections[node].Add(node2);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
