using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.Input;
using CurvedUI;
using UnityEngine.UI;



public class PointerManager : MonoBehaviour
{


    //customRay
    #pragma warning disable 0649
        [SerializeField]
        CurvedUISettings mySettings;
        [SerializeField]
        Transform pivot;
        [SerializeField]
        float sensitivity = 0.1f;
        // Vector2 lastMouse;
        Vector3 lastMouse;
        [SerializeField]
        GameObject MouseController;
    #pragma warning restore 0649    


    //customRay laser
    #pragma warning disable 0649
        [SerializeField]
        Transform LaserBeamTransform;
        [SerializeField]
        Transform LaserBeamDot;
        [SerializeField]
        bool hideWhenNotAimingAtCanvas = false;
    #pragma warning restore 0649    


    public HandRef rightHandReference;
    public GameObject customRaycastGO;
    public GameObject handPointer;
    public PointerInputToggle toggle;
    public GameObject HandGO; 
    public GameObject LaserBeam;
    public GameObject CameraFollower;
    bool inputBoolChanged;
    // public GameObject hitGO;
    public RaycastHit hit;
    // public bool deleteGO;


    //eyetracking
    public GameObject arrow;
    OVREyeGaze eyeGaze;


    public Button EyeButton;
    public Button HandButton;
    public Button MouseButton;





    // Start is called before the first frame update
    void Start()
    {

        // lastMouse = CurvedUIInputModule.MousePosition;
        lastMouse = Input.mousePosition;
        eyeGaze = GetComponent<OVREyeGaze>();

        EyeButton.onClick.AddListener(EyeToggle);
        HandButton.onClick.AddListener(HandToggle);
        MouseButton.onClick.AddListener(MouseToggle);
        
        if (Application.isEditor)
        {
            MouseButton.Select();
        }else{
            EyeButton.Select();
            
        }

        // m_MyEvent.AddListener(DestroyAGo);






        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftCommand) && Input.GetKeyDown(KeyCode.Alpha0))
        {

        customRaycastGO.transform.localPosition = Vector3.zero;
        customRaycastGO.transform.localEulerAngles = Vector3.zero;

        }

        if(Input.GetKey(KeyCode.LeftCommand) && Input.GetKeyDown(KeyCode.Alpha1))
        {

            EyeButton.Select();

        }

        if(Input.GetKey(KeyCode.LeftCommand) && Input.GetKeyDown(KeyCode.Alpha2))
        {

            HandButton.Select();

        }

        if(Input.GetKey(KeyCode.LeftCommand) && Input.GetKeyDown(KeyCode.Alpha3))
        {

            MouseButton.Select();

        }




        /// <summary>
        /// A simple script to make the pointer follow mouse movement and pass the control ray to canvsa
        /// </summary>

            //need if mouse exists otherwise thows errors

            //find mouse delta
            // Vector3 mouseDelta = CurvedUIInputModule.MousePosition - lastMouse;
            // lastMouse = CurvedUIInputModule.MousePosition;

            Vector3 mouseDelta = Input.mousePosition - lastMouse;
            lastMouse = Input.mousePosition;


            
            //adjust transform angle
            pivot.localEulerAngles += new Vector3(-mouseDelta.y, mouseDelta.x, 0) * sensitivity;
            
            //pass ray and button state to CurvedUIInputModule
            // var myRay = new Ray(MouseController.transform.position, MouseController.transform.forward);
            var myRay = new Ray(customRaycastGO.transform.position, customRaycastGO.transform.forward);
            CurvedUIInputModule.CustomControllerRay = myRay;
            



        /// <summary>
        /// This class contains code that controls the visuals (only!) of the laser pointer.
        /// </summary>

            //get direction of the controller
           // Ray myRay = new Ray(this.transform.position, this.transform.forward);


            //make laser beam hit stuff it points at.
            
            if(LaserBeamTransform && LaserBeamDot) {
                //change the laser's length depending on where it hits
                float length = 10;

                //RaycastHit hit;
                if (Physics.Raycast(myRay, out hit, length, CurvedUIInputModule.Instance.RaycastLayerMask))
                {

                    // Debug.Log(hit.collider.gameObject.tag);

                    //get focusedGO
                    if(hit.collider.gameObject && Input.GetKey(KeyCode.LeftCommand) && Input.GetKeyDown(KeyCode.W)){

                        hit.collider.gameObject.BroadcastMessage("DestroyThisGO");

                    }

                    //get focusedGO
                    // if(hit.collider.gameObject != null && CurvedUIInputModule.CustomControllerButtonState == true){
                    //     hitGO = hit.collider.gameObject; //set hitGO to the gameobject that was hit
                    // }
                    
                    // length = Vector3.Distance(hit.point, customRaycastGO.transform.position);

                    //Find if we hit a canvas
                    CurvedUISettings cuiSettings = hit.collider.GetComponentInParent<CurvedUISettings>();
                    if (cuiSettings != null)
                    {
                        //find if there are any canvas objects we're pointing at. we only want transforms with graphics to block the pointer. (that are drawn by canvas => depth not -1)
                        int selectablesUnderPointer = cuiSettings.GetObjectsUnderPointer().FindAll(x => x != null && x.GetComponent<Graphic>() != null && x.GetComponent<Graphic>().depth != -1).Count;

                        // length = selectablesUnderPointer == 0 ? 10000 : Vector3.Distance(hit.point, customRaycastGO.transform.position);
                    }
                    else if (hideWhenNotAimingAtCanvas) length = 0;
                }
                else if (hideWhenNotAimingAtCanvas) length = 0;


                //set the leangth of the beam
                LaserBeamTransform.localScale = LaserBeamTransform.localScale.ModifyZ(length);
            }

            


        
    }



    void EyeToggle(){

        customRaycastGO.transform.SetParent(CameraFollower.transform);
        customRaycastGO.transform.localPosition = Vector3.zero;
        customRaycastGO.transform.localEulerAngles = Vector3.zero;
        customRaycastGO.transform.rotation = eyeGaze.transform.rotation;

        LaserBeam.SetActive(false);
        HandGO.SetActive(false);

        if(Input.GetKeyDown(KeyCode.LeftCommand)){
                CurvedUIInputModule.CustomControllerButtonState = true;
        }

    }

    void HandToggle(){

        CurvedUIInputModule.CustomControllerButtonState = rightHandReference.GetIndexFingerIsPinching();
        HandGO.SetActive(true);
        LaserBeam.SetActive(true);
        customRaycastGO.transform.SetParent(handPointer.transform);

        customRaycastGO.transform.localPosition = Vector3.zero;
        customRaycastGO.transform.localEulerAngles = Vector3.zero;

    }

    void MouseToggle(){

        LaserBeam.SetActive(false);
        CurvedUIInputModule.CustomControllerButtonState = Input.GetMouseButton(0);
        
        // CurvedUIInputModule.LeftMouseButton;
        HandGO.SetActive(false);
        customRaycastGO.transform.SetParent(CameraFollower.transform);

        customRaycastGO.transform.localPosition = Vector3.zero;
        customRaycastGO.transform.localEulerAngles = Vector3.zero;

    }


}
