using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class ToggleInvertMouse : MonoBehaviour {

    public bool isInverted = false;
    public FirstPersonController fpc;

    private void Start()
    {
        fpc = GetComponent<FirstPersonController>();
        isInverted = fpc.m_MouseLook.YSensitivity < 0;
    }

    public void ToggleInvertY(bool inversion)
    {
        float yValABS = Mathf.Abs(fpc.m_MouseLook.YSensitivity);
        fpc.m_MouseLook.YSensitivity = inversion ? yValABS : yValABS * - 1;
        isInverted = inversion;
    }

    private void changeMouseSettings()
    {
        Input.GetAxis("Mouse Y");
    }
}
