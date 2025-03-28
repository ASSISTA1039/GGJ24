using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningEffect : MonoBehaviour
{
    public GameObject lightningPrefab;        // ��������ϵͳԤ����
    public float minTime = 1f;                // �������ʱ����
    public float maxTime = 3f;                // �����ʱ����
    public float lightningDuration = 0.2f;    // �������ʱ��
    public float lightningRadius = 5f;        // ����Ӱ��İ뾶����Χ��

    private float nextLightningTime;

    void Start()
    {
        // ��ʼ���´��������ɵ�ʱ��
        SetNextLightningTime();
    }

    void Update()
    {
        // ÿ֡����Ƿ������������ʱ��
        if (Time.time >= nextLightningTime)
        {
            // �ڷ���λ����������
            CreateLightning();

            // �����´����������ʱ��
            SetNextLightningTime();
        }
    }

    void SetNextLightningTime()
    {
        // ���������һ�������ʱ����
        nextLightningTime = Time.time + Random.Range(minTime, maxTime);
    }

    void CreateLightning()
    {
        // ���������λ��
        Vector3 lightningPosition = transform.position + Random.insideUnitSphere * lightningRadius;

        // ʵ������������ϵͳ
        GameObject lightning = Instantiate(lightningPrefab, lightningPosition, Quaternion.identity);

        // ��������ĳ���ʱ�䣨�ر���������Ч����
        Destroy(lightning, lightningDuration);
    }
}
