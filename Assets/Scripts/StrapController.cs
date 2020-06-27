using System.Collections;
using TMPro.EditorUtilities;
using UnityEngine;


namespace YH_Class
{
    public class StrapController : MonoBehaviour
    {
      
        public UnityEngine.Color StrapColor;
        public Camera mainCamera;
        public GameObject bird;
        //public GameManager gameManager;

        

        private float StrapHeight = 0.1f;
        private float StrapMaxLength = 1.5f;
        private float StrapMaxPower = 20;
        private bool StartDrag = false;
        private bool Shoting = false;
        public GameObject InnerStrap;
        public GameObject OuterStrap;
        private Vector2 MousePosition;
        private Vector2 ShotingMousePosition;
        private Vector2 InnerPos;
        private Vector2 OuterPos;
        private Vector2 BetweenStrapCenter;
        private Vector3 cameraOriginPosition;
        private Rect availableArea;
        private CamFollow camfllow;
        // Start is called before the first frame update
        LineRenderer InnerLine;
        LineRenderer OuterLine;
        //private CamFollow camfllow;
        public delegate void ShotingDo();
        public event ShotingDo shotingEventHandler;
        private void Start()
        {
            InnerPos = InnerStrap.transform.position;
            OuterPos = OuterStrap.transform.position;
            BetweenStrapCenter = Vector3.Lerp(OuterPos, InnerPos, 0.5f);
            availableArea.width = 1;
            availableArea.height = 1;
            availableArea.x = BetweenStrapCenter.x - availableArea.width / 2;
            availableArea.y = BetweenStrapCenter.y - availableArea.height / 2;

            CreateStrap(ref InnerLine, StrapHeight, StrapColor);
            InnerLine.sortingLayerName = "InnerBirdGun";
            InnerLine.alignment = LineAlignment.TransformZ;

            CreateStrap(ref OuterLine, StrapHeight, StrapColor);
            OuterLine.sortingLayerName = "OuterBirdGun";
            OuterLine.alignment = LineAlignment.TransformZ;

            Vector3 PosForDrawLine = BetweenStrapCenter;

            SetStrapLine(InnerPos, OuterPos, PosForDrawLine);
            cameraOriginPosition = mainCamera.transform.position;
            camfllow = mainCamera.GetComponent<CamFollow>();
            //ReloadBirds(gameManager.GetNextBird());
            shotingEventHandler += ShotingBird;
        }
        // Update is called once per frame
        void Update()
        {
            YH_Debug.DebugUtil.DrawRect(availableArea);
            if (!Shoting)
                MouseInput();
            else
            {
                shotingEventHandler();
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
            obj.GetComponentInChildren<CapsuleCollider2D>().enabled = false;
            bird = obj;
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
                MousePosition = mainCamera.ScreenToWorldPoint(MousePosition);
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

                SetStrapLine(InnerPos, OuterPos, PosForDrawLine);
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
                MousePosition = mainCamera.ScreenToWorldPoint(MousePosition);

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
            bird.GetComponentInChildren<CapsuleCollider2D>().enabled = true;

            rgidBdy.AddForce(shotingDir * fForce, ForceMode2D.Impulse);

            bird.GetComponentInChildren<BirdAnimationChanger>().birdState = eBirdState.FLY;
            camfllow.bird = bird.transform.Find(bird.name);
            Shoting = false;
            bird = null;
        }
        private void SetStrapLine(Vector2 inner,Vector2 outer,Vector3 stretchDest)
        {
            stretchDest.z = -1;
            InnerLine.SetPosition(0, inner);
            InnerLine.SetPosition(1, stretchDest);

            stretchDest.z = -2;
            OuterLine.SetPosition(0, outer);
            OuterLine.SetPosition(1, stretchDest);
        }
        IEnumerator StrapReturn()
        {
            for(float f = 0; f <= 1;)
            {
                f += StrapMaxPower * Time.deltaTime;
                Vector2 lerped = Vector2.Lerp(ShotingMousePosition, BetweenStrapCenter, f);

                Vector3 PosForDrawLine = lerped;
                SetStrapLine(InnerPos, OuterPos, PosForDrawLine);
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
