using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using RabbetGameEngine.Debugging;
using RabbetGameEngine.Models;
using RabbetGameEngine.SubRendering;
using System.Collections.Generic;
using System.Linq;

namespace RabbetGameEngine
{
    public enum RenderType
    {
        none,
        MARKER_GUI_START,
        guiCutout,
        MARKER_TRANSPARENT_START,
        guiText, 
        guiTransparent,
        MARKER_GUI_END,
        iSpheresTransparent,
        trianglesTransparent,
        quadsTransparent,
        MARKER_LERP_START,
        lerpISpheresTransparent,
        lerpTrianglesTransparent,
        lerpQuadsTransparent,
        MARKER_TRANSPARENT_END,
        lerpText3D,
        lerpISpheres,
        lerpTriangles,
        lerpQuads,
        lerpLines,
        MARKER_LERP_END,
        text3D,
        triangles,
        quads,
        lines,
        iSpheres,
        spriteCylinder
    }
    //TODO: implement optional fisheye distortion to solve large fov scaling issues, and optional full screen dithering if banding becomes an issue.
    public static class Renderer
    {
        private static int privateTotalDrawCallCount;
        private static Matrix4 projectionMatrix;
        private static Matrix4 orthographicMatrix;
        private static bool useFrameBuffer = true;
       
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
            BatchUtil.init();
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
            GL.Enable(EnableCap.ProgramPointSize);
            GL.Enable(EnableCap.VertexProgramPointSize);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Multisample);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.LineWidth(1);
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathUtil.radians(GameSettings.fov), GameInstance.aspectRatio, 0.1F, GameSettings.maxDrawDistance);
            orthographicMatrix = Matrix4.CreateOrthographic(GameInstance.gameWindowWidth, GameInstance.gameWindowHeight, 0.1F, 1.0F);
            staticDraws = new Dictionary<string, StaticRenderObject>();
            SkyboxRenderer.init();
            FrameBuffer.init();
        }

        /*Called each time the game window is resized*/
        public static void onResize()
        {
            GL.Viewport(GameInstance.get.getGameWindowSize());
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)MathUtil.radians(GameSettings.fov), GameInstance.aspectRatio, 0.1F, 1000.0F);
            orthographicMatrix = Matrix4.CreateOrthographic(GameInstance.gameWindowWidth, GameInstance.gameWindowHeight, 0.1F, 1.0F);
            GUIManager.onWindowResize();
            FrameBuffer.onResize();
        }

        public static void requestRender(RenderType type, Texture tex, Model mod, int renderLayer = 0)
        {
            BatchManager.requestRender(type, tex, mod, renderLayer);
        }

        public static void requestRender(PointCloudModel mod, bool transparency, bool lerp)
        {
            BatchManager.requestRender(mod, transparency, lerp);
        }

        public static void requestRender(PointParticle point, bool transparency)
        {
            BatchManager.requestRender(point, transparency);
        }

        public static void requestRender(Sprite3D s, Texture tex)
        {
            BatchManager.requestRender(s, tex);
        }

        public static void requestRender(PointParticle point, PointParticle prevTickPoint, bool transparency)
        {
            BatchManager.requestRender(point, prevTickPoint, transparency);
        }
        public static void onTick()
        {
            SkyboxRenderer.onTick();
        }

        public static void onTickEnd()
        {
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathUtil.radians(GameSettings.fov), GameInstance.aspectRatio, 0.1F, GameInstance.get.getDrawDistance());
        }
        public static void doRenderUpdate()
        {
            Profiler.beginEndProfile("renderUpdate");
            BatchManager.preRenderUpdate();
            GUIManager.requestRender();
            if (GameInstance.get.currentPlanet != null)
            {
                GameInstance.get.currentPlanet.onRenderUpdate();
            }
            BatchManager.postRenderUpdate();
            Profiler.beginEndProfile("renderUpdate");
        }

        /*Called before all draw calls*/
        private static void preRender()
        {
            if(useFrameBuffer)
            FrameBuffer.prepareToRenderToOffScreenTexture();
            GL.Clear(ClearBufferMask.DepthBufferBit);
            privateTotalDrawCallCount = 0;
        }

        public static void renderAll()
        {
            preRender();
            SkyboxRenderer.drawSkybox(GameInstance.get.thePlayer.getViewMatrix());
            drawAllStaticRenderObjects();
            BatchManager.drawAllWorld(GameInstance.get.thePlayer.getViewMatrix(), GameInstance.get.currentPlanet.getFogColor());
            if(!useFrameBuffer)
            BatchManager.drawAllGUI(GameInstance.get.thePlayer.getViewMatrix(), GameInstance.get.currentPlanet.getFogColor());
            postRender();
        }
        
        /*Called after all draw calls*/
        private static void postRender()
        {
            GameInstance.get.SwapBuffers();
            if(useFrameBuffer)
            {
                FrameBuffer.renderOffScreenTexture();
                BatchManager.drawAllGUI(GameInstance.get.thePlayer.getViewMatrix(), GameInstance.get.currentPlanet.getFogColor());
            }
        }

        public static void addStaticDrawTriangles(string name, string textureName, Model data)
        {
            if (staticDraws.TryGetValue(name, out StaticRenderObject s))
            {
                s.delete();
                staticDraws.Remove(name);
            }
            staticDraws.Add(name, StaticRenderObject.createSROTriangles(textureName, data));
        }

        public static void addStaticDrawLines(string name, Model data)
        {
            if (staticDraws.TryGetValue(name, out StaticRenderObject s))
            {
                s.delete();
                staticDraws.Remove(name);
            }
            staticDraws.Add(name, StaticRenderObject.createSROLines(data));
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
            SkyboxRenderer.deleteVAO();
        }
        public static Matrix4 projMatrix { get => projectionMatrix; }
        public static int totalDraws { get { return privateTotalDrawCallCount; } set { privateTotalDrawCallCount = value; } }
        public static Matrix4 orthoMatrix { get => orthographicMatrix; }
        public static Vector3 camPos { get => GameInstance.get.thePlayer.getLerpEyePos(); }

        public static Vector2 viewPortSize { get => useFrameBuffer ? FrameBuffer.size : new Vector2(GameInstance.gameWindowWidth, GameInstance.gameWindowHeight);}

    }
}
 