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
        }
        void Update()
        {
            if (bird != null && bird.parent.gameObject.activeSelf )
            {
                Vector3 newPos = transform.position;
                newPos.x = bird.position.x;
                newPos.y = bird.position.y;

                newPos.x = Mathf.Clamp(newPos.x,
                    leftBoundary+ (camSize * (Screen.width / Screen.height)),
                    rightBoundary - (camSize * (Screen.width / Screen.height)));

                newPos.y = Mathf.Clamp(newPos.y,
                   BottomBoundary + camSize,
                   topBoundary - camSize);
                transform.position = newPos;
            }
            else
            {
                camSize = originSize;
                camPosition = originPosition;
                //cam.transform.position = originPosition;
                //cam.orthographicSize = originSize;
            }
        }
    }
}
