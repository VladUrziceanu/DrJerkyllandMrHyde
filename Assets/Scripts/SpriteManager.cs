using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SpriteManager : NetworkBehaviour {

    public Sprite[] playerSpriter;

    [SyncVar(hook = "OnChangeSprite")]
    public int spriteNumber;

    // Use this for initialization
    void Start () {
        playerSpriter = Resources.LoadAll<Sprite>("Players");
        GetComponent<SpriteRenderer>().sprite = playerSpriter[spriteNumber];
    }

    // Update is called once per frame
    void Update () {
        
    }

    public void setSpriteNumber(int number)
    {
        if (!isServer)
        {
           return;
        }
        spriteNumber = number;
    }

    public void OnChangeSprite(int num)
    {
        spriteNumber = num;
    }
}
