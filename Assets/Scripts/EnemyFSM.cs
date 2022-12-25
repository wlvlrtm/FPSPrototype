using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { 
    None = -1,
    Idle = 0,
    Wander,
    Pursuit,
    Attack,
}

public class EnemyFSM : MonoBehaviour {

    [Header("Pursuit")]
    [SerializeField]
    private float targetRecognitionRange = 8;
    [SerializeField]
    private float pursuitLimitRange = 10;

    [Header("Attack")]
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform projectileSpawnPoint;
    [SerializeField]
    private float attackRange = 5;
    [SerializeField]
    private float attackRate = 1;

    private float lastAttackTime = 0;
    private EnemyState enemyState = EnemyState.None;
    private Status status;
    private NavMeshAgent navMeshAgent;
    private Transform target;
    private EnemyMemoryPool enemyMemoryPool;


    public void Setup(Transform target, EnemyMemoryPool enemyMemoryPool) {
        this.status = GetComponent<Status>();
        this.navMeshAgent = GetComponent<NavMeshAgent>();
        this.target = target;
        this.navMeshAgent.updateRotation = false;   // 오브젝트 회전은 수동으로 조정
        this.enemyMemoryPool = enemyMemoryPool;
    }
    
    private void OnEnable() {
        ChangeState(EnemyState.Idle);   // 대기 상태로 시작
    }

    private void OnDisable() {
        StopCoroutine(this.enemyState.ToString());
        this.enemyState = EnemyState.None;
    }

    public void ChangeState(EnemyState newState) {
        if (enemyState == newState) {
            return;
        }

        StopCoroutine(this.enemyState.ToString());  // 이전 상태 종료

        this.enemyState = newState;                 // 상태 변경

        StartCoroutine(this.enemyState.ToString()); // 신규 상태 등록
    }

    private IEnumerator Idle() {
        StartCoroutine("AutoChangeFromIdleToWander");

        while(true) {
            // TODO
            CalculateDistanceToTargetAndSelectState();
            yield return null;
        }
    }

    private IEnumerator AutoChangeFromIdleToWander() {
        int changeTime = Random.Range(1, 5);    // 1 ~ 5초 중 랜덤 대기
        
        yield return new WaitForSeconds(changeTime);

        ChangeState(EnemyState.Wander);     // Idle -> Wander
    }

    private IEnumerator Wander() {
        float currentTime = 0;
        float maxTime = 10;
        
        this.navMeshAgent.speed = this.status.WalkSpeed;

        this.navMeshAgent.SetDestination(CalculateWanderPosition());

        Vector3 to = new Vector3(this.navMeshAgent.destination.x, 0, this.navMeshAgent.destination.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
        
        transform.rotation = Quaternion.LookRotation(to - from);

        while(true) {
            currentTime += Time.deltaTime;

            to = new Vector3(this.navMeshAgent.destination.x, 0, this.navMeshAgent.destination.z);
            from = new Vector3(transform.position.x, 0, transform.position.z);

            if ((to - from).sqrMagnitude < 0.01f || currentTime >= maxTime) {
                ChangeState(EnemyState.Idle);
            }

            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }   

    private IEnumerator Pursuit() {
        while(true) {
            this.navMeshAgent.speed = this.status.RunSpeed;
            this.navMeshAgent.SetDestination(this.target.position);

            LookRotationToTarget();
            CalculateDistanceToTargetAndSelectState();

            yield return null;
        }
    }

    private IEnumerator Attack() {
        this.navMeshAgent.ResetPath();

        while(true) {
            LookRotationToTarget();
            CalculateDistanceToTargetAndSelectState();

            if (Time.time - this.lastAttackTime > this.attackRate) {
                this.lastAttackTime = Time.time;

                GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                clone.GetComponent<EnemyProjectile>().Setup(target.position);
            }

            yield return null;
        }
    }

    private void LookRotationToTarget() {
        Vector3 to = new Vector3(this.target.position.x, 0, target.position.z);
        Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

        transform.rotation = Quaternion.LookRotation(to - from);
    }

    private void CalculateDistanceToTargetAndSelectState() {
        if (target == null) {
            return;
        }

        float distance = Vector3.Distance(this.target.position, transform.position);

        if (distance <= attackRange) {
            ChangeState(EnemyState.Attack);
        }
        else if (distance <= this.targetRecognitionRange) {
            ChangeState(EnemyState.Pursuit);
        }
        else if (distance >= this.pursuitLimitRange) {
            ChangeState(EnemyState.Wander);
        }
    }

    private Vector3 CalculateWanderPosition() {
        float wanderRadius = 10;
        int wanderJitter = 0;
        int wanderJitterMin = 0;
        int wanderJitterMax = 360;
        
        Vector3 rangePosition = Vector3.zero;
        Vector3 rangeScale = Vector3.one * 100.0f;

        wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
        Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

        targetPosition.x = Mathf.Clamp(targetPosition.x, rangePosition.x - rangeScale.x * 0.5f, rangePosition.x + rangeScale.x * 0.5f);
        targetPosition.y = 0.0f;
        targetPosition.z = Mathf.Clamp(targetPosition.z, rangePosition.z - rangeScale.z * 0.5f, rangePosition.z + rangeScale.z * 0.5f);

        return targetPosition;
    }

    private Vector3 SetAngle(float radius, int angle) {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Cos(angle) * radius;
        position.z = Mathf.Sin(angle) * radius;

        return position;
    }

    public void TakeDamage(int damage) {
        bool isDie = this.status.DecreaseHP(damage);

        if (isDie == true) {
            this.enemyMemoryPool.DeactivateEnemy(gameObject);
        }
    }

    private void OnDrawGizoms() {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, this.navMeshAgent.destination - transform.position);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, this.targetRecognitionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, this.pursuitLimitRange);

        Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}