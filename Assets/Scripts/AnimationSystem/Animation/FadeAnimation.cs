using UnityEngine;
using DG.Tweening;
using TMPro;
using AnimationSystem;
using UnityEngine.UI;

public class FadeAnimation : BaseAnimation
{
    [Header("Transparency Settings")]

    [Tooltip("Whether to use the current alpha (transparency) value as the starting alpha.")]
    [SerializeField] private bool _useCurrentAlphaAsStart;

    [Tooltip("The initial alpha (transparency) value to start the animation from if not using the current alpha.")]
    [SerializeField][Range(0f, 1f)] private float _initialAlpha;

    [Tooltip("The target alpha (transparency) value to animate to, between 0 (fully transparent) and 1 (fully opaque).")]
    [SerializeField][Range(0f, 1f)] private float _targetAlpha;

    private enum FadeComponentType { TextMeshPro, SpriteRenderer, Image, None }
    private FadeComponentType _componentType;

    private TextMeshProUGUI _textMeshPro;
    private SpriteRenderer _spriteRenderer;
    private Image _image;

    private void Awake()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _image = GetComponent<Image>();

        if (_textMeshPro != null) _componentType = FadeComponentType.TextMeshPro;
        else if (_spriteRenderer != null) _componentType = FadeComponentType.SpriteRenderer;
        else if (_image != null) _componentType = FadeComponentType.Image;
    }

    public override void PlayAnimation()
    {
        float startAlpha = _useCurrentAlphaAsStart ? GetCurrentAlpha() : _initialAlpha;
        SetAlpha(startAlpha);

        switch (_componentType)
        {
            case FadeComponentType.TextMeshPro:
                CurrentTween = _textMeshPro.DOFade(_targetAlpha, Duration)
                    .SetEase(EaseType)
                    .SetLoops(Loops, LoopType)
                    .SetDelay(Delay);
                break;

            case FadeComponentType.SpriteRenderer:
                CurrentTween = _spriteRenderer.DOFade(_targetAlpha, Duration)
                    .SetEase(EaseType)
                    .SetLoops(Loops, LoopType)
                    .SetDelay(Delay);
                break;

            case FadeComponentType.Image:
                CurrentTween = _image.DOFade(_targetAlpha, Duration)
                    .SetEase(EaseType)
                    .SetLoops(Loops, LoopType)
                    .SetDelay(Delay);
                break;

            default:
                Debug.LogWarning("FadeAnimation: The animation component was not found.");
                break;
        }
    }

    public override void StopAnimation()
    {
        base.StopAnimation();
    }

    private float GetCurrentAlpha()
    {
        return _componentType switch
        {
            FadeComponentType.TextMeshPro => _textMeshPro.color.a,
            FadeComponentType.SpriteRenderer => _spriteRenderer.color.a,
            FadeComponentType.Image => _image.color.a,
            _ => 1f
        };
    }

    private void SetAlpha(float alpha)
    {
        switch (_componentType)
        {
            case FadeComponentType.TextMeshPro:
                Color textColor = _textMeshPro.color;
                textColor.a = alpha;
                _textMeshPro.color = textColor;
                break;

            case FadeComponentType.SpriteRenderer:
                Color spriteColor = _spriteRenderer.color;
                spriteColor.a = alpha;
                _spriteRenderer.color = spriteColor;
                break;

            case FadeComponentType.Image:
                Color imageColor = _image.color;
                imageColor.a = alpha;
                _image.color = imageColor;
                break;
        }
    }
}