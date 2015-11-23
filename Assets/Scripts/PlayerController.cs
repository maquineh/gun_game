using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

	Rigidbody rigidyBody;

	Vector3 velocidade;

	void Start () {
		rigidyBody = GetComponent<Rigidbody>();
	}

	public void Move (Vector3 _velocidade) {
		velocidade = _velocidade;
	}

	public void LookAt(Vector3 lookPoint){
		Vector3 heightCorrectedpoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
		transform.LookAt (heightCorrectedpoint);
	}

	void FixedUpdate(){
		rigidyBody.MovePosition (rigidyBody.position + velocidade * Time.fixedDeltaTime);
	}
}
