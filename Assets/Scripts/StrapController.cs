using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO.IsolatedStorage;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;

namespace YH_Class
{
    public class StrapController : MonoBehaviour
    {
        public GameObject InnerStrap;
        public GameObject OuterStrap;
        public UnityEngine.Color StrapColor;
        public Camera CvtCamera;
        public GameObject bird;
        public float StrapHeight = 0.1f;
        public float StrapMaxLength = 1.5f;
        public float StrapMaxPower = 20;
        private bool StartDrag = false;
        private bool Shoting = false;

        Vector2 MousePosition;
        Vector2 ShotingMousePosition;

        private Vector2 InnerPos;
        private Vector2 OuterPos;
        private Vector2 BetweenStrapCenter;
        
        private Rect availableArea;
        // Start is called before the first frame update
        LineRenderer InnerLine;
        LineRenderer OuterLine;

        void Start()
        {

            InnerPos = InnerStrap.transform.position;
            OuterPos = OuterStrap.transform.position;
            BetweenStrapCenter = Vector3.Lerp(OuterPos, InnerPos, 0.5f);
            availableArea.width = 1;
            availableArea.height = 1;
            availableArea.x = BetweenStrapCenter.x - availableArea.width/2;
            availableArea.y = BetweenStrapCenter.y - availableArea.height/2;

            CreateStrap(ref InnerLine, StrapHeight, StrapColor);
            InnerLine.sortingLayerName = "InnerBirdGun";
            InnerLine.alignment = LineAlignment.TransformZ;

            CreateStrap(ref OuterLine, StrapHeight, StrapColor);
            OuterLine.sortingLayerName = "OuterBirdGun";
            OuterLine.alignment = LineAlignment.TransformZ;

            Vector3 PosForDrawLine = BetweenStrapCenter;
            PosForDrawLine.z = -1;
            InnerLine.SetPosition(0, InnerPos);
            InnerLine.SetPosition(1, PosForDrawLine);

            OuterLine.SetPosition(0, OuterPos);
            OuterLine.SetPosition(1, PosForDrawLine);

            ReloadBirds(bird);

            YH_SingleTon.YH_ObjectPool.Instance.LoadAllPrefabs();
        }

        // Update is called once per frame
        void Update()
        {
            YH_Debug.DebugUtil.DrawRect(availableArea);
            if(!Shoting)
                MouseInput();
            else
            {
                ShotingBird();
            }
            //bird.transform.forward  = Vector3.Normalize(BetweenStrapCenter - MousePosition);



        }
        private void MouseInput()
        {
            if(bird != null)
            {
                MouseClick();
                MouseDrag();
                MouseButtonUp();
            }
           
        }
        public void ReloadBirds(GameObject obj)
        {
            obj.transform.position = BetweenStrapCenter;
            obj.GetComponentInChildren<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }
        private void CreateStrap(ref LineRenderer line,float width,UnityEngine.Color color)
        {
            line = new GameObject("Line").AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Diffuse"));
            line.positionCount = 2;
            line.startWidth = width;
            line.endWidth = width;
            line.startColor = color;
            line.endColor = color;
            line.useWorldSpace = false;
            line.receiveShadows = false;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        private void MouseDrag()
        {
            if (StartDrag && Input.GetMouseButton(0))
            {
                // 마우스 왼쪽 버튼을 누르고 있는 도중의 처리
                MousePosition = Input.mousePosition;
                MousePosition = CvtCamera.ScreenToWorldPoint(MousePosition);
                Vector2 direction = Vector3.Normalize(MousePosition - BetweenStrapCenter);
                if (Vector3.Cross(new Vector3(0, 1, 0), direction).z < 0)
                {
                    //반대쪽 각도로 드래그 허용 안함.
                    return;
                }
                float cosTheta = Vector2.Dot(direction, new Vector2(0, 1));

                Vector2 NewMousePosition = MousePosition;
                if ((NewMousePosition - BetweenStrapCenter).magnitude > StrapMaxLength)
                {
                    //Vector2 mouseDirection = Vector3.Normalize(MousePosition - BetweenStrapCenter);
                    NewMousePosition = BetweenStrapCenter + direction * StrapMaxLength;
                    MousePosition = NewMousePosition;
                }

                Vector3 PosForDrawLine = NewMousePosition;
                PosForDrawLine.z = -1;
                InnerLine.SetPosition(0, InnerPos);
                InnerLine.SetPosition(1, PosForDrawLine);

                OuterLine.SetPosition(0, OuterPos);
                OuterLine.SetPosition(1, PosForDrawLine);

                bird.transform.position = PosForDrawLine;

                //bird의 진행방향에 따라 회전.y축정렬을 -90하여 x축으로 바꿔줌.
                bird.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Acos(cosTheta)*Mathf.Rad2Deg - 90);
                //lr.SetPosition(2, OuterPos);
            }
        }
        private void MouseClick()
        {
            // 마우스 왼쪽 버튼을 눌렀을 때의 처리
            if (Input.GetMouseButtonDown(0))
            {
                MousePosition = Input.mousePosition;
                MousePosition = CvtCamera.ScreenToWorldPoint(MousePosition);

                if (true == availableArea.Contains(MousePosition))
                {
                    StartDrag = true;
                    Debug.Log("mouse in Area");
                }
                else
                    StartDrag = false;
            }
        }
        private void MouseButtonUp()
        {
            if (StartDrag && Input.GetMouseButtonUp(0))
            {
                StartDrag = false;
                if ((MousePosition - BetweenStrapCenter).magnitude < 0.2)
                {
                    Shoting = false;
                    //Debug.Log("Shoting False");
                }
                else
                {
                    ShotingMousePosition = MousePosition;
                    Shoting = true;

                    StartCoroutine(StrapReturn());
                   
                }
                // 마우스 왼쪽 버튼을 뗄 때의 처리
            }
        }
        
      
        private void ShotingBird()
        {
            //슈팅 방향 설정.
            Vector2 shotingDir = (BetweenStrapCenter - ShotingMousePosition).normalized ;
            //슈팅 파워 설정.
            // (strapMaxLength / length) * MaxForce;
            float strapLength = (BetweenStrapCenter - ShotingMousePosition).magnitude;
            float fForce = (strapLength / StrapMaxLength) * StrapMaxPower;

            //bird세팅.
            Rigidbody2D rgidBdy = bird.GetComponentInChildren<Rigidbody2D>();
            rgidBdy.bodyType = RigidbodyType2D.Dynamic;
            rgidBdy.AddForce(shotingDir * fForce, ForceMode2D.Impulse);
            Shoting = false;
            bird = null;
        }

        IEnumerator StrapReturn()
        {
            for(float f = 0; f <= 1;)
            {
                f += StrapMaxPower * Time.deltaTime;
                Vector2 lerped = Vector2.Lerp(ShotingMousePosition, BetweenStrapCenter, f);

                Vector3 PosForDrawLine = lerped;
                PosForDrawLine.z = -1;
                InnerLine.SetPosition(0, InnerPos);
                InnerLine.SetPosition(1, PosForDrawLine);

                OuterLine.SetPosition(0, OuterPos);
                OuterLine.SetPosition(1, PosForDrawLine);
                if (f >= 1)
                {
                    Shoting = false;
                    break;
                }
                yield return null;
            }
           
        }
    }
   
}
