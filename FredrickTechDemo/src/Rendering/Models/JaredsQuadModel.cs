using FredrickTechDemo.FredsMath;
using System;

namespace FredrickTechDemo.Models
{
    class JaredsQuadModel : ModelDrawable
    {
        private static float[] jaredsQuadVerticesXYZ = new float[]
        {/*   x      y      z   */
            -1.0F, -1.0F, 0.0F,//*vertex 0 bottom left*//*
             1.0F, -1.0F, 0.0F,//*vertex 1 bottom right*//*
            -1.0F,  1.0F, 0.0F,//*vertex 2 top left*//*
             1.0F,  1.0F, 0.0F//*vertex 3 top right*//*
        };

        private static float[] jaredsQuadVerticesRGB = new float[]
        {/*   r      g      b   */
             1.0F, 1.0F, 0.0F,//*vertex 0 bottom left*//*
             0.0F, 1.0F, 0.0F,//*vertex 1 bottom right*//*
             1.0F, 0.0F, 0.0F,//*vertex 2 top left*//*
             0.0F, 0.0F, 1.0F//*vertex 3 top right*//*
        };

        private static float[] jaredsQuadVerticesUV = new float[]
        {/*   u      v   */
            0.0F, 0.0F,//*vertex 0 bottom left*//*
            1.0F, 0.0F,//*vertex 1 bottom right*//*
            0.0F, 1.0F,//*vertex 2 top left*//*
            1.0F, 1.0F//*vertex 3 top right*//*
        };
        private static UInt32[] jaredsQuadIndices = new UInt32[] //order of vertices in counter clockwise direction for both triangles of quad. counter clock wise is opengl default for front facing.
        {
            0, 1, 2,//first triangle    
            1, 3, 2 //second triangle
        };

        public JaredsQuadModel() : base(@"..\..\src\Rendering\Shaders\ColourTextureShader3D.shader", @"..\..\res\texture\aie.png", jaredsQuadVerticesXYZ, jaredsQuadVerticesRGB, jaredsQuadVerticesUV, jaredsQuadIndices) { }
        
        protected override void init()
        {
            base.init();
            //translationMatrix = Matrix4F.createTranslationMatrix(0.2F, -1.0F, -2.0F);
            onTick(new Vector3F(), new Vector3F()); // this is done in init so the model is correctly transformed on the first few frames of display
        }
        
        public void onTick(Vector3F translate, Vector3F rotate)
        {
            prevModelMatrix = modelMatrix;//store current state in previous model matrix for interpolation
            modelMatrix = Matrix4F.rotate(rotate) * Matrix4F.translate(translate);
        }

    }
}
