using UnityEngine;
using System.Collections;

public class PlayerInputControl : MonoBehaviour
{

    // Use this for initialization
    public float forwardmovespeed = 10.0f;
    public float gravity = 10.0f;
    public float atkmovespeed = 5.0f;
    public float atktimelimit = 0.8f;
    public float atktimecombolimit1 = 0.8f;
    public float atktimecombolimit2 = 0.8f;
    public float atktimecombolimit3 = 0.8f;
    private int tempstate;
    private int playerstate = 0;
    private int combostate = 0;
    private float bevelmovespeed;   //45度斜着移动的速度
    private CharacterController controller;
    private Vector3 movedirection;
    private float atktimer;
    private float atkcombotimer1;
    private float atkcombotimer2;
    private float atkcombotimer3;
    private bool canturn;
    
    void Start()
    {
        canturn = true;
        controller = GetComponent<CharacterController>();
        playerstate = 0;
    }

    // Update is called once per frame
    void Update()
    {
        UserInputControl();
        string state = UserStateControl();
        Debug.Log(state);
        ApplyGravity();
        
        if (controller.isGrounded)
        {
            movedirection = new Vector3();
        }
    }

    void UserInputControl()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            UserAtkControl();
        }

        if (((Input.GetKey(KeyCode.W)) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) && canturn)
        {
            UserMoveControl();
        }

        if (!Input.anyKeyDown && !Input.anyKey)
        {
            tempstate = 0;
        }
    }

    void UserMoveControl()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            tempstate = 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.eulerAngles = new Vector3(0, 90, 0);
            tempstate = 3;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            tempstate = 5;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.eulerAngles = new Vector3(0, 270, 0);
            tempstate = 7;
        }
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            transform.eulerAngles = new Vector3(0, 45, 0);
            tempstate = 2;
        }
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
        {
            transform.eulerAngles = new Vector3(0, 135, 0);
            tempstate = 4;
        }
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W))
        {
            transform.eulerAngles = new Vector3(0, 315, 0);
            tempstate = 8;
        }
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
        {
            transform.eulerAngles = new Vector3(0, 225, 0);

            tempstate = 6;
        }
        movedirection = transform.TransformDirection(new Vector3(0, movedirection.y, forwardmovespeed));
        
    }

    void UserAtkControl()
    {
        //二段连击
        if(playerstate == 10 && atktimer >=0.5 && atktimer<= atktimelimit)
        {
            Debug.LogWarning("二段攻击");
            combostate = 1;

        }
        else
        {
            tempstate = 10;
            if (atktimer == 0)
            {
                UserAtkTimer(tempstate);
            }
        }
        
    }

    void UserAtkTimer(int tempstate)
    {
        if (tempstate == 10)
        {
            atktimer = atktimer + Time.deltaTime;
        }

        if (tempstate == 11)
        {
            atkcombotimer1 = atkcombotimer1 + Time.deltaTime;
        }
    }
        
    string UserStateControl()
    {
        string state;

        //进入并保持攻击状态
        if (0 < atktimer && atktimer <= atktimelimit)
        {
            if (atktimer >= 0.3 && atktimer <= 0.5)
            {
                canturn = false;
                movedirection = transform.TransformDirection(new Vector3(0, movedirection.y, atkmovespeed));
                controller.Move(movedirection * Time.deltaTime);
            }

            
            playerstate = 10;
            UserAtkTimer(playerstate);
            state = "攻击状态中" + atktimer;

            BroadcastMessage("UserAnimatorControl", playerstate, SendMessageOptions.RequireReceiver); //通知AnimatorControl播放攻击1动画

            return state;
        }
        canturn = true;
        //进入二段攻击
        if (combostate == 1 && atktimer > atktimelimit)
        {
            playerstate = 11;
            UserAtkTimer(playerstate);

            if (atkcombotimer1 > 0 && atkcombotimer1 <= atktimecombolimit1 )
            {
                BroadcastMessage("UserAnimatorControl", playerstate, SendMessageOptions.RequireReceiver); //通知AnimatorControl播放攻击2动画

                if (atkcombotimer1 >= 0.216 && atkcombotimer1 <= 0.347)
                {
                    canturn = false;
                    movedirection = transform.TransformDirection(new Vector3(0, movedirection.y, atkmovespeed));
                    controller.Move(movedirection * Time.deltaTime);
                }

                
                state = "二段攻击中";
                return state;

            }
            canturn = true;
            atkcombotimer1 = 0;
            combostate = 0;
        }
        //离开攻击状态，计时器清零
        atktimer = 0;

        //进入移动状态
        if (tempstate > 0 && tempstate< 10)
        {
            playerstate = tempstate;
            state = "移动状态中";
            BroadcastMessage("UserAnimatorControl", playerstate, SendMessageOptions.RequireReceiver); //通知AnimatorControl播放跑步动画
            controller.Move(movedirection * Time.deltaTime);
            return state;
        }

        playerstate = 0;
        BroadcastMessage("UserAnimatorControl", playerstate, SendMessageOptions.RequireReceiver);
        state = "待机中";
        return state;

    }

    void ApplyGravity()
    {
        movedirection.y -= gravity * Time.deltaTime;
    }
}