using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DoorInput : MonoBehaviour
{
    // Player Input
    [SerializeField] private TextMeshPro UseText;
    [SerializeField] private Transform Camera;
    [SerializeField] float MaxUseDistance = 5f;
    [SerializeField] private LayerMask UserLayers;

    public void OnUse()
    {
        if(Physics.Raycast(
               Camera.position,
               Camera.forward,
               out RaycastHit hit,
               MaxUseDistance,
               UserLayers))
        {
            if(hit.collider.TryGetComponent<Door>(out Door door))
            {
                if(door.IsOpen)
                {
                    door.Close();
                }
                else
                {
                    door.Open(transform.position);
                }
            }
        }
    }

    private void Update()
    {
        if(Physics.Raycast(
               Camera.position,
               Camera.forward,
               out RaycastHit hit,
               MaxUseDistance,
               UserLayers)
           && hit.collider.TryGetComponent<Door>(out Door door))
        {
            if(door.IsOpen)
            {
                UseText.SetText("Close \"E\"");
            }
            else
            {
                UseText.SetText("Open \"E\"");
            }
            UseText.gameObject.SetActive(true);
            UseText.transform.position = 
                hit.point - (hit.point - Camera.position).normalized * 0.01f;
            // Show the text wherever we hit, closer to player
            UseText.transform.rotation = 
                Quaternion.LookRotation((hit.point - Camera.position).normalized);
            // Show the text looking towards the player
        }
        else
        {
            UseText.gameObject.SetActive(false);
        }
    }

}
