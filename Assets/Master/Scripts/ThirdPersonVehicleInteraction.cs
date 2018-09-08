using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonVehicleInteraction : MonoBehaviour {

    public Transform interactionSphere;
    public ThirdPersonRagdoll tpRagdoll;

    public LayerMask layerMask;
    public PlayerUI playerUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !tpRagdoll.ragdolling)
        {
            if (!playerUI.inVehicle)
                CheckInteraction();
            else
            {
                playerUI.ToggleVehicle(null);
                Debug.LogWarning("Attempting exit!");
            }
        }
    }

    string curstatus;
    public void CheckInteraction()
    {
        Collider[] hitColliders = Physics.OverlapSphere(interactionSphere.position, 1.48f, layerMask, QueryTriggerInteraction.Collide);
        curstatus = ("Searching vehicle in colliders: " + hitColliders.Length);
        foreach (Collider c in hitColliders)
        {
            if (c.GetComponent<VehicleSeatTrigger>())
            {
                playerUI.ToggleVehicle(c.GetComponent<VehicleSeatTrigger>().vp);
                curstatus = ("Found vehicle! " + c.GetComponent<VehicleSeatTrigger>().vp);
                break;
            }
        }
        Debug.LogWarning(curstatus);
    }
}
