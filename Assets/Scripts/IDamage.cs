using UnityEngine;
using System.Collections;

public interface IDamage {

	//void GetHit(float damage, RaycastHit hit);
	void GetHit(float damage, Vector3 hitPoint, Vector3 hitDirection);
    void GetDamage(float damage);

}
