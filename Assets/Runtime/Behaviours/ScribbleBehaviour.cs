using UnityEngine;

namespace Resat.Behaviours
{
    public class ScribbleBehaviour : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer? _spriteRenderer;

        [SerializeField]
        private Sprite? _spriteOne;

        [SerializeField]
        private Sprite? _spriteTwo;

        [SerializeField]
        private float _speed = 1;

        private void Update()
        {
            if (_spriteRenderer == null)
                return;
            
            if (Time.time % _speed > _speed * 0.5f)
            {
                _spriteRenderer.sprite = _spriteOne;
            }
            else
            {
                _spriteRenderer.sprite = _spriteTwo;
            }
        }
    }
}