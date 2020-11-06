using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using RabbetGameEngine.Debugging;
using RabbetGameEngine.GUI;
using RabbetGameEngine.Models;
using RabbetGameEngine.SubRendering;
using System.Collections.Generic;
using System.Linq;

namespace RabbetGameEngine
{
    public static class Renderer
    {
        private static int privateTotalDrawCallCount;
        private static Matrix4 projectionMatrix;
        private static Matrix4 orthographicMatrix;
       
        /// <summary>
        /// A list of all requested static renders
        /// </summary>
        private static Dictionary<string, StaticRenderObject> staticDraws;
        
        /*Called before any rendering is done*/
        public static void init()
        {
            ShaderUtil.loadAllFoundShaderFiles();
            TextureUtil.loadAllFoundTextureFiles();
            MeshUtil.loadAllFoundModelFiles();
            Application.infoPrint("OpenGL Version: " + GL.GetString(StringName.Version));
            Application.infoPrint("OpenGL Vendor: " + GL.GetString(StringName.Vendor));
            Application.infoPrint("Shading Language Version: " + GL.GetString(StringName.ShadingLanguageVersion));
            Application.infoPrint("Loaded " + ShaderUtil.getShaderCount() + " shaders.");
            Application.infoPrint("Loaded " + TextureUtil.getTextureCount() + " textures.");
            Application.infoPrint("Loaded " + MeshUtil.getModelCount() + " models.");
            GL.Viewport(GameInstance.get.getGameWindowSize());
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.PointSprite);
            GL.Enable(EnableCap.VertexProgramPointSize);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.LineWidth(1);
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathUtil.radians(GameSettings.fov), GameInstance.aspectRatio, 0.1F, GameSettings.maxDrawDistance);
            orthographicMatrix = Matrix4.CreateOrthographic(GameInstance.gameWindowWidth, GameInstance.gameWindowHeight, 0.1F, 1.0F);
            staticDraws = new Dictionary<string, StaticRenderObject>();
            SkyboxRenderer.init();
        }

        /*Called each time the game window is resized*/
        public static void onResize()
        {
            GL.Viewport(GameInstance.get.getGameWindowSize());
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)MathUtil.radians(GameSettings.fov), GameInstance.aspectRatio, 0.1F, 1000.0F);
            orthographicMatrix = Matrix4.CreateOrthographic(GameInstance.gameWindowWidth, GameInstance.gameWindowHeight, 0.1F, 1.0F);
            GUIManager.onWindowResize();
        }

        /*called once per frame*/
        public static void onFrame()
        {
            
        }

        public static void requestRender(BatchType type, Texture tex, Model mod)
        {
            Profiler.beginEndProfile("batching");
            BatchManager.requestRender(type, tex, mod);
            Profiler.beginEndProfile("batching");
        }

        public static void requestRender(PointCloudModel mod, bool transparency, bool lerp)
        {
            Profiler.beginEndProfile("batching");
            BatchManager.requestRender(mod, transparency, lerp);
            Profiler.beginEndProfile("batching");
        }

        public static void requestRender(PointParticle point, bool transparency)
        {
            Profiler.beginEndProfile("batching");
            BatchManager.requestRender(point, transparency);
            Profiler.beginEndProfile("batching");
        }

        public static void requestRender(PointParticle point, PointParticle prevTickPoint, bool transparency)
        {
            Profiler.beginEndProfile("batching");
            BatchManager.requestRender(point, prevTickPoint, transparency);
            Profiler.beginEndProfile("batching");
        }
        public static void beforeTick()
        {
            Profiler.beginEndProfile("batching");
            BatchManager.beforeTick();
            Profiler.beginEndProfile("batching");
            HitboxRenderer.beforeTick();
        }
        public static void onTickEnd()
        {
            Profiler.beginEndProfile("batching");
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathUtil.radians(GameSettings.fov), GameInstance.aspectRatio, 0.1F, GameInstance.get.getDrawDistance());
            BatchManager.onTickEnd();
            Profiler.beginEndProfile("batching");
            HitboxRenderer.onTickEnd();
        }

        /*Called before all draw calls*/
        private static void preRender()
        {
            GL.Clear(ClearBufferMask.DepthBufferBit);
            privateTotalDrawCallCount = 0;
        }

        public static void renderAll()
        {
            preRender();
            SkyboxRenderer.drawSkybox(projectionMatrix, GameInstance.get.thePlayer.getViewMatrix());
            drawAllStaticRenderObjects();
            BatchManager.drawAll(GameInstance.get.thePlayer.getViewMatrix(), GameInstance.get.currentPlanet.getFogColor());
            postRender();
        }
        
        /*Called after all draw calls*/
        private static void postRender()
        {
            GameInstance.get.SwapBuffers();
        }

        public static void addStaticDrawTriangles(string name, string textureName, string shaderName, Model data)
        {
            if (staticDraws.TryGetValue(name, out StaticRenderObject s))
            {
                s.delete();
                staticDraws.Remove(name);
            }
            staticDraws.Add(name, StaticRenderObject.createSROTriangles(textureName, shaderName, data));
        }

        public static void addStaticDrawTriangles(string name, string textureName, Model data)
        {
            if (staticDraws.TryGetValue(name, out StaticRenderObject s))
            {
                s.delete();
                staticDraws.Remove(name);
            }
            staticDraws.Add(name, StaticRenderObject.createSROTriangles(textureName, ShaderUtil.trianglesName, data));
        }

        public static void addStaticDrawLines(string name, string textureName, string shaderName, Model data)
        {
            if (staticDraws.TryGetValue(name, out StaticRenderObject s))
            {
                s.delete();
                staticDraws.Remove(name);
            }
            staticDraws.Add(name, StaticRenderObject.createSROLines(textureName, shaderName, data));
        }

        public static void addStaticDrawLines(string name, string textureName, Model data)
        {
            if (staticDraws.TryGetValue(name, out StaticRenderObject s))
            {
                s.delete();
                staticDraws.Remove(name);
            }
            staticDraws.Add(name, StaticRenderObject.createSROLines(textureName, ShaderUtil.linesName, data));
        }

        public static void addStaticDrawPoints(string name, PointParticle[] data, bool transparency)
        {
            if (staticDraws.TryGetValue(name, out StaticRenderObject s))
            {
                s.delete();
                staticDraws.Remove(name);
            }
            staticDraws.Add(name, StaticRenderObject.createSROPoints(data, transparency));
        }
        public static void removeStaticDraw(string name)
        {
            if (staticDraws.TryGetValue(name, out StaticRenderObject s))
            {
                s.delete();
                staticDraws.Remove(name);
            }
        }

        private static void drawAllStaticRenderObjects()
        {
            for(int i = 0; i < staticDraws.Count; ++i)
            { 
                staticDraws.ElementAt(i).Value.draw(GameInstance.get.thePlayer.getViewMatrix(), GameInstance.get.currentPlanet.getFogColor());
                totalDraws++;
            }
        }

        public static void onToggleFullscreen()
        {
            if (GameSettings.fullscreen)
            {
                GameInstance.get.WindowState = WindowState.Fullscreen;
            }
            else
            {
                GameInstance.get.WindowState = WindowState.Normal;
            }

        }

        /*deletes all loaded opengl assets*/
        public static void onClosing()
        {
            foreach (StaticRenderObject s in staticDraws.Values)
            {
                s.delete();
            }
            BatchManager.deleteAll();
            ShaderUtil.deleteAll();
            TextureUtil.deleteAll();
        }
        public static Matrix4 projMatrix { get => projectionMatrix; }
        public static int totalDraws { get { return privateTotalDrawCallCount; } set { privateTotalDrawCallCount = value; } }
        public static Matrix4 orthoMatrix { get => orthographicMatrix; }
        public static Vector3 camPos { get => GameInstance.get.thePlayer.getLerpEyePos(); }

    }
}
 