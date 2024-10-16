using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.IK;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Movement : MonoBehaviour
{

    private GameObject qutjrGameObject; // Base
    private GameObject upperArmGameObject; // Upper Arm
    private GameObject lowerArmGameObject; // Lower Arm
    private GameObject wristGameObject; // Wrist
    private GameObject groundGameObject;

    private GameObject qutjrGameObject2; // Base
    private GameObject upperArmGameObject2; // Upper Arm
    private GameObject lowerArmGameObject2; // Lower Arm
    private GameObject wristGameObject2; // Wrist
    private GameObject groundGameObject2;

    private IGB283Vector3[] originalVertices;

    public GameObject child;
    public Vector3 jointLocation;
    public Vector3 jointOffset;
    private float angle;
    private float lastAngle;
    private float lastNodAngle = 0f;

    private float lastSlumpAngle = 0f;
    private float lastSlumpAngle2 = 0f;

    private bool isSlumpted = false;
    private bool isSlumpted2 = false;

    private bool hasReturnedToOriginalAngle = false;
    private bool hasReturnedToOriginalAngle2 = false;
    
    private float slumptTimer = 0f;
    private float slumptTimer2 = 0f;
    
    private float slumptDuration = 5f;
    private float slumptDuration2 = 5f;

    private float slumptAngle = 65f;
    private float slumptAngle2 = -65f;


    private float highJumpDirection = 1f;
    private float highJumpSpeed = 10f;
    private float highJumpHeight = 3f;
    private bool isHighJumping = false;


    private List<IGB283Vector3> jumpCurvePoints = new List<IGB283Vector3>();
    private float forwardJumpT = 0f;
    private float forwardJumpDuration = 1f;


    private float forwardJumpDirection = 1f;
    private float forwardJumpSpeed = 2f;
    private float forwardJumpHeight = 2f;
    private float forwardJumpDistance = 5f;
    private bool isForwardJumping = false;

    private float jumpDirection = -1f;
    private float jumpSpeed = 5f;
    private float jumpHeight = 2f;

    private float direction = -1f;
    private float direction2 = 1f;
    private float translationSpeed = 5;

    private float groundY = 0.25f;


    private const int QUTjrStartAngle = 0;
    private const int UpperArmStartAngle = 80;
    private const int LowerArmStartAngle = 20;

    private const int UpperArmStartAngle2 = -80;
    private const int LowerArmStartAngle2 = -20;





    void Start()
    {

        if (child != null)
        {
            child.GetComponent<Movement>().MoveByOffset(jointOffset);

        }
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
                    child.GetComponent<Movement>().RotateAroundPoint(jointLocation, UpperArmStartAngle, lastAngle);
                }
                break;


            case "Lower Arm":
                {
                    lowerArmGameObject = GameObject.Find("Lower Arm");
                    child.GetComponent<Movement>().RotateAroundPoint(jointLocation, LowerArmStartAngle, lastAngle);
                }
                break;


            case "Wrist":
                {
                    wristGameObject = GameObject.Find("Wrist");

                }
                break;

            case "QUTjr2":
                {
                    qutjrGameObject2 = GameObject.Find("QUTjr2");

                }
                break;

            case "Upper Arm2":
                {
                    upperArmGameObject2 = GameObject.Find("Upper Arm2");
                    child.GetComponent<Movement>().RotateAroundPoint(jointLocation, UpperArmStartAngle2, lastAngle);
                }
                break;


            case "Lower Arm2":
                {
                    lowerArmGameObject2 = GameObject.Find("Lower Arm2");
                    child.GetComponent<Movement>().RotateAroundPoint(jointLocation, LowerArmStartAngle2, lastAngle);
                }
                break;


            case "Wrist2":
                {
                    wristGameObject2 = GameObject.Find("Wrist");

                }
                break;




        }





        MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            originalVertices = meshFilter.mesh.vertices.Clone() as IGB283Vector3[];
            meshFilter.mesh.RecalculateBounds();
        }






    }

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

    public IGB283Vector3 CalculateBSplinePoint(float t, IGB283Vector3 p0, IGB283Vector3 p1, IGB283Vector3 p2)
    {

        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        IGB283Vector3 point = (p0 * uu) + (p1 * 2 * u * t) + (p2 * tt);
        return point;
    }

    public void RotateAroundPointWithoutUpdatingJoints(IGB283Vector3 point, float deltaAngle)
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
    void MoveObjectBetweenTwoPoints(IGB283Vector3 pointOne, IGB283Vector3 pointTwo, GameObject gameObject,
                                   ref float direction, float translatingSpeed, ref int rotationSign,
                                   IGB283Vector3 jointLocation)
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







    void Jump(GameObject gameObject, float jumpSpeed, ref float jumpDirection, float jumpHeight, float groundY)
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



    private bool IsOnGround()
    {

        float currentY = GetObjectCenter(qutjrGameObject).y;
        bool onGround = Mathf.Abs(currentY - groundY) <= 0.1f;

        return onGround;
    }


    void JumpForward(GameObject gameObject, float jumpSpeed, float jumpHeight, float jumpDistance, ref float direction,
                     IGB283Vector3 startPosition, List<IGB283Vector3> jumpCurvePoints, float forwardJumpDuration,
                     ref float forwardJumpT, float groundY, IGB283Vector3 jointLocation, ref int rotationSign)
    {
        if (forwardJumpT == 0f)
        {
            jumpCurvePoints.Clear();
            jumpCurvePoints.Add(startPosition);

            IGB283Vector3 peakPoint = new IGB283Vector3(
                startPosition.x + (jumpDistance / 2) * direction,
                startPosition.y + jumpHeight,
                startPosition.z
            );
            jumpCurvePoints.Add(peakPoint);

            IGB283Vector3 endPoint = new IGB283Vector3(
                startPosition.x + jumpDistance * direction,
                groundY,
                startPosition.z
            );
            jumpCurvePoints.Add(endPoint);
        }

        forwardJumpT += (jumpSpeed * Time.deltaTime) / forwardJumpDuration;
        forwardJumpT = Mathf.Clamp01(forwardJumpT);

        IGB283Vector3 p0 = jumpCurvePoints[0];
        IGB283Vector3 p1 = jumpCurvePoints[1];
        IGB283Vector3 p2 = jumpCurvePoints[2];
        IGB283Vector3 newPosition = CalculateBSplinePoint(forwardJumpT, p0, p1, p2);
        IGB283Vector3 currentPosition = GetObjectCenter(gameObject);
        IGB283Vector3 targetPosition = newPosition;
        IGB283Vector3 movementVector = targetPosition - currentPosition;

        gameObject.GetComponent<Movement>().MoveByOffset(movementVector);

        if (forwardJumpT >= 1f)
        {
            isForwardJumping = false;
            forwardJumpT = 0f;
        }

        if (targetPosition.x >= 30f || targetPosition.x <= -30f)
        {
            direction *= -1f;
            gameObject.GetComponent<Movement>().RotateAroundPointY(jointLocation, new Vector3(0, 180, 0));
            gameObject.GetComponent<Movement>().FlipRotationSignRecursively();
            forwardJumpT = 0f;
        }
    }





    void ChangeDirection(KeyCode keyCode, ref float directionGameObject, GameObject gameObject, float desiredDirection)
    {

        if (Input.GetKeyDown(keyCode) && direction != desiredDirection)
        {
            directionGameObject = desiredDirection;
            gameObject.GetComponent<Movement>().RotateAroundPointY(jointLocation, new Vector3(0, 180, 0));
            gameObject.GetComponent<Movement>().FlipRotationSignRecursively();
        }
    }


    void LockGameObjectToGround(GameObject gameObject)
    {
        IGB283Vector3 currentPosition = GetObjectCenter(gameObject);
        float step = jumpSpeed * Time.deltaTime;
        float newY = Mathf.MoveTowards(currentPosition.y, groundY, step);
        float deltaY = newY - currentPosition.y;
        IGB283Vector3 movementVector = new IGB283Vector3(0, deltaY, 0);
        gameObject.GetComponent<Movement>().MoveByOffset(movementVector);
    }

    private void RestoreOriginalVertices()
    {
        MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
        if (meshFilter != null && originalVertices != null)
        {
            meshFilter.mesh.vertices = originalVertices.Select(v => (Vector3)v).ToArray();
            meshFilter.mesh.RecalculateBounds();
        }



        if (child != null)
        {
            child.GetComponent<Movement>().RestoreOriginalVertices();
        }
    }



    void Nodding(GameObject gameObject, float range, float speed)
    {
        float newAngle = Mathf.Sin(Time.time * speed) * range;
        float deltaAngle = rotationSign * (newAngle - lastNodAngle);
        child.GetComponent<Movement>().RotateAroundPointWithoutUpdatingJoints(jointLocation, deltaAngle);
        lastNodAngle = newAngle;
        MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.mesh.RecalculateBounds();
        }
    }





    void Update()
    {
        IGB283Vector3 pointOne = new IGB283Vector3(30, 0.5f, 0);
        IGB283Vector3 pointTwo = new IGB283Vector3(-30, 0.5f, 0);



        KeyCode slumpingKey = KeyCode.Z;
        KeyCode highJumpKey = KeyCode.W;
        KeyCode forwardJumpKey = KeyCode.S;
        KeyCode changeDirectionKeyPositive = KeyCode.D;
        KeyCode changeDirectionKeyNegative = KeyCode.A;
        
        KeyCode slumpingKey2 = KeyCode.Slash;
        KeyCode highJumpKey2 = KeyCode.UpArrow;
        KeyCode forwardJumpKey2 = KeyCode.DownArrow;
        KeyCode changeDirectionKeyPositive2 = KeyCode.RightArrow;
        KeyCode changeDirectionKeyNegative2 = KeyCode.LeftArrow;

        HandleSlumping(slumpingKey, ref isSlumpted, ref hasReturnedToOriginalAngle, ref slumptTimer, slumptDuration, qutjrGameObject, child, ref rotationSign, ref lastSlumpAngle, jointLocation, slumptAngle); 
        if (qutjrGameObject != null && !isSlumpted)
        {
            HandleJumping(qutjrGameObject, highJumpKey, forwardJumpKey, ref isHighJumping, highJumpSpeed, ref highJumpDirection, highJumpHeight, ref isForwardJumping, forwardJumpSpeed, forwardJumpHeight, forwardJumpDistance, ref forwardJumpDirection, ref forwardJumpT, forwardJumpDuration, ref direction, jumpCurvePoints, groundY);

            HandleAutomaticMovement(pointOne, pointTwo, qutjrGameObject,  ref direction, translationSpeed, jumpSpeed, ref jumpDirection, jumpHeight, groundY, ref isHighJumping, ref isForwardJumping, ref isSlumpted);

            HandleChangeDirection(changeDirectionKeyPositive, ref direction,qutjrGameObject, 1f, jointLocation);

            HandleChangeDirection(changeDirectionKeyNegative, ref direction, qutjrGameObject, -1f, jointLocation);
        }

        if (upperArmGameObject != null && !isSlumpted)
        {
            Nodding(upperArmGameObject, 20, 5, ref lastNodAngle, rotationSign, jointLocation);
        }

        HandleSlumping(slumpingKey2, ref isSlumpted2, ref hasReturnedToOriginalAngle2, ref slumptTimer2, slumptDuration2, qutjrGameObject2, child, ref rotationSign, ref lastSlumpAngle2, jointLocation, slumptAngle2); 
        if (qutjrGameObject2 != null && !isSlumpted2)
        {
            HandleJumping(qutjrGameObject2, highJumpKey2, forwardJumpKey2, ref isHighJumping, highJumpSpeed, ref highJumpDirection, highJumpHeight, ref isForwardJumping, forwardJumpSpeed, forwardJumpHeight, forwardJumpDistance, ref forwardJumpDirection, ref forwardJumpT, forwardJumpDuration, ref direction2, jumpCurvePoints, groundY);

            HandleAutomaticMovement(pointOne, pointTwo, qutjrGameObject2, ref direction2, translationSpeed, jumpSpeed, ref jumpDirection, jumpHeight, groundY, ref isHighJumping, ref isForwardJumping, ref isSlumpted2);

            HandleChangeDirection(changeDirectionKeyPositive2, ref direction2, qutjrGameObject2, 1f, jointLocation);

            HandleChangeDirection(changeDirectionKeyNegative2, ref direction2, qutjrGameObject2, -1f, jointLocation);
        }

        if (upperArmGameObject2 != null && !isSlumpted2)
        {
            Nodding(upperArmGameObject2, 20, 5, ref lastNodAngle, rotationSign, jointLocation);
        }
    }

    void HandleSlumping(KeyCode slumpingKey,ref bool isSlumpted,ref bool hasReturnedToOriginalAngle, ref float slumptTimer,float slumptDuration, GameObject gameObject, GameObject child, ref int rotationSign, ref float lastSlumpAngle, IGB283Vector3 jointLocation, float slumptAngle)
    {
        if (Input.GetKeyDown(slumpingKey) && !isSlumpted)
        {
            isSlumpted = true;
            hasReturnedToOriginalAngle = false;
        }

        if (isSlumpted && child != null)
        {
            slumptTimer += Time.deltaTime;

            if (slumptTimer <= slumptDuration)
            {
                if (gameObject != null)
                {
                    LockGameObjectToGround(gameObject);
                }

                if (child != null && gameObject != null)
                {
                    float normalizedTime = slumptTimer / slumptDuration;
                    float newAngle = Mathf.Sin(normalizedTime * Mathf.PI) * slumptAngle;
                    float deltaAngle = rotationSign * (newAngle - lastSlumpAngle);
                    child.GetComponent<Movement>().RotateAroundPointWithoutUpdatingJoints(jointLocation, deltaAngle);
                    lastSlumpAngle = newAngle;
                    MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
                    if (meshFilter != null)
                    {
                        meshFilter.mesh.RecalculateBounds();
                    }
                }

            }
            else
            {
                isSlumpted = false;
                hasReturnedToOriginalAngle = true;
                RestoreOriginalVertices();
                ResetLastAngleRecursively();
                lastSlumpAngle = 0;
                slumptTimer = 0f;
            }
        }
    }

    void HandleJumping(
        GameObject gameObject, KeyCode highJumpKey, KeyCode forwardJumpKey, ref bool isHighJumping, float highJumpSpeed, ref float highJumpDirection, float highJumpHeight, ref bool isForwardJumping, float forwardJumpSpeed, float forwardJumpHeight, float forwardJumpDistance,
        ref float forwardJumpDirection, ref float forwardJumpT, float forwardJumpDuration, ref float direction, List<IGB283Vector3> jumpCurvePoints, float groundY)
    {
        if (Input.GetKeyDown(highJumpKey) && !isHighJumping)
        {
            isHighJumping = true;
            highJumpDirection = 1f;
        }

        if (Input.GetKeyDown(forwardJumpKey) && !isForwardJumping)
        {
            isForwardJumping = true;
            forwardJumpDirection = 1f;
            forwardJumpT = 0f;
        }

        if (isHighJumping)
        {
            Jump(gameObject, highJumpSpeed, ref highJumpDirection, highJumpHeight, groundY);
            if (highJumpDirection < 0 && GetObjectCenter(gameObject).y <= groundY + 0.1f)
            {
                if (Input.GetKey(highJumpKey))
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
        else if (isForwardJumping)
        {
            IGB283Vector3 startPosition = GetObjectCenter(gameObject);
            JumpForward(gameObject, forwardJumpSpeed, forwardJumpHeight, forwardJumpDistance, ref direction, startPosition, jumpCurvePoints, forwardJumpDuration, ref forwardJumpT, groundY, jointLocation, ref rotationSign);

            if (forwardJumpDirection < 0 && GetObjectCenter(gameObject).y <= groundY + 0.1f)
            {
                if (Input.GetKey(forwardJumpKey))
                {
                    isForwardJumping = true;
                    forwardJumpDirection = 1f;
                }
                else
                {
                    isForwardJumping = false;
                }
            }
        }
    }

    void HandleAutomaticMovement(IGB283Vector3 pointOne, IGB283Vector3 pointTwo, GameObject gameObject, ref float direction, float translationSpeed, float jumpSpeed, ref float jumpDirection, float jumpHeight, float groundY, ref bool isHighJumping, ref bool isForwardJumping, ref bool isSlumpted)
    {
        if (!isHighJumping && !isForwardJumping && !isSlumpted)
        {
            MoveObjectBetweenTwoPoints(
                pointOne,
                pointTwo,
                gameObject,
                ref direction,
                translationSpeed,
                ref rotationSign,
                jointLocation);

            Jump(gameObject, jumpSpeed, ref jumpDirection, jumpHeight, groundY);
        }
    }

    void HandleChangeDirection(KeyCode keyCode, ref float directionGameObject, GameObject gameObject, float desiredDirection, IGB283Vector3 jointLocation)
    {
        if (Input.GetKeyDown(keyCode) && directionGameObject != desiredDirection)
        {
            directionGameObject = desiredDirection;
            gameObject.GetComponent<Movement>().RotateAroundPointY(jointLocation, new Vector3(0, 180, 0));
            gameObject.GetComponent<Movement>().FlipRotationSignRecursively();
        }
    }

    void Nodding(GameObject gameObject, float range, float speed, ref float lastNodAngle, int rotationSign, IGB283Vector3 jointLocation)
    {
        float newAngle = Mathf.Sin(Time.time * speed) * range;
        float deltaAngle = rotationSign * (newAngle - lastNodAngle);
        child.GetComponent<Movement>().RotateAroundPointWithoutUpdatingJoints(jointLocation, deltaAngle);
        lastNodAngle = newAngle;
        MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.mesh.RecalculateBounds();
        }
    }

}



