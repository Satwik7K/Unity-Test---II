using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BowlingUI : MonoBehaviour
{
    [Header("Scene References")]
    public BallController   ball;
    public BounceMarker     bounceMarker;
    public MeterController  meter;
    public Transform        bowlerPosition;  

    [Header("UI Buttons")]
    public Button swingButton;
    public Button spinButton;
    public Button bowlButton;

    [Header("UI Labels / Feedback")]
    public TextMeshProUGUI  deliveryLabel;   
    public TextMeshProUGUI  strengthLabel;   
    
    private DeliveryType  currentDelivery  = DeliveryType.Swing;
    private SwingDirection swingDir        = SwingDirection.Left;
    private SpinType      currentSpinType  = SpinType.OffSpin;
    private bool          awaitingBowl     = true;  

    void Start()
    {
       
        swingButton.onClick.AddListener(OnSwingSelected);
        spinButton.onClick.AddListener(OnSpinSelected);
        bowlButton.onClick.AddListener(OnBowlPressed);

       
        meter.StartMeter();
        UpdateLabel();
    }

   

    void OnSwingSelected()
    {
        currentDelivery = DeliveryType.Swing;
        ball.deliveryType = DeliveryType.Swing;
        UpdateLabel();
    }

    void OnSpinSelected()
    {
        currentDelivery = DeliveryType.Spin;
        ball.deliveryType = DeliveryType.Spin;
        UpdateLabel();
    }

    void OnChangeSide()
    {
        if (currentDelivery == DeliveryType.Swing)
        {
            swingDir = (swingDir == SwingDirection.Left)
                       ? SwingDirection.Right
                       : SwingDirection.Left;
            ball.swingDirection = swingDir;
        }
        else
        {
            currentSpinType = (currentSpinType == SpinType.OffSpin)
                              ? SpinType.LegSpin
                              : SpinType.OffSpin;
            ball.spinType = currentSpinType;
        }
        UpdateLabel();
    }

    void OnBowlPressed()
    {
        if (!awaitingBowl) return;

    
        float strength = meter.StopAndSample();

        if (strengthLabel != null)
            strengthLabel.text = $"Strength: {Mathf.RoundToInt(strength * 100f)}%";

        
        bounceMarker.SetInputEnabled(false);

        ball.ResetBall(bowlerPosition.position);

        float swingPct = (currentDelivery == DeliveryType.Swing) ? strength : 0f;
        float spinPct  = (currentDelivery == DeliveryType.Spin)  ? strength : 0f;

        ball.Launch(bounceMarker.GetMarkerPosition(), swingPct, spinPct);

        awaitingBowl = false;

       
        Invoke(nameof(ResetDelivery), 3f);
    }

    void ResetDelivery()
    {
        ball.ResetBall(bowlerPosition.position);
        bounceMarker.SetInputEnabled(true);
        meter.StartMeter();
        awaitingBowl = true;

        if (strengthLabel != null)
            strengthLabel.text = "";
    }

    
    void UpdateLabel()
    {
        if (deliveryLabel == null) return;

        if (currentDelivery == DeliveryType.Swing)
            deliveryLabel.text = $"SWING    {swingDir}";
        else
            deliveryLabel.text = $"SPIN    {currentSpinType}";
    }
}
