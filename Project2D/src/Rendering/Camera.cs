using FredsMath;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredrickTechDemo
{
    class Camera
    {
        private float pitch, yaw;
        private double mPosX;
        private double mPosY;
        private double oldMouseX;
        private double oldMouseY;
        private Matrix4 viewMatrix;
        private Vector3 camUpVector;
        private Vector3 up;
        private Vector3 camFrontVector;
        private Vector3 camRightVector;
        private Vector3 camTarget;
        private Vector3 camDirection;
        private EntityPlayer parent;

        public Camera(EntityPlayer parentEntity)
        {
            this.parent = parentEntity;
            camTarget = Vector3.Zero;
            camDirection = Vector3.Normalize(parentEntity.getPosition() - camTarget);
            up = Vector3.UnitY;
            camRightVector = Vector3.Normalize(Vector3.Cross(up, camDirection));
            camFrontVector = new Vector3(0.0F, 0.0F, -1.0F);
            camUpVector = Vector3.Cross(camDirection, camRightVector);
            viewMatrix = Matrix4.LookAt(parentEntity.getPosition(), camTarget, up);
        }

        public Matrix4 getViewMatrix()
        {
            return this.viewMatrix;
        }
    }
}
