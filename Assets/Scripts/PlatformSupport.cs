using UnityEngine;

public class PlatformSupport
{
    public static bool IsMobile()
    {
#if UNITY_EDITOR
        if (UnityEngine.SystemInfo.deviceType != UnityEngine.Device.SystemInfo.deviceType) // Using Device Simulator
            return true;
#endif
        return Application.isMobilePlatform;
    }

    public static bool IsMainPointerDown()
    {
        if (IsMobile())
        {
            Touch? touch = Input.touchCount > 0 ? Input.GetTouch(0) : null;
            return touch.HasValue && touch.Value.phase != TouchPhase.Ended && touch.Value.phase != TouchPhase.Canceled;
        }
        else
            return Input.GetMouseButtonDown(0);
    }

    public static Vector2 GetMainPointerWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(IsMobile() ? Input.GetTouch(0).position : Input.mousePosition);
    }
}
