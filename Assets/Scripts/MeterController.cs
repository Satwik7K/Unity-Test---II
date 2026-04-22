using UnityEngine;
using UnityEngine.UI;


public class MeterController : MonoBehaviour
{
    [Header("Meter UI References")]
    public RectTransform indicatorLine;  
    public float meterHeight = 300f;      

    [Header("Speed")]
    public float oscillateSpeed = 1.5f;   

   
    private float meterT = 0f;
    private float direction = 1f;
    private bool isRunning = false;

    private float sampledStrength = 0f;

    void Update()
    {
        if (!isRunning) return;

        meterT += direction * oscillateSpeed * Time.deltaTime;

        if (meterT >= 1f) { meterT = 1f; direction = -1f; }
        if (meterT <= 0f) { meterT = 0f; direction =  1f; }

        UpdateIndicatorPosition();
    }

    void UpdateIndicatorPosition()
    {
        if (indicatorLine == null) return;

        float halfH = meterHeight * 0.5f;
        float yPos  = Mathf.Lerp(halfH, -halfH, meterT);
        indicatorLine.anchoredPosition = new Vector2(indicatorLine.anchoredPosition.x, yPos);
    }

    public void StartMeter()
    {
        meterT    = 0f;
        direction = 1f;
        isRunning = true;
    }


    public float StopAndSample()
    {
        isRunning = false;

        
        float distFromCenter = Mathf.Abs(meterT - 0.5f) * 2f; 
        sampledStrength = 1f - distFromCenter;                

        return sampledStrength;
    }

    public float GetLastSampledStrength() => sampledStrength;
}
