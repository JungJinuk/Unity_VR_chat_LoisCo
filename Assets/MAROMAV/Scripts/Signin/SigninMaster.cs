using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace MAROMAV.CoworkSpace
{
    public class SigninMaster : MonoBehaviour
    {
        public void InitData()
        {
            PlayerPrefs.DeleteAll();
        }

        [SerializeField]
        private GameObject signinPanel;

        [SerializeField]
        private GameObject registerPanel;

        [SerializeField]
        private GameObject popupMessage;

        [SerializeField]
        private InputField signinEmail;

        [SerializeField]
        private InputField signinPassword;

        [SerializeField]
        private InputField registerEmail;

        [SerializeField]
        private InputField registerPassword;

        [SerializeField]
        private InputField registerConfirmPassword;

        // [SerializeField]
        // private Color invalidColor;

        // [SerializeField]
        // private Color validColor;

        private Text popupMessageText;
        private string message = string.Empty;

        private enum Mode { SIGN_IN, REGISTER }
        private Mode mode = Mode.SIGN_IN;

        private Coroutine selfDestroyPopupMessageCoroutine;

        void Start()
        {
            Init();
        }

        private void ShowPopupMessage(string message, Transform transform, float selfDestroyTime = 5f)
        {
            popupMessage.SetActive(true);
            popupMessageText.text = message;
            popupMessage.transform.position = transform.position;
            if (selfDestroyPopupMessageCoroutine != null)
            {
                StopCoroutine(selfDestroyPopupMessageCoroutine);
            }
            selfDestroyPopupMessageCoroutine = StartCoroutine(SelfDestroyPopupMessage(selfDestroyTime));
        }

        private IEnumerator SelfDestroyPopupMessage(float selfDestroyTime = 5f)
        {
            yield return new WaitForSeconds(selfDestroyTime);
            InitPopupMessage();
        }

        private void InitPopupMessage()
        {
            popupMessage.SetActive(false);
            popupMessageText.text = string.Empty;
            message = string.Empty;
        }

        private void SiginInMode()
        {
            registerEmail.text = string.Empty;
            registerPassword.text = string.Empty;
            registerConfirmPassword.text = string.Empty;
            signinPanel.SetActive(true);
            registerPanel.SetActive(false);
            mode = Mode.SIGN_IN;
        }

        private void RegisterMode()
        {
            signinEmail.text = string.Empty;
            signinPassword.text = string.Empty;
            signinPanel.SetActive(false);
            registerPanel.SetActive(true);
        }

        private void Init()
        {
            SiginInMode();
            popupMessageText = popupMessage.GetComponentInChildren<Text>();
            InitPopupMessage();
        }

        //  sign in 버튼 클릭
        public void OnSigninButtonClick()
        {
            string email = signinEmail.text,
            password = signinPassword.text;

            if (IsValidEmail(email))
            {
                Debug.Log("Try sign in");
                TrySignin(email, password);
            }
        }

        //  sign up 버튼 클릭
        public void OnSignupButtonClick()
        {
            RegisterMode();
        }

        //  create account 버튼 클릭
        public void OnCreateAccountButtonClick()
        {
            string email = registerEmail.text,
            password = registerPassword.text,
            confirmPassword = registerConfirmPassword.text;

            if (IsValidEmail(email) && IsValidPassword(password, confirmPassword))
            {
                Debug.Log("Try create account");
                TryCreateAccount(email, password);
            }
        }

        //  서버에서 로그인 시도
        private void TrySignin(string email, string password)
        {
            for (int i = 0; i < Entity.users.Count; ++i)
            {
                if (Entity.users[i].email == email)
                {
                    if (Entity.users[i].password == password)
                    {
                        Debug.Log("Login success.");
                        Entity.currentUserIndex = i;
                        SceneMaster.Instance.LoadSceneTo("LoisCo_Lobby_2D");

                        return;
                    }
                    else
                    {
                        message = "The password is incorrect.";
                        ShowPopupMessage(message, signinPassword.transform);
                        Debug.Log(message);
                        signinPassword.Select();
                        return;
                    }
                }
            }

            message = "The email address is incorrect.";
            ShowPopupMessage(message, signinEmail.transform);
            Debug.Log(message);
            signinEmail.Select();
        }

        //  서버에서 계정 생성 시도
        private void TryCreateAccount(string email, string password)
        {
            for (int i = 0; i < Entity.users.Count; ++i)
            {
                if (Entity.users[i].email == email)
                {
                    message = "Account already exist.";
                    ShowPopupMessage(message, signinEmail.transform);
                    Debug.Log(message);
                    signinEmail.Select();
                    return;
                }
            }

            //  이메일 인증 추가


            //  ============ 임시 저장 =============
            DummyUserData.Instance.AddNewAccount(email, password);

            //  ===================================
            SiginInMode();
        }

        //  올바른 Email 형식 체크
        private bool IsValidEmail(string email)
        {
            bool isValidForm = false;
            if (string.IsNullOrEmpty(email))
            {
                message = "Email address is not entered.";
                Debug.Log(message);

                if (mode == Mode.SIGN_IN)
                {
                    ShowPopupMessage(message, signinEmail.transform);
                    signinEmail.Select();
                }
                else if (mode == Mode.REGISTER)
                {
                    ShowPopupMessage(message, registerEmail.transform);
                    registerEmail.Select();
                }

                return false;
            }

            isValidForm = Regex.IsMatch(email,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$");

            if (!isValidForm)
            {
                message = "Invalid email address.";
                Debug.Log(message);

                if (mode == Mode.SIGN_IN)
                {
                    ShowPopupMessage(message, signinEmail.transform);
                    signinEmail.Select();
                }
                else if (mode == Mode.REGISTER)
                {
                    ShowPopupMessage(message, registerEmail.transform);
                    registerEmail.Select();
                }

                return false;
            }

            return true;
        }

        //  올바른 비밀번호 형식 체크
        private bool IsValidPassword(string password, string confirmPassword)
        {
            bool isValidForm = false;

            if (string.IsNullOrEmpty(password))
            {
                message = "Password is not entered.";
                ShowPopupMessage(message, registerPassword.transform);
                Debug.Log(message);
                registerPassword.Select();
                return false;
            }

            if (string.IsNullOrEmpty(confirmPassword))
            {
                message = "Confirm password is not entered.";
                ShowPopupMessage(message, registerConfirmPassword.transform);
                Debug.Log(message);
                registerConfirmPassword.Select();
                return false;
            }

            if (!password.Equals(confirmPassword))
            {
                message = "The confirm password is different.";
                ShowPopupMessage(message, registerConfirmPassword.transform);
                Debug.Log(message);
                registerConfirmPassword.Select();
                return false;
            }

            isValidForm = Regex.IsMatch(password, @"^[a-zA-Z0-9]");

            if (password.Length < 6 || !isValidForm)
            {
                message = "Password must be at least 6 digits in combination of letters, numbers.";
                ShowPopupMessage(message, registerConfirmPassword.transform);
                Debug.Log(message);
                registerPassword.Select();
                return false;
            }

            return true;
        }
    }
}
