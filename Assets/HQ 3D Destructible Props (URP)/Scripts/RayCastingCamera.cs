using UnityEngine;

/// <summary> Use this script to make raycasting from main camera to mouse positions</summary>
public class RayCastingCamera : MonoBehaviour
{
    /// <summary> Which layers do we want to raycast, layers that are not in this list will be ignored </summary>
    public LayerMask raycastingLayers;

    private Camera raysCAMERA = null;

    private void Start() => raysCAMERA = Camera.main;



    private void LateUpdate()
    {
        // When player clicks left mouse button do raycast from camera to mouse point
        if (Input.GetMouseButtonDown(0))
            RaycastFromMainCameraToMousePosition();
    }



    /// <summary>
    /// Raycasts rays from main camera to mouse position
    /// </summary>
    /// <returns></returns>
    private string RaycastFromMainCameraToMousePosition() 
    {
        // Used only for debug and function return, to know what was hit
        string result = "Nothing was hit by raycast";

        RaycastHit hit;
        Ray ray = raysCAMERA.ScreenPointToRay(Input.mousePosition);

        // This is Raycast function that returns what is hit from main camera to mouse
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastingLayers))
        {
            // Object we hit
            Transform _hitObj = hit.transform;

            // Only for debug
            Debug.DrawRay(raysCAMERA.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

            // If object has DestructibleObject script on it, destroy raycasted hit object
            if (_hitObj.GetComponent<DestructibleObject>() != null) 
            {
                // Calls function on hit object to destroy it
                _hitObj.GetComponent<DestructibleObject>().DestroyObj();

                // Used only for debug and function return, to know what was hit
                result = "Destructible object was hit " + _hitObj.name;
            }
            else
                // Used only for debug and function return, to know what was hit
                result = "Indestructible object was hit " + _hitObj.name;
        }
        else
        {
            // Only for debug
            Debug.DrawRay(raysCAMERA.transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);

            // Used only for debug and function return, to know what was hit
            result = "Nothing was hit by raycast";
        }
        
        Debug.Log("Raycasting: " + result);
        return result;
    }
}
