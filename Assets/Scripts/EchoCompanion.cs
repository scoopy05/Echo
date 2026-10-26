using UnityEngine;
using UnityEngine.InputSystem;

public class EchoCompanion : MonoBehaviour
{
    public enum EchoState
    {
        Following,
        Deploying,
        Distracting,
        Returning
    }

    [Header("Current State")]
    [SerializeField] private EchoState currentState = EchoState.Following;
    public EchoState CurrentState => currentState;

    [Header("Player Target")]
    public Transform playerTransform;
    public Transform cameraTransform;
    public Vector3 shoulderOffset = new Vector3(0.65f, 1.5f, -0.3f);

    [Header("Movement & Hover")]
    public float followSmoothTime = 0.25f;
    public float deploySpeed = 11f;
    public float returnSpeed = 13f;
    public float rotationSpeed = 8f;
    public float bobFrequency = 2.5f;
    public float bobAmplitude = 0.08f;

    [Header("Distraction Mechanics")]
    public float maxDeployDistance = 25f;
    public float distractDuration = 5f;
    public LayerMask hitLayers = ~0; // hits everything by default

    [Header("Visual & Light References")]
    public Light spotLight;
    public Light auraLight;
    public Renderer coreRenderer;

    [Header("Light Colors")]
    public Color normalColor = new Color(0.2f, 0.85f, 1f, 1f); // Neon Cyan
    public Color distractColor = new Color(1f, 0.6f, 0.1f, 1f); // Amber Alarm
    public float pulseSpeed = 8f;

    [Header("UI Crosshair")]
    public bool showAimReticle = true;

    // Internal state
    private Vector3 currentVelocity;
    private Vector3 deployTargetPos;
    private float distractTimer;
    private Material coreMaterialInstance;
    private static readonly int EmissionColorProp = Shader.PropertyToID("_EmissionColor");

    void Start()
    {
        // Auto-find player if not assigned
        if (playerTransform == null)
        {
            PlayerMovement pm = Object.FindFirstObjectByType<PlayerMovement>();
            if (pm != null)
            {
                playerTransform = pm.transform;
            }
            else
            {
                GameObject p = GameObject.FindWithTag("Player");
                if (p != null) playerTransform = p.transform;
            }
        }

        // Auto-find main camera if not assigned
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Grab core material instance for color shifting
        if (coreRenderer != null)
        {
            coreMaterialInstance = coreRenderer.material;
            SetDroneColor(normalColor, 2f);
        }

        // Initial position near player if just spawned
        if (playerTransform != null)
        {
            transform.position = GetShoulderPosition();
            transform.rotation = playerTransform.rotation;
        }
    }

    void Update()
    {
        HandleInput();

        switch (currentState)
        {
            case EchoState.Following:
                UpdateFollowing();
                break;
            case EchoState.Deploying:
                UpdateDeploying();
                break;
            case EchoState.Distracting:
                UpdateDistracting();
                break;
            case EchoState.Returning:
                UpdateReturning();
                break;
        }
    }

    private void HandleInput()
    {
        // Deploy / Recall toggle with 'E' key
        bool ePressed = false;
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ePressed = true;
        }

        if (ePressed)
        {
            if (currentState == EchoState.Following)
            {
                TryDeploy();
            }
            else
            {
                // Instant recall if already deployed or distracting
                Recall();
            }
        }
    }

    private void TryDeploy()
    {
        if (cameraTransform == null) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxDeployDistance, hitLayers))
        {
            // Hover 0.6m in front/above the hit surface
            deployTargetPos = hit.point + (hit.normal * 0.6f);
            currentState = EchoState.Deploying;
            SetDroneColor(distractColor, 2.5f);
            Debug.Log(
    "Ray hit: " + hit.collider.name +
    " | Distance: " + hit.distance
);
        }
    }

    public void Recall()
    {
        currentState = EchoState.Returning;
        SetDroneColor(normalColor, 2f);
    }

    private void UpdateFollowing()
    {
        if (playerTransform == null) return;

        Vector3 targetPos = GetShoulderPosition();
        
        // Gentle hovering bob
        float bob = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        targetPos.y += bob;

        // Smooth glide toward shoulder
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, followSmoothTime);

        // Smoothly face in player's forward direction
        Quaternion targetRot = Quaternion.LookRotation(playerTransform.forward, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

        // Reset light to normal steady state
        if (spotLight != null) spotLight.intensity = Mathf.Lerp(spotLight.intensity, 3f, Time.deltaTime * 5f);
    }

    private void UpdateDeploying()
    {
        // Fly directly toward the distraction point
        transform.position = Vector3.MoveTowards(transform.position, deployTargetPos, deploySpeed * Time.deltaTime);

        // Face towards destination
        Vector3 dir = (deployTargetPos - transform.position).normalized;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * 2f * Time.deltaTime);
        }

        // Arrived at target
        if (Vector3.Distance(transform.position, deployTargetPos) < 0.2f)
        {
            currentState = EchoState.Distracting;
            distractTimer = distractDuration;
        }
    }

    private void UpdateDistracting()
    {
        distractTimer -= Time.deltaTime;

        // Alarm pulse effect on lights and emissive core
        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f; // 0 to 1
        float intensity = Mathf.Lerp(1.5f, 5.5f, pulse);

        if (spotLight != null) spotLight.intensity = intensity;
        if (auraLight != null) auraLight.intensity = intensity * 0.6f;

        // Subtle scanning rotation while distracting
        transform.Rotate(Vector3.up, 60f * Time.deltaTime, Space.World);

        // Auto-recall when duration expires
        if (distractTimer <= 0f)
        {
            Recall();
        }
    }

    private void UpdateReturning()
    {
        if (playerTransform == null)
        {
            currentState = EchoState.Following;
            return;
        }

        Vector3 targetPos = GetShoulderPosition();
        transform.position = Vector3.MoveTowards(transform.position, targetPos, returnSpeed * Time.deltaTime);

        // Face back toward the player
        Vector3 dir = (targetPos - transform.position).normalized;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * 2f * Time.deltaTime);
        }

        // Close enough to resume normal follow
        if (Vector3.Distance(transform.position, targetPos) < 0.4f)
        {
            currentState = EchoState.Following;
            SetDroneColor(normalColor, 2f);
        }
    }

    private Vector3 GetShoulderPosition()
    {
        return playerTransform.position + playerTransform.TransformDirection(shoulderOffset);
    }

    private void SetDroneColor(Color color, float emissionMultiplier)
    {
        if (spotLight != null) spotLight.color = color;
        if (auraLight != null) auraLight.color = color;

        if (coreMaterialInstance != null)
        {
            coreMaterialInstance.SetColor("_BaseColor", color);
            coreMaterialInstance.SetColor(EmissionColorProp, color * emissionMultiplier);
        }
    }

    void OnGUI()
    {
        if (!showAimReticle || currentState != EchoState.Following) return;

        // Draw a small, crisp reticle dot in center screen
        float size = 6f;
        float x = (Screen.width - size) * 0.5f;
        float y = ((Screen.height - size) * 0.5f);

        GUI.color = new Color(0.3f, 0.9f, 1f, 0.75f);
        GUI.DrawTexture(new Rect(x, y, size, size), Texture2D.whiteTexture);
    }

    void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(GetShoulderPosition(), 0.2f);
            Gizmos.DrawLine(playerTransform.position, GetShoulderPosition());
        }

        if (currentState == EchoState.Deploying || currentState == EchoState.Distracting)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(deployTargetPos, 0.3f);
        }
    }
}
