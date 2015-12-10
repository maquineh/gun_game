using UnityEngine;
using System.Collections;

public class Bullets : MonoBehaviour {

	float speed = 9;
	float damage =1;

    float lifeTime = 3;
    float skinWidth = .1f;
	public LayerMask collisionMask;
    public LayerMask[] collisionsMasks;
	public void SetSpeed(float _speed){
		speed = _speed;
	}

	// Use this for initialization
	void Start () {
        Destroy(gameObject, lifeTime);

        Collider[] colisaoInicial = Physics.OverlapSphere(transform.position,1f, collisionMask);
	    if(colisaoInicial.Length > 0){
            OnHitObject(colisaoInicial[0], transform.position);
        }
    }
	
	// Update is called once per frame
	void Update () {
		float distanciaMovida = speed * Time.deltaTime;
		CheckCollisions(distanciaMovida);
		transform.Translate(Vector3.left*distanciaMovida);
	}

	void CheckCollisions(float distanciaMovida){
		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, distanciaMovida + skinWidth, collisionMask)){
			OnHitObject(hit.collider, hit.point);
		}
	}

	/*void OnHitObject(RaycastHit hit){
		//print(hit.collider.gameObject.name);
		IDamage damageObject = hit.collider.GetComponent<IDamage>();
		if (damageObject != null) {
			damageObject.GetHit(damage, hit);
		}
		GameObject.Destroy(gameObject);
	}*/

    void OnHitObject(Collider col, Vector3 hitPoint)
    {
        //print(hit.collider.gameObject.name);
        IDamage damageObject = col.GetComponent<IDamage>();
        if (damageObject != null)
        {
			damageObject.GetHit(damage, hitPoint, transform.up);
        }
        GameObject.Destroy(gameObject);
    }
}
