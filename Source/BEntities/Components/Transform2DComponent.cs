﻿using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace BEntities.Components
{
    public class Transform2DComponent : BaseComponent, INotifyPropertyChangedExtended
    {
        [Flags]
        private enum TransformFlags : byte
        {
            WorldMatrixIsDirty = 1,
            LocalMatrixIsDirty = 2,
            All = LocalMatrixIsDirty | WorldMatrixIsDirty
        }

        private TransformFlags _flags = TransformFlags.All;
        private Vector2 _scale = Vector2.One;
        private Matrix2D _localMatrix;
        private Transform2DComponent _parent = null;
        private Matrix2D _worldMatrix;
        private Vector2 _position;
        private Vector2 _origin;
        private float _rotation;
        private int _zIndex = 0;

        /// <summary>
        /// Gets the world position of object
        /// </summary>
        public Vector2 WorldPosition => WorldMatrix.Translation;

        /// <summary>
        /// Gets the world scale of object
        /// </summary>
        public Vector2 WorldScale => WorldMatrix.Scale;

        /// <summary>
        /// Gets the world rotation of object
        /// </summary>
        public float WorldRotation => WorldMatrix.Rotation;

        /// <summary>
        /// Gets or sets local position
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                LocalMatrixBecameDirty();
                WorldMatrixBecameDirty();
            }
        }

        /// <summary>
        /// Gets or sets local origin position
        /// </summary>
        public Vector2 Origin
        {
            get { return _origin; }
            set
            {
                _origin = value;
                LocalMatrixBecameDirty();
                WorldMatrixBecameDirty();
            }
        }

        /// <summary>
        /// Gets or sets local rotation in radians
        /// </summary>
        public float Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                LocalMatrixBecameDirty();
                WorldMatrixBecameDirty();
            }
        }

        public float RotationDegrees
        {
            get { return MathHelper.ToDegrees(_rotation); }
            set
            {
                float radians = MathHelper.ToRadians(value);
                Rotation = radians;
            }
        }

        /// <summary>
        /// Gets or sets local Z Index
        /// </summary>
        public int ZIndex
        {
            get { return _zIndex; }
            set
            {
                if (_zIndex == value)
                    return;

                int oldZIndex = _zIndex;
                _zIndex = value;
                OnPropertyChanged(oldZIndex, value);
            }
        }

        /// <summary>
        /// Gets or sets local scale
        /// </summary>
        public Vector2 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                LocalMatrixBecameDirty();
                WorldMatrixBecameDirty();
            }
        }

        /// <summary>
        /// Gets the local matrix
        /// </summary>
        public Matrix2D LocalMatrix
        {
            get
            {
                RecalculateLocalMatrixIfNecessary();
                return _localMatrix;
            }
        }

        /// <summary>
        /// Gets the world matrix
        /// </summary>
        public Matrix2D WorldMatrix
        {
            get
            {
                RecalculateWorldMatrixIfNecessary();
                return _worldMatrix;
            }
        }

        /// <summary>
        /// Gets or sets parent transform component
        /// </summary>
        public Transform2DComponent Parent
        {
            get { return _parent; }
            set
            {
                if (_parent == value)
                    return;

                Transform2DComponent parent = Parent;
                _parent = value;
                OnParentChanged(parent, value);
                OnPropertyChanged(parent, value);
            }
        }

        internal event Action TransformBecameDirty;

        public override void Reset()
        {
            Parent = null;
            _flags = TransformFlags.All;
            _localMatrix = Matrix2D.Identity;
            _worldMatrix = Matrix2D.Identity;
            _position = Vector2.Zero;
            _origin = Vector2.Zero;
            _rotation = 0.0f;
            _scale = Vector2.One;
            _zIndex = 0;
        }

        public void GetLocalMatrix(out Matrix2D matrix)
        {
            RecalculateLocalMatrixIfNecessary();
            matrix = _localMatrix;
        }

        public void GetWorldMatrix(out Matrix2D matrix)
        {
            RecalculateWorldMatrixIfNecessary();
            matrix = _worldMatrix;
        }

        protected internal void LocalMatrixBecameDirty()
        {
            _flags = _flags | TransformFlags.LocalMatrixIsDirty;
        }

        protected internal void WorldMatrixBecameDirty()
        {
            _flags = _flags | TransformFlags.WorldMatrixIsDirty;
            Action transformBecameDirty = TransformBecameDirty;
            transformBecameDirty?.Invoke();
        }

        private void OnParentChanged(Transform2DComponent oldParent, Transform2DComponent newParent)
        {
            for (Transform2DComponent transform2DComponent = oldParent;
                transform2DComponent != null;
                transform2DComponent = transform2DComponent.Parent)
                transform2DComponent.TransformBecameDirty -= ParentOnTransformBecameDirty;
            for (Transform2DComponent transform2DComponent = newParent;
                transform2DComponent != null;
                transform2DComponent = transform2DComponent.Parent)
                transform2DComponent.TransformBecameDirty += ParentOnTransformBecameDirty;
        }

        private void ParentOnTransformBecameDirty()
        {
            _flags = _flags | TransformFlags.All;
        }

        private void RecalculateWorldMatrixIfNecessary()
        {
            if ((_flags & TransformFlags.WorldMatrixIsDirty) == 0)
                return;
            RecalculateLocalMatrixIfNecessary();
            RecalculateWorldMatrix(ref _localMatrix, out _worldMatrix);
            _flags = _flags & ~TransformFlags.WorldMatrixIsDirty;
        }

        private void RecalculateLocalMatrixIfNecessary()
        {
            if ((_flags & TransformFlags.LocalMatrixIsDirty) == 0)
                return;
            RecalculateLocalMatrix(out _localMatrix);
            _flags = _flags & ~TransformFlags.LocalMatrixIsDirty;
            WorldMatrixBecameDirty();
        }

        private void RecalculateWorldMatrix(ref Matrix2D localMatrix, out Matrix2D matrix)
        {
            if (Parent != null)
            {
                Parent.GetWorldMatrix(out matrix);
                Matrix2D.Multiply(ref localMatrix, ref matrix, out matrix);
            }
            else
                matrix = localMatrix;
        }

        private void RecalculateLocalMatrix(out Matrix2D matrix)
        {
            matrix = Matrix2D.CreateScale(_scale) * Matrix2D.CreateRotationZ(_rotation) *
                     Matrix2D.CreateTranslation(_position - (_origin * _scale));
        }

        protected virtual void OnPropertyChanged(object oldValue, object newValue,
            [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedExtendedEventArgs(oldValue, newValue, propertyName));
        }

        public event PropertyChangedExtendedEventHandler PropertyChanged;
    }
}
