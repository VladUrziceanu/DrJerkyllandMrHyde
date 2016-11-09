using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class BoardNetworkManager : NetworkBehaviour
{

    public int rows = 12;
    public int columns = 12;
    int min = 10;
    int max = 20;

    public BoardNetworkManager()
    {
    }

    // Use this for initialization
    void Start()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
