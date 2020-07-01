using UnityEngine;
using System.Collections;
using YH_SingleTon;
using System.ComponentModel;
using YH_Class;

namespace YH_SingleTon
{
    public class CameraManager : Singleton<CameraManager>
    {

        public Transform bird = null;
        public Camera cam;
        public Vector3 originPosition;
        //[SerializeField]
        public float originSize;
        public bool isTestMode = false;
        private float camSize;
        private Vector3 camPosition;

        float leftBoundary;
        float rightBoundary;
        float topBoundary;
        float BottomBoundary;

        public Rect worldRect;
        float screenWidthFactor;
        static WaitForSeconds wait01Sec = new WaitForSeconds(0.1f);
        static WaitForSeconds wait60per1Sec = new WaitForSeconds(1.0f / 60.0f);
        enum CameraState { IDLE, FOLLOW, RETURN_ORIGIN }
        CameraState camState = CameraState.IDLE;
        public void Init()
        {
            StopAllCoroutines();
            cam = GameObject.Find("Main Camera").GetComponent<Camera>();
            if (cam == null)
            {
                Debug.LogError("Main Camera object is null");
            }
            worldRect = GameObject.Find("WorldRect").GetComponent<WorldArea>().worldRect;
            if (worldRect == null)
            {
                Debug.LogError("worldRect object is null");
            }
            camSize = originSize;
            camPosition = originPosition;

            leftBoundary = YH_SingleTon.WorldArea.Instance.worldRect.xMin;
            rightBoundary = YH_SingleTon.WorldArea.Instance.worldRect.xMax;
            topBoundary = YH_SingleTon.WorldArea.Instance.worldRect.yMin;
            BottomBoundary = YH_SingleTon.WorldArea.Instance.worldRect.yMax;
            worldRect = YH_SingleTon.WorldArea.Instance.worldRect;
            screenWidthFactor = (float)Screen.width / (float)Screen.height;
            if(gameObject.activeInHierarchy)
                StartCoroutine(CheckState());
            routine = null;
        }
        BirdAnimationChanger animChanger;
        Coroutine routine = null;
        IEnumerator CheckState()
        {

            while (true)
            {
                if (bird != null)
                {

                    animChanger = bird.GetComponent<BirdAnimationChanger>();
                    switch (camState)
                    {
                        case CameraState.IDLE:
                            {
                                if (animChanger.birdState == eBirdState.FLY && routine == null)
                                {
                                    camState = CameraState.FOLLOW;
                                    routine = StartCoroutine(FollowCamera());
                                }
                            }
                            break;
                    }
                }
                yield return wait01Sec;
            }

        }
        //void Update()
        //{

        //    YH_Debug.DebugUtil.DrawRect(leftBoundary, topBoundary, rightBoundary, BottomBoundary);
        //    if (bird != null && bird.gameObject.activeSelf)
        //    {

        //    }
        //    else
        //    {
        //        camSize = originSize;
        //        camPosition = originPosition;
        //        cam.transform.position = originPosition;
        //        cam.orthographicSize = originSize;
        //        bird = null;
        //    }

        //}
        IEnumerator SetOriginState()
        {
            camState = CameraState.IDLE;

            for (int i = 0; i < 30; ++i)
            {
                camSize = Mathf.Lerp(camSize, originSize, (1.0f/30)* i);
                camPosition = Vector3.Lerp(cam.transform.position, originPosition, (1.0f / 30.0f) * i);
                cam.transform.position = camPosition;
                cam.orthographicSize = camSize;
                yield return wait60per1Sec;
            }
            bird = null; routine = null;
        }
        IEnumerator FollowCamera()
        {
            Vector3 newPos;
            while (true)
            {
                if (animChanger.birdState == eBirdState.COLLIDED)
                {
                    camState = CameraState.RETURN_ORIGIN;
                    StartCoroutine(SetOriginState());
                    break;
                }
                newPos = cam.transform.position;
                newPos.x = bird.position.x;
                //newPos.y = bird.position.y;

                newPos.x = Mathf.Clamp(newPos.x,
                    leftBoundary + (camSize * screenWidthFactor),
                    rightBoundary - (camSize * screenWidthFactor));

                //newPos.y = Mathf.Clamp(newPos.y,
                //   BottomBoundary + camSize,
                //   topBoundary - camSize);
                cam.transform.position = newPos;

               // t += Time.deltaTime;
               //camSize =  Mathf.Lerp(originSize, originSize - 1, t);
                yield return wait60per1Sec;
            }

        }
    }
}
