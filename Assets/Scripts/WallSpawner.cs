using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class WallSpawner : NetworkBehaviour {

    [SerializeField]
    public int rows = 16;
    [SerializeField]
    public int columns = 16;
    public GameObject wallPrefab;
    public int totalWalls = 35;


    public override void OnStartServer()
    {
        // Setup board
        for (int i = -1; i < columns + 1; i++)
        {
            for (int j = -1; j < rows + 1; j++)
            {
                if (i == -1 || i == columns || j == -1 || j == rows)
                {
                    Vector3 spawn = new Vector3(i, j, 0);
                    GameObject wall = (GameObject)Instantiate(wallPrefab, spawn, Quaternion.identity);
                    NetworkServer.Spawn(wall);
                }
            }
        }

        for (int i = 0; i < totalWalls; i++)
        {
            Vector3 spawn = new Vector3(Random.Range(1, rows - 1), Random.Range(1, columns - 1), 0);
            GameObject wall = (GameObject)Instantiate(wallPrefab, spawn, Quaternion.identity);
            NetworkServer.Spawn(wall);
        }

    }

    public override void OnStartClient()
    {
        ClientScene.RegisterPrefab(wallPrefab);
    }

    // Use this for initialization
    void Start ()
    {

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
