using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public enum State { Chasing, Attacking, Ragdolled, Recovering }

    [Header("Target")]
    public Transform target;
    public string playerTagFallback = "Player";

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float attackRange = 1.8f;
    public float stopRange = 1.4f;

    [Header("Attack")]
    public float attackCooldown = 2f;
    public Transform punchPoint;
    public float punchRadius = 0.5f;
    public float punchWindowStart = 0.2f;
    public float punchWindowDuration = 0.25f;
    public LayerMask playerLayers;

    [Header("Recovery")]
    public float recoveryTime = 0.5f;

    private NavMeshAgent agent;
    private Animator animator;
    private Ragdoll ragdoll;
    private State state = State.Chasing;
    private float lastAttackTime = -999f;

    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ragdoll = GetComponent<Ragdoll>();

        agent.speed = moveSpeed;
        agent.stoppingDistance = stopRange;
    }

    void Start() {
        if (target == null) {
            GameObject p = GameObject.FindGameObjectWithTag(playerTagFallback);
            if (p != null) target = p.transform;
        }

        if (target == null)
            Debug.LogError($"{name}: EnemyAI has no target. Assign one or tag the player '{playerTagFallback}'.");
    }

    void Update() {
        if (ragdoll != null && ragdoll.IsRagdolled) {
            if (state != State.Ragdolled) EnterRagdolled();
            return;
        }

        if (state == State.Ragdolled) {
            StartCoroutine(Recover());
            return;
        }

        if (state == State.Recovering) return;
        if (target == null) return;

        float dist = Vector3.Distance(transform.position, target.position);

        switch (state) {
            case State.Chasing:
                Chase(dist);
                break;
            case State.Attacking:
                FaceTarget();
                break;
        }

        UpdateAnimatorSpeed();
    }

    void Chase(float dist) {
        if (agent.enabled) agent.SetDestination(target.position);

        if (dist <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            StartAttack();
    }

    void StartAttack() {
        state = State.Attacking;
        lastAttackTime = Time.time;

        if (agent.enabled) agent.isStopped = true;
        if (animator != null) animator.SetTrigger("Punch");

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine() {
        yield return StartCoroutine(PunchHitCheck());

        yield return new WaitForSeconds(0.3f);

        if (state == State.Attacking) {
            state = State.Chasing;
            if (agent.enabled) agent.isStopped = false;
        }
    }

    IEnumerator PunchHitCheck() {
        if (punchPoint == null) {
            Debug.LogWarning($"{name}: PunchPoint not assigned on EnemyAI.");
            yield break;
        }

        yield return new WaitForSeconds(punchWindowStart);

        float elapsed = 0f;
        var alreadyHit = new HashSet<Collider>();

        while (elapsed < punchWindowDuration) {
            Collider[] hits = Physics.OverlapSphere(punchPoint.position, punchRadius, playerLayers);

            foreach (Collider c in hits) {
                if (alreadyHit.Contains(c)) continue;
                alreadyHit.Add(c);
                Debug.Log($"{name} hit {c.name}");
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    void EnterRagdolled() {
        state = State.Ragdolled;
        StopAllCoroutines();
        if (agent.enabled) agent.enabled = false;
    }

    IEnumerator Recover() {
        state = State.Recovering;

        yield return null;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas)) {
            transform.position = hit.position;
            agent.enabled = true;
            agent.ResetPath();
        } else {
            Debug.LogWarning($"{name}: could not find navmesh after ragdoll. Agent stays disabled.");
        }

        yield return new WaitForSeconds(recoveryTime);
        state = State.Chasing;
    }

    void FaceTarget() {
        if (target == null) return;
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(dir), 10f * Time.deltaTime);
    }

    void UpdateAnimatorSpeed() {
        if (animator == null) return;
        float speed = agent.enabled ? agent.velocity.magnitude : 0f;
        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (punchPoint != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(punchPoint.position, punchRadius);
        }
    }
}
