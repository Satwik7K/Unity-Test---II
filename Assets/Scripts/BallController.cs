using UnityEngine;

public enum DeliveryType { Swing, Spin }
public enum SwingDirection { Left, Right }
public enum SpinType { OffSpin, LegSpin }

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    [Header("Delivery Settings")]
    public DeliveryType deliveryType = DeliveryType.Swing;
    public SwingDirection swingDirection = SwingDirection.Left;
    public SpinType spinType = SpinType.OffSpin;

    [Header("Ball Speed")]
    public float ballSpeed = 18f;

    [Header("Swing Settings (Phase I)")]
    [Range(0f, 1f)]
    public float swingStrength = 0.5f;  
    public float maxSwingForce = 6f;    

    [Header("Spin Settings (Phase II)")]
    [Range(0f, 1f)]
    public float spinStrength = 0.5f;  
    public float maxSpinDeflection = 4f; 

    [Header("Bounce Settings")]
    public float bounceRetainSpeed = 0.55f; 


    private Rigidbody rb;
    private bool hasBounced = false;
    private bool isLaunched  = false;

    private Vector3 launchPosition;
    private float   totalForwardDistance;   
    private Vector3 postBounceDirection;    

    
public void Launch(Vector3 bounceTarget, float swingPct, float spinPct)
{
    swingStrength = swingPct;
    spinStrength  = spinPct;

    hasBounced = false;
    isLaunched = true;
    launchPosition = transform.position;

    Vector3 flatDelta = bounceTarget - launchPosition;
    flatDelta.y = 0f;
    totalForwardDistance = flatDelta.magnitude;

   
    float distance = totalForwardDistance;
    float horizontalSpeed = ballSpeed;          

    
    float timeToTarget = distance / horizontalSpeed;

    
    float g = Mathf.Abs(Physics.gravity.y);
    float startY = launchPosition.y;
    float targetY = bounceTarget.y;             
    float Vy = (targetY - startY + 0.5f * g * timeToTarget * timeToTarget) / timeToTarget;

    
    Vector3 flatDir = flatDelta.normalized;
    Vector3 launchVel = flatDir * horizontalSpeed;
    launchVel.y = Vy;

    rb.isKinematic = false;
    rb.linearVelocity = launchVel;
}

    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; 
    }

    void FixedUpdate()
    {
        if (!isLaunched) return;

        if (!hasBounced && deliveryType == DeliveryType.Swing)
            ApplySwingForce();

        if (hasBounced)
            LockPostBounceDirection();
    }

    
    void ApplySwingForce()
    {
        if (totalForwardDistance <= 0f) return;

       
        Vector3 flatTravelled = transform.position - launchPosition;
        flatTravelled.y = 0f;
        float progress = Mathf.Clamp01(flatTravelled.magnitude / totalForwardDistance);

       
        float curve = progress * progress;

        float side = (swingDirection == SwingDirection.Left) ? -1f : 1f;
        float force = side * curve * swingStrength * maxSwingForce;

        rb.AddForce(Vector3.right * force, ForceMode.Acceleration);
    }

    
    void OnCollisionEnter(Collision collision)
    {
        if (hasBounced) return;
        if (!collision.gameObject.CompareTag("Ground")) return;

        hasBounced = true;

       
        Vector3 vel = rb.linearVelocity;

        
        vel.y = Mathf.Abs(vel.y) * bounceRetainSpeed;

        if (deliveryType == DeliveryType.Spin)
        {
            float side = (spinType == SpinType.OffSpin) ? 1f : -1f;
            float deflection = side * spinStrength * maxSpinDeflection;
            vel += Vector3.right * deflection;
        }

        rb.linearVelocity = vel;

       
        postBounceDirection = vel.normalized;
    }

  
    void LockPostBounceDirection()
    {
        
        float speed = rb.linearVelocity.magnitude;
        if (speed < 0.1f) return;

        
        if (rb.linearVelocity.y < -0.5f) return;

        Vector3 flatDir = postBounceDirection;
        flatDir.y = 0f;
        if (flatDir.sqrMagnitude < 0.01f) return;

        Vector3 currentFlat = rb.linearVelocity;
        currentFlat.y = 0f;

       
        Vector3 corrected = Vector3.MoveTowards(currentFlat, flatDir.normalized * currentFlat.magnitude,
                                                 Time.fixedDeltaTime * 5f);
        rb.linearVelocity = new Vector3(corrected.x, rb.linearVelocity.y, corrected.z);
    }

public void ResetBall(Vector3 startPos)
{   
    rb.isKinematic = false;
    rb.linearVelocity  = Vector3.zero;   
    rb.angularVelocity = Vector3.zero;   
    rb.isKinematic = true;               
    transform.position = startPos;
    hasBounced = false;
    isLaunched = false;
}
}
