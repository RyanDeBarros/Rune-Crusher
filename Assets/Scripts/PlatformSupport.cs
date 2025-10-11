//#define OVERRIDE_PLATFORM_AS_MOBILE

using System.Collections.Generic;
using UnityEngine;

public class PlatformSupport : MonoBehaviour
{
    [System.Serializable]
    public class Pair
    {
        public GameObject desktopObject;
        public GameObject mobileObject;
    }

    [SerializeField] private List<Pair> objects;

    private void Awake()
    {
        objects.ForEach(pair => SelectGameObject(pair.desktopObject, pair.mobileObject));
    }

    public static bool IsMobile()
    {
#if UNITY_EDITOR && OVERRIDE_PLATFORM_AS_MOBILE
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
            return Input.GetMouseButton(0);
    }

    public static Vector2 GetMainPointerWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(IsMobile() ? Input.GetTouch(0).position : Input.mousePosition);
    }

    public static void SelectGameObject(GameObject desktopObject, GameObject mobileObject)
    {
        if (IsMobile())
        {
            desktopObject.SetActive(false);
            mobileObject.SetActive(true);
        }
        else
        {
            desktopObject.SetActive(true);
            mobileObject.SetActive(false);
        }
    }
}
