using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MAROMAV.CoworkSpace
{
    public class GamePermissions : MonoBehaviour
    {
        //  사용자에게 받아야 하는 권한 명령 리스트. [안드로이드]
        private static string[] permissionNames = { "android.permission.RECORD_AUDIO", "android.permission.WRITE_EXTERNAL_STORAGE" };

        private static List<GvrPermissionsRequester.PermissionStatus> permissionList =
            new List<GvrPermissionsRequester.PermissionStatus>();

        public static GamePermissions Instance { get; private set; }

        //  권한 부여 여부
        public bool IsRecodePermissionAccepted
        {
            get
            {
                return GvrPermissionsRequester.Instance.IsPermissionGranted(permissionNames[0]);
            }
        }

        void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// 해당 앱에 대한 권한을 요청한다
        /// </summary>
        /// <param name="message">권한 요청 시에 사용자에 표현해줄 UI Text</param>
        public void RequestPermissions(Text message)
        {
            GvrPermissionsRequester permissionRequester = GvrPermissionsRequester.Instance;
            if (permissionRequester == null)
            {
                message.text = "Permission requester cannot be initialized.";
                return;
            }
            Debug.Log("Permissions.RequestPermisions: Check if permission has been granted");
            if (!permissionRequester.IsPermissionGranted(permissionNames[0]))
            {
                Debug.Log("Permissions.RequestPermisions: Permission has not been previously granted");
                if (permissionRequester.ShouldShowRational(permissionNames[0]))
                {
                    message.text = "This game needs to access external storage.  Please grant permission when prompted.";
                }
                permissionRequester.RequestPermissions(permissionNames,
                    (GvrPermissionsRequester.PermissionStatus[] permissionResults) =>
                    {
                        permissionList.Clear();
                        permissionList.AddRange(permissionResults);
                        string msg = "";
                        foreach (GvrPermissionsRequester.PermissionStatus p in permissionList)
                        {
                            msg += p.Name + ": " + (p.Granted ? "Granted" : "Denied") + "\n";
                        }
                        message.text = msg;
                    });
            }
            else
            {
                message.text = "ExternalStorage permission already granted!";
                GameView.Instance.ShowWelcomePanel();
            }
        }
    }
}