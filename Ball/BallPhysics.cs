using UnityEngine;

/// <summary>
/// Contains the final movement vector and gravity, and apply those to a transform
/// Use this as an object/variable in other monobehaviour scrip
/// </summary>

[System.Serializable]
public class BallPhysics
{
    #region Unity Inspector Fields

    [Tooltip("Gravity will be calculated during runtime if BallBounce is used in the controller script, if not you can set it here")]
    public float gravity = -9.8f;

    #endregion

    /// <summary>
    /// our final velocity vector to be applied to the transform 
    /// </summary>
    [HideInInspector]
    public Vector3 velocity; 

    /// <summary>
    /// is the ball moving up?
    /// </summary>
    [HideInInspector]
    public bool IsGoingUp { get { return velocity.y > 0; } } 

    /// <summary>
    /// Calculates the gravity that is needed to allow the ball to get to a bounceHeight in timeToApex
    /// </summary>
    /// <param name="bounceHeight">How high can the ball bounce</param>
    /// <param name="timeToApex">How long it takes to get to bounceHeight</param>
    public void CalculateGravity(float bounceHeight, float timeToApex)
    {
        //Kinematics equations, gravity needs to be calculated based on the specified jump height and time it takes to get there 
        gravity = -(2 * bounceHeight) / Mathf.Pow(timeToApex, 2);
    }

    
     /* Since we are applying gravity constantly with our own physics, 
        we want to check if raycast hit anything, then we set the velocity.y back to 0, 
        so our ball don't fall through the platform. 
        We don't want to set velocity.y to 0 when bouncing, or else the ball won't bounce */
    /// <summary>
    /// 
    /// </summary>
    /// <param name="raycastCollisionDetection">The collision detection we are using</param>
   public void ApplyVerticalCollision(RaycastCollisionDetection raycastCollisionDetection)
    {
        if (raycastCollisionDetection.hasCollided && IsGoingUp == false)
        {
            velocity.y = 0;
        }
    }

    /// <summary>
    /// Takes in a transform and applies a movement vector to the translation
    /// </summary>
    /// <param name="transform">The transform we are moving</param>
    public void ApplyPhysics(Transform transform)
    {
        velocity.y += gravity * Time.fixedDeltaTime; // Our own constant gravity being applied
        transform.Translate(velocity * Time.fixedDeltaTime); //Our own simulation of velocity physics with translation
    }

    /// <summary>
    /// Resets the velocity of the ball to a zero vector, should call when game is lost/won
    /// </summary>
    public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }
}
