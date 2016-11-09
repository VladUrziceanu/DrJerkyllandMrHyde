using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class BoardManager : NetworkBehaviour {

    public int rows = 8;
    public int columns = 8;
    public GameObject floorTile;
    public GameObject wallTile;

    private Transform boardHolder;
    public BoardNetworkManager manager;

    private List<Vector3> gridPositions = new List<Vector3>();

    void InitialiseList()
    {
        gridPositions.Clear();

        for(int i = 1; i < columns - 1; i++)
        {
            for(int j = 1; j < rows - 1; j++)
            {
                gridPositions.Add(new Vector3(i, j, 0f));
            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        for(int i = -1; i < columns + 1; i++)
        {
            for(int j = -1; j < rows + 1; j++)
            {
                GameObject instance = Instantiate(floorTile, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    public void SetupScene()
    {
        BoardSetup();
        InitialiseList();
    }

	// Use this for initialization
	void Start () {
        manager = new BoardNetworkManager();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    
}
