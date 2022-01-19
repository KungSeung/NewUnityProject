using nuitrack.Frame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UserFrame, ColorFrame, RayCast
/// </summary>

namespace Frame
{
    /// <summary>
    /// UserFrame, ColorFrame 생성과 RayCast를 통한 바닥감지
    /// </summary>
    /// <remarks>
    /// 1. Canvas에서 RawImage 타입을 담을 UserFrameImg, ColorFrameImg 생성<br />
    /// UserFrame과 ColorFrame을 텍스쳐화 한 후 Load한다<br />
    /// 2. Calibration 후 바닥감지[구현예정] -> 바닥감지 후 Plane 생성<br />
    /// 3. RayCast를 통해서 바닥과 충돌이 감지되면 해당 지점을 Virtual Pivot이라고 설정<br />
    /// Virtual Pivot의 Pos를 중심으로 SkeletonFrame 생성할 예정<br /> 
    /// </remarks>
    public class CreateFrame : MonoBehaviour
    {
        public RaycastHit hit;
        public float MaxDistance = 15f;                   // Ray의 최대 감지거리(길이)

        /// <summary>
        ///  RawImage 타입의 Color Frame 을 담을 변수
        /// </summary>
        [SerializeField] public RawImage rawColorImg;
        /// <summary>
        ///  RawImage 타입의 User Frame 을 담을 변수
        /// </summary>
        [SerializeField] RawImage rawUserImg;

        /// <summary>
        /// 가상원점
        /// </summary>
        public Vector3 VirtualPivot;                

        public int Btn_Count = 0;

        /// <summary>
        /// Game이 시작되면 UserFrame, ColorFrame Load, RayCasting
        /// </summary>
        /// <remarks>
        /// UserFrame은 9:16, ColorFrame은 3:4 비율<br />
        /// Ray를 통해서 바닥의 장판감지, hit point로 pivot 설정
        /// </remarks>
        public void Start()
        {
            nuitrack.Skeleton skeleton = CurrentUserTracker.CurrentSkeleton;
            NuitrackManager.onColorUpdate += LoadColorFrame;                                        // ColorFrame 360 : 640 -> 9 : 16 화면비율
            NuitrackManager.onUserTrackerUpdate += LoadUserFrame;                           // UserFrame 480 : 640 -> 3 : 4 화면비율

            // Test할땐 Ray를 쏴서 직관적이게 확인가능
            // Debug.DrawRay(transform.position, transform.forward * MaxDistance, Color.red, 0.3f);            

            if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, 1000))    
            {
                if (hit.collider.name == "Plane")                                                                // 장판인식 -> 카메라에서 Ray를 쏴서 Plane에 충돌한다면 인식했다고 생각
                {
                    hit.transform.GetComponent<MeshRenderer>().material.color = Color.black;                      // 장판 검은색으로 변경
                    VirtualPivot = hit.point;                                                                                                                           // 충돌 위치 & 회전축(Pivot) 
                }
            }
        }

        /// <summary>
        /// [구현예정]<br />
        /// 촬영 3번(정면, 측면, 후면) 완료시 다음 씬으로 넘어가기<br />
        /// 해당 위치에 UserFrame, ColorFrame 소멸 
        /// </summary>
        /// <example>
        /// <code>
        ///  if (Btn_Count == 3) 
          ///  {
        ///        ColorFrameDestroy();
        ///         UserFrameDestroy();
          ///  }
        /// </code>
        /// </example>
    public void Update()
        {
            if (Btn_Count == 3) 
            {
                ColorFrameDestroy();    
                UserFrameDestroy();    
            }
        }

        /// <summary>
        ///  ColorFrame의 생성, Texture화
        /// </summary>
        /// <param name="colorframe"></param>
        public void LoadColorFrame(nuitrack.ColorFrame colorframe)
        {
            rawColorImg.texture = colorframe.ToTexture2D();   // ColorFrame 텍스쳐화
        }

        /// <summary>
        /// ColorFrame의 소멸
        /// </summary>
        public void ColorFrameDestroy()
        {
            NuitrackManager.onColorUpdate -= LoadColorFrame;
        }

        /// <summary>
        /// UserFrame의 생성, Texture화
        /// </summary>
        /// <param name="userframe"></param>
        public void LoadUserFrame(nuitrack.UserFrame userframe)
        {
            rawUserImg.texture = userframe.ToTexture2D();
            
        }

        /// <summary>
        /// UserFrame의 소멸
        /// </summary>
        public void UserFrameDestroy()
        {
            NuitrackManager.onUserTrackerUpdate -= LoadUserFrame;
        }
    }
}
