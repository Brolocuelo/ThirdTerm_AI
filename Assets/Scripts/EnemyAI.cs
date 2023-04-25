using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform player;

    private NavMeshAgent _agent;

    private float visionRange = 20f;
    private float attackRange = 10f;

    private bool playerInVisionRange;
    private bool playerInAttackRange;

    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private Transform[] waypoints;
    private int totalWaypoints;
    private int nextPoint;

    [SerializeField] private Transform[] bullet;
    [SerializeField] private Transform[] spawnPoint;
    private float timeBetweenAttacks;
    private bool canAttack;
    private float upAttackForce = 5f;
    private float forwardAttackForce = 8f;



    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        totalWaypoints = waypoints.Length;
        nextPoint = 1;
        canAttack = true;
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        playerInVisionRange = Physics.CheckSphere(pos, visionRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(pos, attackRange, playerLayer);

        if(!playerInVisionRange && !playerInAttackRange)
        {
            Patrol();
        }

        if (playerInVisionRange && !playerInAttackRange)
        {
            Chase();
        }

        if (!playerInVisionRange && playerInAttackRange)
        {
            Attack();
        }

        //_agent.SetDestination(player.position);
    }

    private void Patrol()
    {
        if (Vector3.Distance(transform.position, waypoints[nextPoint].position) < 2.5f)
        {
            nextPoint++;
            if (nextPoint == totalWaypoints)
            {
                nextPoint = 0;
            }
            transform.LookAt(waypoints[nextPoint].position);
        }
        _agent.SetDestination(waypoints[nextPoint].position);
    }

    private void Chase()
    {
        _agent.SetDestination(player.position);
        transform.LookAt(player);
    }

    private void Attack()
    {
        if (canAttack)
        {
            //Rigidbody rigidbody = Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            //rigidbody.AddForce(transform.forward * forwardAttackForce, ForceMode.Impulse);
            //rigidbody.AddForce(transform.up * upAttackForce, ForceMode.Impulse);

            canAttack = false;
            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        canAttack = true;
    }

    private void OnDrawGizmos()
    {
        //Esfera Vision
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        //Esfera Ataque
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
