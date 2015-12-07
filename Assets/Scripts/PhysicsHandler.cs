using UnityEngine;
using System.Collections;

public class PhysicsHandler : MonoBehaviour
{
    [SerializeField]
    private float DragK;

    [SerializeField]
    private float Mass = 1;

    private Vector2 Velocity;

    private Vector2 Force;

    private Vector2 Acceleration;

    [SerializeField]
    private float _JumpPower;

    public float JumpPower
    {
        get
        {
            return _JumpPower;
        }
    }

    [SerializeField]
    private float _MovePower;

    public float MovePower
    {
        get
        {
            return _MovePower;
        }
    }

    private bool _OnGround = false;

    public bool OnGround
    {
        get
        {
            return _OnGround;
        }
        set
        {
            _OnGround = value;
        }
    }

    private float PortalTimer = 0;
    private float PortalTimerMax = 1;
    
	// Use this for initialization
	void Start ()
    {
        Velocity = Vector2.zero;
	}  

    /// <summary>
    /// Apply the velocity
    /// </summary>
    private void HandleMovement()
    {
        Vector3 position = transform.position;        
        position += new Vector3(Velocity.x, Velocity.y, 0);
        transform.position = position;
    }

    /// <summary>
    /// Resolve all the forces currently acting upon the object
    /// </summary>
    private void ResolveForces()
    {
        Vector2 drag = DragK * Velocity.sqrMagnitude * -(Velocity.normalized);
        Force.x += drag.x;
        Acceleration = Force / Mass;        
        Velocity += Acceleration * Time.fixedDeltaTime;
        Force = Vector2.zero;
    }

    private void HandleGravity()
    {
        Force += Vector2.down * PhysicsHelpers.GravityConst * Time.fixedDeltaTime;
    }

    private void HandleTimers()
    {
        if (PortalTimer > 0)
        {
            PortalTimer -= Time.fixedDeltaTime;
        }
    }
    
    private void FixedUpdate()
    {        
        HandleMovement();
        HandleGravity();
        ResolveForces();
        HandleTimers();
    }

    public void Hit(GameObject obj, Vector2 MTV)
    {
        if (obj.tag == "Solid" 
            && !obj.name.Contains("Collectible")
            && obj.GetComponent<Portal>() == null)
        {
            if (MTV.x != 0)
            {
                Velocity.x = 0;
            }
            if (MTV.y != 0)
            {
                if (MTV.y < 0)
                {
                    OnGround = true;
                }
                Velocity.y = 0;
            }
        }
        else if(obj.name.Contains("Collectible"))
        {
            if(gameObject.name == "Player")
            {
                GameObject.Find("GameController").GetComponent<GameController>().AddScore(true);
            }
            else
            {
                GameObject.Find("GameController").GetComponent<GameController>().AddScore(false);
            }
            GameObject.Find("GameController").GetComponent<GameController>().ResetCollectible();
        }
        //if the colliding object is a portal, teleport and destroy the pair of portals
        else if(obj.GetComponent<Portal>() != null)
        {
            if (PortalTimer <= 0)
            {
                Portal currentObjectPortal = obj.GetComponent<Portal>();
                if (currentObjectPortal.OtherPortal != null)
                {
                    transform.position = currentObjectPortal.OtherPortal.gameObject.transform.position;
                    PortalTimer = PortalTimerMax;
                    (GameObject.FindObjectOfType(typeof(PortalController)) as PortalController).PortalUsed();
                }
            }
        }
    }

    public void StopFallingDown()
    {
        Velocity.y = 0;
        _OnGround = true;
    }

    /// <summary>
    /// Add an impulse to the body. Useful for explosions, jumping etc
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="magnitude"></param>
    public void AddImpulse(Vector2 direction, float magnitude)
    {
        //we expect the direction vector to be normalised, but we normalise it again just to be sure
        direction.Normalize();
        Vector2 impulse = direction * magnitude;
        Velocity += impulse;
    }

    /// <summary>
    /// Add force to the body. Useful for constant forces like wind
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="magnitude"></param>
    public void AddForce(Vector2 direction, float magnitude)
    {
        direction.Normalize();
        Vector2 force = direction * magnitude;
        Force += force;
    }
}
