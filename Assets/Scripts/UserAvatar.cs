using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nuitrack.Frame;
using System;

/// <summary>
/// User Skeleton Frame 생성
/// </summary>
namespace User
{
    /// <summary>
    /// Skeleton Frame을 구성하는 Joint, JointConnection 생성
    /// </summary>
    public class UserAvatar : MonoBehaviour    // 3D Skeleton
    {
        [SerializeField] [Range(0f, 1f)] public const float scale = 0.001f;    // Pos 스케일링용 상수

        /// <summary>
        /// Joint 생성 배열
        /// </summary>
        public nuitrack.JointType[] typeJoint;
        /// <summary>
        /// 입력된 Size에 해당하는 GameObject 배열
        /// </summary>
        public GameObject[] CreatedJoint;
        /// <summary>
        /// Joint를 나타낼 prefab
        /// </summary>
        public GameObject PrefabJoint;
        

        [Serializable]
        public struct Connection                          // Joint Connection(관절 연결) 구조체
        {
            public nuitrack.JointType From;         // Joint 시작점
            public nuitrack.JointType To;              // Joint 도착점
        }

        /// <summary>
        /// JointConnection 생성 배열
        /// </summary>
        public Connection[] connectJoint;
        /// <summary>
        /// 입력된 Size에 해당하는 GameObject 배열
        /// </summary>
        public GameObject[] CreatedConnect;
        /// <summary>
        /// JointConnection을 나타낼 prefab
        /// </summary>
        public GameObject PrefabLine;

        /// <summary>
        /// 
        /// </summary>
        public Vector3 VirtualPivot;

        /// <summary>
        /// Unity에서 설정해준 Size값이 Length로 설정<br />
        /// Length값에 따라 Joint와 JointConnection 생성
        /// </summary>
        public void Start()
        {
            CreateJoint(typeJoint, typeJoint.Length);
            CreateJointConnection(connectJoint, connectJoint.Length);
        }

        /// <summary>
        /// Joint 생성
        /// </summary>
        /// <param name="TypeJoint"></param>
        /// <param name="JointLength"></param>
        public void CreateJoint(nuitrack.JointType[] TypeJoint, int JointLength)
        {
            CreatedJoint = new GameObject[JointLength];                // 입력 Size만큼 Joint 배열 생성 

            for (int i = 0; i < JointLength; i++)                                   // Joint 생성
            {
                CreatedJoint[i] = Instantiate(PrefabJoint);                 // 붙여진 prefab을 바탕으로 GameObeject 생성
                CreatedJoint[i].transform.SetParent(transform);      // 부모는 sc가 붙여진 GameObject

            }
        }

        /// <summary>
        ///  ConnectionJoint 생성
        /// </summary>
        /// <param name="ConnectJoint"></param>
        /// <param name="ConnectionLength"></param>
        public void CreateJointConnection(Connection[] ConnectJoint, int ConnectionLength)
        {
            CreatedConnect = new GameObject[ConnectionLength];  // 입력 Size만큼 ConnectJoint 배열 생성 

            for (int i = 0; i < ConnectionLength; i++)                                     // Joint Connection 생성
            {
                CreatedConnect[i] = Instantiate(PrefabLine);                // 붙여진 prefab을 바탕으로 GameObeject 생성
                CreatedConnect[i].transform.SetParent(transform);   // 부모는 sc가 붙여진 GameObject
            }
        }

        /// <summary>
        /// User가 있으면 생성된 Joint와 JointConnection을 위치시킴
        /// </summary>
        public void Update()
        {
            if (CurrentUserTracker.CurrentUser != 0)               // Nuitrack을 통해서 UserTracking : User의 유무 Check
            {
                nuitrack.Skeleton skeleton = CurrentUserTracker.CurrentSkeleton;             // 현재 감지된 User Skeleton

                JointPos(skeleton, typeJoint.Length);
                JointConnectionPos(skeleton, connectJoint.Length);
            }
            
        }

        /// <summary>
        /// Joint Pos설정
        /// </summary>
        /// <param name="skeleton"></param>
        /// <param name="JointLength"></param>
        public void JointPos(nuitrack.Skeleton skeleton, int JointLength)
        {
            for (int i = 0; i < typeJoint.Length; i++)                                   // Joint 생성
            {
                nuitrack.Joint joint = skeleton.GetJoint(typeJoint[i]);
                Vector3 newPosition = scale * joint.ToVector3();                         // 스케일링 : 얻은 데이터를 미터 -> 밀리미터 로 변환
                CreatedJoint[i].transform.position = newPosition;                   // Joint Pos 재설정
            }
        }
        ///
        /// ReTest
        /// 
        
