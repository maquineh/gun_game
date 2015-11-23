using UnityEngine;
using System.Collections;

public class LivingObject : MonoBehaviour, IDamage {
	protected float energy;
	public float initEnergy;
	protected bool isDead;

	//Evento para indicar a morte
	public event System.Action OnDeath;


	//Quando levar um tiro chama o metodo
	/*public void GetHit(float damage, RaycastHit hit){
        GetDamage(damage);
	}*/

	//Quando levar um tiro chama o metodo
	public virtual void GetHit(float damage, Vector3 hitPoint, Vector3 hitDirection){
		GetDamage(damage);
	}


    public virtual void GetDamage(float damage){
        energy = energy - damage;
        if (energy <= 0 && !isDead)
        {
            Die();
        }
    }

	protected virtual void Start(){
		energy = initEnergy;
	}

	protected void Die(){
		isDead = true;

		if(OnDeath != null){
			OnDeath();
		}

		GameObject.Destroy (gameObject);
	}
}
