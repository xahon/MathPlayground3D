using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUi : MonoBehaviour
{
    [SerializeField]
    private Button resetViewBtn;

    [SerializeField]
    private Toggle use2dViewToggle;

    [SerializeField]
    private Toggle useLiveUpdateToggle;

    [SerializeField]
    private Image codePanel;

    [SerializeField]
    private RectTransform userInputPanel;

    [SerializeField]
    private GameObject userInputSliderPrefab;

    [SerializeField]
    private TMP_InputField codeInputField;

    [SerializeField]
    private InterpreterFrontend interpreterFrontend;

    [SerializeField]
    private CameraMovement cameraMovement;

    private void Start()
    {
        if (codePanel.gameObject.activeSelf)
        {
            codeInputField.ActivateInputField();
        }

        resetViewBtn.onClick.AddListener(() =>
        {
            cameraMovement.ResetView();
        });

        userInputPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            codePanel.gameObject.SetActive(!codePanel.gameObject.activeSelf);
            if (codePanel.gameObject.activeSelf)
            {
                codeInputField.ActivateInputField();
            }
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            interpreterFrontend.RunCode(codeInputField.text);
        }
    }

    public RectTransform DefineUserInput(string varname, string type)
    {
        RectTransform result = null;
        if (type == "float_slider")
        {
            result = Instantiate(userInputSliderPrefab, userInputPanel.transform).GetComponent<RectTransform>();
            result.SetParent(userInputPanel.transform);
        }

        if (result != null)
        {
            userInputPanel.gameObject.SetActive(true);
            result.name = "input_param__" + varname;
            LayoutRebuilder.ForceRebuildLayoutImmediate(userInputPanel.GetComponent<RectTransform>());
        }

        return result;
    }

    public void ResetUserInput()
    {
        for (int i = 0; i < userInputPanel.transform.childCount; i++)
        {
            Destroy(userInputPanel.transform.GetChild(i).gameObject);
        }
        userInputPanel.gameObject.SetActive(false);
    }

    public void EnableCodeInput()
    {
        codeInputField.interactable = true;
    }

    public void DisableCodeInput()
    {
        codeInputField.interactable = false;
    }
}
