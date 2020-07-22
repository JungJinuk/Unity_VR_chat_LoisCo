using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MAROMAV.CoworkSpace
{
    public class PermissionRequestPresenter : MonoBehaviour
    {
        [SerializeField]
        Text _message;

        public void OnRequestPermissionButtonClick()
        {
            GamePermissions.Instance.RequestPermissions(_message);
        }
    }
}
