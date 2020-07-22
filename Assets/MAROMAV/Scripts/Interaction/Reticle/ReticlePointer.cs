using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// Draws a circular reticle in front of any object that the user points at.
/// The circle dilates if the object is clickable.
public class ReticlePointer : GvrBasePointer
{
    private bool shouldGazeAgain = false;

    public Image reticle;

    /// Minimum distance of the reticle (in meters).
    public const float RETICLE_DISTANCE_MIN = 0.45f;

    /// Maximum distance of the reticle (in meters).
    public float maxReticleDistance = 10.0f;

    /// Sorting order to use for the reticle's renderer.
    /// Range values come from https://docs.unity3d.com/ScriptReference/Renderer-sortingOrder.html.
    /// Default value 32767 ensures gaze reticle is always rendered on top.
    [Range(-32767, 32767)]
    public int reticleSortingOrder = 32767;

    // Current distance of the reticle (in meters).
    // Getter exposed for testing.
    public float ReticleDistanceInMeters { get; private set; }

    public override float MaxPointerDistance { get { return maxReticleDistance; } }

    public override void OnPointerEnter(RaycastResult raycastResultResult, bool isInteractive)
    {
        SetPointerTarget(raycastResultResult.worldPosition, isInteractive);
    }

    public override void OnPointerHover(RaycastResult raycastResultResult, bool isInteractive)
    {

        if (isInteractive && !shouldGazeAgain)
        {
            Button interactiveButton = GetButtonFromRaycastResult(raycastResultResult.gameObject);
            if (interactiveButton != null)
            {
                reticle.fillAmount -= Time.deltaTime;
                if (reticle.fillAmount <= 0f)
                {
                    interactiveButton.OnSubmit(null);
                    shouldGazeAgain = true;
                    reticle.fillAmount = 1f;
                }
            }
        }
    }

    private Button GetButtonFromRaycastResult(GameObject raycastResult)
    {
        Button button;
        button = raycastResult.GetComponent<Button>();
        if (button == null)
        {
            button = raycastResult.GetComponentInParent<Button>();

            if (button == null)
            {
                return null;
            }
        }
        return button;
    }

    public override void OnPointerExit(GameObject previousObject)
    {
        ReticleDistanceInMeters = maxReticleDistance;
        reticle.fillAmount = 1f;
        shouldGazeAgain = false;
    }

    public override void OnPointerClickDown() { }

    public override void OnPointerClickUp() { }

    public override void GetPointerRadius(out float enterRadius, out float exitRadius)
    {
        enterRadius = 0.0f;
        exitRadius = 0.0f;
    }

    protected override void Start()
    {
        base.Start();
    }

    private bool SetPointerTarget(Vector3 target, bool interactive)
    {
        if (base.PointerTransform == null)
        {
            Debug.LogWarning("Cannot operate on a null pointer transform");
            return false;
        }

        Vector3 targetLocalPosition = base.PointerTransform.InverseTransformPoint(target);

        ReticleDistanceInMeters =
          Mathf.Clamp(targetLocalPosition.z, RETICLE_DISTANCE_MIN, maxReticleDistance);
        if (interactive)
        {
        }
        else
        {
        }
        return true;
    }
}
