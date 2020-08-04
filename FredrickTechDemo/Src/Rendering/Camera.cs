using FredrickTechDemo.FredsMath;
using OpenTK.Input;
using System;

namespace FredrickTechDemo
{
    /*This class represents a virtual point at which the world should be rendered from. It takes in a player and will be bound to the players head.*/
    public class Camera
    {
        private double pitch, yaw;
        private int mouseDeltaX;
        private int mouseDeltaY;
        private Matrix4F viewMatrix;
        private Vector3D camUpVector;
        private Vector3D up;
        private Vector3D camFrontVector;
        private Vector3D camRightVector;
        private Vector3D camTargetVector;
        private Vector3D camDirectionVector;
        private EntityPlayer parent;

        /*Class for a camera controlled by a mouse. The camera will be attached to a player entity. The 
          camera movement will control the players yaw and head pitch, which will then controll which
          direction the player moves in.*/
        public Camera(EntityPlayer parentEntity)
        {
            this.parent = parentEntity;
            yaw = -90.0F;
            camTargetVector = new Vector3D(0.0F);
            camDirectionVector = Vector3D.normalize(parentEntity.getEyePosition() - camTargetVector);
            up = new Vector3D(0.0F, 1.0F, 0.0F);
            camRightVector = Vector3D.normalize(Vector3D.cross(up, camDirectionVector));
            camFrontVector = new Vector3D(0.0F, 0.0F, -1.0F);
            camUpVector = Vector3D.cross(camDirectionVector, camRightVector);
            viewMatrix = Matrix4F.lookAt(parentEntity.getEyePosition(), camTargetVector, up);
        }

        /*Called every FRAME (not tick), will update view matrix depending on interpolated player position.
          also updates mouse input and rotates camera dependingly.*/
        public void onUpdate()
        {
            updateMouseDeltas();
            Input.centerMouse();
            pitch -= mouseDeltaY * GameSettings.mouseSensitivity;
            yaw += mouseDeltaX * GameSettings.mouseSensitivity;
            if (yaw > 360.0F) { yaw = 0.0F; }
            if (yaw < -360.0F) { yaw = 0.0F; }
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
            parent.setHeadPitch(pitch);

            camDirectionVector.x =(float) (Math.Cos(MathUtil.radians(yaw)) * Math.Cos(MathUtil.radians(pitch)));
            camDirectionVector.y =(float) Math.Sin(MathUtil.radians(pitch));
            camDirectionVector.z =(float) (Math.Sin(MathUtil.radians(yaw)) * Math.Cos(MathUtil.radians(pitch)));
            camFrontVector = Vector3D.normalize(camDirectionVector);
            camRightVector = Vector3D.normalize(Vector3D.cross(up, camDirectionVector));
            camUpVector = Vector3D.cross(camDirectionVector, camRightVector);
            viewMatrix = Matrix4F.lookAt(parent.getLerpEyePos(), parent.getLerpEyePos() + camFrontVector, camUpVector);
        }

        /*updates mouse input for camera*/
        private void updateMouseDeltas()
        {
            MouseState mouseState = Mouse.GetCursorState();

            /*compares the current curor pos to the center of screen location each frame. The cursor will then be reset to cetner after taking value.*/
            mouseDeltaX = mouseState.X - GameInstance.windowCenterX;
            mouseDeltaY = mouseState.Y - GameInstance.windowCenterY;
        }
        public Matrix4F getViewMatrix()
        {
            return this.viewMatrix;
        }
    }
}
