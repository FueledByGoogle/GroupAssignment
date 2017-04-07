﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

	public Camera playerCamera;
	public Character character;
	public int roomsCleared;

	public float moveSpeed = 1f;
	public bool movementDisabled;

	//Archery
	public Bow bow;
	public GameObject archery;						//Used to hide archery related elements when not using the bow
	public GameObject fakeArrow;					//Fake arrow only displayed when player clicks Fire and bow begins to draw
	public GameObject rHand;
	public GameObject lHand;
	public bool usingBow;
	public bool arrowDrawn;
	private bool bowZoomed;
	private float bowZoomValue = 45f;

	/* When different swords are implemented this won't work because
	 * we're pointing to a specific word here, so instead we'll be pointing to the
	 * instantiated sword later on
	 */
	public Sword sword;

	void Start () {
		DontDestroyOnLoad (this);
		character = GetComponent<Character> ();
		movementDisabled = false;
		roomsCleared = 0;
		//Bow related
		usingBow = false;
		archery.SetActive (false);
		bowZoomed = false;
		Cursor.lockState = CursorLockMode.Locked;	//Remove cursor from player view
	}

	void Update () {
		if (Input.GetKeyDown("escape")) {
			Cursor.lockState = CursorLockMode.None;
		}


		if (Input.GetKeyDown ("2")) {
			usingBow = true;
		} else if (Input.GetKeyDown ("1")) {
			
			//Switching off archery
			fakeArrow.SetActive (false);

			archery.SetActive (false);
			usingBow = false;
			bow.stretching = false;
			bow.bowReady= false;
			bow.factor = 0f;

			//Enable Melee
			rHand.SetActive (true);
			lHand.SetActive (true);
			playerCamera.fieldOfView = 60f;
			bowZoomed = false;
		}

		Bow ();

		if (usingBow == false) {
			SwordAttack ();
			if (Time.time >= character.shieldCoolDown) {
				Defense ();
			}
		}

//		if (character.health <= 0) {
//			SceneManager.LoadScene ("Tutorial");
//
//		}
	}

	void FixedUpdate () {
		PlayerControl ();
		character.Jump ();
	}

	private void PlayerControl (){

		Movement ();
		if (movementDisabled)
			character.animator.SetBool ("Walking", false);
	}
		
	void Bow() {

		if (usingBow == true) {
			
			archery.SetActive (true);

			//Graphics
			rHand.SetActive (false);
			lHand.SetActive (false);
			if (arrowDrawn == false && bow.stretching == false) {	//This prevents an arrow from appearing on initial active of bow
				fakeArrow.SetActive (false);
			}

			//Shooting
			if (Input.GetMouseButton (0)) {

				bowZoomed = true;
				//When arrow is not drawn and player wants to draw arrow, display the fakeArrow
				if (arrowDrawn == false && bow.releasing == false) {
					fakeArrow.SetActive (false);
					arrowDrawn = true;
				}
				//Draw until bow becomes ready
				if (bow.bowReady == false) {
					fakeArrow.SetActive (true);
					bow.Stretch ();
				}
				//Bow ready to fire
				if (bow.stretching == true && bow.bowReady == true && arrowDrawn == true) {
					bow.Fire ();
					fakeArrow.SetActive (false);
					bow.Release ();
					arrowDrawn = false;
				}
				//revert back to default F0W
				if (bow.releasing == true) {
					playerCamera.fieldOfView = 60f;
					bowZoomed = false;
				}
			}
			//only zoom when arrow is drawn and stretching
			if (playerCamera.fieldOfView != bowZoomValue && bowZoomed)
				playerCamera.fieldOfView = Mathf.Lerp (playerCamera.fieldOfView, bowZoomValue, 0.05f);
		}
	}

	void Defense() {
		
		if (Input.GetMouseButton (1)) {
			character.animator.SetBool ("Shield_Up", true);
			character.animator.SetBool ("Defense_Broken", false);
		} else {
			character.animator.SetBool ("Shield_Up", false);
		}
	}

	void Movement () {

//		character.Jump ();
		/*TODO: Implement strafing where moving forward and sideways 
		 * will be normalized so walking sideways and forwards at the same time
		 *TODO: Ability to sprint for a short amount of time
		 */

		if ((Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.A) || 
			Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.S)) && character.isGrounded && !character.animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Hold_Defense"))
			character.animator.SetBool ("Walking", true);

		if (Input.GetKeyUp (KeyCode.D) || Input.GetKeyUp (KeyCode.A) ||
			Input.GetKeyUp (KeyCode.W) || Input.GetKeyUp (KeyCode.S) || !character.isGrounded)
			character.animator.SetBool ("Walking", false);

//		if (character.isGrounded) {

			if (Input.GetKey (KeyCode.D)) {					//Right Movement
					character.Strafe(false);
			} else if (Input.GetKey (KeyCode.A)) {			//Left Movement
					character.Strafe (true);
			}
			
			if (Input.GetKey (KeyCode.W)) {					//Forward Movement
				character.Move (true);
				if (character.rigidBody.velocity.z == 0f) {
//					transform.position = new Vector3 (transform.position.x, transform.position.y + 0.05f, transform.position.z + 0.05f);
				}
			} else if (Input.GetKey (KeyCode.S)) {			//Backwards Movement
				character.Move (false);
			} else {
				character.animator.SetBool("Walking", false);
			}



//		}
//		float h = Input.GetAxisRaw("Horizontal");
//		float v = Input.GetAxisRaw ("Vertical");
//
//		Vector3 movement = new Vector3 (h, 0f, v);
//
//
//		if (v == 1f ) {
////			movement = movement.normalized * 2f * Time.deltaTime;
////			character.rigidBody.MovePosition (transform.localPosition + movement);
//
//
//			character.rigidBody.MovePosition (movement);
//		} else if (v == -1f) {
//			movement = (-1) * movement.normalized * 2f * Time.deltaTime;
//			character.rigidBody.MovePosition (transform.localPosition - movement);
//		}


	}

	void SwordAttack () {
		//MouseButtonDown b/c MouseButton refreshes too quickly and multiple swings will be inputed
		if (!character.animator.GetCurrentAnimatorStateInfo (0).IsName ("Player_Sword_Attack")) {
			
			sword.hasCollided = false;

			if (Input.GetMouseButtonDown (0) && !character.animator.GetCurrentAnimatorStateInfo (0).IsName ("Player_Sword_Attack")) {
				character.animator.SetTrigger ("Normal_Attack");
			}
		} 
	
	}
}
