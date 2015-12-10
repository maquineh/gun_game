using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerController))]
public class Player : LivingObject {

	public float speedConst = 6;
	Camera camera;

	PlayerController controlador;
	GunController controladorArma;
    public Animator anim;
	// Use this for initialization
	protected override void Start () {
		base.Start();
		controlador = GetComponent<PlayerController> ();
		controladorArma = GetComponent<GunController> ();
		camera = Camera.main;

        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

		//Movimentos
		Vector3 movimento = new Vector3 (Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));
		Vector3 velocidade = movimento.normalized * speedConst;
		controlador.Move(velocidade);

        Animating(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		//Mirar
		Ray ray = camera.ScreenPointToRay (Input.mousePosition);
		Plane plane = new Plane (Vector3.up, Vector3.zero);
		float rayDistancia;

		if(plane.Raycast(ray,out rayDistancia)){
			Vector3 point = ray.GetPoint(rayDistancia);
			//Debug.DrawLine(ray.origin, point, Color.red);
			controlador.LookAt(point);
		}

		//Armas
		if (Input.GetMouseButton (0)) {
			controladorArma.Atirar();
		}

	}

    void Animating(float h, float v)
    {
        bool walking = (h != 0f || v != 0f); //true
        anim.SetBool("IsWalking", walking);
    }

}
