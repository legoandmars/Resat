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

        [SerializeField]
        private float _describbleSpeed;
        
        private bool _describbling = false;
        
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

            if (_describbling)
            {
                var amount = _describbleSpeed * Time.deltaTime;
                // _spriteRenderer.color = ColorWithAlpha(_spriteRenderer.color, _spriteRenderer.color.a - (_describbleSpeed * Time.deltaTime));
                _spriteRenderer.transform.localScale = new Vector3(_spriteRenderer.transform.localScale.x - amount  / 1.5f, _spriteRenderer.transform.localScale.y - amount, 1);

                if (_spriteRenderer.transform.localScale.y <= 0)
                {
                    _spriteRenderer.transform.localScale = Vector3.zero;
                    enabled = false;
                }
            }

            // kill update when finished
            /*if (_spriteRenderer.color.a <= 0f)
            {
                _spriteRenderer.color = ColorWithAlpha(_spriteRenderer.color, 0f);
                enabled = false;
            }*/
        }

        private Color ColorWithAlpha(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public void Describble()
        {
            _describbling = true;
        }
    }
}