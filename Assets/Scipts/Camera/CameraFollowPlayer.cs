using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public static CameraFollowPlayer instance;
    private void Awake()
    {
        instance = this; 
    }

    [SerializeField] private Transform player;
    [SerializeField] private Transform camera;
    [SerializeField] private float maxXDistance;
    [SerializeField] private float maxYDistance;
    [SerializeField] private float speedIdle;
    [SerializeField] private float speedWalk;
    [SerializeField] private float speedRun;
    [SerializeField] private float zoomIdle;
    [SerializeField] private float zoomWalk;
    [SerializeField] private float zoomRun;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float anchorWalk;
    [SerializeField] private float anchorRun;
    [SerializeField] private float snapTime;
    [SerializeField] private float timeElapsedX;
    [SerializeField] private float zoomDelay;
    [SerializeField] private float targetY;
    [SerializeField] private float targetYOffset;
    [SerializeField] private float lerpFactor;
    [SerializeField] private float offsetDirection;
    [SerializeField] private float cameraPlatformThreshold;
    [SerializeField] private float screenShakeDuration;
    [SerializeField] private float screenShakeMin;
    [SerializeField] private float screenShakeMax;
    [SerializeField] private bool shakeDisabled;

    private float screenShakeTimeElapsed = 0;


    private Vector3Int cameraOffset = new Vector3Int(0, 0, -10);
    private Camera thisCamera;
    private bool cameraMoving;
    private bool isShaking;
    private bool shakeUp;

    void Start()
    {

        thisCamera = GetComponentInChildren<Camera>();
        PlayerManager.movementDirectionChanged += ResetLerp;
        PlayerManager.hasGrounded += ChangePlatform;
        PlayerManager.jumped += ChangeOffset;
        PlayerManager.slamLanded += StartScreenShakeVertical;
        PlayerManager.dashStarted += StartScreenShakeHorizontal;
        PlayerManager.dashHit += FullScreenShake;
    }

    private void OnDestroy()
    {
        PlayerManager.movementDirectionChanged -= ResetLerp;
        PlayerManager.hasGrounded -= ChangePlatform;
        PlayerManager.jumped -= ChangeOffset;
        PlayerManager.slamLanded -= StartScreenShakeVertical;
        PlayerManager.dashStarted -= StartScreenShakeHorizontal;
        PlayerManager.dashHit -= FullScreenShake;
    }

    public void SetPlayer(Transform player)
    {
        this.player = player;
    }
    private void StartScreenShakeHorizontal(Vector2 direction)
    {
        if (shakeDisabled) return;
        //isShaking = true;
        //_Direction =Vector2.one;
    }
    private void StartScreenShakeVertical()
    {
        if (shakeDisabled) return;
        isShaking = true;
        noiseDirection = Vector2.up;
    }
    private void FullScreenShake()
    {
        if (shakeDisabled) return;
        isShaking = true;
        noiseDirection = Vector2.one;
    }
    private void ResetLerp()
    {
        cameraMoving = false;
        timeElapsedX = 0;
    }

    private void ChangeOffset()
    {
        offsetDirection = -1;
    }
    private void StartSnap()
    {
        Debug.Log("Snap");
        cameraMoving = true;
        timeElapsedX = 0;
    }

    private void ChangePlatform(float y)
    {
        if (Mathf.Abs(targetY - y) < cameraPlatformThreshold)
        {
            return;
        }

        if (y < targetY && Mathf.Abs(y - targetY) > 0.3f)  
            offsetDirection = -1; //Change this to 1 to revert to have both up and down offsets
        else
            offsetDirection = -1;

        targetY = y;
    }

    // Update is called once per frame
    void Update()
    {
        if (isShaking)
        {
            ScreenShake();

        }
    }

    void FixedUpdate()
    {
        if(player == null) return;

        if(isShaking)
        {
            return;
        }

        float offset=0;
        /*
        switch(PlayerManager.Instance.movementDirection)
        {
            case WalkingLeftState:
                offset = -1;
                break;
            case WalkingRightState: 
                offset = 1;
                break;
            case RunningLeftState: 
                offset = -2; 
                break;
            case RunningRightState: 
                offset = 2;
                break;
        }
        */
        camera.position = new Vector3(Mathf.Lerp(camera.position.x, player.position.x+offset, lerpFactor), camera.position.y, camera.position.z);
        if((player.position.y - camera.position.y) > maxYDistance)
            camera.position = new Vector3(camera.position.x, player.position.y - maxYDistance, camera.position.z);
        else if((player.position.y - camera.position.y) < -maxYDistance)
            camera.position = new Vector3(camera.position.x, player.position.y + maxYDistance, camera.position.z);
        else
            HandleVerticalMovement();

        //HandleHorizontalMovement();

    }
    private void HandleVerticalMovement()
    {
        camera.position = new Vector3(camera.position.x, Mathf.Lerp(camera.position.y, targetY + offsetDirection * targetYOffset, lerpFactor/5), camera.position.z);
    }
    private void HandleHorizontalMovement()
    {
        switch (PlayerManager.Instance.movementDirection)
        {
            case IdleState:
                if (!cameraMoving && (camera.position.x - player.position.x < -anchorWalk / 2 || camera.position.x - player.position.x > anchorWalk / 2))
                    StartSnap();
                if (cameraMoving)
                {
                    //thisCamera.orthographicSize = Mathf.Lerp(thisCamera.orthographicSize, zoomIdle, (timeElapsed - zoomDelay) / snapTime);
                    camera.position = new Vector3(Mathf.Lerp(camera.position.x, player.position.x, (timeElapsedX - zoomDelay) / (snapTime * 10)), camera.position.y, -10);
                }
                break;
            case WalkingRightState:
                if (!cameraMoving && camera.position.x - player.position.x < -anchorWalk / 2)
                    StartSnap();
                if (cameraMoving)
                {
                    //.orthographicSize = Mathf.Lerp(thisCamera.orthographicSize, zoomWalk, (timeElapsed - zoomDelay) / snapTime);
                    camera.position = new Vector3(Mathf.Lerp(camera.position.x, player.position.x + anchorWalk, timeElapsedX / snapTime), camera.position.y, -10);
                }
                break;
            case WalkingLeftState:
                if (!cameraMoving && player.position.x - camera.position.x < -anchorWalk / 2)
                    StartSnap();
                if (cameraMoving)
                {
                    //thisCamera.orthographicSize = Mathf.Lerp(thisCamera.orthographicSize, zoomWalk, (timeElapsed - zoomDelay) / snapTime);
                    camera.position = new Vector3(Mathf.Lerp(camera.position.x, player.position.x - anchorWalk, timeElapsedX / snapTime), camera.position.y, -10);
                }
                break;
            case RunningRightState:
                if (!cameraMoving && camera.position.x - player.position.x < -anchorRun / 2)
                    StartSnap();
                if (cameraMoving)
                {
                    //thisCamera.orthographicSize = Mathf.Lerp(thisCamera.orthographicSize, zoomRun, (timeElapsed - zoomDelay) / snapTime);
                    camera.position = new Vector3(Mathf.Lerp(camera.position.x, player.position.x + anchorRun, timeElapsedX / snapTime), camera.position.y, -10);
                }
                break;
            case RunningLeftState:
                if (!cameraMoving && player.position.x - camera.position.x < -anchorRun / 2)
                    StartSnap();
                if (cameraMoving)
                {
                    //thisCamera.orthographicSize = Mathf.Lerp(thisCamera.orthographicSize, zoomRun, (timeElapsed - zoomDelay) / snapTime);
                    camera.position = new Vector3(Mathf.Lerp(camera.position.x, player.position.x - anchorRun, timeElapsedX / snapTime), camera.position.y, -10);
                }
                break;
        }
    }

    public void ScreenShake()
    {
        if(screenShakeTimeElapsed >= screenShakeDuration)
        {
            screenShakeTimeElapsed = 0;
            transform.localPosition = Vector3.zero;
            isShaking = false;
        }
        screenShakeTimeElapsed += Time.fixedUnscaledDeltaTime;
        Debug.Log(screenShakeTimeElapsed);
        var sin = Mathf.Sin(shakeSpeed * (seed.x + seed.y + screenShakeTimeElapsed));
        var direction = noiseDirection + GetNoise();
        direction.Normalize();
        var delta = direction * sin;
        transform.localPosition = delta * maxShake;
    }

    [Tooltip("We won't move further than this distance from neutral.")]
    [Range(0.01f, 10f)]
    public float maxShake = 0.3f;
    [Range(0.01f, 50f)]
    public float shakeSpeed = 20f;

    [Tooltip("0 follows _Direction exactly. 3 mostly ignores _Direction and shakes in all directions.")]
    [Range(0f, 3f)]
    public float noiseMagnitude = 0.3f;
    public Vector2 seed;
    public Vector2 noiseDirection;
    Vector2 GetNoise()
    {
        var time = Time.realtimeSinceStartup;
        return noiseMagnitude
            * new Vector2(
                Mathf.PerlinNoise(seed.x, time) - 0.5f,
                Mathf.PerlinNoise(seed.y, time) - 0.5f
                );
    }
}
