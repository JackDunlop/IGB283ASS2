using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Movement : MonoBehaviour
{

    private GameObject qutjrGameObject; // Base
    private GameObject upperArmGameObject; // Upper Arm
    private GameObject lowerArmGameObject; // Lower Arm
    private GameObject wristGameObject; // Wrist

    public GameObject child;
    public Vector3 jointLocation;
    public Vector3 jointOffset;
    public float angle;
    public float lastAngle;

    private float headAngleRotationStart = 150f;
    private float headAngleDirection = -1;
    private float headSpeed = 75;


    private float direction = -1f;
    private float translationSpeed = 5;


    public void MoveByOffset(Vector3 offset)
    {
        Matrix3x3 T = Matrix3x3.Translate(offset);

        MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = T.MultiplyPoint(vertices[i]);
        }
        mesh.vertices = vertices;

        jointLocation = T.MultiplyPoint(jointLocation);

        if (child != null)
        {
            child.GetComponent<Movement>().MoveByOffset(offset);
        }
    }

    public void RotateAroundPoint(Vector3 point, float angle, float lastAngle)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        float lastAngleRad = lastAngle * Mathf.Deg2Rad;

        Matrix3x3 T1 = Matrix3x3.Translate(-point);
        Matrix3x3 R1 = Matrix3x3.Rotate(-lastAngleRad);
        Matrix3x3 T2 = Matrix3x3.Translate(point);
        Matrix3x3 R2 = Matrix3x3.Rotate(angleRad);
        Matrix3x3 M = T2 * R2 * R1 * T1;

        MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = M.MultiplyPoint(vertices[i]);
        }
        mesh.vertices = vertices;

        jointLocation = M.MultiplyPoint(jointLocation);

        if (child != null)
        {
            child.GetComponent<Movement>().RotateAroundPoint(point, angle, lastAngle);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        switch (gameObject.name)
        {
            case "QUTjr":
            {
                    qutjrGameObject = GameObject.Find("QUTjr");
            }
            break;

            case "Upper Arm":
            {
                    upperArmGameObject = GameObject.Find("Upper Arm");
            }
            break;


            case "Lower Arm":
            {
                    lowerArmGameObject = GameObject.Find("Lower Arm");
            }
            break;


            case "Wrist":
            {
                    wristGameObject = GameObject.Find("Wrist");
            }
            break;


        }
        if (child != null)
        {
            child.GetComponent<Movement>().MoveByOffset(jointOffset);
            child.GetComponent<Movement>().RotateAroundPoint(jointLocation, angle, lastAngle);
        }
        MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.mesh.RecalculateBounds();
        }


    }


    void WobbleHead()
    {
        if (lowerArmGameObject != null)
        {
            headAngleRotationStart += headSpeed * headAngleDirection * Time.deltaTime;


            if (headAngleRotationStart <= 90f)
            {
                headAngleRotationStart = 90f;
                headAngleDirection = 1f;
            }
            else if (headAngleRotationStart >= 150f)
            {
                headAngleRotationStart = 150f;
                headAngleDirection = -1f;
            }


            angle = headAngleRotationStart;
            child.GetComponent<Movement>().RotateAroundPoint(jointLocation, angle, lastAngle);
        }


        lastAngle = angle;

        MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.mesh.RecalculateBounds();
        }
    }
    public static IGB283Vector3 GetObjectCenter(GameObject gameObject)
    {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        Mesh gameObjectMesh = meshFilter.mesh;
        IGB283Vector3[] vertices = gameObjectMesh.vertices.Select(v => (IGB283Vector3)v).ToArray();
        IGB283Vector3 center = new IGB283Vector3(0, 0, 0);
        foreach (IGB283Vector3 vertex in vertices)
        {
            center += vertex;
        }
        center /= vertices.Length;
        return center;
    }

    public static void MoveObject(GameObject gameObject, IGB283Vector3 position)
    {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        Mesh gameObjectMesh = meshFilter.mesh;

        IGB283Vector3[] vertices = gameObjectMesh.vertices.Select(v => (IGB283Vector3)v).ToArray();

        Matrix3x3 translationMatrix = Matrix3x3.Translate(position);

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = translationMatrix.MultiplyPoint(vertices[i]);
        }

        gameObjectMesh.vertices = vertices.Select(v => (Vector3)v).ToArray();
        gameObjectMesh.RecalculateBounds();
    }

    void MoveObjectBetweenTwoPoints(IGB283Vector3 pointOne, IGB283Vector3 pointTwo, GameObject gameObject, ref float direction, float translatingSpeed)
    {
        IGB283Vector3 currentPosition = GetObjectCenter(gameObject);

        IGB283Vector3 targetPosition = direction < 0 ? pointTwo : pointOne;
        IGB283Vector3 directionVector = (targetPosition - currentPosition).normalized;
        float distanceToMove = translatingSpeed * Time.deltaTime;
        IGB283Vector3 movementVector = directionVector * distanceToMove;

        gameObject.GetComponent<Movement>().MoveByOffset(movementVector);

        float distanceToTarget = IGB283Vector3.Distance(currentPosition, targetPosition);
        if (distanceToTarget <= 1)
        {


            direction *= -1;

        }
    }

    // Update is called once per frame
    void Update()
    {
       
        WobbleHead();
        IGB283Vector3 pointOne = new IGB283Vector3(10,0,0);
        IGB283Vector3 pointTwo = new IGB283Vector3(-10,0,0);
        if(qutjrGameObject != null)
        {
            MoveObjectBetweenTwoPoints(pointOne, pointTwo, qutjrGameObject,ref direction, translationSpeed);
        }
        
       
  


    }
}
