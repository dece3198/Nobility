using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MonsterState
{
    Idle, Walk, Roll, Hit, Attack, Die
}

public class MonsterIdle : BaseState<TestMonster>
{
    public override void Enter(TestMonster state)
    {
    }

    public override void Exit(TestMonster state)
    {
    }

    public override void FixedUpdate(TestMonster state)
    {
    }

    public override void Update(TestMonster state)
    {
        state.viewDetector.FindTarget();
        if (state.viewDetector.Target != null)
        {
            state.ChangeState(MonsterState.Walk);
        }
    }
}

public class MonsterWalk : BaseState<TestMonster>
{
    private float speed;

    public override void Enter(TestMonster state)
    {
        state.animator.SetBool("Move", true);
    }

    public override void Exit(TestMonster state)
    {
    }

    public override void FixedUpdate(TestMonster state)
    {
        Vector3 dir = state.viewDetector.Target.transform.position - state.transform.position;
        Vector3 dirVec = dir.normalized;

        if(Vector3.Distance(state.transform.position, state.viewDetector.Target.transform.position) < 15f)
        {
            state.animator.SetFloat("Blend", 1f);
            speed = 3f;
        }
        else
        {
            state.animator.SetFloat("Blend", 0f);
            speed = 1.5f;
        }

        Vector3 moveVec = dirVec * speed * Time.fixedDeltaTime;
        state.transform.rotation = Quaternion.LookRotation(dirVec);
        state.rigid.MovePosition(state.rigid.position + moveVec);
        state.rigid.linearVelocity = Vector3.zero;
        if(Vector3.Distance(state.transform.position, state.viewDetector.Target.transform.position) < 2f)
        {
            state.ChangeState(MonsterState.Attack);
        }
    }

    public override void Update(TestMonster state)
    {

    }
}

public class MonsterRoll : BaseState<TestMonster>
{
    public override void Enter(TestMonster state)
    {
        state.animator.SetTrigger("BackStep");
        state.rigid.linearVelocity = Vector3.zero;
        Vector3 backDir = -state.transform.forward;
        state.rigid.linearVelocity = backDir * 10f;
        state.StartCoroutine(RollCo(state));

    }

    public override void Exit(TestMonster state)
    {
    }

    public override void FixedUpdate(TestMonster state)
    {
    }

    public override void Update(TestMonster state)
    {

    }

    private IEnumerator RollCo(TestMonster state)
    {
        yield return new WaitForSeconds(0.5f);
        state.ChangeState(MonsterState.Walk);
    }
}

public class MonsterAttack : BaseState<TestMonster>
{
    public override void Enter(TestMonster state)
    {
        state.animator.SetTrigger("Attack1");
        state.rigid.linearVelocity = Vector3.zero;
        state.StartCoroutine(AttackCo(state));
    }

    public override void Exit(TestMonster state)
    {
    }

    public override void FixedUpdate(TestMonster state)
    {
    }

    public override void Update(TestMonster state)
    {

    }

    private IEnumerator AttackCo(TestMonster state)
    {
        float time = 1f;

        while(time > 0)
        {
            time -= Time.deltaTime;
            Vector3 dir = state.viewDetector.Target.transform.position - state.transform.position;
            Vector3 dirVec = dir.normalized;
            state.transform.rotation = Quaternion.LookRotation(dirVec);
            yield return null;
        }

        state.ChangeState(MonsterState.Roll);
    }
}


public class TestMonster : Monster, IInteractable
{
    [SerializeField] private float hp;
    public float Hp
    {
        get { return hp; }
        set 
        { 
            hp = value;
            hpBar.value = Mathf.Clamp01(hp / maxHp);
        }
    }
    public float maxHp;

    public MonsterState monsterState;

    public Animator animator;
    public ViewDetector viewDetector;
    public Rigidbody rigid;
    [SerializeField] private Slider hpBar;
    [SerializeField] SkinnedMeshRenderer[] renderers;
    [SerializeField] private TextManager textManager;
    MaterialPropertyBlock block;

    private StateMachine<MonsterState, TestMonster> stateMachine = new StateMachine<MonsterState, TestMonster>();

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        viewDetector = GetComponent<ViewDetector>();    
        block = new MaterialPropertyBlock();
        stateMachine.Reset(this);
        stateMachine.AddState(MonsterState.Idle, new MonsterIdle());
        stateMachine.AddState(MonsterState.Walk, new MonsterWalk());
        stateMachine.AddState(MonsterState.Roll, new MonsterRoll());
        stateMachine.AddState(MonsterState.Attack, new MonsterAttack());
    }

    private void OnEnable()
    {
        maxHp = Hp;
        ChangeState(MonsterState.Idle);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void TakeHit(float damage, TextType textType)
    {
        Hp -= damage;
        textManager.ShowDamageText(damage, textType);
        StartCoroutine(HitCo());
    }

    public void ChangeState(MonsterState state)
    {
        stateMachine.ChangeState(state);
        monsterState = state;
    }

    private IEnumerator HitCo()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].GetPropertyBlock(block);
            block.SetColor("_BaseColor", Color.red);
            renderers[i].SetPropertyBlock(block);
        }
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].GetPropertyBlock(block);
            block.SetColor("_BaseColor", Color.wheat);
            renderers[i].SetPropertyBlock(block);
        }
    }
}
