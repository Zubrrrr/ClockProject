using UnityEngine;
using DG.Tweening;
using AnimationSystem;
using UnityEngine.UI;
using TMPro;

public class ColorAnimation : BaseAnimation
{
    [Header("Animation Mode Settings")]

    [Tooltip("Defines the animation mode: 'Standard' for a single color transition or 'Flash' for a quick color shift back to the initial color.")]
    [SerializeField] private AnimationMode _animationMode;

    [Header("Color Settings")]

    [Tooltip("Whether to use the current color of the object as the starting color.")]
    [SerializeField] private bool _useCurrentColorAsStart;

    [Tooltip("The initial color to start the animation from, if not using the current color.")]
    [SerializeField] private Color _initialColor;

    [Tooltip("The target color to animate to.")]
    [SerializeField] private Color _targetColor;

    private enum ColorComponentType { TextMeshPro, SpriteRenderer, Image, None }
    private enum AnimationMode { Standard, Flash }

    private ColorComponentType _componentType;

    private TextMeshProUGUI _textMeshPro;
    private SpriteRenderer _spriteRenderer;
    private Image _image;

    private void Awake()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _image = GetComponent<Image>();

        if (_textMeshPro != null) _componentType = ColorComponentType.TextMeshPro;
        else if (_spriteRenderer != null) _componentType = ColorComponentType.SpriteRenderer;
        else if (_image != null) _componentType = ColorComponentType.Image;
    }

    public override void PlayAnimation()
    {
        StopAnimation();

        if (_animationMode == AnimationMode.Standard)
        {
            Color startColor = _useCurrentColorAsStart ? GetCurrentColor() : _initialColor;
            SetColor(startColor);

            switch (_componentType)
            {
                case ColorComponentType.TextMeshPro:
                    CurrentTween = _textMeshPro.DOColor(_targetColor, Duration)
                        .SetEase(EaseType)
                        .SetLoops(Loops, LoopType)
                        .SetDelay(Delay);
                    break;

                case ColorComponentType.SpriteRenderer:
                    CurrentTween = _spriteRenderer.DOColor(_targetColor, Duration)
                        .SetEase(EaseType)
                        .SetLoops(Loops, LoopType)
                        .SetDelay(Delay);
                    break;

                case ColorComponentType.Image:
                    CurrentTween = _image.DOColor(_targetColor, Duration)
                        .SetEase(EaseType)
                        .SetLoops(Loops, LoopType)
                        .SetDelay(Delay);
                    break;

                default:
                    Debug.LogWarning("ColorAnimation: The color animation component was not found.");
                    break;
            }
        }
        else if (_animationMode == AnimationMode.Flash)
        {
            SetColor(_targetColor);

            CurrentTween = DOTween.To(() => _targetColor, x => SetColor(x), _initialColor, Duration)
                .SetEase(EaseType)
                .SetDelay(Delay);
        }
    }

    public override void StopAnimation()
    {
        base.StopAnimation();
    }

    private Color GetCurrentColor()
    {
        return _componentType switch
        {
            ColorComponentType.TextMeshPro => _textMeshPro.color,
            ColorComponentType.SpriteRenderer => _spriteRenderer.color,
            ColorComponentType.Image => _image.color,
            _ => Color.white
        };
    }

    private void SetColor(Color color)
    {
        switch (_componentType)
        {
            case ColorComponentType.TextMeshPro:
                _textMeshPro.color = color;
                break;

            case ColorComponentType.SpriteRenderer:
                _spriteRenderer.color = color;
                break;

            case ColorComponentType.Image:
                _image.color = color;
                break;
        }
    }
}