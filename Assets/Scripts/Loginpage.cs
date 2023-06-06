using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;



public class Loginpage : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI TopText;
    [SerializeField] TextMeshProUGUI MessageText;

    [Header("Login")]
    [SerializeField] TMP_InputField EmailLoginInput;
    [SerializeField] TMP_InputField PasswordLoginput;
    [SerializeField] GameObject LoginPage;

    [Header("Registry")]
    [SerializeField] TMP_InputField UsernameRegistryInput;
    [SerializeField] TMP_InputField EmailRegisterInput;
    [SerializeField] TMP_InputField PasswordRegistryInput;
    [SerializeField] GameObject RegistryPage;

    [Header("Recovery")]
    [SerializeField] TMP_InputField EmailRecoveryInput;
    [SerializeField] GameObject RecoverPage;





    void Start()
    {

    }

    public void Login()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = EmailLoginInput.text,
            Password = PasswordLoginput.text,
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    private void OnLoginSuccess(LoginResult Result)
    {
        SceneManager.LoadScene("PlayGame");
    }

    public void RegisterUser()
    {
        var request = new RegisterPlayFabUserRequest
        {
            DisplayName = UsernameRegistryInput.text,
            Email = EmailRegisterInput.text,
            Password = PasswordRegistryInput.text,

            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnregisterSucces, OnError);
    }

    private void OnError(PlayFabError Error)
    {
        MessageText.text = Error.ErrorMessage;
        Debug.Log(Error.GenerateErrorReport());
    }

    private void OnregisterSucces(RegisterPlayFabUserResult Result)
    {
        MessageText.text = "New account is created";
        OpenLoginPage();
    }

    public void RecoverUser()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = EmailRecoveryInput.text,
            TitleId = "F67F4",
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnRecoverySuccess, OnErrorRecovery);
    }

    private void OnRecoverySuccess(SendAccountRecoveryEmailResult obj)
    {
        OpenLoginPage();
        MessageText.text = "Recovery Mail Sent";
    }

    private void OnErrorRecovery(PlayFabError Error)
    {
        MessageText.text = "No Email Found";
    }

    public void OpenLoginPage()
    {
        LoginPage.SetActive(true);
        RegistryPage.SetActive(false);
        RecoverPage.SetActive(false);
        TopText.text = "Login";
    }

    public void OpenRegistryPage()
    {
        LoginPage.SetActive(false);
        RegistryPage.SetActive(true);
        RecoverPage.SetActive(false);
        TopText.text = "Registry";
    }

    public void OpenRecover()
    {
        LoginPage.SetActive(false);
        RegistryPage.SetActive(false);
        RecoverPage.SetActive(true);
        TopText.text = "Recover";
    }
}
