using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour {

    [SerializeField] private float panSpeed;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float rotateSpeedKeys;
    [SerializeField] private float rotateSpeedMouse;
    [SerializeField] private float draggingSpeed;
    [SerializeField] private float maxZoom;
    [SerializeField] private float minZoom;
    
    private bool isDragging;
    private Vector3 lastMousePosition;
    private Vector3 currentMousePosition;

    private Camera cam;

    private void Start() {
        cam = Camera.main;
    }

    private void Update() {
        currentMousePosition = Input.mousePosition;

        Pan();
        Zoom();
        Rotate();
        Drag();

        lastMousePosition = Input.mousePosition;
    }

    private void Pan() {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (x != 0||y != 0) {
            Vector3 dir = new Vector3(x, 0, y);
            dir = transform.worldToLocalMatrix.inverse * dir;
            transform.position += dir.normalized * panSpeed * DistanceToCamera().magnitude * Time.deltaTime;
        }
    }

    private void Zoom() {
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel")*2;
        if (scroll != 0) {
            Vector3 zoomAmount = DistanceToCamera() * -scroll * zoomSpeed * Time.deltaTime;
            if((cam.transform.position+zoomAmount-transform.position).magnitude<maxZoom || (cam.transform.position + zoomAmount - transform.position).magnitude > minZoom) {
                return;
            }
            cam.transform.position += zoomAmount;
        }
    }

    private void Rotate() {
        if(Input.GetKey(KeyCode.Q)){
            transform.Rotate(new Vector3(0, rotateSpeedKeys, 0));
        }
        if (Input.GetKey(KeyCode.E)) {
            transform.Rotate(new Vector3(0, -rotateSpeedKeys, 0));
        }

        if (Input.GetMouseButton(1)) {
            transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * rotateSpeedMouse, 0));
        }
    }

    private void Drag() {
        if (Input.GetMouseButtonDown(2) && !EventSystem.current.IsPointerOverGameObject()) {
            isDragging = true;
            
        }
        if (Input.GetMouseButtonUp(2) && !EventSystem.current.IsPointerOverGameObject()) {
            isDragging = false;
        }

        if (isDragging) {
            Vector3 dist = lastMousePosition - currentMousePosition;
            dist.z = dist.y;
            dist.y = 0;
            transform.Translate(dist * DistanceToCamera().magnitude * draggingSpeed * Time.deltaTime);
        }
    }

    private Vector3 DistanceToCamera() {
        return cam.transform.position - this.transform.position;
    }
}
