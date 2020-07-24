using FredsMath;
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
        private Matrix4F viewMatrix;
        private Vector3F camUpVector;
        private Vector3F camFrontVector;
        private EntityPlayer parent;

        public Camera(EntityPlayer parentEntity)
        {
            this.parent = parentEntity;
            viewMatrix = new Matrix4F();
            camUpVector = new Vector3F(0.0F, 1.0F, 0.0F);
            camFrontVector = new Vector3F(0.0F, 0.0F, -1.0F);
            viewMatrix = MathUtil.lookAt(parentEntity.getPosition(), parentEntity.getPosition() + camFrontVector, camUpVector);
        }

        public Matrix4F getViewMatrix()
        {
            return this.viewMatrix;
        }
    }
}
