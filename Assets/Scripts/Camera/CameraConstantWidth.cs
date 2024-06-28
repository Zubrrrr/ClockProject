using UnityEngine;

public class CameraConstantWidth : MonoBehaviour
{
    [SerializeField] private Vector2 _defaultResolution = new Vector2(2200, 1000);
    [SerializeField][Range(0f, 1f)] private float _widthOrHeight = 0;

    private Camera _componentCamera;

    private int _screenWidth;
    private int _screenHeight;

    private float _initialSize;
    private float _targetAspect;
    private float _initialFov;
    private float _horizontalFov = 120f;
    private float _aspectRatioCoefficient;

    private void Start()
    {
        InitializeCamera();
        CalculateInitialParameters();
    }

    private void Update()
    {
        if (ScreenResolutionChanged())
        {
            UpdateResolution();
            _aspectRatioCoefficient = _screenWidth / (float)_screenHeight;
        }

        UpdateCameraView();
    }

    private void InitializeCamera() => _componentCamera = GetComponent<Camera>();

    private void CalculateInitialParameters()
    {
        _initialSize = _componentCamera.orthographicSize;
        _targetAspect = _defaultResolution.x / _defaultResolution.y;
        _initialFov = _componentCamera.fieldOfView;
        _horizontalFov = CalcVerticalFov(_initialFov, 1 / _targetAspect);
    }

    private bool ScreenResolutionChanged()
    {
        return Screen.width != _screenWidth || Screen.height != _screenHeight;
    }

    private void UpdateCameraView()
    {
        if (_componentCamera.orthographic)
        {
            UpdateOrthographicCamera();
        }
        else
        {
            UpdatePerspectiveCamera();
        }
    }

    private void UpdateOrthographicCamera()
    {
        float constantWidthSize = _initialSize * (_targetAspect / _componentCamera.aspect);
        _componentCamera.orthographicSize = Mathf.Lerp(constantWidthSize, _initialSize, _widthOrHeight);
    }

    private void UpdatePerspectiveCamera()
    {
        float constantWidthFov = CalcVerticalFov(_horizontalFov, _componentCamera.aspect);
        _componentCamera.fieldOfView = Mathf.Lerp(constantWidthFov, _initialFov, _widthOrHeight);
    }

    private float CalcVerticalFov(float hFovInDeg, float aspectRatio)
    {
        float hFovInRads = hFovInDeg * Mathf.Deg2Rad;
        float vFovInRads = 2 * Mathf.Atan(Mathf.Tan(hFovInRads / 2) / aspectRatio);

        return vFovInRads * Mathf.Rad2Deg;
    }

    private void UpdateResolution()
    {
        _screenWidth = Screen.width;
        _screenHeight = Screen.height;

        Debug.Log("Current screen resolution: " + _screenWidth + "x" + _screenHeight);
    }
}