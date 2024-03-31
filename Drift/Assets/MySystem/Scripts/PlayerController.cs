using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IPunObservable
{
    [SerializeField] private bool _cameraModeFree = true;
    [SerializeField] private float _torque = 8000f;
    [SerializeField] private float _breakTorque = 8000f;
    [SerializeField] private float _rearWheelAngleMax = 50f;
    [SerializeField] private float _velocityLeftEachFramePercent = 99.5f;
    [SerializeField] private float _antyFlipAngle = 70f;

    private Rigidbody _rgBody;
    private Transform _cameraHolder;
    private TextMeshProUGUI _cameraText;
    private Light _light;
    private TextMeshProUGUI _lightText;
    private Camera _camera;
    private PointsManager _pointsManagerScript;
        

    private WheelCollider[] _frontWheelsCol = new WheelCollider[2];
    private WheelCollider[] _rearWheelsCol = new WheelCollider[2];
    private WheelCollider[] _allWheelsCol = new WheelCollider[4];
    private Transform[] _allWheelsTransforms = new Transform[4];


    private PhotonView _photonView;
    private bool _isRed;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Garage")
        {
            Destroy(this);
            return;
        }
        _rgBody = GetComponent<Rigidbody>();
        _cameraHolder = transform.Find("CameraHolder");
        _cameraText = GameObject.Find("TextCameraMode").GetComponent<TextMeshProUGUI>();
        _light = GameObject.Find("Directional Light").GetComponent<Light>();
        _lightText = GameObject.Find("TextLightMode").GetComponent<TextMeshProUGUI>();
        _camera = Camera.main;
        _pointsManagerScript = GetComponent<PointsManager>();


        _photonView = GetComponent<PhotonView>();
        var wheelsCollidersTransform = transform.Find("Wheels").Find("Colliders");
        _frontWheelsCol[0] = _allWheelsCol[0] = wheelsCollidersTransform.Find("FrontLeftWheel").GetComponent<WheelCollider>();
        _frontWheelsCol[1] = _allWheelsCol[1] = wheelsCollidersTransform.Find("FrontRightWheel").GetComponent<WheelCollider>();
        _rearWheelsCol[0] = _allWheelsCol[2] = wheelsCollidersTransform.Find("RearLeftWheel").GetComponent<WheelCollider>();
        _rearWheelsCol[1] = _allWheelsCol[3] = wheelsCollidersTransform.Find("RearRightWheel").GetComponent<WheelCollider>();

        var WheelMeshesTransform = transform.Find("Wheels").Find("Meshes");
        _allWheelsTransforms[0] = WheelMeshesTransform.Find("FrontLeftWheel");
        _allWheelsTransforms[1] = WheelMeshesTransform.Find("FrontRightWheel");
        _allWheelsTransforms[2] = WheelMeshesTransform.Find("RearLeftWheel");
        _allWheelsTransforms[3] = WheelMeshesTransform.Find("RearRightWheel");
    }
    private void Update()
    {
        if (_photonView.IsMine)
        {
            

            //breaking
            bool breaking = false;
            if (Input.GetKey(KeyCode.Space))
            {
                breaking = true;
                foreach (var wheelColl in _rearWheelsCol)
                {
                    wheelColl.brakeTorque = _breakTorque;
                }
                foreach (var wheelColl in _frontWheelsCol)
                {
                    wheelColl.brakeTorque = _breakTorque/2f;
                }
            }
            else
            {
                foreach (var wheelColl in _rearWheelsCol)
                {
                    wheelColl.brakeTorque = 0f;
                }
                foreach (var wheelColl in _frontWheelsCol)
                {
                    wheelColl.brakeTorque = 0f;
                }
            }
            
            // torque
            foreach (var wheel in _allWheelsCol)
            {
                if(breaking)  wheel.motorTorque = 0;
                else wheel.motorTorque = Input.GetAxis("Vertical") * _torque;
            }
            //steering
            foreach (var item in _frontWheelsCol)
            {
                item.steerAngle = Input.GetAxis("Horizontal") * _rearWheelAngleMax;
            }
           

            //syncro transform
            for (int i = 0; i < 4; i++)
            {
                Vector3 pos;
                Quaternion quat;
                _allWheelsCol[i].GetWorldPose(out pos, out quat);
                _allWheelsTransforms[i].rotation = quat;
                _allWheelsTransforms[i].position = pos;
            }

            //camera section 
            Debug.DrawRay(transform.position, _rgBody.velocity.normalized * 10f);   //add camera to the white vector
            Debug.DrawRay(transform.position, transform.forward * 10f, UnityEngine.Color.blue);
            _cameraHolder.position = transform.position+ _rgBody.velocity.normalized * -5f;
            if(!_cameraModeFree)_cameraHolder.LookAt(transform.position);
            

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _cameraModeFree=!_cameraModeFree;
                if( _cameraModeFree )
                {
                    _cameraHolder.localRotation = Quaternion.Euler(0, 0, 0);
                    _cameraText.text = "press TAB to cahnge Camera mode --> Free Camera";

                }
                if (!_cameraModeFree) _cameraText.text = "press TAB to cahnge Camera mode --> Strict Camera";
            }
            _pointsManagerScript.CalculatePoints(_rgBody.velocity, transform.forward);

            //slowing if no imput section
            if (Input.GetAxisRaw("Vertical") == 0) _rgBody.velocity = _rgBody.velocity /100 * _velocityLeftEachFramePercent;

            //antyflip
            if (Input.anyKeyDown)
            {
                if ((transform.eulerAngles.x > _antyFlipAngle && transform.eulerAngles.x < 360 - _antyFlipAngle) || (transform.eulerAngles.z > _antyFlipAngle && transform.eulerAngles.z < 360 - _antyFlipAngle))
                {
                    Debug.Log("antyflip");
                    transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }

            //light
            if (Input.GetKeyDown(KeyCode.L))
            {
                _light.enabled = !_light.enabled;
                if (_light.enabled)
                {
                    _lightText.text = "press L to cahnge Light mode --> On";
                    
                    _camera.backgroundColor = new Color32(113, 181, 183, 255); 
                }
                else
                {
                    _lightText.text = "press L to cahnge Light mode --> Off";
                    _camera.backgroundColor = UnityEngine.Color.black;
                }
            }
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)  //sending or reading some variable, depending who is server
    {
        if (stream.IsWriting) { stream.SendNext(_isRed); }
        else { _isRed = (bool)stream.ReceiveNext(); }
    }
    public enum CameraMode
    {
        Free,
        Strict
    }




}
