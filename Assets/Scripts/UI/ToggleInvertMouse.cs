using UnityEngine;

public class ToggleInvertMouse : MonoBehaviour {

    public bool isInverted = false;

    private void Start()
    {

    }

    public void ToggleInvertY(bool inversion)
    {

    }

    private void changeMouseSettings()
    {
        Input.GetAxis("Mouse Y");
    }
}
