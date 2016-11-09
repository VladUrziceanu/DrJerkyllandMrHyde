using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerNetworkSetup : NetworkBehaviour {

    public override void OnStartLocalPlayer()
    {
        GetComponent<Player>().enabled = true;
    }
}