        /// <summary>
        /// JointConnection Pos설정
        /// </summary>
        /// <remarks>
        /// scale값을 통해서 JointConnection의 Pos 설정<br />
        /// Pos는 기본적으로 Joint From과 To의 중점에 위치<br /> 
        /// 회전값은 Joint From 에서 To를 바라보게 설정
        /// </remarks>
        /// <param name="skeleton"></param>
        /// <param name="ConnectionLength"></param>
        public void JointConnectionPos(nuitrack.Skeleton skeleton, int ConnectionLength)
        {
            for (int i = 0; i < connectJoint.Length; i++)                                                            // Joint Connection 생성 
            {
                nuitrack.Joint From = skeleton.GetJoint(connectJoint[i].From);
                nuitrack.Joint To = skeleton.GetJoint(connectJoint[i].To);
                Vector3 newPosition = (From.ToVector3() + To.ToVector3()) * 0.5f;                                    // 관절의 중심점 
                newPosition *= scale;                                                                                                               // 스케일링
                CreatedConnect[i].transform.position = newPosition;                                                            // Joint Connect Pos 재설정
                CreatedConnect[i].transform.rotation = Quaternion.FromToRotation(From.ToVector3(), To.ToVector3());  // From에서 To를 바라보게 회전값 설정
            }
        }

    }

}
/*public class UserAvatar : MonoBehaviour    // 3D Skeleton
{
    [SerializeField] [Range(0f, 1f)] const float scale = 0.001f;    // Pos 스케일링용 상수

    /// <summary>
    /// Joint 생성 배열, GameObject 배열, prefab
    /// </summary>
    public nuitrack.JointType[] typeJoint;   
    GameObject[] CreatedJoint;                    
    public GameObject PrefabJoint;              

    [Serializable]    
    public struct Connection                          // Joint Connection(관절 연결) 구조체
    {
        public nuitrack.JointType From;         // Joint 시작점
        public nuitrack.JointType To;              // Joint 도착점
    }

    /// <summary>
    /// JointConnection 생성 배열, GameObject 배열, prefab
    /// </summary>
    public Connection[] connectJoint;             
    GameObject[] CreatedConnect;                   
    public GameObject PrefabLine;                   

    void Start()
    {
        CreateJoint(typeJoint, typeJoint.Length);
        CreateJointConnection(connectJoint, connectJoint.Length);
    }

    /// <summary>
    /// Joint 생성
    /// </summary>
    /// <param name="TypeJoint"></param>
    /// <param name="JointLength"></param>
    void CreateJoint(nuitrack.JointType[] TypeJoint, int JointLength)
	{
        CreatedJoint = new GameObject[JointLength];                // 입력 Size만큼 Joint 배열 생성 

        for (int i = 0; i < JointLength; i++)                                   // Joint 생성
        {
            CreatedJoint[i] = Instantiate(PrefabJoint);                 // 붙여진 prefab을 바탕으로 GameObeject 생성
            CreatedJoint[i].transform.SetParent(transform);      // 부모는 sc가 붙여진 GameObject

        }
    }

    /// <summary>
    ///  ConnectionJoint 생성
    /// </summary>
    /// <param name="ConnectJoint"></param>
    /// <param name="ConnectionLength"></param>
    void CreateJointConnection(Connection[] ConnectJoint, int ConnectionLength)
	{
        CreatedConnect = new GameObject[ConnectionLength];  // 입력 Size만큼 ConnectJoint 배열 생성 

        for (int i = 0; i < ConnectionLength; i++)                                     // Joint Connection 생성
        {
            CreatedConnect[i] = Instantiate(PrefabLine);                // 붙여진 prefab을 바탕으로 GameObeject 생성
            CreatedConnect[i].transform.SetParent(transform);   // 부모는 sc가 붙여진 GameObject
        }
    }
    
    void Update()
    {
        if (CurrentUserTracker.CurrentUser != 0)               // Nuitrack을 통해서 UserTracking : User의 유무 Check
        {
            nuitrack.Skeleton skeleton = CurrentUserTracker.CurrentSkeleton;             // 현재 감지된 User Skeleton

            JointPos(skeleton, typeJoint.Length);
            JointConnectionPos(skeleton, connectJoint.Length);
        }
    }

    /// <summary>
    /// Joint Pos설정
    /// </summary>
    /// <param name="skeleton"></param>
    /// <param name="JointLength"></param>
    void JointPos(nuitrack.Skeleton skeleton, int JointLength)
	{
        for (int i = 0; i < typeJoint.Length; i++)                                   // Joint 생성
        {
            nuitrack.Joint joint = skeleton.GetJoint(typeJoint[i]);                  
            Vector3 newPosition = scale * joint.ToVector3();                         // 스케일링 : 얻은 데이터를 미터 -> 밀리미터 로 변환
            CreatedJoint[i].transform.position = newPosition;                   // Joint Pos 재설정
        }
    } 

    /// <summary>
    /// JointConnection Pos설정
    /// </summary>
    /// <param name="skeleton"></param>
    /// <param name="ConnectionLength"></param>
    void JointConnectionPos(nuitrack.Skeleton skeleton, int ConnectionLength)
	{
        for (int i = 0; i < connectJoint.Length; i++)                                                            // Joint Connection 생성 
        {
            nuitrack.Joint From = skeleton.GetJoint(connectJoint[i].From);                                    
            nuitrack.Joint To = skeleton.GetJoint(connectJoint[i].To);                                           
            Vector3 newPosition = (From.ToVector3() + To.ToVector3()) * 0.5f;                                    // 관절의 중심점 
            newPosition *= scale;                                                                                                               // 스케일링
            CreatedConnect[i].transform.position = newPosition;                                                            // Joint Connect Pos 재설정
            CreatedConnect[i].transform.rotation = Quaternion.FromToRotation(From.ToVector3(), To.ToVector3());  // From에서 To를 바라보게 회전값 설정
        }
    }

}
*/