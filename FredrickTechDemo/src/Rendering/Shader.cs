using System;
using System.IO;
using FredsMath;
using OpenTK.Graphics.OpenGL;

namespace FredrickTechDemo.src.Rendering
{
    struct shaderProgramSource // simple struct for reading both shaders from one file
    {
        public String vertexSource;
        public String fragmentSource;
    };

    class Shader : IDisposable
    {
        private bool disposed = false;
        private int id;

        public Shader(String filePath)
        {
            shaderProgramSource source = parseShaderFile(filePath);
            this.id = createShader(source);
        }

        private shaderProgramSource parseShaderFile(String path)
        {
            byte type = 0;//vertex = 1, fragment - 2, none = 0
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
                        type = 1;
                    }
                    else if (currentLine.Contains("fragment"))
                    {
                        type = 2;
                    }
                }
                else if (type == 1)//vertex shader
                {
                    vertexSource += (currentLine + "\n");
                }
                else if (type == 2)//fragment shader
                {
                    fragmentSource += (currentLine + "\n");
                }
            }

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
                Application.error(infoLog);
                return 0;
            }

            return id;
        }
        public void Use()
        {
            GL.UseProgram(id);
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

        public void setUniformMat4F(String name, Matrix4F matrix)
        {
            GL.UniformMatrix4(getUniformLocation(name), 1, false, ref matrix.row0.x );
        }

        private int getUniformLocation(String name)// later will cache uniforms
        {
            return GL.GetUniformLocation(id, name);
        }
    }
}
