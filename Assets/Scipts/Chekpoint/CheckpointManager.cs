using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;
    public Transform currentCheckpoint;
    public GameObject playerPrefab;
    public static event Action refreshLevel;
    public string nextLevelName;
    public bool introLevel;

    private void Awake()
    {
        instance = this; 
    }
    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();   
    }

    public void SpawnPlayer()
    {
        GameObject spawnedPlayer = Instantiate(playerPrefab, currentCheckpoint.position, Quaternion.identity);
        CameraFollowPlayer.instance.SetPlayer(spawnedPlayer.transform);
        if(introLevel)
        {
            PlayerManager.Instance.introLevel = introLevel;
        }
    }

    public void SetChekpoint(Transform chekpoint)
    {
        currentCheckpoint = chekpoint;
    }

    public void PlayerDied()
    {
        Destroy(PlayerManager.Instance.gameObject);
        SpawnPlayer();
        refreshLevel?.Invoke();
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(nextLevelName);
    }
}
