using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Properties")]
    public MinMaxFloat MinMaxSearchTicks = new MinMaxFloat(1.5f, 3f);
    public MinMaxFloat MinMaxWalkDistance = new MinMaxFloat(4f, 10f);
    public float SearchDistance = 10f;
    public float RecursiveRaycastDistance = 5f;
    public float TimeToStartFollowingPlayer = 2f;
    public float KnockbackTime = 1f;
    public float KnockbackForce = .5f;
    public LayerMask LayerMask;

    private NavMeshAgent agent;
    private EnemyController controller;
    private AgentBehaviour currentBeheaviour;
    private bool playerSeen;
    private bool shouldShoot;
    private Vector3 currentMovingDirection;
    private Vector3 lastFramePosition;
    private bool beingKnockedback;
    private bool shouldMove = true;

    public AgentBehaviour CurrentBehaviour => currentBeheaviour;
    public bool ShouldShoot => shouldShoot;
    public Vector3 MovingDirection => currentMovingDirection;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<EnemyController>();
    }

    public void Init()
    {
        StopAllCoroutines();

        currentBeheaviour = AgentBehaviour.Searching;
        playerSeen = false;
        shouldShoot = false;
        shouldMove = true;

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        if (agent.isOnNavMesh) agent.isStopped = false;

        beingKnockedback = false;
        controller.Rigidbody.isKinematic = true;
        controller.Rigidbody.velocity = Vector2.zero;

        if (gameObject.activeSelf) StartCoroutine(BehaviourCR());
    }

    private void Update()
    {
        if (!shouldMove) return;

        if (GameManager.PlayerAlive)
        {
            switch (currentBeheaviour)
            {
                case AgentBehaviour.Searching:
                    SearchForPlayer();
                    break;
                case AgentBehaviour.Following:
                    FollowPlayer();
                    break;
            }
        }

        currentMovingDirection = (transform.position - lastFramePosition).normalized;
        lastFramePosition = transform.position;
    }

    public void Knockback(Vector2 bulletPos)
    {
        if (!beingKnockedback) StartCoroutine(KnockbackCR(bulletPos));
    }

    private IEnumerator KnockbackCR(Vector2 bulletPos)
    {
        beingKnockedback = true;
        shouldShoot = false;
        agent.isStopped = true;
        controller.Rigidbody.isKinematic = false;
        controller.Rigidbody.AddForce(((Vector2)transform.position - bulletPos).normalized * KnockbackForce);

        yield return new WaitForSeconds(KnockbackTime);

        beingKnockedback = false;
        shouldShoot = true;
        controller.Rigidbody.isKinematic = true;
        controller.Rigidbody.velocity = Vector2.zero;
        agent.isStopped = false;
    }

    private IEnumerator BehaviourCR()
    {
        while (currentBeheaviour == AgentBehaviour.Searching && !playerSeen)
        {
            var walkDistance = MinMaxWalkDistance.random;
            var randomDirection = GetRandomDirectionRecursive(walkDistance, 5);
            
            NavMesh.SamplePosition(randomDirection, out var hit, walkDistance, 1);
            var finalPosition = hit.position;

            MoveToPosition(finalPosition);

            yield return new WaitForSeconds(MinMaxSearchTicks.random);
        }
    }

    private Vector2 GetRandomDirectionRecursive(float walkDistance, int iterations)
    {
        var randomDirection = Random.insideUnitCircle * walkDistance;
        randomDirection += (Vector2)transform.position;

        var hit = Physics2D.Raycast(transform.position, randomDirection, RecursiveRaycastDistance, LayerMask);
        if (hit && iterations > 0)
        {
            return GetRandomDirectionRecursive(walkDistance, iterations - 1);
        }

        return randomDirection;
    }

    private void SearchForPlayer()
    {
        if (GameManager.Player.Invisible || playerSeen) return;

        var pTransform = GameManager.Player.transform;
        var hit = Physics2D.Raycast(transform.position, pTransform.position - transform.position, SearchDistance, LayerMask);

        if (hit && hit.transform == pTransform)
        {
            playerSeen = true;
            Invoke("StartFollowingPlayer", TimeToStartFollowingPlayer);
        }
    }

    private void StartFollowingPlayer()
    {
        currentBeheaviour = AgentBehaviour.Following;
        shouldShoot = true;
    }

    private void FollowPlayer()
    {
        var pTransform = GameManager.Player.transform;

        MoveToPosition(pTransform.position);
    }

    private void MoveToPosition(Vector3 newPosition)
    {
        agent.SetDestination(newPosition);
    }

    public void StopMovement()
    {
        agent.isStopped = true;
        shouldMove = false;

        StopAllCoroutines();
    }
}

public enum AgentBehaviour
{
    Searching,
    Following
}
