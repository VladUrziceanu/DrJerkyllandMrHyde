using UnityEngine;
using UnityEngine.Networking;

public class BoardManager : NetworkBehaviour
{

    [SerializeField]
    public int rows = 16;
    [SerializeField]
    public int columns = 16;
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public int totalWalls = 35;
    Vector3 spawn;


    public override void OnStartServer()
    {
        // Setup board
        for (int i = -1; i < columns + 1; i++)
        {
            for (int j = -1; j < rows + 1; j++)
            {
                if (i == -1 || i == columns || j == -1 || j == rows)
                {
                    spawn = new Vector3(i, j, 0);
                    GameObject wall = (GameObject)Instantiate(wallPrefab, spawn, Quaternion.identity);
                    NetworkServer.Spawn(wall);
                }
                else
                {
                    spawn = new Vector3(i, j, 0);
                    GameObject floor = (GameObject)Instantiate(floorPrefab, spawn, Quaternion.identity);
                    NetworkServer.Spawn(floor);
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
        ClientScene.RegisterPrefab(floorPrefab);
    }
}
