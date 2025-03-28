using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatableBlock : MonoBehaviour
{
    public Transform PivotBlock;
    public Vector3 RotationAxis = Vector3.up;
    public float RotateTime = 0.5f;
    private bool isRotating = false;
    private int rotateAngle = 90;

    private void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out BlockAct blockAct))
            {
                blockAct.MouseDownEvent += _ => StartCoroutine(TryRotate());
            }
        }
    }

    private IEnumerator TryRotate()
    {
        if (isRotating || !CanRotate())
        {
            yield break;
        }

        isRotating = true;
        yield return StartCoroutine(DoRotateAnimation(rotateAngle));
        isRotating = false;
        NotifyBlocks();
    }

    private IEnumerator DoRotateAnimation(int angle)
    {
        float time = 0;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.AngleAxis(angle, RotationAxis) * startRotation;

        while (time < RotateTime)
        {
            float progress = time / RotateTime;
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, progress);
            time += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation; // ȷ�����սǶ�
    }

    private bool CanRotate()
    {
        HashSet<Vector3Int> occupiedBlocks = new HashSet<Vector3Int>();

        // ��ȡ��ǰ���з���λ��
        foreach (Transform child in transform)
        {
            occupiedBlocks.Add(Vector3Int.RoundToInt(child.position));
        }

        // Ԥ����ת���λ��
        foreach (Transform child in transform)
        {
            Vector3 rotatedPos = GetPositionAfterRotation(child.position, rotateAngle);
            Vector3Int roundedPos = Vector3Int.RoundToInt(rotatedPos);

            if (occupiedBlocks.Contains(roundedPos))
            {
                return false; // ��ת��λ�ñ�ռ�ã��޷���ת
            }
        }

        return true;
    }

    private Vector3 GetPositionAfterRotation(Vector3 position, int angle)
    {
        Vector3 pivot = PivotBlock.position;
        Quaternion rot = Quaternion.AngleAxis(angle, RotationAxis);
        return pivot + rot * (position - pivot);
    }

    private void NotifyBlocks()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out BlockAct blockAct))
            {
                blockAct.Act();
            }
        }
    }
}