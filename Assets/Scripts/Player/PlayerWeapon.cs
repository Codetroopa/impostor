﻿using UnityEngine;
using UnityEngine.Networking;

public class PlayerWeapon : NetworkBehaviour {

    public LayerMask toHit;

    IWeapon weapon;
    AudioSource2D audio;
    PlayerController con;
    RaycastHit2D hit;

    [SerializeField]
    GameObject gunLocation;
    [SerializeField]
    Camera cam;

    void Start() {
        weapon = GameManager.Instance.GetPlayerWeapon();
        con = GetComponent<PlayerController>();
        audio = GetComponent<AudioSource2D>();
    }

    void Update() {
        CheckTrigger();
    }

    void CheckTrigger() {
        if (!isLocalPlayer) {
            return;
        }

        if (Input.GetButtonDown("Fire1") && con.gunDrawn) {
            CmdFireShot(cam.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    [Command]
    void CmdFireShot(Vector3 clickPos) {
        Vector2 direction = clickPos - gunLocation.transform.position;
        Debug.DrawRay(gunLocation.transform.position, direction, Color.red);
        hit = Physics2D.Raycast(gunLocation.transform.position, direction, weapon.Range, toHit);

        if (!weapon.Shoot()) {
            EmptyMagSound();
            return;
        }

        if (hit) {
            // Make sure we can't hit ourself
            if (hit.transform.gameObject.Equals(gameObject)) {
                DebugConsole.Instance.Log("You hit yourself, doofus");
                return;
            }

            if (hit.transform.tag == "Player") {
                PlayerHealth hp = hit.transform.GetComponent<PlayerHealth>();
                hp.InflictDamage(weapon.Damage);
            }

            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Obstacle")) {
                BaseDestructible hitObj = hit.transform.GetComponent<BaseDestructible>();
                if (hitObj) {
                    hitObj.OnHit(weapon.Damage);
                }
            }

        }
    }

    // Played when a shot is successfully fired
    void SoundBlast() {
        audio.Play();
    }

    // Played when a shot is fired with no bullets left
    void EmptyMagSound() {

    }

    void MuzzleFlash() {
        //TODO: implement custom muzzle flash
    }
}
