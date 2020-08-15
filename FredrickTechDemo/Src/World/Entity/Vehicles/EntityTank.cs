using FredrickTechDemo.FredsMath;
using FredrickTechDemo.Models;
using System;

namespace FredrickTechDemo
{
    public class EntityTank : EntityVehicle
    {
        private double projectileHopupAngle = 7.2F;
        private double bodyYaw;
        private double barrelPitch;
        private readonly double barrelLength = 6.75D;
        Random rand = new Random();
        public EntityTank() : base()
        {
            this.entityModel = new EntityTankModel(this);
            this.hasModel = true;
            mountingOffset = new Vector3D(pos.x, pos.y + 2, pos.z);
        }
        public EntityTank(Vector3D initialPos) : base(initialPos)
        {
            this.entityModel = new EntityTankModel(this);
            this.hasModel = true;
            mountingOffset = new Vector3D(pos.x, pos.y + 2, pos.z);
        }

        public override void onTick()
        {
            base.onTick();//do first
            if (mountingEntity != null)
            {
                mountingOffset = new Vector3D(pos.x, pos.y + 2, pos.z);
                bodyYaw = mountingEntity.getYaw() + 90;
                barrelPitch = mountingEntity.getHeadPitch() + projectileHopupAngle;
            }
        }

        /*Called by base ontick()*/
        /*When called, aligns vectors according to the entities state and rotations.*/
        protected override void alignVectors()
        {
            /*correcting front vector based on new pitch and yaw*/
            frontVector.x = (double)(Math.Cos(MathUtil.radians(yaw)));
            frontVector.z = (double)(Math.Sin(MathUtil.radians(yaw)));
            frontVector.normalize();
        }

        /*Called by base ontick()*/
        /*Changes velocity based on state and movement vector, movement vector is changed by movement functions such as walkFowards()*/
        protected override void moveByMovementVector()
        {
            //modify walk speed here i.e slows, speed ups etc
            double walkSpeedModified = driveSpeed;

            if (!isGrounded)walkSpeedModified = 0.02D;//reduce movespeed when jumping or mid air 


            //change velocity based on movement
            //movement vector is a unit vector.
            movementVector.normalize();//normalize vector so vehicle is same speed in any direction
            rotateYaw((movementVector.x > 0 ? 1 : (movementVector.x < 0 ? -1 : 0))  * turnRate);//rotate wheels, if reversing then rotate tank in opposite direction
            velocity += frontVector * movementVector.z * walkSpeedModified;//fowards and backwards movement
            movementVector *= 0;//reset movement vector
        }

        /*called when player left clicks while driving this vehicle*/
        public override void onLeftClick()
        {
            currentPlanet.spawnEntityInWorld(new EntityTankProjectile(getMuzzleLocation(), getMuzzleFrontVector(), barrelPitch, bodyYaw));
        }

        private Vector3D getMuzzleLocation()
        {
            Matrix4F barrelLengthTranslationMatrix = Matrix4F.translate(new Vector3F(0, 0, -(float)barrelLength)) * ((EntityTankModel)entityModel).getBarrelModelMatrix();
            Vector3D result = new Vector3D();
            result.x += barrelLengthTranslationMatrix.row3.x;
            result.y += barrelLengthTranslationMatrix.row3.y;
            result.z += barrelLengthTranslationMatrix.row3.z;
            return result;
        }
        private Vector3D getMuzzleFrontVector()
        {
            Vector3D result = new Vector3D();
            result.x = (double)(Math.Cos(MathUtil.radians(mountingEntity.getYaw()))) * (double)(Math.Cos(MathUtil.radians(barrelPitch)));
            result.y = (double)Math.Sin(MathUtil.radians(barrelPitch));
            result.z = (double)(Math.Sin(MathUtil.radians(mountingEntity.getYaw()))) * (double)(Math.Cos(MathUtil.radians(barrelPitch)));
            result.normalize();
            return result;
        }
        public override void rotateYaw(double amount)
        {
            base.rotateYaw(amount);
            this.mountingEntity.rotateYaw(amount);
        }
        public double getBodyYaw { get => bodyYaw;}
        public double getBarrelPitch { get => barrelPitch;}
    }
}
