using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody Rig;
    public float MoveSpeed=2;
    public float JumpSpeed=5;
    public LayerMask layerMask;
    public LayerMask waterLayer;
    public Animator animator;


    public float yOffset;                            //主角相对于地面碰撞体的高度
    bool isGround;
    public bool isInwater = false;
    public bool isInwaterbool = false;

    public Transform CurBoxCollider;                 //主角当前脚底下的碰撞体
    void Start()
    {
        Rig = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isInwater && !isInwaterbool)
        {
            AudioManager.Instance.PlayEffect("IntoWater");
            isInwaterbool = true;
        }
        if (!isInwater && isInwaterbool)
        {
            AudioManager.Instance.PlayEffect("OutoWater");
            isInwaterbool = false;
        }


        RaycastHit hit;

        isGround = Physics.Raycast(transform.position + new Vector3(0, 0, 0), Vector3.down, out hit, 0.6f, layerMask);

        if(hit.transform != null)
        {
            if (CurBoxCollider != null && hit.transform != CurBoxCollider && CurBoxCollider.gameObject.tag == "Block0")//黑色方块踩上去消失
            {
                CurBoxCollider.position += new Vector3(20f, 20f, 20f);
            }
            CurBoxCollider = hit.transform;
            yOffset = transform.position.y - CurBoxCollider.position.y;    //记录主角与地面的相对高度
        }
        if (Physics.CheckSphere(transform.position+new Vector3(0,0.4f,0) , 0.6f, waterLayer))
        {
            


            isInwater = true;
            // 获取检测到的 WaterLayer 物体
            Collider[] waterColliders = Physics.OverlapSphere(transform.position + new Vector3(0, 0.4f, 0), 0.6f, waterLayer, QueryTriggerInteraction.Collide);

            if (waterColliders.Length > 0)
            {
                Transform waterParent = waterColliders[0].transform.parent; // 获取第一个水的父对象
                if (waterParent != null)
                {
                    transform.parent = waterParent; // 将当前物体的父对象设为水的父对象
                }
            }
            Rigidbody rb = GetComponent<Rigidbody>();
            Vector3 customGravity = new Vector3(0, 8f, 0);
            rb.AddForce(customGravity, ForceMode.Force);
        }
        else
        {
            isInwater = false;
        }
    }

    /// <summary>
    /// 角色移动
    /// </summary>
    /// <param name="h">角色横向移动的数值</param>
    /// <param name="v">角色纵向移动的数值</param>
    /// <param name="isJump">是否跳跃</param>
    public void Move(Vector2 input, bool isJump,bool Inwater)
    {

        if (Inwater)
            return;//如果在水中，取消此移动方式
        
        RotationType type = GameManager.Instance.rotationType;

        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized; // 去掉y分量
        Vector3 cameraRight = Camera.main.transform.right;

        // 基于摄像机方向计算角色的移动方向
        RaycastHit hit;
        Vector3 desiredMove = cameraForward * input.y + cameraRight * input.x;
        desiredMove *= MoveSpeed;
        // 应用角色的移动速度
        Vector3 velocity = new Vector3(desiredMove.x, 0, desiredMove.z);

        //Debug.Log(velocity);
        //Rig.velocity = velocity;

        // 控制角色朝向移动方向
        if (desiredMove.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMove), Time.deltaTime * 4f);
        }
        //vel = new Vector3(velocity.x, velocity.y, velocity.z);
        if (velocity != new Vector3(0, 0, 0))
        {
            if (!Physics.Raycast(transform.position + 2f*velocity * Time.deltaTime, Vector3.down, out hit, 0.6f, layerMask)&& !Physics.Raycast(transform.position + 2f * velocity * Time.deltaTime, Vector3.down, out hit, 0.6f, waterLayer))//没有路的情况
            {
                velocity = new Vector3(0, 0, 0);
            }
        }
        Vector3 vel = new Vector3(velocity.x, velocity.y, velocity.z);
        Rig.velocity = new Vector3(vel.x, Rig.velocity.y, vel.z);
        /*
        if (isJump && isGround)//跳跃
        {
            Rig.AddForce(JumpSpeed * Vector3.up, ForceMode.Impulse);
        }
        Rig.velocity = new Vector3(vel.x, Rig.velocity.y, vel.z);
        */

        /*
        if(GameManager.Instance.isThird)
        {

            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized; // 去掉y分量
            Vector3 cameraRight = Camera.main.transform.right;

            // 基于摄像机方向计算角色的移动方向
            Vector3 desiredMove = cameraForward * input.y + cameraRight * input.x;
            desiredMove *= MoveSpeed;

            // 应用角色的移动速度
            Vector3 velocity = new Vector3(desiredMove.x, Rig.velocity.y, desiredMove.z);
            //Rig.velocity = velocity;

            // 控制角色朝向移动方向
            if (desiredMove.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMove), Time.deltaTime * 2f);
            }
            vel = new Vector3(velocity.x, velocity.y, velocity.z);
        }
        else if (type == RotationType.Up)
        {
            vel = Quaternion.Euler(0, 135, 0) * new Vector3(input.x, 0, input.y) * MoveSpeed;
        }
        else
        {
            Vector3 dir = Vector3.zero;
            switch (GameManager.Instance.rotationType)
            {
                case RotationType.Front:
                    dir = Vector3.right;
                    break;
                case RotationType.Right:
                    dir = Vector3.forward;
                    break;
                case RotationType.Back:
                    dir = Vector3.left;
                    break;
                case RotationType.Left:
                    dir = Vector3.back;
                    break;
            }
            vel = dir * MoveSpeed * input.x;
        }

        if (isJump && isGround)
        {
            Rig.AddForce(JumpSpeed * Vector3.up, ForceMode.Impulse);
        }
        Rig.velocity = new Vector3(vel.x, Rig.velocity.y, vel.z);

        */
    }

    public void InwaterMove(Vector2 input,bool isJump, bool Inwater)
    {
        if (!Inwater)
            return;//不在水中
        float movespeed = 0.5f * MoveSpeed;

        Vector3 vel;
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized; // 去掉y分量
        Vector3 cameraRight = Camera.main.transform.right;

        // 基于摄像机方向计算角色的移动方向
        Vector3 desiredMove = cameraForward * input.y + cameraRight * input.x;
        desiredMove *= movespeed;

        // 应用角色的移动速度
        Vector3 velocity = new Vector3(desiredMove.x, Rig.velocity.y, desiredMove.z);
        //Rig.velocity = velocity;

        // 控制角色朝向移动方向
        if (desiredMove.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMove), Time.deltaTime * 2f);
        }
        vel = new Vector3(velocity.x, velocity.y, velocity.z);

        RaycastHit hit;
        if (velocity != new Vector3(0, 0, 0))
        {
            if (!Physics.Raycast(transform.position + 2f * velocity * Time.deltaTime, Vector3.down, out hit, 0.6f, layerMask) && !Physics.Raycast(transform.position + 2f * velocity * Time.deltaTime, Vector3.down, out hit, 0.6f, waterLayer))//没有路的情况
            {
                velocity = new Vector3(0, 0, 0);
            }
        }

        if (isJump)
        {
            Rig.AddForce(0.5f * JumpSpeed * Vector3.up, ForceMode.Impulse);//TODO：后续调整参数
        }
        Rig.velocity = new Vector3(vel.x, Rig.velocity.y, vel.z);
    }

    public void FollowCollider()
    {
        if (CurBoxCollider != null)
        {
            transform.position = new Vector3(CurBoxCollider.position.x,
            CurBoxCollider.position.y + yOffset, CurBoxCollider.position.z);
        }
    }
}
