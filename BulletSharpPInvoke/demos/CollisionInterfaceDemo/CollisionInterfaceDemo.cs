﻿using BulletSharp;
using BulletSharp.Math;
using DemoFramework;
using System;

namespace CollisionInterfaceDemo
{
    class DrawingResult : ContactResultCallback
    {
        private Vector3 _blue = new Vector3(0, 0, 1);
        private DynamicsWorld _world;

        public DrawingResult(DynamicsWorld world)
        {
            _world = world;
        }

        public override float AddSingleResult(ManifoldPoint cp,
            CollisionObjectWrapper colObj0Wrap, int partId0, int index0,
            CollisionObjectWrapper colObj1Wrap, int partId1, int index1)
        {
            Vector3 ptA = cp.PositionWorldOnA;
            Vector3 ptB = cp.PositionWorldOnB;
            _world.DebugDrawer.DrawLine(ref ptA, ref ptB, ref _blue);
            return 0;
        }
    };

    class CollisionInterfaceDemo : Demo
    {
        Vector3 eye = new Vector3(6, 4, 1);
        Vector3 target = new Vector3(0, 3, 0);

        CollisionObject objectA, objectB;
        DrawingResult renderCallback;

        Vector3 boxMin = new Vector3(-1, -1, -1);
        Vector3 boxMax = new Vector3(1, 1, 1);
        Vector3 white = new Vector3(1, 1, 1);

        protected override void OnInitialize()
        {
            Freelook.SetEyeTarget(eye, target);

            Graphics.SetFormText("BulletSharp - Collision Interface Demo");

            IsDebugDrawEnabled = true;
        }

        protected override void OnInitializePhysics()
        {
            // collision configuration contains default setup for memory, collision setup
            CollisionConf = new DefaultCollisionConfiguration();
            Dispatcher = new CollisionDispatcher(CollisionConf);

            Broadphase = new AxisSweep3(
                new Vector3(-1000, -1000, -1000),
                new Vector3(1000, 1000, 1000));

            World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, null, CollisionConf);
            World.Gravity = new Vector3(0, -10, 0);

            renderCallback = new DrawingResult(World);

            var boxA = new BoxShape(1.0f) { Margin = 0 };
            var boxB = new BoxShape(0.5f) { Margin = 0 };
            CollisionShapes.Add(boxA);
            CollisionShapes.Add(boxB);

            Quaternion rotA = new Quaternion(0.739f, -0.204f, 0.587f, 0.257f);
            rotA.Normalize();

            objectA = new CollisionObject
            {
                CollisionShape = boxA,
                WorldTransform = Matrix.RotationQuaternion(rotA) * Matrix.Translation(0, 3, 0)
            };
            objectB = new CollisionObject
            {
                CollisionShape = boxB,
                WorldTransform = Matrix.Translation(0, 4.248f, 0)
            };

            //World.AddCollisionObject(objectA);
            World.AddCollisionObject(objectB);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            Matrix transform = objectA.WorldTransform;
            Vector3 position = transform.Origin;
            transform.Origin = Vector3.Zero;
            transform *= Matrix.RotationYawPitchRoll(0.1f * FrameDelta, 0.05f * FrameDelta, 0);
            transform.Origin = position;
            objectA.WorldTransform = transform;

            if (IsDebugDrawEnabled)
            {
                World.DebugDrawer.DrawBox(ref boxMin, ref boxMax, ref transform, ref white);
                World.ContactTest(objectA, renderCallback);
            }
        }

        public override void ExitPhysics()
        {
            renderCallback.Dispose();

            base.ExitPhysics();
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            using (Demo demo = new CollisionInterfaceDemo())
            {
                GraphicsLibraryManager.Run(demo);
            }
        }
    }
}
