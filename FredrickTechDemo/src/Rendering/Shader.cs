using FredrickTechDemo.FredsMath;
using OpenTK.Graphics.OpenGL;
using System;
using System.IO;

namespace FredrickTechDemo
{
    struct shaderProgramSource // simple struct for reading both shaders from one file
    {
        public String vertexSource;
        public String fragmentSource;
    };
    enum Type // simple enum for sorting lines of shader code
    {
        NONE,
        VERTEX,
        FRAGMENT
    };

    class Shader : IDisposable
    {
        private bool disposed = false;
        private int id;
        private String debugShaderPath;
        public Shader(String filePath)
        {
            debugShaderPath = filePath;
            shaderProgramSource source = parseShaderFile(filePath);
            this.id = createShader(source);
        }

        private shaderProgramSource parseShaderFile(String path)
        {
            Type type = Type.NONE;
            String currentLine = "";
            String vertexSource = "";
            String fragmentSource = "";
            StreamReader reader = new StreamReader(path);

            while((currentLine = reader.ReadLine()) != null)
            { 
                if (currentLine.Contains("#shader"))
                {
                    if (currentLine.Contains("vertex"))
                    {
                        type = Type.VERTEX;
                    }
                    else if (currentLine.Contains("fragment"))
                    {
                        type = Type.FRAGMENT;
                    }
                }
                else if (type == Type.VERTEX)
                {
                    vertexSource += (currentLine + "\n");
                }
                else if (type == Type.FRAGMENT)
                {
                    fragmentSource += (currentLine + "\n");
                }
            }
            reader.Close();
            shaderProgramSource result;
            result.vertexSource = vertexSource;
            result.fragmentSource = fragmentSource;
            return result;
        }

        private int createShader(shaderProgramSource source)//creates a shader program from both the fragment and vertex shader source provided in the struct, returns program id
        {
            int program = GL.CreateProgram();
            int vsh = compileShader(ShaderType.VertexShader, source.vertexSource);
            int fsh = compileShader(ShaderType.FragmentShader, source.fragmentSource);

            GL.AttachShader(program, vsh);
            GL.AttachShader(program, fsh);
            GL.LinkProgram(program);
            GL.ValidateProgram(program);
            GL.DeleteShader(vsh);
            GL.DeleteShader(fsh);

            return program;
        }

        private int compileShader(ShaderType type, String source)//compiles the given source code depending on the given type and returns the shader id
        {
            int id = GL.CreateShader(type);
            GL.ShaderSource(id, source);
            GL.CompileShader(id);

            //print any errors
            String infoLog = GL.GetShaderInfoLog(id);
            if (infoLog != System.String.Empty)
            {
                Application.error("Error when compiling shader!\ntype: " + (type == ShaderType.VertexShader ? "vertex shader" : "fragment shader") + "\nmessage log: " + infoLog + "\nShader File Path: " + debugShaderPath);
                return 0;
            }

            return id;
        }
        public void use()
        {
            GL.UseProgram(id);
        }

        public void setUniformMat4F(String name, Matrix4F matrix)
        {
            GL.UniformMatrix4(getUniformLocation(name), 1, false, ref matrix.row0.x );
        }

        public void setUniformVec3F(String name, Vector3F vec)
        {
            GL.Uniform3(getUniformLocation(name), (double)vec.x, (double)vec.y, (double)vec.z);
        }

        public void setUniformVec2F(String name, Vector2F vec)
        {
            GL.Uniform2(getUniformLocation(name), (double)vec.x, (double)vec.y);
        }

        private int getUniformLocation(String name)// later will cache uniforms
        {
            return GL.GetUniformLocation(id, name);
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
