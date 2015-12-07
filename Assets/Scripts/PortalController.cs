using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PortalController : MonoBehaviour {

    [SerializeField]
    private GameObject PortalPrefab;

    private List<GameObject> Portals;

    private float NodeDistanceTreshold = 4;

    private SprintBotController SprintBot;
    private EnforcerBotController EnforcerBot;
    private Node[] Nodes;
    private PhysicsHelpers PhysicsHelpersScript;

    private float PortalsTTL = -1;
    private float PortalsTTLMax = 2;

	// Use this for initialization
	void Start () {
        Portals = new List<GameObject>();
        SprintBot = (SprintBotController)GameObject.FindObjectOfType(typeof(SprintBotController));
        EnforcerBot = (EnforcerBotController)GameObject.FindObjectOfType(typeof(EnforcerBotController));
        Nodes = (Node[])GameObject.FindObjectsOfType(typeof(Node));
        PhysicsHelpersScript = (PhysicsHelpers)GameObject.FindObjectOfType(typeof(PhysicsHelpers));
	}

    private void SpawnPortal(Vector3 position)
    {
        if(Portals.Count == 2)
        {
            GameObject portalToRemove = Portals[0];
            Portals.RemoveAt(0);
            Destroy(portalToRemove);
        }
        GameObject currentPortal = Instantiate(PortalPrefab, position, Quaternion.identity) as GameObject;
        Portals.Add(currentPortal);
        if(Portals.Count == 2)
        {
            currentPortal.GetComponent<Portal>().OtherPortal = Portals[0];
            Portals[0].GetComponent<Portal>().OtherPortal = currentPortal;
            currentPortal.GetComponent<Node>().AddNeighbour(Portals[0]);
            Portals[0].GetComponent<Node>().AddNeighbour(currentPortal);
            SprintBot.PortalsChanged();
        }

        foreach(var node in Nodes)
        {
            if(node == null)
            {
                continue;
            }
            float distanceToNode = PhysicsHelpers.DistanceBetweenObjects(currentPortal, node.gameObject);
            if(distanceToNode < NodeDistanceTreshold)
            {
                if (PhysicsHelpersScript.CheckVisibility(node.gameObject.transform.position, currentPortal.transform.position))
                {
                    node.GetComponent<Node>().AddNeighbour(currentPortal);
                    currentPortal.GetComponent<Node>().AddNeighbour(node.gameObject);
                }
            }
        }
    }

    public void SpawnPortalsPair(Vector3 point1, Vector3 point2)
    {
        Vector3 spawningOffset = new Vector3(0, 0.5f, 0);
        SpawnPortal(point1 + spawningOffset);
        SpawnPortal(point2 + spawningOffset);
        PortalsTTL = PortalsTTLMax;  
    }

    /// <summary>
    /// When a portal has been used remove all portals 
    /// </summary>
    public void PortalUsed()
    {
        for (int i = 0; i < Portals.Count; i++)
        {
            if (Portals[i] != null)
            {
                Destroy(Portals[i]);
            }
        }
        Portals.Clear();
        SprintBot.PortalsChanged();
    }

    /// <summary>
    /// Handle the mouse input
    /// </summary>
    private void HandleMouse()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            mousePosition.z = 0;
            SpawnPortal(mousePosition);
        }
    }
	
	// Update is called once per frame
	void Update () {
        HandleMouse();
        HandlePortalsTTL();
	}

    private void HandlePortalsTTL()
    {
        if (PortalsTTL >= 0)
        {
            PortalsTTL -= Time.deltaTime;
            if (PortalsTTL <= 0)
            {
                foreach (var portal in Portals)
                {
                    Destroy(portal);
                }
                Portals.Clear();
            }
        }
    }
}
