using FredrickTechDemo.FredsMath;
using OpenTK.Input;
using System;

namespace FredrickTechDemo
{
    /*This class represents a virtual point at which the world should be rendered from. It takes in a player and will be bound to the players head.*/
    class Camera
    {
        private float pitch, yaw;
        private int mouseDeltaX;
        private int mouseDeltaY;
        private Matrix4F viewMatrix;
        private Vector3F camUpVector;
        private Vector3F up;
        private Vector3F camFrontVector;
        private Vector3F camRightVector;
        private Vector3F camTargetVector;
        private Vector3F camDirectionVector;
        private EntityPlayer parent;

        /*Class for a camera controlled by a mouse. The camera will be attached to a player entity. The 
          camera movement will control the players yaw and head pitch, which will then controll which
          direction the player moves in.*/
        public Camera(EntityPlayer parentEntity)
        {
            this.parent = parentEntity;
            yaw = -90.0F;
            camTargetVector = new Vector3F(0.0F);
            camDirectionVector = Vector3F.normalize(parentEntity.getPosition() - camTargetVector);
            up = new Vector3F(0.0F, 1.0F, 0.0F);
            camRightVector = Vector3F.normalize(Vector3F.cross(up, camDirectionVector));
            camFrontVector = new Vector3F(0.0F, 0.0F, -1.0F);
            camUpVector = Vector3F.cross(camDirectionVector, camRightVector);
            viewMatrix = Matrix4F.lookAt(parentEntity.getPosition(), camTargetVector, up);
        }

        /*Called every FRAME (not tick), will update view matrix depending on interpolated player position.
          also updates mouse input and rotates camera dependingly.*/
        public void onUpdate()
        {
            updateMouseDeltas();
            Input.centerMouse();
            pitch -= mouseDeltaY * GameSettings.mouseSensitivity;
            yaw += mouseDeltaX * GameSettings.mouseSensitivity;
            /*cap pitch so cam and entity can not flip*/
            if (pitch > 90.0F)
            {
                pitch = 90.0F;
            }
            else if (pitch < -90.0F)
            {
                pitch = -90.0F;
            }

            parent.setYaw(yaw);
            parent.setheadPitch(pitch);

            camDirectionVector.x =(float) (Math.Cos(MathUtil.radians(yaw)) * Math.Cos(MathUtil.radians(pitch)));
            camDirectionVector.y =(float) Math.Sin(MathUtil.radians(pitch));
            camDirectionVector.z =(float) (Math.Sin(MathUtil.radians(yaw)) * Math.Cos(MathUtil.radians(pitch)));
            camFrontVector = Vector3F.normalize(camDirectionVector);
            camRightVector = Vector3F.normalize(Vector3F.cross(up, camDirectionVector));
            camUpVector = Vector3F.cross(camDirectionVector, camRightVector);
            viewMatrix = Matrix4F.lookAt(parent.getLerpPos(), parent.getLerpPos() + camFrontVector, camUpVector);
        }

        /*updates mouse input for camera*/
        private void updateMouseDeltas()
        {
            MouseState mouseState = Mouse.GetCursorState();

            /*compares the current curor pos to the center of screen location each frame. The cursor will then be reset to cetner after taking value.*/
            mouseDeltaX = mouseState.X - GameInstance.mouseCenterX;
            mouseDeltaY = mouseState.Y - GameInstance.mouseCenterY;
        }
        public Matrix4F getViewMatrix()
        {
            return this.viewMatrix;
        }
    }
}
