using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum PlayerState
{
    Idle = 0,
    Walk,
    Run,
    Crouch,
    Exhausted,
    Jump
}

public class PlayerController : MonoBehaviour
{
    [Header("Player Setting")]
    public bool crouchToggle = false;
    float modelHeight;

    [Header("Player Movement Info")]
    public float runSpeed;
    public float walkSpeed;
    public float minJumpForce;
    public float maxJumpForce;
    private float jumpStartTime;
    public float jumpLimitTime;
    private float horiz;
    private float vert;
    private Vector3 move;
    public bool isHiding;

    [Header("Stamina")] 
    public float maxRunStamina; //최대 스태미나
    public float currentRunStamina; //현재 스태미나
    public float staminaCost; //달리기할 때 비용
    public float staminaRegen;
    public float exhaustDelay;
    private float currentExhaustDelay;

    public PlayerState currentState;

    private PlayerState lastState = PlayerState.Idle;

    public bool canMove = true;
    private bool isGround;
    private bool isRunning;
    private bool isExhausted;
    private bool jumping = false;

    public int currentFloor = 1;
    public Transform checkObstacle;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;
    Rigidbody rb;
    public Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        modelHeight = GetComponent<CapsuleCollider>().height;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentRunStamina = maxRunStamina;
        currentExhaustDelay = exhaustDelay;
        currentState = PlayerState.Idle;

        SoundManager.instance.PlayAudio("MainBGM");
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();
        TryJump();
        GetInput();
        HandleStamina();
        UpdateState();

        if (currentState != lastState)
        {
            switch (lastState)
            {
                case PlayerState.Walk:
                    SoundManager.instance.StopAudio("Walking");
                    break;
                case PlayerState.Run:
                    SoundManager.instance.StopAudio("Running");
                    break;
                case PlayerState.Exhausted:
                    SoundManager.instance.StopAudio("Exhausted");
                    break;
            }
            switch (currentState)
            {
                case PlayerState.Walk:
                    SoundManager.instance.PlayAudio("Walking");
                    break;
                case PlayerState.Run:
                    SoundManager.instance.PlayAudio("Running");
                    break;
                case PlayerState.Exhausted:
                    SoundManager.instance.PlayAudio("Exhausted");
                    break;
            }
            lastState = currentState;
        }
        if (canMove)
        {
            switch (currentState)
            {
                case PlayerState.Idle:
                    anim.SetInteger("State", 0);
                    break;
                case PlayerState.Walk:
                    Walk();
                    break;
                case PlayerState.Run:
                    RunMovementOnly();
                    break;
                case PlayerState.Exhausted:
                    Exhaust();
                    break;
                case PlayerState.Jump:
                    AirWalk();
                    break;
            }
        } else anim.SetInteger("State", 0);

