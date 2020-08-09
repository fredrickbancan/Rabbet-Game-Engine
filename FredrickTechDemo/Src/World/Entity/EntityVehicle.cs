using FredrickTechDemo.FredsMath;
using System;

namespace FredrickTechDemo
{
    class EntityVehicle : Entity
    {
        protected Vector3D frontVector;//vector pointing to the direction the entity is facing
        protected Vector3D upVector;
        protected Vector3D movementVector; //a unit vector representing this entity's movement values. z is front and backwards, x is side to side.
        protected double headPitch;//The degrees of the head of the vehicle, for example, a tank has a head (barrel) which can rotate up and down
        public static readonly double defaultDriveSpeed = 0.3572F;
        protected double driveSpeed = defaultDriveSpeed;
        public EntityVehicle() : base()
        {
            frontVector = new Vector3D(0.0F, 0.0F, -1.0F);
            upVector = new Vector3D(0.0F, 1.0F, 0.0F);
            movementVector = new Vector3D(0.0F, 0.0F, 0.0F);
        }

        public EntityVehicle(Vector3D pos) : base(pos)
        {
            frontVector = new Vector3D(0.0F, 0.0F, -1.0F);
            upVector = new Vector3D(0.0F, 1.0F, 0.0F);
            movementVector = new Vector3D(0.0F, 0.0F, 0.0F);
        }

        public override void onTick()
        {
            base.onTick();//do first

            alignVectors();

            moveByMovementVector();
        }

        /*Changes velocity based on state and movement vector, movement vector is changed by movement functions such as walkFowards()*/
        private void moveByMovementVector()
        {
            //modify walk speed here i.e slows, speed ups etc
            double walkSpeedModified = driveSpeed;

            if (!isGrounded) walkSpeedModified = 0.0072D; else walkSpeedModified = 0.02D;//reduce movespeed when jumping or mid air and reduce movespeed when flying as to not accellerate out of control


            //change velocity based on movement
            //movement vector is a unit vector.
            movementVector.Normalize();//normalize vector so player is same speed in any direction
            velocity += frontVector * movementVector.z * walkSpeedModified;//fowards and backwards movement
           // velocity += Vector3D.normalize(Vector3D.cross(frontVector, upVector)) * movementVector.x * walkSpeedModified;//strafing movement

            movementVector *= 0;//reset movement vector
        }

        /*When called, aligns vectors according to the entities state and rotations.*/
        private void alignVectors()
        {
            /*correcting front vector based on new pitch and yaw*/
            frontVector.x = (double)(Math.Cos(MathUtil.radians(yaw)) * Math.Cos(MathUtil.radians(headPitch)));
            frontVector.z = (double)(Math.Sin(MathUtil.radians(yaw)) * Math.Cos(MathUtil.radians(headPitch)));
            frontVector.Normalize();
        }

        public void setHeadPitch(double Pitch)
        {
            this.headPitch = Pitch;
        }
        public double getHeadPitch()
        {
            return this.headPitch;
        }

        public void driveFowards()
        {
            movementVector.z++;
        }
        public void driveBackwards()
        {
            movementVector.z--;
        }

        public void turnLeft()
        {

        }
        public void turnRight()
        {

        }
    }
}
