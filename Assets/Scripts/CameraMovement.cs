using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform objectToFolow;
    public float followSpeed = 10f;
    public float sensitivity = 100;

    // ī�޶� �� ����. ������ ������
    public float maxX = 70f;
    public float minX = -60f;


    public float rotX;
    public float rotY;

    public Transform realCamera;
    public Vector3 dirNormalized;
    public Vector3 finalDir;

    // ī�޶� �浹 ����.
    public float minDistance;
    public float maxDistance;

    public float finalDistance;

    public float smoothness = 10f;

    // Start is called before the first frame update
    void Start()
    {
        // ����Ƽ���� ���Ϸ��ޱ��� ��������� '������'�̶�� ���� ������ ���ʹϾ��� ����������
        // https://hub1234.tistory.com/21

        rotX = transform.localEulerAngles.x;
        rotY = transform.localEulerAngles.y;

        dirNormalized = realCamera.localPosition.normalized;
        //magnitude : ������ ũ��
        finalDistance = realCamera.localPosition.magnitude;

    }

    // Update is called once per frame
    void Update()
    {
        rotX -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, minX, maxX);
        


        Quaternion rot = Quaternion.Euler(rotX, rotY, 0f);
        transform.rotation = rot;

    }

    void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, objectToFolow.position, followSpeed * Time.deltaTime);

        // ���� -> ����
        finalDir = transform.TransformPoint(dirNormalized * maxDistance);

        RaycastHit hit;

        if (Physics.Linecast(transform.position, finalDir, out hit))
        {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            finalDistance = maxDistance;
        }
        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalized * finalDistance, Time.deltaTime * smoothness);
    }
}
