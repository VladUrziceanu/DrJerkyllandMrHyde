using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    [Serializable]
    public class Count {
        public int min, max;

        public Count (int min, int max)
        {
            this.min = min;
            this.max = max;
        }
    }

    public int rows = 8;
    public int columns = 8;
    public Count wallCount = new Count(5, 9);
    public GameObject floorTile;
    public GameObject wallTile;

    private Transform boardHolder;
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
                GameObject toInstantiate = floorTile;
                if (i == -1 || i == columns || j == -1 || j == rows)
                    toInstantiate = wallTile;
                GameObject instance = Instantiate(toInstantiate, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;

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

    void LayoutObjectAtRandom(GameObject tile, int min, int max)
    {
        int objectCount = Random.Range(min, max + 1);

        for(int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            Instantiate(tile, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene()
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(wallTile, wallCount.min, wallCount.max);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
