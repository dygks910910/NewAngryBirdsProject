using UnityEngine;
using System.Collections;

namespace YH_Class
{
    public class CamFollow : MonoBehaviour
    {

        public Transform bird;
        public GameObject WorldRect;
        Camera cam;
        private Vector3 originPosition;
        private float originSize;
        private float camSize;
        private Vector3 camPosition;
        private WorldArea worldArea;
       
        float leftBoundary;
        float rightBoundary;
        float topBoundary;
        float BottomBoundary;

        public Rect worldRect;
        float screenWidthFactor;
        private void Start()
        {
            cam = GetComponent<Camera>();
            worldArea = WorldRect.GetComponent<WorldArea>();
            originPosition = transform.position;
            originSize = cam.orthographicSize;
            camSize = originSize;
            camPosition = originPosition;

            leftBoundary = worldArea.worldRect.xMin;
            rightBoundary = worldArea.worldRect.xMax;
            topBoundary = worldArea.worldRect.yMin;
            BottomBoundary = worldArea.worldRect.yMax;
            worldRect = worldArea.worldRect;
            screenWidthFactor = (float)Screen.width / (float)Screen.height;
        }
        void Update()
        {
            YH_Debug.DebugUtil.DrawRect(leftBoundary,topBoundary,rightBoundary,BottomBoundary);

            //if (Input.GetKeyDown(KeyCode.RightArrow))
            //{
            //    Vector3 newPos = transform.position;
            //    newPos.x = newPos.x+1;
            //    newPos.x = Mathf.Clamp(newPos.x,
            //        leftBoundary +(camSize * screenWidthFactor),
            //        rightBoundary - (camSize * screenWidthFactor));
            //    transform.position = newPos;

            //}
            //else if (Input.GetKeyDown(KeyCode.LeftArrow))
            //{
            //    Vector3 newPos = transform.position;
            //    newPos.x = newPos.x - 1;
            //    newPos.x = Mathf.Clamp(newPos.x,
            //         leftBoundary + (camSize * screenWidthFactor),
            //         rightBoundary - (camSize * screenWidthFactor));
            //    transform.position = newPos;
            //}


            if (bird != null && bird.parent.gameObject.activeSelf)
            {
                Vector3 newPos = transform.position;
                newPos.x = bird.position.x;
                newPos.y = bird.position.y;

                newPos.x = Mathf.Clamp(newPos.x,
                    leftBoundary + (camSize * screenWidthFactor),
                    rightBoundary - (camSize * screenWidthFactor));

                newPos.y = Mathf.Clamp(newPos.y,
                   BottomBoundary + camSize,
                   topBoundary - camSize);
                transform.position = newPos;
            }
            else
            {
                camSize = originSize;
                camPosition = originPosition;
                cam.transform.position = originPosition;
                cam.orthographicSize = originSize;
                bird = null;
            }
        }
        public void SetOriginState()
        {
            camSize = originSize;
            camPosition = originPosition;
            cam.transform.position = originPosition;
            cam.orthographicSize = originSize;
            bird = null;
        }
    }
}
