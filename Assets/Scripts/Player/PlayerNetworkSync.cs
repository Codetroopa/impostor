﻿using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// This script will control how the local client interacts with other player's position/rotation through the server
/// </summary>
public class PlayerNetworkSync : NetworkBehaviour {

    public float smoothFactor = 5f;

    [SyncVar]
    Vector3 syncPos;
    [SyncVar]
    Quaternion syncRot;

    [SerializeField]
    Behaviour[] disabledComponents;

    Transform sprite;
    PlayerController con;

    void Start() {
        // Disable duplicate components used by other clients
        if (!isLocalPlayer) {
            foreach (Behaviour component in disabledComponents) {
                component.enabled = false;
            }
        }

        con = GetComponent<PlayerController>();
        sprite = GetComponentInChildren<SpriteRenderer>().transform;
    }

    public override void OnStartClient() {
        base.OnStartClient();
        Debug.Log("Registering Player");
        GameManager.Instance.PopulatePlayerList();
    }

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        GameManager.Instance.PopulatePlayerList();
        GameManager.Instance.SetLocalPlayer(gameObject.GetInstanceID());

		//TODO: Remove once done training dataset
		GameManager.Instance.PopulateAIList();
    }

    void Update() {
        TransmitMyPlayer();
        SmoothPlayers();
        if (Input.GetKeyDown(KeyCode.Y)) {
            Destroy(gameObject);
        }
    }

    // This gets called on all NON LOCAL clients to smooth out their movement
    void SmoothPlayers() {
        if (!isLocalPlayer) {
            transform.position = Vector3.Lerp(transform.position, syncPos, smoothFactor * Time.deltaTime);
            sprite.rotation = syncRot;
        }
    }

    // Send each local player's position + rotation
    [Command]
    void CmdServerPosition(Vector3 pos, Quaternion rot) {
        syncPos = pos;
        syncRot = rot;
    }

    [ClientCallback]
    void TransmitMyPlayer() {
        // Only transmit info if we are the local player
        if (isLocalPlayer) {
            CmdServerPosition(transform.position, sprite.rotation);
        }
    }
}
