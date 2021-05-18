using UnityEngine;

/// <summary>
/// Contains the settings and functionaility to bouncing, can be treated as a jump.
/// Use this as an object/variable in other monobehaviour scripts
/// </summary>

[System.Serializable]
public class BallBounce
{
    #region Unity Inspector Fields

    [Tooltip("How high the ball should bounce")]
    public float bounceHeight = 10f;

    [Tooltip("How long should it take for the ball to get to bounceHeight")]
    public float timeToApex = 0.3f;

    #endregion

    [HideInInspector]
    public float bounceVelocity; //Jump Velocity is being calculated on start with gravity and timeToApex

    /// <summary>
    /// Initializes the bounceVelovity by calculating it with the gravity
    /// </summary>
    /// <param name="gravity">The gravity needed to calculate the bounceVelocity</param>
    public void Init(float gravity)
    {
        ////Next jump velocity is calculated based on the previously calculated gravity
        bounceVelocity = Mathf.Abs(gravity) * timeToApex;
    }

    /// <summary>
    /// Sets the movement vector's y to bounceVelocity
    /// </summary>
    /// <param name="velocity">The movement vector</param>
    public void Bounce(ref Vector3 velocity)
    {
        velocity.y = bounceVelocity;
    }

}
