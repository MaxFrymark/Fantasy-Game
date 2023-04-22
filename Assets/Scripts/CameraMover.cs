using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] Camera c;
    [SerializeField] float cameraMaxSize;
    [SerializeField] float cameraMinSize;
    [SerializeField] float cameraResizeSpeed;

    [SerializeField] float cameraMoveSpeed;
    [SerializeField] CameraBorder[] cameraBorders;
    CameraBorder activeCameraBorder = null;
    
    void Update()
    {
        if(Input.mouseScrollDelta.y < 0)
        {
            ZoomOut();
        }

        else if(Input.mouseScrollDelta.y > 0)
        {
            ZoomIn();
        }

        if(activeCameraBorder != null)
        {
            MoveCamera();
        }
    }

    private void ZoomOut()
    {
        if (c.orthographicSize < cameraMaxSize)
        {
            c.orthographicSize += cameraResizeSpeed * Time.deltaTime;
            if (c.orthographicSize > cameraMaxSize)
            {
                c.orthographicSize = cameraMaxSize;
            }
        }

        ResizeBorder();
    }

    private void ZoomIn()
    {
        if (c.orthographicSize > cameraMinSize)
        {
            c.orthographicSize -= cameraResizeSpeed * Time.deltaTime;
            if (c.orthographicSize < cameraMinSize)
            {
                c.orthographicSize = cameraMinSize;
            }
        }

        ResizeBorder();
    }

    private void MoveCamera()
    {
        if(activeCameraBorder == cameraBorders[0])
        {
            transform.Translate(Vector2.up * cameraMoveSpeed * Time.deltaTime);
        }
        else if(activeCameraBorder == cameraBorders[1])
        {
            transform.Translate(Vector2.down * cameraMoveSpeed * Time.deltaTime);
        }
        else if (activeCameraBorder == cameraBorders[2])
        {
            transform.Translate(Vector2.right * cameraMoveSpeed * Time.deltaTime);
        }
        else if (activeCameraBorder == cameraBorders[3])
        {
            transform.Translate(Vector2.left * cameraMoveSpeed * Time.deltaTime);
        }
        else if (activeCameraBorder == cameraBorders[4])
        {
            transform.Translate(new Vector2(1, 1) * cameraMoveSpeed * Time.deltaTime);
        }
        else if (activeCameraBorder == cameraBorders[5])
        {
            transform.Translate(new Vector2(-1, 1) * cameraMoveSpeed * Time.deltaTime);
        }
        else if (activeCameraBorder == cameraBorders[6])
        {
            transform.Translate(new Vector2(1, -1) * cameraMoveSpeed * Time.deltaTime);
        }
        else if (activeCameraBorder == cameraBorders[7])
        {
            transform.Translate(new Vector2(-1, -1) * cameraMoveSpeed * Time.deltaTime);
        }
    }

    public void SetActiveCameraBorder(CameraBorder cameraBorder)
    {
        activeCameraBorder = cameraBorder;
    }

    private void ResizeBorder()
    {
        float cameraSize = c.orthographicSize;
        cameraBorders[0].transform.localPosition = new Vector3(0, cameraSize, 10);
        cameraBorders[0].GetBorder().size = new Vector2(cameraSize * 4 * 0.8f, 1);

        cameraBorders[1].transform.localPosition = new Vector3(0, -cameraSize, 10);
        cameraBorders[1].GetBorder().size = new Vector2(cameraSize * 4 * 0.8f, 1);

        cameraBorders[2].transform.localPosition = new Vector3(cameraSize * 2, 0, 10);
        cameraBorders[2].GetBorder().size = new Vector2(1, cameraSize * 2 * 0.8f);

        cameraBorders[3].transform.localPosition = new Vector3(-cameraSize * 2, 0, 10);
        cameraBorders[3].GetBorder().size = new Vector2(1, cameraSize * 2 * 0.8f);

        cameraBorders[4].transform.localPosition = new Vector3(cameraSize * 2, cameraSize, 10);
        cameraBorders[4].GetBorder().size = new Vector2(cameraSize * 4 * 0.2f, cameraSize * 2 * 0.2f);

        cameraBorders[5].transform.localPosition = new Vector3(-cameraSize * 2, cameraSize, 10);
        cameraBorders[5].GetBorder().size = new Vector2(cameraSize * 4 * 0.2f, cameraSize * 2  * 0.2f);

        cameraBorders[6].transform.localPosition = new Vector3(cameraSize * 2, -cameraSize, 10);
        cameraBorders[6].GetBorder().size = new Vector2(cameraSize * 4 * 0.2f, cameraSize * 2 * 0.2f);

        cameraBorders[7].transform.localPosition = new Vector3(-cameraSize * 2, -cameraSize, 10);
        cameraBorders[7].GetBorder().size = new Vector2(cameraSize * 4 * 0.2f, cameraSize * 2 * 0.2f);
    }
}
