using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.IK;

public class Movement : MonoBehaviour
{

    private GameObject qutjrGameObject; // Base
    private GameObject upperArmGameObject; // Upper Arm
    private GameObject lowerArmGameObject; // Lower Arm
    private GameObject wristGameObject; // Wrist
    private GameObject groundGameObject;

    public GameObject child;
    public Vector3 jointLocation;
    public Vector3 jointOffset;
    public float angle;
    public float lastAngle;


    private float jumpDirection = -1f;
    private float jumpSpeed = 10f;
    public float jumpHeight = 0.5f;

    private float direction = -1f;
    private float translationSpeed = 5;

    private float groundY = 0.25f;
    private float peakY;
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



    public void RotateAroundPointY(Vector3 point, Vector3 angles)
    {


        Matrix4x4 rotationMatrix = Matrix3x3.RotateCustom(angles * Mathf.Deg2Rad);


        Matrix4x4 T1 = Matrix4x4.Translate(-point);
        Matrix4x4 T2 = Matrix4x4.Translate(point);


        Matrix4x4 M = T2 * rotationMatrix * T1;


        MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = M.MultiplyPoint(vertices[i]);
        
        }

       
        jointLocation = M.MultiplyPoint(jointLocation);


        if (child != null)
        {
            child.GetComponent<Movement>().RotateAroundPointY(point, angles);
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();

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

        groundGameObject = GameObject.Find("Ground");

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

       

        peakY = groundY + jumpHeight;


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

    public void ResetLastAngleRecursively()
    {
        lastAngle = 0f;

        if (child != null)
        {
            child.GetComponent<Movement>().ResetLastAngleRecursively();
        }
    }

    private int rotationSign = 1; 
    public void FlipRotationSignRecursively()
    {
        rotationSign *= -1;

        if (child != null)
        {
            child.GetComponent<Movement>().FlipRotationSignRecursively();
        }
    }
    void MoveObjectBetweenTwoPoints(IGB283Vector3 pointOne, IGB283Vector3 pointTwo, GameObject gameObject, ref float direction, float translatingSpeed)
    {
        IGB283Vector3 currentPosition = GetObjectCenter(gameObject);


        IGB283Vector3 targetPosition = direction < 0 ? pointTwo : pointOne;
        IGB283Vector3 directionVector = (targetPosition - currentPosition).normalized;
        float distanceToMove = translatingSpeed * Time.deltaTime;
        IGB283Vector3 movementVector = directionVector * distanceToMove;

        gameObject.GetComponent<Movement>().MoveByOffset(movementVector);

    
        IGB283Vector3 newPosition = GetObjectCenter(gameObject);
        float distanceToTarget = IGB283Vector3.Distance(newPosition, targetPosition);


        if (distanceToTarget <= 2)
        {
            direction *= -1;
            gameObject.GetComponent<Movement>().RotateAroundPointY(jointLocation, new Vector3(0, 180, 0));
            gameObject.GetComponent<Movement>().FlipRotationSignRecursively();
        
        }
      
    }


  
    public void RotateAroundPointWithoutUpdatingJoints(Vector3 point, float deltaAngle)
    {
        float deltaAngleRad = deltaAngle * Mathf.Deg2Rad;

        Matrix3x3 T1 = Matrix3x3.Translate(-point);
        Matrix3x3 R = Matrix3x3.Rotate(deltaAngleRad);
        Matrix3x3 T2 = Matrix3x3.Translate(point);
        Matrix3x3 M = T2 * R * T1;

        MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = M.MultiplyPoint(vertices[i]);
        }
        mesh.vertices = vertices;

       

        if (child != null)
        {
            child.GetComponent<Movement>().RotateAroundPointWithoutUpdatingJoints(point, deltaAngle);
        }
    }

    void Nodding(GameObject gameObject, float range, float speed)
    {
        
            float newAngle = Mathf.Sin(Time.time * speed) * range;

          
            float deltaAngle = rotationSign * (newAngle - lastAngle);

            child.GetComponent<Movement>().RotateAroundPointWithoutUpdatingJoints(jointLocation, deltaAngle);

            lastAngle = newAngle;

            MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilter.mesh.RecalculateBounds();
            }
      
    }




    void Jump(GameObject gameObject, float jumpSpeed, ref float jumpDirection)
    {
        IGB283Vector3 currentPosition = GetObjectCenter(gameObject);
        float targetY = (jumpDirection > 0) ? peakY : groundY;

        float step = jumpSpeed * Time.deltaTime;
        float newY = Mathf.MoveTowards(currentPosition.y, targetY, step);
        float deltaY = newY - currentPosition.y;

  
        IGB283Vector3 movementVector = new IGB283Vector3(0, deltaY, 0);
        gameObject.GetComponent<Movement>().MoveByOffset(movementVector);


        if (Mathf.Abs(newY - targetY) <= 0.01f)
        {
            jumpDirection *= -1; 
        }
    }



    void Update()
    {
     
        
        IGB283Vector3 pointOne = new IGB283Vector3(30, 0.5f, 0);
        IGB283Vector3 pointTwo = new IGB283Vector3(-30, 0.5f, 0);
      
   
       
        if (qutjrGameObject != null)
        {
            IGB283Vector3 currentPostion = GetObjectCenter(this.gameObject);
            IGB283Vector3 postionOfGameObjectBetweenGround = new IGB283Vector3(currentPostion.x, 0, currentPostion.z);
            IGB283Vector3 targetJump = new IGB283Vector3(currentPostion.x, jumpHeight, currentPostion.z);
            Jump(qutjrGameObject, jumpSpeed, ref jumpDirection);
            MoveObjectBetweenTwoPoints(pointOne, pointTwo, qutjrGameObject, ref direction, translationSpeed);
         

        }

        if (lowerArmGameObject != null)
        {
            Nodding(lowerArmGameObject, 25, 5);

        }
        if (upperArmGameObject != null)
        {
            Nodding(upperArmGameObject, 15, 5);

        }



    }
}
