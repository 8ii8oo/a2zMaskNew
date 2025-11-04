using UnityEngine;

public class playerMove : MonoBehaviour
{
    public float speed = 0.1f;
    public float jumpPower = 1f;
    private int maxJumpCount = 2;
    public int currentJumpCount = 0;
    public bool isGround = true;
    public Rigidbody2D rigid;
    public GameObject mask;
    Rigidbody2D maskRigid;
    public float health = 100f;

    void Start()
    {
        speed = Time.deltaTime;
        rigid = GetComponent<Rigidbody2D>();
        maskRigid = mask.GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        PlayerMover();
        playerAttack();
        
        maskRigid.transform.position = this.transform.position;
    }

    void PlayerMover()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.Translate(speed, 0, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.Translate(-speed, 0, 0);
        }
        

    
        if (Input.GetKeyDown(KeyCode.Space) && currentJumpCount < maxJumpCount) //2단 점프
        {
            rigid.linearVelocity = new Vector2(0, jumpPower);
            currentJumpCount++;
            isGround = false;
            //rigid.velocity = Vector2.zero 점프 다른방법
            //rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse); 22
        }
    }
    void OnCollisionEnter2D(Collision2D collision) //점프 초기화
    {
        if (collision.gameObject.tag == "floor")
        {
            currentJumpCount = 0;
            isGround = true;
        }
    }

    void playerAttack()
    {
        if (mask.activeSelf == false && Input.GetKeyDown(KeyCode.Q)) //가면착용 Q (스킬전환)
        {
            mask.SetActive(true);
        }
        else if (mask.activeSelf == true && Input.GetKeyDown(KeyCode.Q))
        {
                mask.SetActive(false);
        }
    }
}
