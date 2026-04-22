using UnityEngine;


public class BounceMarker : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Bounds (keep marker on pitch)")]
    public float minZ = 5f;    
    public float maxZ = 16f;  
    public float minX = -1.5f; 
    public float maxX = 1.5f;  

    private bool inputEnabled = true;

    void Update()
    {
        if (!inputEnabled) return;

        float h = 0f, v = 0f;

        if (Input.GetKey(KeyCode.W)) v += 1f;
        if (Input.GetKey(KeyCode.S)) v -= 1f;
        if (Input.GetKey(KeyCode.D)) h += 1f;
        if (Input.GetKey(KeyCode.A)) h -= 1f;

        Vector3 move = new Vector3(h, 0f, v) * moveSpeed * Time.deltaTime;
        Vector3 newPos = transform.position + move;

        
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);
        newPos.y = 0.01f; 

        transform.position = newPos;
    }

        public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }

    public Vector3 GetMarkerPosition() => transform.position;
}
