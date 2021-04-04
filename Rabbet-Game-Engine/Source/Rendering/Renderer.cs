using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
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
        guiLines,
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

    public static class Renderer
    {
        private static int privateTotalDrawCallCount;
        private static int privateTotalFBODrawCallCount;
        private static Matrix4 projectionMatrix;
        private static Matrix4 orthographicMatrix;
        private static bool usePostProcessing = true;
        private static int lineWidthPixels = 0;
        private static bool initialized = false;
        /// <summary>
        /// A list of all requested static renders
        /// </summary>
        private static Dictionary<string, StaticRenderObject> staticDraws;
        
        /*Called before any rendering is done*/
        public static unsafe void init()
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
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            lineWidthPixels = (GameInstance.realScreenHeight / 900) + 1;
            GL.LineWidth(lineWidthPixels);
            staticDraws = new Dictionary<string, StaticRenderObject>();
            SkyboxRenderer.init();
            PostProcessing.init();

            MonitorHandle m = GameInstance.get.CurrentMonitor;
            VideoMode mode = *GLFW.GetVideoMode(m.ToUnsafePtr<Monitor>());
            GameInstance.screenWidth = mode.Width;
            GameInstance.screenHeight = mode.Height;
            int hw = GameInstance.screenWidth / 2;
            int hh = GameInstance.screenHeight / 2;
            GameInstance.get.ClientRectangle = new Box2i(hw - hw / 2, hh - hh / 2, hw + hw / 2, hh + hh / 2);
            GameInstance.windowWidth = GameInstance.get.ClientRectangle.Size.X;
            GameInstance.windowHeight = GameInstance.get.ClientRectangle.Size.Y;
            GameInstance.get.Context.MakeCurrent();
            onResize();
            initialized = true;
        }

        /*Called each time the game window is resized*/
        public static void onResize()
        {
            GL.Viewport(GameInstance.get.getGameWindowSize());
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)MathUtil.radians(GameSettings.fov.floatValue), GameInstance.aspectRatio, 0.1F, 1000.0F);
            orthographicMatrix = Matrix4.CreateOrthographic(GameInstance.gameWindowWidth, GameInstance.gameWindowHeight, 0.1F, 100.0F);
            GUIManager.onWindowResize();
            PostProcessing.onResize();
            BatchManager.onWindowResize();
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
        public static void doWorldRenderUpdate()
        {
            Profiler.startSection("renderUpdate");
            BatchManager.preWorldRenderUpdate(GameInstance.get.currentWorld);
            SkyboxRenderer.onUpdate();
            if (GameInstance.get.currentWorld != null)
            {
                GameInstance.get.currentWorld.onRenderUpdate();
            }
            BatchManager.postWorldRenderUpdate();
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathUtil.radians(GameSettings.fov.floatValue), GameInstance.aspectRatio, 0.1F, GameInstance.get.getDrawDistance());
            Profiler.endCurrentSection();
        }
        public static void doGUIRenderUpdate()
        {
            Profiler.startSection("guiRenderUpdate");
            BatchManager.preGUIRenderUpdate(GameInstance.get.currentWorld);
            GUIManager.requestRender();
            BatchManager.postGUIRenderUpdate();
            Profiler.endCurrentSection();
        }

        /*Called before all draw calls*/
        private static void preRender()
        {
            if(usePostProcessing)
            PostProcessing.beforeRender();
            GL.Clear(ClearBufferMask.DepthBufferBit);
            privateTotalDrawCallCount = 0;
            privateTotalFBODrawCallCount = 0;
        }

        public static void onVideoSettingsChanged()
        {
            PostProcessing.onVideoSettingsChanged();
            BatchManager.onVideoSettingsChanged();
        }
        public static void renderAll()
        {
            preRender();
            Profiler.startSection("renderWorld");
            SkyboxRenderer.drawSkybox(GameInstance.get.thePlayer.getViewMatrix());
            drawAllStaticRenderObjects();
            BatchManager.drawAllWorld();
            if(!usePostProcessing)
            {
                Profiler.startSection("renderGUI");
                BatchManager.drawAllGUI();
                Profiler.endCurrentSection();
            }
            Profiler.endCurrentSection();
            postRender();
        }
        
        /*Called after all draw calls*/
        private static void postRender()
        {
            if(usePostProcessing)
            {
                Profiler.startSection("renderGUI");
                PostProcessing.doPostProcessing();
                BatchManager.drawAllGUI();
                Profiler.endCurrentSection();
            }
            GameInstance.get.SwapBuffers();
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
                staticDraws.ElementAt(i).Value.draw(GameInstance.get.thePlayer.getViewMatrix(), GameInstance.get.currentWorld.getFogColor());
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
            if (!initialized) return;
            foreach (StaticRenderObject s in staticDraws.Values)
            {
                s.delete();
            }
            BatchManager.deleteAll();
            ShaderUtil.deleteAll();
            TextureUtil.deleteAll();
            SkyboxRenderer.deleteVAO();
            PostProcessing.onClosing();
        }
        public static Matrix4 projMatrix { get => projectionMatrix; }
        public static int totalDraws { get { return privateTotalDrawCallCount; } set { privateTotalDrawCallCount = value; } }
        public static int totalFBODraws { get { return privateTotalFBODrawCallCount; } set { privateTotalFBODrawCallCount = value; } }
        public static Matrix4 orthoMatrix { get => orthographicMatrix; }
        public static Vector3 camPos { get => GameInstance.get.thePlayer.getLerpEyePos(); }
        public static int defaultLineWidthInPixels { get => lineWidthPixels; }
        public static Vector2 viewPortSize { get => usePostProcessing ? PostProcessing.viewPortSize : new Vector2(GameInstance.gameWindowWidth, GameInstance.gameWindowHeight);}

    }
}
 