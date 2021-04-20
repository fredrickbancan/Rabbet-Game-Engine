using OpenTK.Mathematics;
using System;

namespace RabbetGameEngine
{
    /*This class represents a virtual point at which the world should be rendered from. It takes in a player and will be bound to the players head.*/
    public class Camera
    {
        protected float pitch;
        protected float yaw = -90.0F;
        protected Matrix4 viewMatrix;
        protected Vector3 camUpVector;
        protected static readonly Vector3 up = new Vector3(0.0F, 1.0F, 0.0F);
        protected Vector3 camFrontVector;
        protected Vector3 camRightVector;
        protected Vector3 camDirectionVector;
        protected Vector3 camPos;

        public Camera(Vector3 initialPos)
        {
            camPos = initialPos;
            onFrame(1.0F);
        }

        public virtual void onFrame(float ptnt)
        {
            /*cap yaw so cam can not flip*/
            if (pitch >= 90.0F)
            {
                pitch = 89.999F;
            }
            else if (pitch <= -90.0F)
            {
                pitch = -89.999F;
            }

            camDirectionVector.X = (float)(Math.Cos(MathUtil.radians(yaw)) * Math.Cos(MathUtil.radians(pitch)));
            camDirectionVector.Y = (float)Math.Sin(MathUtil.radians(pitch));
            camDirectionVector.Z = (float)(Math.Sin(MathUtil.radians(yaw)) * Math.Cos(MathUtil.radians(pitch)));
            camFrontVector = Vector3.Normalize(camDirectionVector);
            camRightVector = -Vector3.Normalize(Vector3.Cross(up, camDirectionVector));
            camUpVector = Vector3.Cross(camDirectionVector, -camRightVector);
            viewMatrix = Matrix4.LookAt(camPos, camPos + camFrontVector, camUpVector);
        }

        public virtual void onTick(float ts)
        {

        }

        public Matrix4 getViewMatrix()
        {
            return this.viewMatrix;
        }

        public Vector3 getFrontVector()
        {
            return camFrontVector;
        }

        public Vector3 getRightVector()
        {
            return camRightVector;
        }

        public float getYaw()
        {
            return yaw;
        }

        public Vector3 getCamPos()
        {
            return camPos;
        }
        public float getPitch()
        {
            return pitch;
        }
    }
}
