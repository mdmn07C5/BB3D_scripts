using UnityEngine;

/// <summary>
/// Contains the ball's functionality and behaviours, attached onto the ball/player gameobject
/// </summary>
public class BallController : MonoBehaviour
{
    #region Unity Inspector Fields
    
    [Tooltip("A Collision detection system that returns the target based on different types of ray casting detection")]
    [SerializeField]
    private RaycastCollisionDetection collisionDetection;

    [Tooltip("Contains bounce variables and functions")]
    [SerializeField]
    private BallBounce ballBounce;

    [Tooltip("Contains the physics variables and final velocity vector, which is used to move by translating")]
    [SerializeField]
    private BallPhysics ballPhysics;

    [Header("[Events to Raise]")]
    [Tooltip("When the ball hits a non victory platform, this event will be raised")]
    [SerializeField]
    private GameEvent onNormalPlatformImpact;

    [SerializeField]
    [Tooltip("When the ball hits any platform, this event will be raised")]
    private GameEvent onPlatformImpact;

    [Header("[Particles]")]
    [Tooltip("The particle system to spawn when player dies")]
    [SerializeField]
    private GameObject deathFX;

    [Header("[Reset Variable]")]
    [Tooltip("y position where the ball reset after each level")]
    [SerializeField]
    private Vector3 resetPosition = new Vector3(0, 4, 0); 

    #endregion

    private Transform ground; //The ground we detect with raycast
    private GameObject groundBallLastTouched; //The ground the ball has previously touched, used for combo system
    private bool disabled = true; //Disabling the whole movement and functionaility of the ball

    #region Unity Callbacks
    private void Awake()
    {
        //Disable the ball first when the game starts, we will enable it again when the platforms have generated
        disabled = true;
    }

    private void Start()
    {
        collisionDetection.Init();

        ////Kinematics equations, gravity needs to be calculated based on the specified jump height and time it takes to get there 
        ballPhysics.CalculateGravity(ballBounce.bounceHeight, ballBounce.timeToApex);

        //////Next jump velocity is calculated based on the previously calculated gravity
        ballBounce.Init(ballPhysics.gravity);
    }

    /// <summary>
    /// We are detecting the ground and calculating offset variables for our stretch and squash function
    /// 
    /// The original Stretch and Squash function has a hardcoded ground position of 0 at Y axis, 
    /// so we need an offset for our ground to do the right calculations, because our ground is not always at 0.
    /// </summary>
    private void FixedUpdate()
    {
        if (disabled)
            return;

        collisionDetection.CastRayCast();

        /* Since we are applying gravity constantly with our own physics, 
         * we want to check if raycast hit anything, then we set the velocity.y back to 0, 
           so our ball don't fall through the platform. 
           We don't want to set velocity.y to 0 when bouncing, or else the ball won't bounce */
        ballPhysics.ApplyVerticalCollision(collisionDetection);

        if (collisionDetection.collidedObject != null) // If our raycast system detects something
        {
            ground = collisionDetection.collidedObject.transform; // Set the ground variable to the detected gameobject 
            if (ground != null) {
                groundBallLastTouched = ground.gameObject;
            }
        }

        // If raycast hits something, we want the ball to bounce up 
        if (collisionDetection.hasCollided)
        {
            //bounce our ball
            ballBounce.Bounce(ref ballPhysics.velocity);

            //Phone haptic feedback
            HapticFeedback.Generate(UIFeedbackType.ImpactMedium);
            
            //if the platform we're hitting is a brick
            if (ground.GetComponent<PlatformController>() != null)
            {
                //Jiggle the brick
                ground.GetComponent<PlatformController>().CallJiggle();

                if (ground.GetComponent<WinLevel>())
                    ground.GetComponent<WinLevel>().TryWinGame();

                //if not we will raise the events that we need to
                if (ground.GetComponent<VictoryTag>() == null)
                {
                    onNormalPlatformImpact.Raise();
                    onPlatformImpact.Raise();
                }
            }
        }
        //ApplyPhysics recieves a transform and applies the final velocity vector which moves the ball
        ballPhysics.ApplyPhysics(transform);
    }
    #endregion

    /// <summary>
    /// Resets the position of the ball to the value of resetPosition in the inspector,
    /// should be called when a level is won/lost
    /// </summary>
    public void ResetPosition()
    {
        transform.position = resetPosition;
        ballPhysics.ResetVelocity();
    }

    /// <summary>
    /// Disables the ball's movement and functionalities, should be called after ResetPosition
    /// </summary>
    public void DisableBall()
    {
        disabled = true;
    }

    /// <summary>
    /// Enables the ball's movement and functionalities, should be called after the level is done loading
    /// </summary>
    public void EnableBall()
    {
        disabled = false;
    }

    /// <summary>
    /// Spawns a death particle fx
    /// </summary>
    public void SpawnDeathFX()
    {
        GameObject obj = Instantiate(deathFX, transform.position, Quaternion.identity);
        Destroy(obj, .75f);
    }
}