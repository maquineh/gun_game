using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingObject {

	NavMeshAgent pathFinder;
	Transform target;
    LivingObject targetObject;
	Material skinEnemy;

	float distanciaParaAtacar = .5f;
	float periodoEntreAttacks = 1;
	float proximoAttack;

	Color originalColor;


	float enemyCollisionRadius;
	float targetCollisionRadius;
    float damage = 1;
    bool hasTarget;

	public ParticleSystem efeitoMorte;

	public enum State{Idle, Chasing, Attacking};
	State atualState;

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		pathFinder = GetComponent<NavMeshAgent>();
		skinEnemy = GetComponent<Renderer>().material;
        originalColor = skinEnemy.color;

        if (GameObject.FindGameObjectWithTag("Player")!=null){

            atualState = State.Chasing;
            hasTarget = true;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetObject = target.GetComponent<LivingObject>();
            targetObject.OnDeath += OnTargetDeath;
		
		    enemyCollisionRadius = GetComponent<CapsuleCollider>().radius;
		    targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

		    StartCoroutine (UpdatePath());
        }
	}

	public override void GetHit (float damage, Vector3 hitPoint, Vector3 hitDirection)
	{
		if(damage >= energy){
			Destroy(Instantiate(efeitoMorte.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.left, hitDirection)) as GameObject, efeitoMorte.startLifetime);
		}

		base.GetHit (damage, hitPoint, hitDirection);
	}

    void OnTargetDeath(){
        hasTarget = false;
        atualState = State.Idle;
    }
	
	// Update is called once per frame
	void Update () {
		//pathFinder.SetDestination(target.position);
        if(hasTarget){
		    if(Time.time > proximoAttack){
			    float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
			    if(sqrDstToTarget < Mathf.Pow(distanciaParaAtacar + enemyCollisionRadius + targetCollisionRadius,2)){
				    proximoAttack = Time.time + periodoEntreAttacks;
				    StartCoroutine(Attack());
			    }
		    }
        }
	}

	IEnumerator Attack(){
		atualState = State.Attacking;
		pathFinder.enabled = false;

		Vector3 posicaoOriginal = transform.position;
		Vector3 direcaoParaAlvo = (target.position - transform.position).normalized;
		Vector3 posicaoAttack = target.position - direcaoParaAlvo * (enemyCollisionRadius + 
		                                                             targetCollisionRadius);	
		float porcentagem = 0;
		float attackSpeed = 3;

		skinEnemy.color = Color.red;
        bool deuDano = false;
		while(porcentagem <= 1){

            if (porcentagem >= .5f && !deuDano){
                deuDano = true;
                targetObject.GetDamage(damage);
            }

			porcentagem += Time.deltaTime * attackSpeed;
			float interpolacao = (-Mathf.Pow(porcentagem,2) + porcentagem) *4;
			transform.position = Vector3.Lerp(posicaoOriginal, posicaoAttack, interpolacao);

			yield return null;
		}
		skinEnemy.color = originalColor;
		atualState = State.Chasing;
		pathFinder.enabled = true;
	}

	IEnumerator UpdatePath(){
		float refreshRate = .25f;

		while(hasTarget){ 
			if(atualState == State.Chasing){
				Vector3 direcaoParaAlvo = (target.position - transform.position).normalized;
				Vector3 targetPosition = target.position - direcaoParaAlvo * (enemyCollisionRadius + 
				                                                            targetCollisionRadius + 
				                                                              distanciaParaAtacar/2);
				if(!isDead){
					pathFinder.SetDestination(targetPosition);
				}
			}

            yield return new WaitForSeconds(refreshRate);
		}
	}
}
