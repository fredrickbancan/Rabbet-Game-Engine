using FredrickTechDemo.FredsMath;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FredrickTechDemo.Models
{
    /*This class will be responsable for loading models and converting them to vertex arrays for use with rendering.*/
    public class OBJLoader
    {
        private String currentLine = "empty";
        private StreamReader reader;
        private bool hasProcessedFaces = false;
        private List<Vertex> vertexResult;
        private List<Vector3F> positions;
        private List<Vector2F> unorderedUVs; 
        private List<Vector2F> orderedUVs; 
        private List<UInt32> indices;

        public  ModelDrawable loadModelDrawableFromObjFile(String shaderDir, String textureDir, String objFilePath)
        {
            try
            {
                reader = new StreamReader(objFilePath);
            }
            catch(Exception e)
            {
                Application.error("Could not load OBJ File!\nFile Path: " + objFilePath + "\nException: " + e.Message);
                return EntityCactusModel.getNewModelDrawable();//returns cactus model by defualt or failing
            }

            vertexResult = new List<Vertex>();
            positions = new List<Vector3F>();
            unorderedUVs = new List<Vector2F>();
            orderedUVs = new List<Vector2F>();
            indices = new List<UInt32>();
            processAllLines();
            return new ModelDrawable(shaderDir, textureDir, vertexResult.ToArray(), indices.ToArray());
        }

        private void processAllLines()
        {
           while((currentLine = reader.ReadLine()) != null)
            {
                if(currentLine.Contains("#") || currentLine.Contains("mtl") || currentLine.StartsWith("o ") || currentLine.StartsWith("s ") || currentLine.StartsWith("vn "))
                {
                    continue;
                }

                if(currentLine.StartsWith("v "))
                {
                    processVertexPosition();
                }
                if (currentLine.StartsWith("vt "))
                {
                    processUV();
                }
                if (currentLine.StartsWith("f ") && !hasProcessedFaces)
                {
                    processAllFaces();
                }
            }
        }

        private void processVertexPosition()
        {
            float[] vertexPosData = getFloatsFromStringArray(currentLine.Split(' '));
            Vector3F newVertPos = new Vector3F(vertexPosData[0], vertexPosData[1], vertexPosData[2]);
            positions.Add(newVertPos);
        }
        private void processUV()
        {
            float[] vertexUV = getFloatsFromStringArray(currentLine.Split(' '));
            Vector2F newUV = new Vector2F(vertexUV[0], 1 - vertexUV[1]);
            unorderedUVs.Add(newUV);
        }
        private void processAllFaces()
        {
            List<UInt32> uvIndices = new List<UInt32>();

            do//looping through all face lines in obj file
            {
                String[] faceTriples = currentLine.TrimStart("f ".ToCharArray()).Split(' ');

                indices.Add(UInt32.Parse(faceTriples[0][0].ToString()) - 1);
                indices.Add(UInt32.Parse(faceTriples[1][0].ToString()) - 1);
                indices.Add(UInt32.Parse(faceTriples[2][0].ToString()) - 1);

                uvIndices.Add(UInt32.Parse(faceTriples[0][2].ToString()));
                uvIndices.Add(UInt32.Parse(faceTriples[1][2].ToString()));
                uvIndices.Add(UInt32.Parse(faceTriples[2][2].ToString()));

            } while ((currentLine = reader.ReadLine()) != null && currentLine.StartsWith("f"));

            try
            {
               
                for (int i = 0; i < positions.Count; i++)
                {
                    vertexResult.Add(new Vertex(positions.ElementAt(i), ColourF.white.normalVector4F(), orderedUVs.ElementAt(i)));
                }
            }
            catch(Exception e)
            {
                Application.error("OBJLoader could not convert data to vertices!\nException: " + e.Message);
                Console.ReadKey();
            }

            hasProcessedFaces = true;//do last, to make sure there is not mistake with any lines starting with f triggering a new face processing method call
        }

        private float[] getFloatsFromStringArray(String[] strings)
        {
            List<float> result = new List<float>();

            for(int i = 0; i < strings.Length; i++)
            {
                if (float.TryParse(strings[i], out float outFloat))
                {
                    result.Add(outFloat);
                }
            }
            return result.ToArray();
        }
    }
}