        if (currentState != PlayerState.Exhausted) // 탈진 아닐 때만 속도 적용
        {
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        }
    }

    void GetInput()
    {
        if (isExhausted)
        {
            currentState = PlayerState.Exhausted;
            move = Vector3.zero;
            return;
        }

        horiz = Input.GetAxis("Horizontal");
        vert = Input.GetAxis("Vertical");
        move = canMove ? new Vector3(horiz, 0, vert) : Vector3.zero;

        if (Vector3.Magnitude(move) > 1f) move = move.normalized;

        if (isGround)
        {
            if ((horiz != 0 || vert != 0))
            {
                bool runInput = Input.GetKey(KeyCode.LeftShift);

                if (runInput && currentRunStamina > 0)
                {
                    currentState = PlayerState.Run;
                    isRunning = true;
                }
                else
                {
                    currentState = PlayerState.Walk;
                    isRunning = false;
                }
            }
            else
            {
                currentState = PlayerState.Idle;
            }
        }
    }
    void UpdateState()
    {
        if (isExhausted) return;

        if (currentState == PlayerState.Jump) return;
        
        if (move.sqrMagnitude > 0.01f)
        {
            bool runInput = Input.GetKey(KeyCode.LeftShift);
            bool canRun = (currentState == PlayerState.Run) ? currentRunStamina > 0 : currentRunStamina > 10.0f;
            bool obstacleExist = CheckObstacle();

            if (runInput && canRun && !obstacleExist)
            {
                currentState = PlayerState.Run;
                isRunning = true;
            }
            else
            {
                currentState = PlayerState.Walk;
                isRunning = false;
            }
        }
        else
        {
            currentState = PlayerState.Idle;
            isRunning = false;
        }
    }

    bool CheckObstacle()
    {
        if (isHiding) return true;
        bool result = Physics.Raycast(checkObstacle.position, move.normalized, 1.5f, obstacleLayer);

        return result;
    }

    void HandleStamina()
    {
        // 1. 스태미나 증감 계산
        // 달리는 중(Run 상태)이면 감소, 아니면 회복
        bool isRunningState = (currentState == PlayerState.Run);
        float applyCost = isRunningState ? -staminaCost : staminaRegen;

        currentRunStamina += Time.deltaTime * applyCost;
        currentRunStamina = Mathf.Clamp(currentRunStamina, 0, maxRunStamina);

        // 2. [핵심] 탈진 상태 진입 판정 (Run 함수 밖으로 꺼냄)
        // 스태미나가 0 이하가 됐는데, 아직 탈진 판정이 안 났다? -> 즉시 탈진 처리
        if (currentRunStamina <= 0.1f && !isExhausted)
        {
            isExhausted = true;
            currentExhaustDelay = exhaustDelay; // 쿨타임 초기화
            currentState = PlayerState.Exhausted; // [중요] 상태 강제 전환

            // 이동 멈춤 처리
            move = Vector3.zero;
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

            Debug.Log("탈진 발생! 상태 전환됨."); // 디버그용
        }
    }
    void Walk()
    {
        anim.SetInteger("State", 1);

        move *= walkSpeed;
        
        if (Vector3.Magnitude(move) > 0f) transform.forward = Vector3.Slerp(transform.forward, move, Time.deltaTime * 10f);
    }
    void RunMovementOnly()
    {
        anim.SetInteger("State", 2);
        move *= runSpeed;
        if (Vector3.Magnitude(move) > 0f) transform.forward = Vector3.Slerp(transform.forward, move, Time.deltaTime * 10f);
        Managers.instance.enemyManager.OnSoundDetected(transform.position, SoundType.Running, currentFloor);
    }
    void Run()
    {
        if (currentRunStamina > 0 && !isExhausted)
        {
            anim.SetInteger("State", 2);

            move *= runSpeed;

            if (Vector3.Magnitude(move) > 0f) transform.forward = Vector3.Slerp(transform.forward, move, Time.deltaTime * 10f);
            Managers.instance.enemyManager.OnSoundDetected(transform.position, SoundType.Running, currentFloor);
        }
        else
        {
            if (!isExhausted)
            {
                isExhausted = true;
                currentExhaustDelay = exhaustDelay;
                currentState = PlayerState.Exhausted;
                Debug.Log("Stamina Depleted");
            }
        }
    }

    void Exhaust()
    {
        anim.SetInteger("State", 4);
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

        if (currentExhaustDelay < 0)
        {
            isExhausted = false;
            currentExhaustDelay = exhaustDelay;
        }
        
        currentExhaustDelay -= Time.deltaTime;
    }    

    void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGround)
            {
                currentState = PlayerState.Jump;
                anim.SetTrigger("Jump");
                jumping = true;
                jumpStartTime = Time.time;
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, minJumpForce, rb.linearVelocity.z);

               // SoundManager.instance.PlayAudio("Jumping");
            }
        }

        if (jumping && Input.GetKey(KeyCode.Space))
        {
            if (Time.time < jumpStartTime + jumpLimitTime)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, minJumpForce + (maxJumpForce - minJumpForce) * (Time.time - jumpStartTime) / jumpLimitTime, rb.linearVelocity.z);
                currentState = PlayerState.Jump;
            }
            else
            {
                if (isGround) jumping = false;
            }
        }

        if (jumping && Input.GetKeyUp(KeyCode.Space))
        {
            jumping = false; 
        }
    }

    void CheckGround()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * (modelHeight / 2);

        RaycastHit hitInfo;

        bool hit = Physics.Raycast(rayOrigin, Vector3.down, out hitInfo, modelHeight, groundLayer);
        isHiding = Physics.OverlapBox(checkObstacle.position, new Vector3(0.2f, 0.15f, 0.2f), Quaternion.identity, obstacleLayer).Length > 0;

        if (hit)
        {
            isGround = hitInfo.distance < (modelHeight / 2) + 0.1f;
            //rb.useGravity = currentState == PlayerState.Jump;

            if (!isGround)
            {
                if (currentState != PlayerState.Jump)
                {
                    rb.useGravity = false;
                    if(rb.linearVelocity.y > 0)
                    {
                        if (Time.time > jumpStartTime + jumpLimitTime)
                        {
                            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                        }
                    }
                }

                if (!jumping)
                {
                    rb.useGravity = true;
                }
            }
            else
            {
                rb.useGravity = true;
            }
        }
        else
        {
            isGround = false;
            rb.useGravity = true;
            anim.SetBool("JumpEnd", false);
        }
        anim.SetBool("JumpEnd", isGround);
    }

    void AirWalk()
    {
        anim.SetInteger("State", 5);

        move *= walkSpeed;

        if (Vector3.Magnitude(move) > 0f) transform.forward = Vector3.Slerp(transform.forward, move, Time.deltaTime * 10f);
    }
}
