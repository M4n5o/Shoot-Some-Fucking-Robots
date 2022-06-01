using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

public class ShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _aimCamera;
    [SerializeField] private float _normalSensitivity;
    [SerializeField] private float _aimSensitivity;
    [SerializeField] private LayerMask _aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform _aimTargetGM;
    [SerializeField] Rig _aimRig;

    private StarterAssetsInputs _input;
    private ThirdPersonController _controller;
    private Animator _animator;

    private void Awake()
    {
        _input = GetComponent<StarterAssetsInputs>();
        _controller = GetComponent<ThirdPersonController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Vector3 _mouseWorldPosition = Vector3.zero;
        
        //raycast from the center of the screen to find the shooting target position
        Vector2 _screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);

        Transform _hitTransform = null; //creates a variable to store the transform that the "shot" hits

        Ray ray = Camera.main.ScreenPointToRay(_screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit _hitInfo, 999f, _aimColliderLayerMask))
        {
            _aimTargetGM.position = _hitInfo.point;
            _mouseWorldPosition = _hitInfo.point;
            _hitTransform = _hitInfo.transform;
        }

        //check if aim button is pressed and then change the current active camera
        //ALSO, ALL THE AIM BEHAVIOUR GO HERE
        if (_input.aim)
        {
            _aimCamera.gameObject.SetActive(true);
            _controller.SetSensitivity(_aimSensitivity);
            _controller.SetRotateOnMove(false);

            //set "Aim animation layer" to 1
            _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

            //activate the rig IK
            _aimRig.weight = Mathf.Lerp(_aimRig.weight, 1f, Time.deltaTime * 10f);

            //catch the mouse world position to know the direction player should be facing
            Vector3 _worldAimTarget = _mouseWorldPosition;
            _worldAimTarget.y = transform.position.y;
            Vector3 _aimDirection = (_worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, _aimDirection, Time.deltaTime * 20f);

            //if received the shoot input indentify the target
            //ALL THE TARGET IDENTIFICATION AND ITS BEHAVIOR SHOULD GO HERE
            if (_input.shoot)
            {
                if (_hitTransform != null)
                {
                    if (_hitTransform.GetComponent<EnemyTarget>())
                    {
                        //do something when hit target
                        Debug.Log("You've hit an Enemy, kill it!");
                    }
                    else
                    {
                        //Do something when hit something else
                        Debug.Log("You've hit a wall, are you stupid?");
                    }
                }
                _input.shoot = false;
            }
        }
        else
        {
            _aimCamera.gameObject.SetActive(false);
            _controller.SetSensitivity(_normalSensitivity);
            _controller.SetRotateOnMove(true);

            //set "Aim animation layer" back to 0
            _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));

            //turn the rig IK off
            _aimRig.weight = Mathf.Lerp(_aimRig.weight, 0f, Time.deltaTime * 10f);
        }


    }

}
