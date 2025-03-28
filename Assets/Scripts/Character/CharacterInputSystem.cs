using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using GGJ.UnitTools;
using UnityEngine.InputSystem.XInput;


public class CharacterInputSystem : SingletonBase<CharacterInputSystem>
{
    //public Button button;
    private InputController _inputController;
    //����
    public Vector2 playerMovement
    {
        //WASD
        get => _inputController.PlayerInput.Movement.ReadValue<Vector2>();

    }

    public float CameraDistance
    {
        //������
        get => Mouse.current.scroll.y.ReadValue();

    }

    public Vector2 cameraLook
    {
        //����ƶ�
        get => _inputController.PlayerInput.CameraLook.ReadValue<Vector2>();

    }


    private bool _playerJump;
    public bool playerJump
    {
        //�㰴�ո�
        get => _inputController.PlayerInput.Jump.triggered || _playerJump;

    }
    public void SetplayerJump()
    {
        _playerJump = true;
        StartCoroutine(playerJumpCoroutine());
    }

    //Э��
    IEnumerator playerJumpCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        _playerJump = false;
    }


    private bool _playerLAtk;
    private bool disablePlayerLAtk;
    public bool playerLAtk
    {
        //�㰴������
        get => (_inputController.PlayerInput.LAtk.triggered && disablePlayerLAtk) || _playerLAtk;

    }
    /// <summary>
    /// ��������������
    /// </summary>
    public void DisablePlayerLAtk() => disablePlayerLAtk = false;
    /// <summary>
    /// ��������������
    /// </summary>
    public void EnablePlayerLAtk() => disablePlayerLAtk = true;

    
    public void SetPlayerLAtk()
    {
        _playerLAtk = true;
        StartCoroutine(PlayerLAtkCoroutine());
    }
    
    IEnumerator PlayerLAtkCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        _playerLAtk = false;
    }



    //�ڲ�����
    private void Awake()
    {
        if (_inputController == null)
            _inputController = new InputController();
    }

    private void OnEnable()
    {
        _inputController.Enable();
    }
    private void OnDisable()
    {
        _inputController.Disable();
    }
    private void Update()
    {


    }
}
