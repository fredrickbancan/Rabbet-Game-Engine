using FredrickTechDemo.Models;

namespace FredrickTechDemo.SubRendering
{
    class CharModel : Model
    {
        private Character thisChar;
        private int vertexXYZIndex = 2;
        private int vertexUVIndex = 1;
        public CharModel(Character character)
        {
            this.thisChar = character;
            this.constructModel();
        }

        private void constructModel()
        {/*
            //get pixel values
            float x = thisChar.getxOffsetPixels();
            float y = thisChar.getyOffsetPixels();
            float xMax = x + thisChar.getXPixels();
            float yMax = y + thisChar.getYPixels();
            float u = thisChar.getUPixels();
            float v = thisChar.getVPixels();
            float uMax = thisChar.getUPixelMax();
            float vMax = thisChar.getVPixelMax();

            //convert pixel values to vertices in screen coords
            x = MathUtil.normalizeCustom(-1, 1, 0, GameInstance.gameWindowWidth, x);
            y = MathUtil.normalizeCustom(-1, 1, 0, GameInstance.gameWindowHeight, y);
            xMax = MathUtil.normalizeCustom(-1, 1, 0, GameInstance.gameWindowWidth, xMax);
            yMax = MathUtil.normalizeCustom(-1, 1, 0, GameInstance.gameWindowHeight, yMax);
            y = -y;
            yMax = -yMax;
            //convert pixel uv values to opengl uv values for texture atlas
            u = u / font.getImagePixelWidth();
            v = 1 - (v / font.getImagePixelHeight());//1 minus because opengl starts from bottom left and these uv's are from top left
            uMax = uMax / font.getImagePixelWidth();
            vMax = 1 - (vMax / font.getImagePixelHeight());//1 minus because opengl starts from bottom left and these uv's are from top left

            //add vertices at screen coords with uv
            addVertexScreenCoords(x, y, u, v);//Bottom left vertex
            addVertexScreenCoords(xMax, y, uMax, v);//bottom right vertex
            addVertexScreenCoords(x, yMax, u, vMax);//top left vertex
            addVertexScreenCoords(xMax, yMax, uMax, vMax);//top right vertex*/
        }

        /*Add vertices to screen space*/
        private void addVertexScreenCoords(float x, float y, float u, float v)
        {
            this.vertexXYZ[vertexXYZIndex - 2] = x;
            this.vertexXYZ[vertexXYZIndex - 1] = y;
            this.vertexXYZ[vertexXYZIndex - 0] = 0.0F;

            this.vertexUV[vertexUVIndex - 1] = u;
            this.vertexUV[vertexUVIndex - 0] = v;

            //do last
            this.vertexXYZIndex += 3;
            this.vertexUVIndex += 2;
        }
    }
    
}
