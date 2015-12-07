using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinder : MonoBehaviour {

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update () {
	
	}

    public List<Node> FindPath(Node start, Node goal, float handicap)
    {
        List<Node> ClosedSet = new List<Node>();
        List<Node> OpenSet = new List<Node>();
        OpenSet.Add(start);

        Dictionary<Node, Node> CameFrom = new Dictionary<Node, Node>();

        Dictionary<Node, float> GScore = new Dictionary<Node, float>();
        GScore.Add(start, 0);

        Dictionary<Node, float> FScore = new Dictionary<Node, float>();
        FScore.Add(start, GScore[start] + DistanceBetweenNodes(start, goal));

        while(OpenSet.Count > 0)
        {
            Node currentNode = GetLowestFScore(OpenSet, FScore);
            if(currentNode == goal)
            {
                //Debug.Log("PATH FOUND");
                return ReconstructPath(CameFrom, goal);
            }

            OpenSet.Remove(currentNode);
            ClosedSet.Add(currentNode);

            foreach(var neighbour in currentNode.NeighboursP)
            {
                if (neighbour == null)
                {
                    continue;
                }
                Node neighbourNode = neighbour.GetComponent<Node>();
                if(ClosedSet.Contains(neighbourNode))
                {
                    continue;
                }

                float r = Random.Range(0, handicap);
                float currentScore = GScore[currentNode] + DistanceBetweenNodes(currentNode, neighbourNode) + r;
                
                if(!OpenSet.Contains(neighbourNode))
                {
                    OpenSet.Add(neighbourNode);
                }
                else if(currentScore >= GScore[neighbourNode])
                {
                    continue;
                }

                if (CameFrom.ContainsKey(neighbourNode))
                {
                    CameFrom[neighbourNode] = currentNode;
                    GScore[neighbourNode] = currentScore;
                    FScore[neighbourNode] = GScore[neighbourNode] + DistanceBetweenNodes(neighbourNode, goal) + r;
                }
                else
                {
                    CameFrom.Add(neighbourNode, currentNode);
                    GScore.Add(neighbourNode, currentScore);
                    FScore.Add(neighbourNode, GScore[neighbourNode] + DistanceBetweenNodes(neighbourNode, goal) + r);
                }
            }
        }

        return null;
    }

    private List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node goal)
    {
        List<Node> totalPath = new List<Node>();
        totalPath.Add(goal);
        Node c = goal;
        while(cameFrom.ContainsKey(c))
        {
            c = cameFrom[c];
            totalPath.Add(c);
        }
        return totalPath;
    }

    /// <summary>
    /// Return the straight line distance between the two nodes
    /// </summary>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    /// <returns></returns>
    private float DistanceBetweenNodes(Node n1, Node n2)
    {
        if(n1.gameObject.GetComponent<Portal>() != null && n2.gameObject.GetComponent<Portal>() != null)
        {
            return 0;
        }
        return Vector3.Distance(n1.transform.position, n2.transform.position);       
    }

    private Node GetLowestFScore(List<Node> openSet, Dictionary<Node, float> FScore)
    {
        Node minNode = null;
        float minScore = float.MaxValue;
        foreach (var n in openSet)
        {
            if(FScore.ContainsKey(n))
            {
                if(FScore[n] < minScore)
                {
                    minNode = n;
                    minScore = FScore[n];
                }
            }
        }
        return minNode;
    }
}