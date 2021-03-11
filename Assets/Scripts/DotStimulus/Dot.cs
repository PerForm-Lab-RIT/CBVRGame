using ScriptableObjects;
using UnityEngine;

namespace DotStimulus
{
    public class Dot
    {
        private readonly Vector3 _velocity;
        private Vector3 _position;
        private Vector3 _oldPosition;
        private readonly float _scale;
        private readonly Bounds _bounds;
        
        private float _elapsedTime;
        private readonly float _lifetime;
    
        public Dot(Vector3 velocity, Vector3 startingPosition, float scale, Bounds bounds, float lifetime)
        {
            _velocity = velocity;
            _position = startingPosition;
            _elapsedTime = 0;
            _lifetime = lifetime;
            
            _scale = scale;
            _bounds = bounds;
        
            // In case first update puts dot outside circle
            var boundsMax = bounds.max;
            var boundsMin = bounds.min;
                
            var randomPosition = new Vector3(Random.Range(boundsMin.x / 2, boundsMax.x / 2), 
                Random.Range(boundsMin.y / 2, boundsMax.y / 2), 
                Random.Range(boundsMin.z / 2, boundsMax.z / 2));
            _oldPosition = randomPosition;
        }

        public void UpdateDot()
        {
            var boundsMax = _bounds.max;
            var boundsMin = _bounds.min;
            
            if (_elapsedTime > _lifetime)
            {
                var randomPosition = new Vector3(Random.Range(boundsMin.x / 2, boundsMax.x / 2), 
                    Random.Range(boundsMin.y / 2, boundsMax.y / 2), 
                    Random.Range(boundsMin.z / 2, boundsMax.z / 2));
                _position = randomPosition;
                _elapsedTime = 0.0f;
            }
            else
            {
                _position.x += _velocity.x * Time.deltaTime;
                _position.y += _velocity.y * Time.deltaTime;
                _position.z += _velocity.z * Time.deltaTime;
                
                if (_position.x >= _bounds.max.x / 2 || _position.x <= _bounds.min.x / 2)
                    _position.x = _oldPosition.x;
                if (_position.y >= _bounds.max.y / 2 || _position.y <= _bounds.min.y / 2)
                    _position.y = _oldPosition.y;
                if (_position.z >= _bounds.max.z / 2 || _position.z <= _bounds.min.z / 2)
                    _position.z = _oldPosition.z;
            }

            _elapsedTime += Time.deltaTime;
            _oldPosition = -_position;
        }

        public Vector3 GetPosition()
        {
            return _position;
        }
    
        public float GetScale()
        {
            return _scale;
        }
    }
}
