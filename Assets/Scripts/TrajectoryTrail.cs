using UnityEngine;


[RequireComponent(typeof(TrailRenderer))]
public class TrajectoryTrail : MonoBehaviour
{
    private TrailRenderer trail;

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        trail.time = 2f;             
        trail.startWidth = 0.05f;
        trail.endWidth   = 0f;

       
        Gradient g = new Gradient();
        g.SetKeys(
            new GradientColorKey[]  { new GradientColorKey(Color.white, 0f),
                                      new GradientColorKey(Color.white, 1f) },
            new GradientAlphaKey[]  { new GradientAlphaKey(1f, 0f),
                                      new GradientAlphaKey(0f, 1f) }
        );
        trail.colorGradient = g;

       
        trail.material = new Material(Shader.Find("Sprites/Default"));
    }

    
    public void ClearTrail()
    {
        trail.Clear();
    }
}
