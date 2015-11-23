using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour {
	public Transform armaCarregada;
	Gun equippedGun;
	public Gun startingGun;

	public void EquipGun(Gun gunToEquip){

		if(equippedGun!=null){
			Destroy(equippedGun.gameObject);
		}


		equippedGun = Instantiate (gunToEquip, armaCarregada.position, armaCarregada.rotation) as Gun;
		equippedGun.transform.parent = armaCarregada;
	}

	void Start(){
		if (startingGun != null) {
			EquipGun(startingGun);
		}
	}

	public void Atirar(){
		if (equippedGun != null) {
			equippedGun.Atirar();
		}
	}
}
