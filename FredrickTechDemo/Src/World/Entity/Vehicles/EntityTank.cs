using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using System;

namespace FredrickTechDemo
{
    public class EntityTank : EntityVehicle
    {
        private double wheelsYaw;
        private double bodyYaw;
        private double barrelYaw;
        private double barrelPitch;
        private Vector3D barrelPos;
        public EntityTank() : base()
        {
            this.entityModel = new EntityTankModel(this);
            this.hasModel = true;
            wheelsYaw = yaw;
            mountingOffset = new Vector3D(pos.x, pos.y + 2, pos.z);
        }
        public EntityTank(Vector3D initialPos) : base(initialPos)
        {
            this.entityModel = new EntityTankModel(this);
            this.hasModel = true;
            wheelsYaw = yaw;
            mountingOffset = new Vector3D(pos.x, pos.y + 2, pos.z);
        }

        public override void onTick()
        {
            base.onTick();//do first
            mountingOffset = new Vector3D(pos.x, pos.y + 2, pos.z);
            barrelPos = new Vector3D(pos.x, pos.y, pos.z);
            bodyYaw = mountingEntity.getYaw() + 90;
            barrelPitch = mountingEntity.getHeadPitch();
            barrelYaw = bodyYaw;
        }

        /*Called by base ontick()*/
        /*When called, aligns vectors according to the entities state and rotations.*/
        protected override void alignVectors()
        {
            /*correcting front vector based on new pitch and yaw*/
            frontVector.x = (double)(Math.Cos(MathUtil.radians(wheelsYaw)));
            frontVector.z = (double)(Math.Sin(MathUtil.radians(wheelsYaw)));
            frontVector.Normalize();
        }

        /*Called by base ontick()*/
        /*Changes velocity based on state and movement vector, movement vector is changed by movement functions such as walkFowards()*/
        protected override void moveByMovementVector()
        {
            //modify walk speed here i.e slows, speed ups etc
            double walkSpeedModified = driveSpeed;

            if (!isGrounded)walkSpeedModified = 0.02D;//reduce movespeed when jumping or mid air and reduce movespeed when flying as to not accellerate out of control


            //change velocity based on movement
            //movement vector is a unit vector.
            movementVector.Normalize();//normalize vector so vehicle is same speed in any direction
            rotateYaw((movementVector.x > 0 ? 1 : (movementVector.x < 0 ? -1 : 0))  * turnRate);//rotate wheels, if reversing then rotate tank in opposite direction
            velocity += frontVector * movementVector.z * walkSpeedModified;//fowards and backwards movement
            movementVector *= 0;//reset movement vector
        }

        public override void rotateYaw(double amount)
        {
            wheelsYaw += amount;
            bodyYaw += amount;
            barrelYaw += amount;
            this.mountingEntity.rotateYaw(amount);
        }
        public double getWheelsYaw { get => wheelsYaw;}
        public double getBodyYaw { get => bodyYaw;}
        public double getBarrelYaw { get => barrelYaw;}
        public double getBarrelPitch { get => barrelPitch;}
        public Vector3D getBarrelpos { get => barrelPos;}
    }
}
