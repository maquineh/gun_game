using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	public Transform cano;
	public Bullets bullet;
	public float cadenciaTiros = 100;//em ms
	public float velocidadeCano = 35;//em ms

	float proximoTiro; //em ms

	public void Atirar(){

		if (Time.time > proximoTiro) {
			proximoTiro = Time.time + cadenciaTiros/1000;
			Bullets bala = Instantiate (bullet, cano.position, cano.rotation) as Bullets;
			bala.SetSpeed (velocidadeCano);
		}
	}
}
