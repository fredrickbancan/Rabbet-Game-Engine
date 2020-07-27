using FredrickTechDemo.FredsMath;

namespace FredrickTechDemo
{
    /*This class represents a virtual point at which the world should be rendered from. It takes in a player and will be bound to the players head.*/
    class Camera
    {
        private float pitch, yaw;
        private double mPosX;
        private double mPosY;
        private double oldMouseX;
        private double oldMouseY;
        private Matrix4F viewMatrix;
        private Vector3F camUpVector;
        private Vector3F up;
        private Vector3F camFrontVector;
        private Vector3F camRightVector;
        private Vector3F camTargetVector;
        private Vector3F camDirectionVector;
        private EntityPlayer parent;

        public Camera(EntityPlayer parentEntity)
        {
            this.parent = parentEntity;
            camTargetVector = new Vector3F(0.0F);
            camDirectionVector = Vector3F.normalize(parentEntity.getPosition() - camTargetVector);
            up = new Vector3F(0.0F, 1.0F, 0.0F);
            camRightVector = Vector3F.normalize(Vector3F.cross(up, camDirectionVector));
            camFrontVector = new Vector3F(0.0F, 0.0F, -1.0F);
            camUpVector = Vector3F.cross(camDirectionVector, camRightVector);
            viewMatrix = Matrix4F.lookAt(parentEntity.getPosition(), camTargetVector, up);
        }

        public Matrix4F getViewMatrix()
        {
            return this.viewMatrix;
        }
    }
}
