using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using QxFramework.Core;

public class PlayerAct : MonoBehaviour
{
    public int health;
    Transform CurBoxCollider = null;

    public Vector3 targetposition;
    public float raycastDistance = 1f;  // 射线检测的最大距离
    public LayerMask collisionLayer;  // 碰撞层
    public LayerMask waterLayer;
    private Tween moveTween;
    public int money;
    
    private bool deadbool;


    private void Start()
    {
        health = 1;
        GameMgr.Get<IDataManager>().ChangeMoney(0);
        QXData.Instance.Get<PlayerData>().PMoney = 0;
    }
    private void Update()
    {
        CurBoxCollider = GetComponent<PlayerCharacter>().CurBoxCollider;
        EHurt();
        Death();
        money= QXData.Instance.Get<PlayerData>().PMoney; 
    }
    public void BlockDrop(GameObject pref ,float height , int time)//下落的方块
    {
        if (CurBoxCollider == null)
        {
            Debug.Log("主角脚底没有方块");
        }
        else
        {
            GameObject @object = Instantiate(pref, gameObject.transform.position + new Vector3(0, height, 0), Quaternion.identity);
            targetposition = GetComponent<PlayerCharacter>().CurBoxCollider.position + new Vector3(0,1,0);
            moveTween = @object.transform.DOMove(targetposition, time).SetEase(Ease.InOutQuad).OnUpdate(CheckForCollision);//平滑移动到脚下方块上
        }

    }

    public void EHurt()
    {
        if (CurBoxCollider == null)
        {
            Debug.Log("主角脚底没有方块");
        }
        else
        {
            if (CurBoxCollider.gameObject.tag == "Block 1" && !CurBoxCollider.gameObject.GetComponent<BlockAct>().isTheFinalNightmare)
            {
                health -= 1;
            }
            if (CurBoxCollider.gameObject.tag == "Block 4")
            {
                CurBoxCollider.gameObject.GetComponent<BlockAct>().isdes = true;
            }
            if (CurBoxCollider.gameObject.tag == "Block 5"&&QXData.Instance.Get<PlayerData>().PMoney>0)
            {
                this.gameObject.SetActive(false);
                if (QXData.Instance.Get<PlayerData>().PLevel == 2)
                {
                    UIManager.Instance.Close("GameUI");
                    UIManager.Instance.Close("PassUI");
                    GameMgr.Get<IDataManager>().ChangePlayerLevel(3);
                    UIManager.Instance.Open("VideoUI",3,"VideoUI","4");
                }
                else
                    UIManager.Instance.Open("PassUI", 3, "PassUI");
            }
            if (CurBoxCollider.gameObject.tag == "KEY")
            {
                GameMgr.Get<IDataManager>().ChangeMoney(1);
                //NEED Sound
            }
            RaycastHit hit;

            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
            {
                // 如果射线与水体碰撞，说明人物在水中
                if (hit.collider.GetComponent<BlockAct>() != null)
                {
                    if (hit.collider.GetComponent<BlockAct>().isELC)
                    {
                        health -= 1;
                    }
                }
            }
        }

        if (transform.position.y <= -10f)
        {
            health -= 1;
        }

    }



    public void Death()
    {
        Animator animator = this.gameObject.GetComponent<Animator>();
        if (health <= 0)
        {
            //Animator animator=this.gameObject.GetComponent<Animator>;
           
            if (animator != null)
            {
                animator.SetFloat("HP", 0f);
            }
            Debug.Log("玩家死亡");
            if (!deadbool)
            {
                AudioManager.Instance.PlayEffect("Death");
            }
            UIManager.Instance.Open("DeathUI", 4, "DeathUI");
            //TODO：玩家死亡的相关流程
        }
           
        

    }

    void CheckForCollision()
    {
        // 射线从物体当前位置到目标位置进行检测
        RaycastHit hit;
        if (Physics.Raycast(transform.position, targetposition - transform.position, out hit, raycastDistance, collisionLayer))
        {
            // 如果射线检测到碰撞体，停止移动
            Debug.Log("遇到碰撞体，停止移动: " + hit.collider.gameObject.name);
            moveTween.Kill();  // 停止 DoTween 动画
        }
    }

}
