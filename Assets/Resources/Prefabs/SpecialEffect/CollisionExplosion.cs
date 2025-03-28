using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionExplosion : MonoBehaviour
{
    public GameObject explosionEffect; // ��ը��Ч��Ԥ����
    public GameObject newObjectPrefab; // ���ɵ��������Ԥ���壨���������������Ʒ��

    private void OnCollisionEnter(Collision collision)
    {
        // �����ײ�Ķ����Ƿ����ض���ǩ������ "purple"��
        if (collision.gameObject.CompareTag("purple"))
        {
            // ��ȡ��ײ���λ��
            ContactPoint contact = collision.contacts[0];
            Vector3 explosionPosition = contact.point;

            // ʵ������ը��Ч
            GameObject explosion = Instantiate(explosionEffect, explosionPosition, Quaternion.identity);

            // ���ٱ�ը��Ч���趨ʱ����Զ����٣�
            Destroy(explosion, 2f);

            // ������ײ�ķ���
            Destroy(collision.gameObject);
            Destroy(gameObject);

            // ȡ������ײ�����λ������������
            ReplaceWithNewObject(collision.gameObject);
        }
    }

    // �滻����ײ�����λ������������
    void ReplaceWithNewObject(GameObject collidedObject)
    {
        // ��ȡ����ײ�����λ��
        Vector3 collidedPosition = collidedObject.transform.position;

        // �ڱ���ײ�����λ������������
        Instantiate(newObjectPrefab, collidedPosition, Quaternion.identity);
    }
}

