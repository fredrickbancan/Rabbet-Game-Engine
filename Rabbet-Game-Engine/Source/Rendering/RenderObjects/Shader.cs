using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;

namespace RabbetGameEngine
{
    public class Shader : IDisposable
    {
        private enum shaderType // simple enum for sorting lines of shader code
        {
            NONE,
            VERTEX,
            GEOMETRY,
            FRAGMENT
        };
        private struct shaderProgramSource // simple struct for reading both shaders from one file
        {
            public string vertexSource;
            public string geometrySource;
            public string fragmentSource;
        };

        public static readonly string debugDefaultVertexShader = 
            "#version 330 core\n" +
            "void main()\n" +
            "{\n" +
            "gl_Position = vec4(0,0,0,0);\n" +
            "}\n";
        public static readonly string debugDefaultFragmentShader =
            "#version 330 core\n" +
            "void main()\n" +
            "{\n" +
            "discard;\n" +
            "}\n";
        private bool disposed = false;
        private int id;
        private string debugShaderPath;

        private Dictionary<string, int> foundUniforms;
        public Shader(string filePath)
        {
            if(filePath == "debug")
            {
                setDebugShader();
                return;
            }
            debugShaderPath = filePath;
            shaderProgramSource source = parseShaderFile(filePath);
            this.id = createShader(source);
            foundUniforms = new Dictionary<string, int>();
        }

        private void setDebugShader()
        {
            shaderProgramSource source;
            source.vertexSource = debugDefaultVertexShader;
            source.geometrySource = "";
            source.fragmentSource = debugDefaultFragmentShader;
            this.id = createShader(source);
            foundUniforms = new Dictionary<string, int>();
        }
        private shaderProgramSource parseShaderFile(string path)
        {
            shaderType type = shaderType.NONE;
            string currentLine = "";
            string vertexSource = "";
            string geometrySource = "";
            string fragmentSource = "";
            StreamReader reader;
            try
            {
                reader = new StreamReader(path);

                while ((currentLine = reader.ReadLine()) != null)
                {
                    if (currentLine.Contains("#shader"))
                    {
                        if (currentLine.Contains("vertex"))
                        {
                            type = shaderType.VERTEX;
                        }
                        else if (currentLine.Contains("fragment"))
                        {
                            type = shaderType.FRAGMENT;
                        }
                        else if (currentLine.Contains("geometry"))
                        {
                            type = shaderType.GEOMETRY;
                        }
                    }
                    else if (type == shaderType.VERTEX)
                    {
                        vertexSource += (currentLine + "\n");
                    }
                    else if (type == shaderType.FRAGMENT)
                    {
                        fragmentSource += (currentLine + "\n");
                    }
                    else if (type == shaderType.GEOMETRY)
                    {
                        geometrySource += (currentLine + "\n");
                    }
                }
                reader.Close();
                
            }
            catch(Exception e)
            {
                Application.error("Shader failed to load!\nException: " + e.Message + "\nShader path: " + path);
                vertexSource = debugDefaultVertexShader;
                vertexSource = "";
                fragmentSource = debugDefaultFragmentShader;
            }
            shaderProgramSource result;
            result.vertexSource = vertexSource;
            result.geometrySource = geometrySource;
            result.fragmentSource = fragmentSource;
            return result;
        }

        private int createShader(shaderProgramSource source)//creates a shader program from both the fragment and vertex shader source provided in the struct, returns program id
        {
            int program = GL.CreateProgram();
            int vsh = compileShader(OpenTK.Graphics.OpenGL.ShaderType.VertexShader, source.vertexSource);
            int fsh = compileShader(OpenTK.Graphics.OpenGL.ShaderType.FragmentShader, source.fragmentSource);
            int gsh = 0;
            if (source.geometrySource != "")
            {
                gsh = compileShader(OpenTK.Graphics.OpenGL.ShaderType.GeometryShader, source.geometrySource);
                GL.AttachShader(program, gsh);
            }
            GL.AttachShader(program, vsh);
            GL.AttachShader(program, fsh);
            GL.LinkProgram(program);
            GL.ValidateProgram(program);
            GL.DeleteShader(vsh);
            if (gsh != 0)
            {
                GL.DeleteShader(gsh);
            }
            GL.DeleteShader(fsh);

            return program;
        }

        private int compileShader(OpenTK.Graphics.OpenGL.ShaderType type, string source)//compiles the given source code depending on the given type and returns the shader id
        {
            int id = GL.CreateShader(type);
            GL.ShaderSource(id, source);
            GL.CompileShader(id);

            //print any errors
            string infoLog = GL.GetShaderInfoLog(id);
            if (infoLog != System.String.Empty)
            {
                Application.error("\n\nError when compiling shader!\ntype: " + (type == OpenTK.Graphics.OpenGL.ShaderType.VertexShader ? "vertex shader" : type == OpenTK.Graphics.OpenGL.ShaderType.GeometryShader ? "geometry shader" : "fragment shader") + "\nmessage log: " + infoLog + "\nShader File Path: " + debugShaderPath);
                GL.DeleteProgram(id);
                return 0;
            }

            return id;
        }
        public void use()
        {
            GL.UseProgram(id);
        }

        public void setUniformMat4F(string name, Matrix4 matrix)
        {
            GL.UniformMatrix4(getUniformLocation(name), 1, false, ref matrix.Row0.X );
        }

        public void setUniformVec3F(string name, Vector3 vec)
        {
            GL.Uniform3(getUniformLocation(name), vec.X, vec.Y, vec.Z);
        }

        public void setUniformMat4FArray(string name, Matrix4[] arr)
        {
            GL.UniformMatrix4(getUniformLocation(name), arr.Length, false, ref arr[0].Row0.X);
        }

        public void setUniformVec2F(string name, Vector2 vec)
        {
            GL.Uniform2(getUniformLocation(name), vec.X, vec.Y);
        }
        public void setUniform1I(string name, int val)
        {
            GL.Uniform1(getUniformLocation(name), val);
        }
        public void setUniform1F(string name, float val)
        {
            GL.Uniform1(getUniformLocation(name), val);
        }

        public void setUniformFArray(string name, float[] vals)
        {
            GL.Uniform1(getUniformLocation(name), vals.Length, ref vals[0]);
        }

        public void setUniformIArray(string name, int[] vals)
        {
            GL.Uniform1(getUniformLocation(name), vals.Length, ref vals[0]);
        }

        private int getUniformLocation(string name)
        {
            int result;
            if(foundUniforms.TryGetValue(name, out result))//storing previously found uniforms in a dictionary helps to avoid gl calls
            {
                return result;
            }

            result = GL.GetUniformLocation(id, name);
            foundUniforms.Add(name, result);
            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                GL.DeleteProgram(id);

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
