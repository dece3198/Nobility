using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;
    private PlayerInput playerInput;
    private InputAction runAction;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float speedUp;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rollSpeed = 8f;
    private float runValue;
    private float verticlaVelocity;
    public int comboIndex = 0;
    private float comboTimer;
    private float comboTime;

    [SerializeField] private Rigidbody bullet;
    [SerializeField] private Transform startPos;

    private Vector3 moveVec;
    private Vector2 inputVec;

    public bool isMove;
    [SerializeField] private bool isRolling = true;
    public bool isAtk = true;
    public bool isCombo = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        runAction = playerInput.actions["Run"];
    }


    private void Update()
    {
        if(isAtk)
        {
            PlayerMove();
            SpeedUp();
        }
        Gravity();
        AttackTime();
    }


    private void SpeedUp()
    {
        float target = runAction.IsPressed() ? 1f : 0f;
        runValue = Mathf.Lerp(runValue, target, Time.deltaTime * 5f);
        animator.SetFloat("Blend", runValue);
    }

    //플레이어 움직임과 카메라방향에 따른 캐릭터 실시간 방향 조절
    private void PlayerMove()
    {
        isMove = moveVec.magnitude > 0.1f;
        animator.SetBool("Move", isMove);
        Vector3 camforward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camforward.y = 0;
        camRight.y = 0;
        camforward.Normalize();
        camRight.Normalize();

        moveVec = camforward * inputVec.y + camRight * inputVec.x;
        Vector3 velocity = moveVec * (moveSpeed + (runValue * speedUp)) + Vector3.up * verticlaVelocity;
        characterController.Move(velocity * Time.deltaTime);

        if (isMove && moveVec != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveVec), Time.deltaTime * rotateSpeed);
        }
    }

    //중력
    private void Gravity()
    {
        if (characterController.isGrounded)
        {
            if (verticlaVelocity < 0)
            {
                verticlaVelocity = -2f;
            }
        }
        else
        {
            verticlaVelocity += gravity * Time.deltaTime;
        }
    }

    //콤보 시간
    private void AttackTime()
    {
        if(isCombo)
        {
            comboTimer += Time.deltaTime;
            if(comboTimer > comboTime)
            {
                ResetCombo();
            }
        }
    }

    //콤보 공격
    private void OnAttack()
    {
        if(comboIndex == 0)
        {
            comboIndex = 1;
            animator.SetTrigger("Attack1");
        }
        else if (isCombo)
        {
            if(comboIndex < 3)
            {
                comboIndex++;
                animator.SetTrigger("Attack" + comboIndex);
                isCombo = false;
            }
        }
    }

    //콤보 시간 시작
    public void StartCombo(int curStep, float time)
    {
        if(comboIndex == curStep)
        {
            isCombo = true;
            comboTime = time;
            comboTimer = 0;
        }
    }

    //콤보 리셋
    public void ResetCombo()
    {
        comboIndex = 0;
        isCombo = false;
        comboTimer = 0;
    }
    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }
    void OnRoll(InputValue value)
    {
        if (value.isPressed && isRolling)
            StartCoroutine(RollCo());
    }
    void OnFire1(InputValue value)
    {
        if (value.isPressed)
        {
            OnAttack();
        }
    }

    //구르기 코루틴
    private IEnumerator RollCo()
    {
        isRolling = false;
        float time = 0f;
        animator.SetTrigger("Roll");

        Vector3 rollDir = transform.forward;

        while(time < 0.5f)
        {
            characterController.Move(rollDir * rollSpeed * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(5f);
        isRolling = true;
    }
}
