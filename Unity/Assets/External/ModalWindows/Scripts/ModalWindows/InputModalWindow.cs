using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputModalWindow : ModalWindow<InputModalWindow>
{
    [SerializeField] private TMP_InputField inputField;

    private Action<string> onInputFieldDone;

    public InputModalWindow SetInputField(Action<string> onDone, string initialValue = "", string placeholderValue = "Type here")
    {
        inputField.SetTextWithoutNotify(initialValue);
        ((TMP_Text)inputField.placeholder).SetText(initialValue);
        onInputFieldDone = onDone;

        return this;
    }

    protected override void OnBeforeShow()
    {
        inputField.Select();
    }

    void SubmitInput()
    {
        onInputFieldDone?.Invoke(inputField.text);
        onInputFieldDone = null;
        Close();
    }

    protected override void Update()
    {
        base.Update();
        if (inputField.isFocused && inputField.text != "" && Input.GetKeyUp(KeyCode.Return))
            SubmitInput();
    }

    public void UI_InputFieldOKButton()
    {
        SubmitInput();
    }
}
