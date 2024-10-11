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


    private float highJumpDirection = 1f;
    private float highJumpSpeed = 20f;
    private float highJumpHeight = 5f;
    private bool isHighJumping = false;

    private float forwardJumpDirection = 1f;
    private float forwardJumpSpeed = 10f;
    private float forwardJumpHeight = 3f;
    private float forwardJumpDistance = 5f;
    private bool isFowardJumping = false;

    private float jumpDirection = -1f;
    private float jumpSpeed = 5f;
    private float jumpHeight = 2f;

    private float direction = -1f;
    private float translationSpeed = 5;

    private float groundY = 0.25f;

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

       
        IGB283Vector3 targetPosition = direction < 0 ? new IGB283Vector3(pointTwo.x, currentPosition.y, currentPosition.z)
                                                     : new IGB283Vector3(pointOne.x, currentPosition.y, currentPosition.z);

        IGB283Vector3 directionVector = (targetPosition - currentPosition).normalized;
        float distanceToMove = translatingSpeed * Time.deltaTime;
        IGB283Vector3 movementVector = new IGB283Vector3(directionVector.x * distanceToMove, 0, 0); 

        gameObject.GetComponent<Movement>().MoveByOffset(movementVector);

        IGB283Vector3 newPosition = GetObjectCenter(gameObject);
        float distanceToTarget = IGB283Vector3.Distance(newPosition, targetPosition);

        if (distanceToTarget <= 0.1f) 
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




    void Jump(GameObject gameObject, float jumpSpeed, ref float jumpDirection, float jumpHeight)
    {
        IGB283Vector3 currentPosition = GetObjectCenter(gameObject);
        float peakY = groundY + jumpHeight;
        
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

    void JumpFoward(GameObject gameObject, float jumpSpeed, ref float jumpDirection, float jumpHeight, float jumpDistance)
    {
        IGB283Vector3 currentPosition = GetObjectCenter(gameObject);
        float peakY = groundY + jumpHeight;

        float targetY = (jumpDirection > 0) ? peakY : groundY;

        float step = jumpSpeed * Time.deltaTime;
        float newY = Mathf.MoveTowards(currentPosition.y, targetY, step);
        float deltaY = newY - currentPosition.y;
        float newX = Mathf.MoveTowards(currentPosition.x, currentPosition.x + jumpDistance, step);
        float deltaX = newX - currentPosition.x;    

        IGB283Vector3 movementVector = new IGB283Vector3(deltaX, deltaY, 0);
        gameObject.GetComponent<Movement>().MoveByOffset(movementVector);


        if (Mathf.Abs(newY - targetY) <= 0.01f)
        {
            jumpDirection *= -1;
        }
    }


    void ChangeDirection(KeyCode keyCode, ref float directionGameObject, GameObject gameObject, float desiredDirection)
    {
      
        if (Input.GetKeyDown(keyCode) && direction != desiredDirection) {
            directionGameObject = desiredDirection;
            gameObject.GetComponent<Movement>().RotateAroundPointY(jointLocation, new Vector3(0, 180, 0));
            gameObject.GetComponent<Movement>().FlipRotationSignRecursively();
        }
    }

   


    void Update()
    {
        IGB283Vector3 pointOne = new IGB283Vector3(30, 0.5f, 0);
        IGB283Vector3 pointTwo = new IGB283Vector3(-30, 0.5f, 0);

        if (qutjrGameObject != null)
        {
            if (Input.GetKeyDown(KeyCode.W) && !isHighJumping)
            {
                isHighJumping = true;
                highJumpDirection = 1f;
            }

            if (Input.GetKeyDown(KeyCode.S) && !isFowardJumping) { 
                isFowardJumping = true;
                forwardJumpDirection = 1f;
            }

            if (isHighJumping)
            {
                
                Jump(qutjrGameObject, highJumpSpeed, ref highJumpDirection, highJumpHeight);
                if (highJumpDirection < 0 && GetObjectCenter(qutjrGameObject).y <= groundY + 0.1f)
                {
                    if (Input.GetKey(KeyCode.W))
                    {
                        isHighJumping = true; 
                        highJumpDirection = 1f; 
                    }
                    else
                    {
                        isHighJumping = false; 
                    }
                }


            }
            else if (isFowardJumping) // doesnt work fix tmrw
            {
                JumpFoward(qutjrGameObject, forwardJumpSpeed, ref forwardJumpDirection, forwardJumpDirection, forwardJumpDistance);
                if (forwardJumpDirection < 0 && GetObjectCenter(qutjrGameObject).y <= groundY + 0.1f)
                {
                    if (Input.GetKey(KeyCode.S))
                    {
                        isFowardJumping = true;
                        forwardJumpDirection = 1f;
                    }
                    else
                    {
                        isFowardJumping = false;
                    }
                }
            }
            else
            {
               
                Jump(qutjrGameObject, jumpSpeed, ref jumpDirection, jumpHeight);
            }

            if (isHighJumping == false && isFowardJumping == false)
            {

                MoveObjectBetweenTwoPoints(pointOne, pointTwo, qutjrGameObject, ref direction, translationSpeed);

            }
            ChangeDirection(KeyCode.D, ref direction, qutjrGameObject, 1f);
            ChangeDirection(KeyCode.A, ref direction, qutjrGameObject, -1f);
        }

        if (lowerArmGameObject != null)
        {
            Nodding(lowerArmGameObject, 25, 5);
        }
    }

}
