using UnityEngine;

/// <summary>
/// Handles player input and moves the target
/// </summary>

public class PlayerInput : MonoBehaviour
{
    #region Unity Inspector Fields

    [Tooltip("move speed of the target we are moving based on input")]
    [SerializeField]
    private float moveSpeed;

    #endregion

    /// <summary>
    /// The target transform we are moving
    /// </summary>
    private Transform target;

    /// <summary>
    /// The position of input during a previous frame
    /// Used for calculating the moveAmount vector
    /// </summary>
    private Vector2 previousInputPos; 

    /// <summary>
    /// The position of input during a frame
    /// Used for calculating the moveAmount vector
    /// </summary>
    private Vector2 currentInputPos; 

    /// <summary>
    /// The move vector to apply to the target
    /// </summary>
    private Vector2 moveAmount;

    /// <summary>
    /// for disabling the input
    /// </summary>
    private bool disabled;

    #region Unity Callbacks

    private void Start()
    {
        //Disable our input when game stars
        DisableInput();
    }

    private void Update()
    {
        if (disabled)
        {
            moveAmount = Vector2.zero;
            return;
        }

        //On Input down we will set our current and prev position to input pos
        if(Input.GetMouseButtonDown(0))
        {
            currentInputPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, 
                                          Camera.main.ScreenToWorldPoint(Input.mousePosition).z);
            
            previousInputPos = currentInputPos;
        }

        //On Input we will calculate our move vector
        if (Input.GetMouseButton(0))
        {
            CalculateMoveVector();
        }

        //On Input up we will set move vector to the zero vector
        if (Input.GetMouseButtonUp(0))
        {
            moveAmount = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        //set our move vector based on move amounr
        Vector3 targetMoveVector = new Vector3(moveAmount.x, 0, moveAmount.y * 2);

        //move our target
        target.Translate(targetMoveVector * Time.deltaTime * moveSpeed);
    }

    #endregion

    /// <summary>
    /// Calculate the move vector based on previous and current input position, prev - current
    /// </summary>
    private void CalculateMoveVector()
    {
        //get our current input pos
        currentInputPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                                          Camera.main.ScreenToWorldPoint(Input.mousePosition).z);

        //if we moved we will get that directional vector of prev - curr and get the magnitude of that 
        //then we will get the normalized dir and multiply it by the magnitude
        if (currentInputPos != previousInputPos)
        {
            Vector2 temp = currentInputPos - previousInputPos;
            float magnitude = temp.magnitude;
            moveAmount = (currentInputPos - previousInputPos).normalized * magnitude;
            previousInputPos = currentInputPos;
        }
        else
        {
            moveAmount = Vector2.zero;
        }
    }

    /// <summary>
    /// Disables input, call when level is won/lost
    /// </summary>
    public void DisableInput()
    {
        disabled = true;
    }
    
    /// <summary>
    /// Disables input, call when level is won/lost
    /// </summary>
    public void EnableInput()
    {
        disabled = false;
    }

    /// <summary>
    /// Sets the target to move
    /// </summary>
    /// <param name="target">target to move</param>
    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
