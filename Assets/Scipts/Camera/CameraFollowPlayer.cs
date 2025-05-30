using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float maxXDistance;
    [SerializeField] private float maxYDistance;
    [SerializeField] private float movementSpeed;

    private Vector3Int cameraOffset = new Vector3Int(0, 0, -10);

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null) return;

        if (Mathf.Abs(player.position.x - transform.position.x) > maxXDistance || Mathf.Abs(player.position.y - transform.position.y) > maxYDistance)
        {
            transform.position += new Vector3((player.position.x - transform.position.x) * Time.deltaTime * movementSpeed,
                (player.position.y - transform.position.y) * Time.deltaTime * movementSpeed, 0);
            
        }
    }
}
