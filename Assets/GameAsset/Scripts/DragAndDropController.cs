using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DragAndDropController : MonoBehaviour
{
    private bool isDragging = false;
    private Rigidbody currentlyDraggedRigidbody;
    private Vector3 offset;
    private int originalLayer;
    private Animation anim;

    public GarageManager garageOpened;
    

    [Header("Smooth Movement")]
    public float smoothSpeed = 50f;

    void Update()
    {
        PlayerReach playerReach = GetComponent<PlayerReach>();

        if (playerReach != null && playerReach.IsRaycastHit())
        {
            Ray ray =  Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Rigidbody hitRigidbody = hit.collider.GetComponent<Rigidbody>();
                    if (hitRigidbody != null)
                    {
                        if (hitRigidbody.gameObject.tag == "TemporaryTag")
                        {
                            isDragging = true;
                            currentlyDraggedRigidbody = hitRigidbody;

                            originalLayer = currentlyDraggedRigidbody.gameObject.layer;

                            //int temporaryLayer = LayerMask.NameToLayer("TemproryLayer");
                            // currentlyDraggedRigidbody.gameObject.layer = temporaryLayer;

                            offset = currentlyDraggedRigidbody.transform.position - hit.point;

                            currentlyDraggedRigidbody.isKinematic = false;
                            currentlyDraggedRigidbody.useGravity = false;
                        }
                        if (hitRigidbody.gameObject.tag == "GarageDoor")
                        {
                            isDragging = false;
                            currentlyDraggedRigidbody = hitRigidbody;

                            anim = currentlyDraggedRigidbody.gameObject.GetComponent<Animation>();

                           if (currentlyDraggedRigidbody.gameObject.GetComponent<GarageManager>().garageIsOpened == false)
                            {
                                anim.Play("Garage Door");
                                currentlyDraggedRigidbody.gameObject.GetComponent<GarageManager>().garageIsOpened = true;
                            }
                            else
                            {
                                anim.Play("Garage Door Close");
                                currentlyDraggedRigidbody.gameObject.GetComponent<GarageManager>().garageIsOpened = false;
                            }
                            
                        }
                    }
                }
            }
        }
        if (isDragging && currentlyDraggedRigidbody != null)
        {
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, Camera.main.transform.position.y));

            MoveWithCollision(targetPosition);

            if (Input.GetMouseButtonUp(0))
            {
                currentlyDraggedRigidbody.gameObject.layer = originalLayer;

                isDragging = false;
                currentlyDraggedRigidbody.isKinematic = false;
                currentlyDraggedRigidbody.useGravity = true;
                currentlyDraggedRigidbody = null;
            }
        }

        void MoveWithCollision(Vector3 targetPosition)
        {
            currentlyDraggedRigidbody.MovePosition(Vector3.Lerp(currentlyDraggedRigidbody.transform.position, targetPosition, smoothSpeed * Time.deltaTime));
        }
    }
}
