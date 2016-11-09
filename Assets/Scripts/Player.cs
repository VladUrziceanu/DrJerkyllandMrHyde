using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    public float moveTime = 0.1f;
    public LayerMask blockingLayer;
    public Sprite[] playerSpriter;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;
    private bool moving = false;

    public int timer = 0;
    private SpriteManager sm;


    // Use this for initialization
    protected void Start ()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
        initSprite();
        initPosition();
        timer = Random.Range(200, 300);
        
    }

    void OnCollisionEnter()
    {
        Destroy(gameObject);
    }

    private void initSprite()
    {
        
        sm = GetComponent<SpriteManager>();
        playerSpriter = Resources.LoadAll<Sprite>("Players");
        int t = Random.Range(0, playerSpriter.Length);
        sm.setSpriteNumber(t);
        GetComponent<SpriteRenderer>().sprite = playerSpriter[t];
    }

    private void initPosition()
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

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            moving = true;
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        else
        {
            return false;
        }
    }

    // Update is called once per frame
    void Update () {

        if(!isLocalPlayer)
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
