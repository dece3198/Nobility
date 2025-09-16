using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumPower = 5f;
    [SerializeField] private float rollSpeed = 8f;
    private float verticlaVelocity;

    private Vector3 moveVec;
    private Vector2 inputVec;
    public bool isMove;
    [SerializeField] private bool isJump = false;
    [SerializeField] private bool isRolling = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        PlayerMove();
        PlayerJump();
    }

    private void PlayerMove()
    {
        isMove = moveVec.magnitude > 0.1f;
        animator.SetBool("Move", isMove);
        Vector3 velocity = moveVec * moveSpeed + Vector3.up * verticlaVelocity;
        characterController.Move(velocity * Time.deltaTime);
        if(isMove)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveVec), Time.deltaTime * rotateSpeed);
        }
    }

    private void PlayerJump()
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

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
        Vector3 camforward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camforward.y = 0;
        camRight.y = 0;
        camforward.Normalize();
        camRight.Normalize();

        moveVec = camforward * inputVec.y + camRight * inputVec.x;
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed && isJump)
            StartCoroutine(JumpCo());
    }

    void OnRoll(InputValue value)
    {
        if (value.isPressed && isRolling)
            StartCoroutine(RollCo());
    }

    void OnFire1(InputValue value)
    {
        if(value.isPressed)
        {
            //animator.SetTrigger("Shot");
        }
    }

    private IEnumerator JumpCo()
    {
        isJump = false;
        verticlaVelocity = jumPower;
        animator.SetTrigger("Jump");
        yield return new WaitForSeconds(2f);
        isJump = true;  
    }

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
