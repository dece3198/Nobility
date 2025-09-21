using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum MonsterState
{
    Idle, Walk, Run, Roll, Hit, Attack, Die
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
        Vector3 dir = state.player.transform.position - state.transform.position;
        Vector3 dirVec = dir.normalized;
        state.animator.SetFloat("Blend", 0f);
        speed = 1.5f;
        Vector3 moveVec = dirVec * speed * Time.fixedDeltaTime;
        state.transform.rotation = Quaternion.LookRotation(dirVec);
        state.rigid.MovePosition(state.rigid.position + moveVec);
        state.rigid.linearVelocity = Vector3.zero;

        if(Vector3.Distance(state.transform.position, state.player.transform.position) < 2f)
        {
            state.ChangeState(MonsterState.Roll);
        }
        else if (Vector3.Distance(state.transform.position, state.player.transform.position) < 15f)
        {
            state.ChangeState(MonsterState.Run);
        }
    }

    public override void Update(TestMonster state)
    {

    }
}

public class MonsterRun : BaseState<TestMonster>
{
    private float speed;

    public override void Enter(TestMonster state)
    {
        state.animator.SetBool("Move", true);
        if(state.runCo != null)
        {
            state.StopCoroutine(state.runCo);
        }
        state.runCo = RunCo(state);
        state.StartCoroutine(state.runCo);
    }

    public override void Exit(TestMonster state)
    {
    }

    public override void FixedUpdate(TestMonster state)
    {
        Vector3 dir = state.player.transform.position - state.transform.position;
        Vector3 dirVec = dir.normalized;
        state.animator.SetFloat("Blend", 1f);
        speed = 3f;
        Vector3 moveVec = dirVec * speed * Time.fixedDeltaTime;
        state.transform.rotation = Quaternion.LookRotation(dirVec);
        state.rigid.MovePosition(state.rigid.position + moveVec);
        state.rigid.linearVelocity = Vector3.zero;
        if (Vector3.Distance(state.transform.position, state.player.transform.position) < 2f)
        {
            state.ChangeState(MonsterState.Attack);
        }
    }

    public override void Update(TestMonster state)
    {

    }

    private IEnumerator RunCo(TestMonster state)
    {
        yield return new WaitForSeconds(15f);
        if(state.monsterState == MonsterState.Run)
        {
            state.ChangeState(MonsterState.Roll);
        }
    }
}

public class MonsterRoll : BaseState<TestMonster>
{
    public override void Enter(TestMonster state)
    {
        state.animator.SetBool("Move", false);
        if(Random.value < 0.5f)
        {
            state.StartCoroutine(whirindCo(state));
        }
        else
        {
            state.StartCoroutine(RollCo(state));
        }
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

    IEnumerator whirindCo(TestMonster state)
    {
        state.animator.SetTrigger("Attack3");
        yield return new WaitForSeconds(0.5f);
        state.slashs[3].Play();
        yield return new WaitForSeconds(0.5f);
        state.animator.SetBool("Charge", true);
        yield return new WaitForSeconds(0.5f);
        state.slashs[3].Play();
        yield return new WaitForSeconds(1f);
        state.animator.SetBool("Charge", false);
        for(int i = 0; i < 4; i++)
        {
            state.viewDetector.FindAttackTarget();
            if(state.viewDetector.AttackTarget != null)
            {
                Debug.Log("ff");
            }
            yield return new WaitForSeconds(0.1f);
            state.slashs[1].Play();
            yield return new WaitForSeconds(0.2f);
        }
        state.slashs[4].Play();
        yield return new WaitForSeconds(8f);
        state.slashs[4].Stop();
        state.animator.SetBool("Move", true);
        state.ChangeState(MonsterState.Walk);
    }

    private IEnumerator RollCo(TestMonster state)
    {
        yield return state.StartCoroutine(BackStepCo(state));

        if(Random.value < 0.33f)
        {
            yield return state.StartCoroutine(BackStepCo(state));
            yield return new WaitForSeconds(6f);
            state.ChangeState(MonsterState.Walk);
        }
        else
        {
            yield return new WaitForSeconds(6f);
            state.ChangeState(MonsterState.Walk);
        }
    }

    private IEnumerator BackStepCo(TestMonster state)
    {
        state.animator.SetTrigger("BackStep");
        Vector3 dir = state.player.transform.position - state.transform.position;
        Vector3 dirVec = dir.normalized;
        state.transform.rotation = Quaternion.LookRotation(dirVec);
        state.rigid.linearVelocity = Vector3.zero;
        Vector3 backDir = -state.transform.forward;
        state.rigid.linearVelocity = backDir * 10f;
        yield return new WaitForSeconds(3.9f);
        state.slashs[2].Play();
        yield return new WaitForSeconds(0.1f);
        state.animator.SetBool("Attack2", true);
        Vector3 targetDir = (state.player.transform.position - state.transform.position).normalized;
        float stopDistance = 1.5f;
        Vector3 dashPos = state.player.transform.position - targetDir * stopDistance;
        state.transform.DOMove(dashPos, 0.25f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.25f);
        state.animator.SetBool("Attack2", false);
        state.slashs[1].Play();
        state.viewDetector.FindAttackTarget();
        if (state.viewDetector.AttackTarget != null)
        {
            Debug.Log("f");
        }
    }
}

public class MonsterAttack : BaseState<TestMonster>
{
    public override void Enter(TestMonster state)
    {
        state.animator.SetBool("Move", false);
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
        Vector3 dir = state.player.transform.position - state.transform.position;
        Vector3 dirVec = dir.normalized;
        state.transform.rotation = Quaternion.LookRotation(dirVec);
        yield return new WaitForSeconds(1f);
        state.viewDetector.FindTarget();
        if (state.viewDetector.Target != null)
        {
            Debug.Log("f");
        }

        state.slashs[0].Play();
        yield return new WaitForSeconds(1.3f);
        if (Vector3.Distance(state.transform.position, state.player.transform.position) < 5f)
        {
            state.ChangeState(MonsterState.Roll);
        }
        else
        {
            state.ChangeState(MonsterState.Walk);
        }
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
    public ParticleSystem[] slashs;
    [SerializeField] private ParticleSystem particle;
    MaterialPropertyBlock block;
    public IEnumerator runCo;

    private StateMachine<MonsterState, TestMonster> stateMachine = new StateMachine<MonsterState, TestMonster>();
    public GameObject player;

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
        stateMachine.AddState(MonsterState.Run, new MonsterRun());
        stateMachine.AddState(MonsterState.Roll, new MonsterRoll());
        stateMachine.AddState(MonsterState.Attack, new MonsterAttack());
        ChangeState(MonsterState.Idle);
    }

    private void OnEnable()
    {
        maxHp = Hp;
    }

    private void Start()
    {
        player = GameManager.instance.player;
        ChangeState(MonsterState.Walk);
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
        particle.Play();
        StartCoroutine(HitCo());
    }

    public void ChangeState(MonsterState state)
    {
        stateMachine.ChangeState(state);
        monsterState = state;
    }

    private IEnumerator HitCo()
    {
        Time.timeScale = 0.1f;
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].GetPropertyBlock(block);
            block.SetColor("_BaseColor", Color.red);
            renderers[i].SetPropertyBlock(block);
        }
        yield return new WaitForSeconds(0.05f);
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].GetPropertyBlock(block);
            block.SetColor("_BaseColor", Color.wheat);
            renderers[i].SetPropertyBlock(block);
        }
        Time.timeScale = 1f;
    }
}
