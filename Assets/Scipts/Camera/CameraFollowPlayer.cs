using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
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
    [SerializeField] private float timeElapsedY;
    [SerializeField] private float zoomDelay;
    [SerializeField] private float targetY;
    [SerializeField] private float targetYOffset;
    [SerializeField] private float lerpFactor;
    [SerializeField] private float offsetDirection;
    [SerializeField] private float cameraPlatformThreshold;


    private Vector3Int cameraOffset = new Vector3Int(0, 0, -10);
    private Camera thisCamera;
    private bool cameraMoving;


    void Start()
    {
        thisCamera = GetComponent<Camera>();
        PlayerManager.movementDirectionChanged += ResetLerp;
        PlayerManager.hasGrounded += ChangePlatform;
        PlayerManager.jumped += ChangeOffset;
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
        if (Mathf.Abs(transform.position.y - y) < cameraPlatformThreshold)
        {
            return;
        }

        if (y < targetY && Mathf.Abs(y - targetY) > 0.3f) 
        {    
            offsetDirection = 1;
        }
        else
        {
            offsetDirection = -1;

        }

        targetY = y;
        timeElapsedY = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null) return;

        float modifiedLerpFactor = lerpFactor;
        switch (PlayerManager.Instance.movementDirection)
        {
            case RunningLeftState:
                modifiedLerpFactor *= 8;
                break;
            case RunningRightState:
                modifiedLerpFactor *= 8;
                break;
        }
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.position.x, modifiedLerpFactor), transform.position.y, transform.position.z);
        if(Mathf.Abs(player.position.y - transform.position.y) > maxYDistance)
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, player.transform.position.y, lerpFactor), transform.position.z);
        else
            HandleVerticalMovement();

        //HandleHorizontalMovement();

    }
    private void HandleVerticalMovement()
    {
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, targetY + offsetDirection * targetYOffset, 0.005f), transform.position.z);
    }
    private void HandleHorizontalMovement()
    {
        switch (PlayerManager.Instance.movementDirection)
        {
            case IdleState:
                if (!cameraMoving && (transform.position.x - player.position.x < -anchorWalk / 2 || transform.position.x - player.position.x > anchorWalk / 2))
                    StartSnap();
                if (cameraMoving)
                {
                    //thisCamera.orthographicSize = Mathf.Lerp(thisCamera.orthographicSize, zoomIdle, (timeElapsed - zoomDelay) / snapTime);
                    transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.position.x, (timeElapsedX - zoomDelay) / (snapTime * 10)), transform.position.y, -10);
                }
                break;
            case WalkingRightState:
                if (!cameraMoving && transform.position.x - player.position.x < -anchorWalk / 2)
                    StartSnap();
                if (cameraMoving)
                {
                    //.orthographicSize = Mathf.Lerp(thisCamera.orthographicSize, zoomWalk, (timeElapsed - zoomDelay) / snapTime);
                    transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.position.x + anchorWalk, timeElapsedX / snapTime), transform.position.y, -10);
                }
                break;
            case WalkingLeftState:
                if (!cameraMoving && player.position.x - transform.position.x < -anchorWalk / 2)
                    StartSnap();
                if (cameraMoving)
                {
                    //thisCamera.orthographicSize = Mathf.Lerp(thisCamera.orthographicSize, zoomWalk, (timeElapsed - zoomDelay) / snapTime);
                    transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.position.x - anchorWalk, timeElapsedX / snapTime), transform.position.y, -10);
                }
                break;
            case RunningRightState:
                if (!cameraMoving && transform.position.x - player.position.x < -anchorRun / 2)
                    StartSnap();
                if (cameraMoving)
                {
                    //thisCamera.orthographicSize = Mathf.Lerp(thisCamera.orthographicSize, zoomRun, (timeElapsed - zoomDelay) / snapTime);
                    transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.position.x + anchorRun, timeElapsedX / snapTime), transform.position.y, -10);
                }
                break;
            case RunningLeftState:
                if (!cameraMoving && player.position.x - transform.position.x < -anchorRun / 2)
                    StartSnap();
                if (cameraMoving)
                {
                    //thisCamera.orthographicSize = Mathf.Lerp(thisCamera.orthographicSize, zoomRun, (timeElapsed - zoomDelay) / snapTime);
                    transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.position.x - anchorRun, timeElapsedX / snapTime), transform.position.y, -10);
                }
                break;
        }
    }
}
