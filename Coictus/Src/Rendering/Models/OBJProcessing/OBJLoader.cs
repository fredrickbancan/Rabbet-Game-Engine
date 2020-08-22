using Coictus.FredsMath;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Coictus.Models
{
    /*This class will be responsable for loading models and converting them to vertex arrays for use with rendering.*/
    public static class OBJLoader
    {
        private static String currentLine = "empty";
        private static StreamReader reader;
        private static bool successfullyLoaded = false;
        private static List<Vertex> vertexResult;
        private static List<Vector3F> positions;
        private static List<Vector2F> unorderedUVs; 
        private static Vector2F[] orderedUVs; 
        private static List<UInt32> indices;

        /*Takes in a shader,texture and obj file and returns a modeldrawable. If processing fails, will return a default debug model*/
        public static ModelDrawable loadModelDrawableFromObjFile(String shaderDir, String textureDir, String objFilePath, bool useIndices = true)
        {
            if (objFilePath == "none")
            {
                return null;
            }

            try
            {
                reader = new StreamReader(objFilePath);
            }
            catch (Exception e)
            {
                Application.error("Could not load OBJ File!\nFile Path: " + objFilePath + "\nException: " + e.Message);
                return DefaultDebugModel.getNewModelDrawable();//returns model by defualt or failing
            }
            successfullyLoaded = false;
            vertexResult = new List<Vertex>();
            positions = new List<Vector3F>();
            unorderedUVs = new List<Vector2F>();
            indices = new List<UInt32>();

            processAllLines();

            if (!successfullyLoaded)
            {
                return DefaultDebugModel.getNewModelDrawable();//returns model by defualt or failing
            }
            reader.Close();
            return new ModelDrawable(shaderDir, textureDir, vertexResult.ToArray(), useIndices ? indices.ToArray() : null);

        }

        /*reads each line and processes it based on its tag, v is vertex position, vt is uv, and f is a face*/
        private static void processAllLines()
        {
           while((currentLine = reader.ReadLine()) != null && !successfullyLoaded)
            {
                if(currentLine.StartsWith("v "))
                {
                    processVertexPosition();
                }
                if (currentLine.StartsWith("vt "))
                {
                    processUV();
                }
                if (currentLine.StartsWith("f "))
                {
                    processAllFaces();
                }
            }
        }

        /*Adds the vertex positions in the line to the positions list as a vector3f*/
        private static void processVertexPosition()
        {
            float[] vertexPosData = getFloatsFromStringArray(currentLine.Split(' '));
            Vector3F newVertPos = new Vector3F(vertexPosData[0], vertexPosData[1], vertexPosData[2]);
            positions.Add(newVertPos);
        }
        /*Adds the uvs in the line to the unordered uv list as a vector2f*/
        private static void processUV()
        {
            float[] vertexUV = getFloatsFromStringArray(currentLine.Split(' '));
            Vector2F newUV = new Vector2F(vertexUV[0], vertexUV[1]);// other model programs may requre a v flip, blender does not
            unorderedUVs.Add(newUV);
        }

        /*This will read all the face lines and construct the vertices and indices as is in the file.
          A list of ordered uv's will be constructed based on the uv indices in the face lines. This 
          ordered uv list will match the positions list and thus can be placed in the same vertices*/
        private static void processAllFaces()
        {
            try
            {
                List<int> uvIndices = new List<int>();

                do//looping through all face lines in obj file and reading all the indices
                {
                    String[] faceTriples = currentLine.TrimStart("f ".ToCharArray()).Split(' ');
                    String[] vertex1 = faceTriples[0].Split('/');
                    String[] vertex2 = faceTriples[1].Split('/');
                    String[] vertex3 = faceTriples[2].Split('/');

                    indices.Add(UInt32.Parse(vertex1[0].ToString()) - 1);//-1 because obj indices start from 1
                    indices.Add(UInt32.Parse(vertex2[0].ToString()) - 1);
                    indices.Add(UInt32.Parse(vertex3[0].ToString()) - 1);

                    uvIndices.Add(int.Parse(vertex1[1].ToString()) - 1);
                    uvIndices.Add(int.Parse(vertex2[1].ToString()) - 1);
                    uvIndices.Add(int.Parse(vertex3[1].ToString()) - 1);

                } while ((currentLine = reader.ReadLine()) != null && currentLine.StartsWith("f"));

                if (indices.Count != uvIndices.Count)
                {
                    Application.error("OBJ loader detected missmatch in data, UV and Position indices are not the same length!");
                }

                /*Here we take init the ordered uv list, and then for each position index (which we are using as vertex indices) 
                  We take the corrosponding uv index and use it to take the values from the unordered array at that index. This 
                  Manually matches the uv data with the vertex position data so they can be put into vertices.*/
                orderedUVs = new Vector2F[unorderedUVs.Count];
                for (int i = 0; i < uvIndices.Count; i++)
                {
                    orderedUVs[indices.ElementAt(i)] = unorderedUVs.ElementAt(uvIndices.ElementAt(i));
                }

                for (int i = 0; i < positions.Count; i++)
                {
                    vertexResult.Add(new Vertex(positions.ElementAt(i), ColourF.white.normalVector4F(), orderedUVs[i]));
                }
                successfullyLoaded = true;//do last, to make sure there is not mistake with any lines starting with f triggering a new face processing method call
            }
            catch (Exception e)
            {
                Application.error("OBJLoader could not convert data to vertices!\nException: " + e.Message);
                successfullyLoaded = false;
            }
        }

        private static float[] getFloatsFromStringArray(String[] strings)
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
