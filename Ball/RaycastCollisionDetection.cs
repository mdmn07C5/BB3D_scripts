using UnityEngine;

//NOTE: only works well on primitive objects like sphere and cube
//      Currently, only detects one thing the obj has collided with
//     May added multiple collision detection in teh future

/// <summary>
/// This script handles different raycasting detection types and the direction it will be casting in
/// Returns and stores a transform it hits
/// Use this as an object/variable in other monobehaviour scripts
/// </summary>

[System.Serializable]
public class RaycastCollisionDetection
{
    public delegate void CollisionHandler(GameObject target);
    /// <summary>
    /// When the raycast hits something, it will pass the target it hits
    /// </summary>
    public event CollisionHandler onHit;

    /// <summary>
    /// enum to choose what direction to shoot the raycast
    /// </summary>
    public enum Direction
    {
        down,
        up,
        left,
        right
    }

    /// <summary>
    /// what type of ray cast to work with
    /// </summary>
    public enum RayCastType
    {
        rayCast,
        sphereCast,
        overlapSphere
    }

    /// <summary>
    /// contains data on direction of ray cast and max distance of raycast
    /// </summary>
    private class DirAndMaxDist
    {
        public Vector3 dir;
        public float maxDist;
        public DirAndMaxDist()
        {
            Vector3 dir = Vector3.zero;
            maxDist = 0;
        }
    }

    #region Unity Inspector Fields

    [Tooltip("Origin of raycast")]
    [SerializeField]
    private GameObject origin;

    [Tooltip("What direction to cast the ray/detection")]
    [SerializeField]
    private Direction direction;
    
    [Tooltip("Unity's raycast types")]
    [SerializeField]
    private RayCastType rayCastType;

    [Tooltip("Set your layermask to the thing you are trying to detect")]
    [SerializeField]
    private LayerMask layerMask; //what layer should the ray cast detect

    [Tooltip("used for sphereCast and overlapSphere")]
    [SerializeField]
    private float radius = 1; 

    [Tooltip("normal dist... dirAndMaxDist.maxDist * 1 (there will be inaccuracy because of frames)")]
    [SerializeField]
    private float rayCastDistMultiplyer = 1; 

    #endregion

    /// <summary>
    /// The gameobject that this raycast has detected/collided with
    /// </summary>
    [HideInInspector]
    public GameObject collidedObject; //what object the ray cast is currently colliding with

    /// <summary>
    /// has the obj collided with anything?
    /// </summary>
    [HideInInspector]
    public bool hasCollided; 

    private DirAndMaxDist dirAndMaxDist; //data containing max distance and direction of raycast
    private float rayDist; // ray cast distance value

    /// <summary>
    /// Calcualtes the raycast dir and dist
    /// </summary>
    public void Init()
    {
        CalculateDirAndMaxDist(); //getting direction of raycast and max distance of raycast
        rayDist = dirAndMaxDist.maxDist * rayCastDistMultiplyer; // get initial distance the raycast should shoot
    }

    /// <summary>
    /// returns the gameobject that this has collided with
    /// </summary>
    /// <returns></returns>
    public GameObject GetCollidedObject() {
        return collidedObject;
    }

    /// <summary>
    /// Our main raycast function, casts the desired raycast type at the origin
    /// call this function to do main update loop (fixed update)
    /// </summary>
    /// <param name="origin"></param>
    public void CastRayCast(GameObject origin = null) {

        if (origin != null) {
            this.origin = origin;
        }

        RaycastHit hit;

        rayDist = dirAndMaxDist.maxDist * rayCastDistMultiplyer; // get what distance the raycast should shoot

        if (rayCastType == RayCastType.rayCast)
        {
            if (Physics.Raycast(this.origin.transform.position, dirAndMaxDist.dir, out hit, rayDist, layerMask)) // if hit
            {
                GetCollisionObject(hit); //get collision object
            }
            else
            {
                // if nothing hits
                collidedObject = null;
                hasCollided = false;
            }
        } //NEED TO improve this, add variables for origin and radius and such 
        else if (rayCastType == RayCastType.sphereCast)
        {
            if (Physics.SphereCast(this.origin.transform.position, radius, Vector3.down, out hit, rayDist, layerMask))
            {
                GetCollisionObject(hit);
            }
            else
            {
                collidedObject = null;
                hasCollided = false;
            }
        }
        else if (rayCastType == RayCastType.overlapSphere)
        {
            Collider[] hitColliders = Physics.OverlapSphere(this.origin.transform.position, radius, layerMask);
            GetHighestCollisionObject(hitColliders); // get the first thing this object hits. calculated by getting the highest thing the obj collides with
        }
    }

    /// <summary>
    /// get the first thing this object hits. calculated by getting the highest thing the obj collides with
    /// </summary>
    /// <param name="hitColliders">All the colliders that this raycast has hit</param>
    public void GetHighestCollisionObject(Collider[] hitColliders)
    {
        int length = hitColliders.Length;
        if (length > 0) // if obj collides with something, get the higesht thing the obj collided with
        {
            int i = 0;
            GameObject highestHit = hitColliders[0].gameObject;
            for (i = 0; i < length; i++)
            {
                // Debug.Log(hitColliders[i].gameObject);
                if (hitColliders[i].transform.position.y > highestHit.transform.position.y)
                {
                    highestHit = hitColliders[i].gameObject;
                }
            }
            collidedObject = highestHit;
            if (onHit != null)
                onHit(collidedObject);
            hasCollided = true;
        }
        else
        { //no collision detected
            collidedObject = null;
            hasCollided = false;
        }
    }

    /// <summary>
    /// get the thing obj collided with
    /// </summary>
    /// <param name="hit"></param>
    /// <returns></returns>
    public GameObject GetCollisionObject(RaycastHit hit)
    {
        // Debug.Log(hit.transform.gameObject.name);
        collidedObject = hit.transform.gameObject;
        hasCollided = true;
        return collidedObject;
    }

    /// <summary>
    /// calculating direction of ray cast and max direction based on the direction of the raycast chosen
    /// </summary>
    private void CalculateDirAndMaxDist()
    {
        dirAndMaxDist = new DirAndMaxDist();
        Vector3 dir = Vector3.zero;
        float maxDist = 0;

        if (direction == Direction.up)
        {
            dir = Vector3.up;
            maxDist = origin.transform.localScale.y / 2f; // divides by 2, because localScale.y/2 + middle of object = the outmost area of the obj
        }
        else if (direction == Direction.down)
        {
            dir = Vector3.down;
            maxDist = origin.transform.localScale.y / 2f;
        }
        else if (direction == Direction.left)
        {
            dir = Vector3.left;
            maxDist = origin.transform.localScale.x / 2f;
        }
        else
        {
            dir = Vector3.right;
            maxDist = origin.transform.localScale.x / 2f;
        }

        dirAndMaxDist.dir = dir;
        dirAndMaxDist.maxDist = maxDist;
    }

}

