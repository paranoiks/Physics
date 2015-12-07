using UnityEngine;
using System.Collections;

/// <summary>
/// A class to handle all user input
/// Give this class to every object that needs to be user controlled
/// </summary>
public class UserController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
    private void HandleKeyboard()
    {
        PhysicsHandler physicsComponent = gameObject.GetComponent<PhysicsHandler>();
                

        if (Input.GetKeyDown(KeyCode.Space))
        {
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

        float horizontalForceModifier = 0;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            horizontalForceModifier = 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalForceModifier = -1;
        }

        if (physicsComponent != null)
        {
            physicsComponent.AddForce(Vector2.right, physicsComponent.MovePower * horizontalForceModifier);
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleKeyboard();        
    }   

    /// <summary>
    /// this function is very expensive, to be used only in debug
    /// it is called from the cameraDrawer script
    /// </summary>
    public void DrawNodesInSight()
    {
        Node[] nodes = (Node[])GameObject.FindObjectsOfType(typeof(Node));
        GL.Begin(GL.LINES);
        foreach(var node in nodes)
        {
            Vector2 point1 = PhysicsHelpers.GetVector2(gameObject.transform.position);
            Vector2 point2 = PhysicsHelpers.GetVector2(node.gameObject.transform.position);
            bool flag = false;
            Platform[] platforms = (Platform[])GameObject.FindObjectsOfType(typeof(Platform));
            foreach(var platform in platforms)
            {
                if(PhysicsHelpers.RayHit(point1, point2, platform.gameObject.GetComponent<Shape>()))
                {
                    flag = true;
                }
            }
            if(flag == false)
            {
                GL.Vertex3(point1.x, point1.y, 0);
                GL.Vertex3(point2.x, point2.y, 0);
            }
        }
        GL.End();
    }
}
