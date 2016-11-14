using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class Player : NetworkBehaviour
{
    public LayerMask blockingLayer;
    public Sprite[] playerSpriter;
    public Sprite[] enemySpriter;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    [SyncVar]
    public float inverseMoveTime;
    private bool moving = false;

    public int firstMessage = -1;

    [SyncVar]
    public int timer = 0;

    [SyncVar]
    public int playerLocalSprite = 0;

    [SyncVar]
    public int mrHide = 0;

    [SyncVar]
    public int score = 0;
    private const short chatMessage = 131;

    [SyncVar]
    public int currentId = 0;
    [SyncVar]
    public int totalIds = 1;

    [Command]
    public void CmdSetCurrentId(int id)
    {
        currentId = id;
    }

    [Command]
    public void CmdSetTotalId(int total)
    {
        totalIds = total;
    }
    [Command]
    public void CmdInitScore()
    {
        score = 0;
    }
    [Command]
    public void CmdIncScore()
    {
        score++;
    }

    public int currentHide = 0;
    public int aaaa = -1;


    private void ReceiveMessage(NetworkMessage message)
    {
        //reading message
        aaaa = message.ReadMessage<IntegerMessage>().value;

        if (aaaa == currentId)
        {
            CmdChangeSpeed(20.0f);
            CmdSetMrHide(1);
            mrHide = 1;
        }
        else
        {
            CmdChangeSpeed(10.0f);
            CmdSetMrHide(0);
            mrHide = 0;
        }
    }

    public void SendMessage(int value)
    {
        IntegerMessage myMessage = new IntegerMessage();
        //getting the value of the input
        myMessage.value = value;

        //sending to server
        NetworkManager.singleton.client.Send(chatMessage, myMessage);
    }

    private void ServerReceiveMessage(NetworkMessage message)
    {
        IntegerMessage myMessage = new IntegerMessage();
        //we are using the connectionId as player name only to exemplify
        myMessage.value = message.ReadMessage<IntegerMessage>().value;
        //sending to all connected clients
        NetworkServer.SendToAll(chatMessage, myMessage);
    }
    // Use this for initialization
    protected void Start ()
    {
        if (NetworkServer.active)
        {
            //registering the server handler
            //NetworkServer.RegisterHandler(chatMessage, ServerReceiveMessage);
        }
        
        NetworkManager.singleton.client.RegisterHandler(chatMessage, ReceiveMessage);
        CmdInitScore();
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 10.0f;
        initSprite();
        initPosition();
        timer = Random.Range(300, 700);
        CmdSetCurrentId((int)netId.Value);
        currentId = (int)netId.Value;

    }

    void OnCollisionEnter(Collision col)
    {
        Destroy(gameObject);
    }

    private void initSprite()
    {
        playerSpriter = Resources.LoadAll<Sprite>("Players");
        enemySpriter = Resources.LoadAll<Sprite>("Enemy");
        int t = Random.Range(0, playerSpriter.Length);
        CmdChangeSprite(t);
        GetComponent<SpriteRenderer>().sprite = playerSpriter[t];
    }

    public void initPosition()
    {
        int zone, q;
        Vector3 randomPosition = new Vector3(0, 0, 0);

        zone = Random.Range(0, 4);
        q = Random.Range(0, 15);
        if (zone == 0)  //Down
        {
            randomPosition = new Vector3(q, 0, 0);
        }
        else if (zone == 1) //Right
        {
            randomPosition = new Vector3(15, q, 0);
        }
        else if (zone == 2) //Up
        {
            randomPosition = new Vector3(q, 15, 0);
        }
        else if (zone == 3) //Down
        {
            randomPosition = new Vector3(0, q, 0);
        }
        transform.position += randomPosition;
    }
	
    private void onDisable()
    {
        
    }

    private void onTriggerEnter2D (Collider2D other)
    {
        //if(other.tag == "Player")
       // {
            //TODO: trateaza ciocnire cu inamic
       // }
    }

    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainDistance = (transform.position - end).sqrMagnitude;
        while (sqrRemainDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
        if (sqrRemainDistance <= float.Epsilon) moving = false;
    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);
        Player p;

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;
        
        if (hit.transform == null)
        {
            moving = true;
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        else if (hit.collider.gameObject.GetComponent<Player>() != null)
        {
            p = hit.collider.gameObject.GetComponent<Player>();
             if (mrHide == 1)
             {
                moving = true;
                StartCoroutine(SmoothMovement(end));
                CmdIncScore();
                return true;
            } else
            {
                return false;
            }
        } else
        {
            return false;
        }
    }
    void OnGUI()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, new Color(0,0,0));
        texture.Apply();
        GUI.skin.box.normal.background = texture;
        GUI.Box(new Rect(50, 120, 100, 30), GUIContent.none);
        GUI.Label(new Rect(50, 120, 100, 30), "Score: " + (score/2).ToString());
    }
    [Command]
    public void CmdChangeSprite(int number)
    {
        playerLocalSprite = number;
    }

    [Command]
    public void CmdSetMrHide(int number)
    {
        mrHide = number;
    }

    [Command]
    public void CmdDecrementTime()
    {
        timer--;
    }

    [Command]
    public void CmdChangeSpeed(float speed)
    {
        inverseMoveTime = speed;
    }

    // Update is called once per frame
    void Update () {

        if (mrHide == 0)
        {
            GetComponent<SpriteRenderer>().sprite = playerSpriter[playerLocalSprite];
        } else
        {
            GetComponent<SpriteRenderer>().sprite = enemySpriter[0];
        }

        if (isServer && isLocalPlayer)
        {
            if (totalIds != NetworkManager.singleton.numPlayers)   // If a player connected
            {
                CmdSetTotalId(NetworkServer.connections.Count);
            }
        }
            
        if (isServer && isLocalPlayer)
        {
            CmdDecrementTime();
            if (timer <= 0)
            {
                firstMessage = Random.Range(currentId, currentId + totalIds);

                if (firstMessage == currentId)
                {
                    CmdChangeSpeed(20.0f);
                    CmdSetMrHide(1);
                    GetComponent<SpriteRenderer>().sprite = enemySpriter[0];
                } else
                {
                    CmdChangeSpeed(10.0f);
                    CmdSetMrHide(0);
                    GetComponent<SpriteRenderer>().sprite = playerSpriter[playerLocalSprite];
                }
                // SendMessage(firstMessage);
               
                IntegerMessage myMessage = new IntegerMessage();
                //we are using the connectionId as player name only to exemplify
                myMessage.value = firstMessage;
                //sending to all connected clients
               // NetworkServer.SendToAll(chatMessage, myMessage);
               // NetworkServer.SendToClient(firstMessage, chatMessage, myMessage);
                foreach (NetworkConnection element in NetworkServer.connections)
                {
                    NetworkServer.SendToClient(element.connectionId, chatMessage, myMessage);
                }

                timer = Random.Range(300, 700);
            }
        }
        

        if (!isLocalPlayer)
        {
            return;
        }

        int horizontal = 0;
        int vertical = 0;

        if (Input.GetKey(KeyCode.DownArrow) == true) vertical = -1;
        if (Input.GetKey(KeyCode.UpArrow) == true) vertical = 1;
        if (Input.GetKey(KeyCode.LeftArrow) == true) horizontal = -1;
        if (Input.GetKey(KeyCode.RightArrow) == true) horizontal = 1;
        if (horizontal != 0)
            vertical = 0;
            
        
        RaycastHit2D hit;
        
        if ((horizontal != 0 || vertical != 0))
        {
            if (!moving)
            {
                Move(horizontal, vertical, out hit);
            }            
        }
        
    }

}
