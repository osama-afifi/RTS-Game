using UnityEngine;
using System.Collections;

public class CameraControl 
{

    static CameraControl instance;
    public static Camera Cam;
    private Vector3 velocity;
    private float speed;
    private float height;
    private const float yBorderMin = 256f;
    private const float yBorderMax= 10f;
    private const float xBorderMin = 256f;
    private const float xBorderMax = -1f;
    private const float zBorderMin = 15f;
    private const float zBorderMax = 50f;

    public static CameraControl getInstance()
    {
        if (instance == null)
            instance = new CameraControl();
        return instance;
    }
    public CameraControl()
    {
        Cam = Camera.mainCamera;
        Cam.transform.position = new Vector3(0, 250, 0);
        velocity = new Vector3();
        speed = 350;
        height = 30.0f;
    }
    public void GetCameraInput(out bool left, out bool right,out bool up, out bool down, out float mx, out float my, out float mz)
    {
        right = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        left = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        up = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
        down = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        mx = Input.mousePosition.x;
        my = Input.mousePosition.y;
        my = Input.mousePosition.y;
        mz = -1*Input.GetAxis("Mouse ScrollWheel");
    }
    public void Update()
    {
        bool up, down,right,left;
        bool rotUp, rotDown, rotLeft, rotRight;
        rotUp = rotDown = rotLeft = rotRight = false;
        float mx, my, mz;
        GetCameraInput(out left,out right, out up, out down, out mx, out my, out mz);

        rotUp = my > (0.975 * Screen.height);
        rotDown = my < (0.025 * Screen.height);
        rotLeft = mx < (0.025 * Screen.width);
        rotRight = mx > (0.975 * Screen.width);
        Vector3 prevRotation = Cam.transform.rotation.eulerAngles;
        prevRotation.x += rotUp ? -10f : (rotDown ? 10f : 0);
        prevRotation.y += rotLeft ? -10f : (rotRight ? 10f : 0);
        prevRotation.z = 0;
        if (prevRotation.x < 0) prevRotation.x = 0f;
        if (prevRotation.x > 80) prevRotation.x = 80f;
        //prevRotation.y = 358;
        Vector3 velocity = new Vector3();
        if (up || down)
        {
            velocity.z = speed * Mathf.Cos((prevRotation.y / 180.0f) * Mathf.PI);
            velocity.x = speed * Mathf.Sin((prevRotation.y / 180.0f) * Mathf.PI);
            if (down)
            {
                velocity.z *= -1;
                velocity.x *= -1;
            }
        }   
        if (right || left)
        {
          //  Debug.Log("awdale");
            velocity.z = speed * Mathf.Cos(((90 + prevRotation.y) / 180.0f) * Mathf.PI);
            velocity.x = speed * Mathf.Sin(((90 + prevRotation.y) / 180.0f) * Mathf.PI);
            if (left)
            {
                velocity.z *= -1;
                velocity.x *= -1;
            }
        }
        height += mz * speed * 0.02f;

        Vector3 newPos = Cam.transform.position;
        newPos += (velocity * height) / 1200.0f; // kol ma el height yezed sor3et el camera tezed
        newPos.y = height;

        //Debug.Log(newPos);
        //if (newPos.x < xBorderMin) newPos.x = xBorderMin;
        //else if (newPos.x > xBorderMax) newPos.x = yBorderMax;

        //if (newPos.y < yBorderMin) newPos.y = yBorderMin;
        //else if (newPos.y > yBorderMax) newPos.y = yBorderMax;

        //if (newPos.z < zBorderMin) newPos.z = zBorderMin;
        //else if (newPos.z > zBorderMax) newPos.z = zBorderMax;

        Cam.transform.position = Vector3.Lerp(Cam.transform.position, newPos, 4 * Time.deltaTime);
        Cam.transform.localEulerAngles = Vector3.Lerp(Cam.transform.rotation.eulerAngles, prevRotation, 4 * Time.deltaTime);
    }
}
